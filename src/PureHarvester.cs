﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
namespace PureAPI
{
	/// <summary>
	/// Pure API harvester.
	/// </summary>
	public class PureHarvester
	{
		/// <summary>
		/// The Pure API client.
		/// </summary>
		PureClient client;

		public PureHarvester(PureClient client)
		{
			this.client = client;
		}


		/// <summary>
		/// Harvests content in parallel.
		/// </summary>
		/// <remarks>
		/// Assumes dynamic typing.
		/// </remarks>
		/// <param name="endpoint">Endpoint.</param>
		/// <param name="callback">Callback.</param>
		/// <param name="rendering">Rendering.</param>
		/// <param name="pageSize">Page size.</param>
		public void Harvest(string endpoint, Action<dynamic> callback, bool parallel = true, string rendering = "", int pageSize = 25)
		{
			Harvest<dynamic>(endpoint, callback, parallel, rendering, pageSize);
		}

		/// <summary>
		/// Harvests content in parallel.
		/// </summary>
		/// <param name="endpoint">Endpoint.</param>
		/// <param name="callback">Callback.</param>
		/// <param name="rendering">Rendering.</param>
		/// <param name="pageSize">Page size.</param>
		public void Harvest<T>(string endpoint, Action<T> callback, bool useParallelism = true, string rendering = "", int pageSize = 25)
		{
			int numPages = client.Execute(new PureRequest(endpoint)).count / pageSize;
			var pages = Enumerable.Range(0, numPages);

			int threads = useParallelism ? 4 : 1;
			var options = new ParallelOptions { MaxDegreeOfParallelism = threads};

			Parallel.ForEach(pages, options, page =>
			{
				var request = new PureRequest(endpoint);
				request.SetParameter("page", page);
				request.SetParameter("pageSize", pageSize);

				callback(client.Execute(request));
			});
		}

		/// <summary>
		/// Harvests the changes.
		/// </summary>
		/// <param name="changes">Changes.</param>
		/// <param name="callback">Callback.</param>
		public void HarvestChangeset(List<Change> changes, Action<dynamic> callback)
		{
			HarvestChangeset<dynamic>(changes, callback);
		}

		/// <summary>
		/// Harvests the changes.
		/// </summary>
		/// <param name="changes">Changes.</param>
		/// <param name="callback">Callback.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void HarvestChangeset<T>(List<Change> changes, Action<T> callback){
			Parallel.ForEach(changes, change =>
			{
				callback(client.Execute(new PureRequest(change.ResourceUrl)));
			});
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
		public void GetChanges<T>(DateTime date, Action<T> callback)
		{
			string input = date.ToString("yyyy-MM-dd");
			var request = new PureRequest($"changes/{input}");
			bool moreChanges = true;

			while (moreChanges){
				var changes = client.Execute(request);

				if (changes.count > 0)
					callback(changes);

				moreChanges = changes.moreChanges;

				// Pass the resumption token to get next batch of changes
				request = new PureRequest($"changes/{changes.lastId}");
			}
		}

		/// <summary>
		/// Gets the changes.
		/// </summary>
		/// <remarks>
		/// Assumes dynamic typing.
		/// </remarks>
		/// <param name="date">Date.</param>
		/// <param name="callback">Callback.</param>
		public void GetChanges(DateTime date, Action<dynamic> callback)
		{
			GetChanges<dynamic>(date, callback);
		}

		/// <summary>
		/// Returns a queue of changes for given endpoint and operation.
		/// </summary>
		/// <returns>The changes.</returns>
		/// <param name="date">Date.</param>
		/// <param name="contentEndpoint">Endpoint.</param>
		/// <param name="operations">Operations.</param>
		public List<Change> FilterChanges(DateTime date, string contentEndpoint, params string[] operations){

			var result = new Dictionary<string,Change>();

			// change type: CREATE, ADDED [relation], UPDATE

			GetChanges(date, data =>{
				foreach(var item in data.items){

					// Note: changes w/o uuid and version may appear, handle this
					string uuid = $"{item.uuid}";
					string version = $"{item.version}";

					var change = new Change(
						$"{item.changeType}",
						uuid,
						$"{item.familySystemName}",
						version == string.Empty ? -1 : int.Parse(version)
					);

					// can generalize operation to be multiple opps.
					if (change.Endpoint == contentEndpoint)
					{
						if(operations != null && operations.Contains(change.ChangeType)){
							// keep track of unique UUID - no need to download twice
							if(!result.ContainsKey(change.UUID)){
								result.Add(change.UUID, change);
							}
						}
					}
				}
			});

			return result.Select(x => x.Value).ToList();
		}

	}
}
