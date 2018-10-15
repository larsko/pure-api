﻿using System;
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

		int startPage = 0;

		public PureHarvester(PureClient client, int startPage = 0)
		{
			this.client = client;
			this.startPage = startPage;
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
		public void Harvest(string endpoint, Action<dynamic> callback, bool parallel = true, string rendering = "", string fields = "", int pageSize = 50)
		{
			Harvest<dynamic>(endpoint, callback, parallel, rendering, fields, pageSize);
		}

		/// <summary>
		/// Harvests content in parallel.
		/// </summary>
		/// <param name="endpoint">Endpoint.</param>
		/// <param name="callback">Callback.</param>
		/// <param name="rendering">Rendering.</param>
		/// <param name="pageSize">Page size.</param>
		public void Harvest<T>(string endpoint, Action<T> callback, bool useParallelism = true, string rendering = "", string fields = "", int pageSize = 50, int workers = 8)
		{
			int numPages = client.Execute(new PureRequest(endpoint)).count / pageSize;
			var pages = Enumerable.Range(0, numPages).Skip(startPage);

			int threads = useParallelism ? workers : 1;
			var options = new ParallelOptions { MaxDegreeOfParallelism = threads};

			//foreach(var page in pages)
			Parallel.ForEach(pages, options, page =>
			{
				var request = new PureRequest(endpoint);
				request.SetParameter("page", page);
				request.SetParameter("pageSize", pageSize);

				if (!string.IsNullOrEmpty(rendering))
					request.SetParameter("rendering", rendering);

				if(!string.IsNullOrEmpty(fields)) 
					request.SetParameter("fields", fields);
				try{
				callback(client.Execute(request));
				} catch (Exception ex){
					// handle request exception here.
				}
			}
			);

		}

		/// <summary>
		/// Harvests the changes.
		/// </summary>
		/// <param name="changes">Changes.</param>
		/// <param name="callback">Callback.</param>
		public void HarvestChangeset(List<Change> changes, Action<dynamic> callback, string rendering = "")
		{
			HarvestChangeset<dynamic>(changes, callback, rendering);
		}

		/// <summary>
		/// Harvests the changes.
		/// </summary>
		/// <param name="changes">Changes.</param>
		/// <param name="callback">Callback.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void HarvestChangeset<T>(List<Change> changes, Action<T> callback, string rendering = ""){
			Parallel.ForEach(changes, change =>
			{
				try {
					var request = new PureRequest($"{change.ResourceUrl}");
					if(!string.IsNullOrEmpty(rendering))
						request.SetParameter("rendering",rendering);
					callback(client.Execute(request));
				} catch(Exception ex){
					// handle exception here...
				}
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
		public List<Change> FilterChanges(DateTime date, string contentEndpoint = "", params string[] operations){

			var result = new Dictionary<string,Change>();

			// change type: CREATE, ADDED [relation], UPDATE

			GetChanges(date, data =>
			{
				foreach (var item in data.items)
				{

					// Note: changes w/o uuid and version may appear, handle this
					string uuid = $"{item.uuid}";
					string version = $"{item.version}";

					var change = new Change(
						$"{item.changeType}",
						uuid,
						$"{item.familySystemName}",
						version == string.Empty ? -1 : int.Parse(version)
					);

					bool toAdd = true;
					// can generalize to multiple endpoints
					if (contentEndpoint != string.Empty && change.Endpoint != contentEndpoint)
						toAdd = false;

					if (operations != null &&
					   operations.Length > 0 &&
					   !operations.Contains(change.ChangeType))
						toAdd = false;

					// keep track of unique UUID - no need to download twice
					if (toAdd && !result.ContainsKey(change.UUID))
					{
						result.Add(change.UUID, change);
					}
					else
					{
						// do nothing
					}

				}
			});

			return result.Select(x => x.Value).ToList();
		}

	}
}
