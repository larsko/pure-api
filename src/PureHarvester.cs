﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
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

	}
}
