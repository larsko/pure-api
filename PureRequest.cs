using System;
using RestSharp;

namespace PureAPI
{
    /// <summary>
    /// Pure request.
    /// </summary>
	public class PureRequest
	{
		/// <summary>
		/// API client is hardwired for JSON - change this if deserializing XML
		/// </summary>
		const string HTTP_ACCEPT = "application/json";

        /// <summary>
        /// Gets the RestSharp request.
        /// </summary>
        /// <value>The REST equest.</value>
        internal RestRequest RestSharpRequest { get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PureAPI.PureRequest"/> class.
        /// </summary>
        /// <param name="resource">Resource.</param>
		public PureRequest(string resource)
		{
            RestSharpRequest = new RestRequest(resource);
            RestSharpRequest.AddHeader("Accept", HTTP_ACCEPT);
		}

        /// <summary>
        /// Adds the parameter the request.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="val">Value.</param>
        public void AddParameter(string key, object val){
            this.RestSharpRequest.AddParameter(key,val);
        }

        public void SetParameter(string key, object val){
            this.RestSharpRequest.Parameters.Find(x=>x.Name==key).Value=val;
        }
	}
}
