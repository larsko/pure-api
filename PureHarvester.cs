using System;
using System.Collections.Generic;
namespace PureAPI
{
    /// <summary>
    /// Pure API harvester.
    /// </summary>
    public class PureHarvester
    {

        PureClient client;

        public PureHarvester(PureClient client)
        {
            this.client = client;
        }

        public void Harvest(string endpoint, Action<dynamic> callback, string rendering = "", int pageSize = 25)
        {
            Harvest<dynamic>(endpoint, callback, rendering, pageSize);
        }

		/// <summary>
		/// Harvests all data from the specified content type.
		/// </summary>
		/// <returns>The harvest.</returns>
		/// <param name="endpoint">Endpoint.</param>
		/// <param name="callback">Callback.</param>
		/// <param name="rendering">Rendering.</param>
		/// <param name="pageSize">Page size.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Harvest<T>(string endpoint, Action<T> callback, string rendering = "", int pageSize = 25){

            int page = 1;

            var request = new PureRequest(endpoint);
			request.AddParameter("pageSize", pageSize);
			request.AddParameter("page", page);

            bool morePages = false;
            do
            {
                var results = client.Execute(request);
                if(results.items.Count > 0)
                    callback(results);

                request.SetParameter("page", page++);

                morePages = (results.items.Count > 0 && 
                             results.navigationLink != null);
                
            } while (morePages);
        }

        /// <summary>
        /// Gets changes from Pure since a given date.
        /// </summary>
        /// <remarks>
        /// This will yield all changes, so any filtering (e.g. family, changeType) 
        /// has to be implemented in the callback function.
        /// </remarks>
        /// <param name="date">Date.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void GetChanges<T>(DateTime date, Action<T> callback){

            string input = date.ToString("yyyy-MM-dd");
            var request = new PureRequest($"changes/{input}");
            bool moreChanges = false;
            do
            {
                var changes = client.Execute(request);
                if (changes.count > 0)
                    callback(changes);

                moreChanges = changes.moreChanges;

                request = new PureRequest($"changes/{changes.lastId}");

            } while (moreChanges);
        }

    }
}
