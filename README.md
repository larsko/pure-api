# pure-api
This is a light-weight Pure API wrapper written in C#. 

REST responses are automatically deserialized to a dynamic JSON object. This makes it very easy to use as-is. 
This code is intended as a quick and dirty demonstration of how to retrieve data from the Pure API.
If you need type safety, feel free to implement the model on top and use the overloaded methods.

For an introduction to the Pure API, please refer to the Swagger.io documentation. 
The documentation can be found at https://<PURE_URL>/ws/api/59/api-docs.

## Dependencies
- .NET 4.6+ (+ Microsoft.CSharp for Mono users)
- Newtonsoft.Json
- RestSharp

## Usage
Create an API key for the Pure API. You will need to pass the API key for every request. It is recommended to store the configuration values below to your `App.Config` file.
```csharp
string baseUrl = "<PURE_URL>"; // do not include the relative path, i.e. /ws/api
string apiKey = "<PURE_API_KEY>"; // The API key to use - generate this in Pure
string apiVersion = "59"; // API version to use, e.g. 59, 510, 511, etc.
```
Instantiate the Pure Client with the URL, API key and version.
```csharp
var client = new PureClient(baseUrl, apiKey, apiVersion);
```
### Making Requests
Request a particular resource and execute it with the Pure Client.
```csharp
var request = new PureRequest("persons");
```

The `result` returned is a `dynamic` type. 
```csharp
var result = client.Execute(request);
int count = result.count; // total # persons returned given criteria
Console.WriteLine(result.items[0].name.lastName); // print name of first record

```

It is possible to deserialize to a concrete type - simply pass the type parameter `T`:
```csharp
var result = client.Execute<T>(request);
```
Tip: http://jsonutils.com/ is a great class generation tool for JSON that can help save you some time.

### Harvesting Data
A common use case is to download all data from a particular content family, e.g. research output. 
The Pure API implements paging for this purpose, making it possible to download content in batches of the desired size.

Using the harvester is easy, simply instantiate it with the API client:
```csharp
var harvester = new PureHarvester(client);
```
Then download all content from a certain family, e.g. research output:
```csharp
harvester.Harvest("research-outputs", data => {
    // Print the UUID of each publication
    foreach(var item in data.items){
        Console.WriteLine(item.uuid);
    }

});
```
By default harvesting is done in parallel (4 threads), but this can be disabled if serial requests are required.
Note: The harvester uses dynamic typing, so please refer to the API reference for the JSON structure.

### Getting Changes
The Pure API can also return all a list of changes since a given date. 
This is useful to process incremental updates after having harvested the initial dataset (as opposed to downloading everything from scratch each time).

Each change is a tuple (changeType, uuid, familySystemName, version).
Every time content is updated in Pure, the version is incremented and the operation `changeType` is tracked.
When you pass a date to this endpoint, it will return a sequence of changes in chronological order.

Usage is simple:
```csharp
harvester.GetChanges(new DateTime(2017, 06, 15), data => {
    // Print the number of changes in this batch:
    Console.WriteLine(data.count);
    // Print each tuple.
    foreach(var item in data.items){
        Console.WriteLine($"{item.changeType}: {item.uuid} {item.familySystemName} v.{item.version}");
    }
});
```
Use this by passing an `Action<T>` callback to `GetChanges` and sift through changes as desired.
You may want to pass a queue to gather all the changes. 
Then afterwards it is just a matter of making a serial request (using familySystemName and uuid) for each item in the queue.