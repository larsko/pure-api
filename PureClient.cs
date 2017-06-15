using System;
using RestSharp;
using Newtonsoft.Json;

namespace PureAPI
{
    /// <summary>
    /// Pure client.
    /// </summary>
    public class PureClient
    {
        /// <summary>
        /// The API key used to authenticate.
        /// </summary>
        /// <value>The API key.</value>
        string ApiKey { get; set; }

		/// <summary>
		/// RestSharp client.
		/// </summary>
		RestClient client;

        public PureClient(string baseUrl, string apiKey, string version)
        {
            ApiKey = apiKey;

            string ws = $"/ws/api/{version}/";

            var uri = new Uri(new Uri(baseUrl), ws);
            client = new RestClient(uri);
        }

        /// <summary>
        /// Makes an async REST request and invokes a callback function.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="request">Request.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private void Request<T>(RestRequest request, Action<T> callback)
        {
			//todo: add error handling
			Authenticate(request);
            client.ExecuteAsync(request, response =>
            {
                callback(ConversionHelper.Deserialize<T>(response.Content));
            });
        }

        /// <summary>
        /// Makes a serial REST request and deserializes the response.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="request">Request.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private T Request<T>(RestRequest request, Method method = Method.GET)
        {
            request.Method = method;

			Authenticate(request);
            var res = client.Execute(request);
            return ConversionHelper.Deserialize<T>(res.Content);
        }

        /// <summary>
        /// Requests the resource and returns a dynamic object.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="request">Request.</param>
        public dynamic Execute(PureRequest request){
            return Request<dynamic>(request);
        }

        /// <summary>
        /// Requests the resource and serializes the response to the type.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="request">Request.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T Request<T>(PureRequest request){
            return Request<T>(request.RestSharpRequest);
        }

        /// <summary>
        /// Authenticates the specified request, either header or query string.
        /// </summary>
        /// <returns>The authenticated request.</returns>
        /// <param name="request">Request.</param>
        void Authenticate(RestRequest request)
        {
            if(!request.Parameters.Exists(x => x.Name == "api-key"))
            request.AddHeader("api-key", ApiKey);
        }
    }

    /// <summary>
    /// Conversion helper.
    /// </summary>
	public static class ConversionHelper
	{
        /// <summary>
        /// Deserialize the specified data.
        /// </summary>
        /// <returns>The deserialize.</returns>
        /// <param name="data">Data.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Deserialize<T>(string data)
		{
			// RestSharp deserializer does not behave well, using JSON.NET
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
		}
	}
}