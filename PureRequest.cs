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
		/// Adds the parameter.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="val">Value.</param>
		/// <param name="useHeader">If set to <c>true</c> add to headers.</param>
        void AddParameter(string key, object val, bool useHeader = false){
			if(useHeader)
				this.RestSharpRequest.AddHeader(key, val.ToString());
			else
            	this.RestSharpRequest.AddParameter(key,val);
        }

		/// <summary>
		/// Sets the parameter.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="val">Value.</param>
		/// <param name="useHeader">If set to <c>true</c> add to headers.</param>
        public void SetParameter(string key, object val, bool useHeader = false){
			
			var parameter = RestSharpRequest.Parameters.Find(x => x.Name == key);

			if (parameter == null)
				AddParameter(key, val, useHeader);
			else
				parameter.Value = val;
        }
	}
}
