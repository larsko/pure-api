using System;
using System.Collections.Generic;
namespace PureAPI.Model
{
	public class ResearchOutput
	{

		string UUID { get; set; }
		string Title { get; set; }
		bool PeerReview { get; set; }
		int NumberOfPages { get; set; }
		string ManagingOrganisationalUnit { get; set; }
		bool Confidential { get; set; }
		string Source { get; set; }
		string SourceId { get; set; }
		bool ExternallyManaged { get; set; }
		Dictionary<string, string> SecondarySources => new Dictionary<string, string>();

		public ResearchOutput()
		{
		}

	}
}
