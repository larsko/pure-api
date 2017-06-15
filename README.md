# pure-api
This is light-weight Pure API wrapper written in C#.

## Dependencies
- .NET 4.6+
- Newtonsoft.Json
- RestSharp

## Usage
Create an API key for the Pure API. You will need to pass the API key for every request. It is recommended to store the configuration values below to your `App.Config` file.
```csharp
string baseUrl = "<PURE_URL>"; // do not include /ws
string apiKey = "<PURE_API_KEY>";
string apiVersion = "59"; // API version to use
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

The result returned is a `dynamic` type. 
```csharp
var result = client.Execute(request);
int count = result.count; // runtime type-checking
```

It is possible to deserialize to a concrete type - simply pass the type parameter `T`:
```csharp
var result = client.Execute<T>(request);
```
Tip: http://jsonutils.com/ is a great class generation tool for JSON that can help save you some time.

### Harvesting Data and Getting Changes
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
Note: The harvester uses dynamic typing, so please refer to the API reference for the JSON structure.

The Pure API can also return all a list of changes since a given date. 
This is useful to process incremental updates after having harvested the initial dataset (as opposed to downloading everything from scratch each time).

Usage is simple:
```csharp
harvester.GetChanges<dynamic>(new DateTime(2017, 06, 15), data => {
    // Print the number of changes in this batch:
    Console.WriteLine(data.count)
});
```

