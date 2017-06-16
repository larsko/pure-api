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

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PureAPI.PureClient"/> class.
		/// </summary>
		/// <param name="baseUrl">Base URL.</param>
		/// <param name="apiKey">API key.</param>
		/// <param name="version">Version.</param>
        public PureClient(string baseUrl, string apiKey, string version)
        {
            ApiKey = apiKey;

            string ws = $"/ws/api/{version}/";

            var uri = new Uri(new Uri(baseUrl), ws);
            client = new RestClient(uri);
        }

        /// <summary>
        /// Makes a serial REST request and deserializes the response.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="request">Request.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		private T Request<T>(PureRequest request, Method method = Method.GET)
        {
			request.RestSharpRequest.Method = method;

			Authenticate(request);
			var res = client.Execute(request.RestSharpRequest);
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
		/// Authenticate the specified request.
		/// </summary>
		/// <returns>The authenticate.</returns>
		/// <param name="request">Request.</param>
		void Authenticate(PureRequest request)
        {
			request.SetParameter("api-key", ApiKey, true);
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