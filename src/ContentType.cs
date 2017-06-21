using System;
using System.Collections.Generic;
namespace PureAPI
{
	/// <summary>
	/// Available Pure API content types.
	/// </summary>
	public static class ContentType
	{
		/// <summary>
		/// The activities.
		/// </summary>
		public static string Activities = "activities";
		/// <summary>
		/// The applications.
		/// </summary>
		public static string Applications = "applications";
		/// <summary>
		/// The awards.
		/// </summary>
		public static string Awards = "awards";
		/// <summary>
		/// The changes.
		/// </summary>
		public static string Changes = "changes";
		/// <summary>
		/// The classification schemes.
		/// </summary>
		public static string ClassificationSchemes = "classification-schemes";
		/// <summary>
		/// The concepts.
		/// </summary>
		public static string concepts = "concepts";
		/// <summary>
		/// The courses.
		/// </summary>
		public static string courses = "courses";
		/// <summary>
		/// The curricula vitae.
		/// </summary>
		public static string CurriculaVitae = "curricula-vitae";
		/// <summary>
		/// The datasets.
		/// </summary>
		public static string Datasets = "datasets";
		/// <summary>
		/// The equipments.
		/// </summary>
		public static string Equipments = "equipments";
		/// <summary>
		/// The events.
		/// </summary>
		public static string Events = "events";
		/// <summary>
		/// The external organisations.
		/// </summary>
		public static string ExternalOrganisations = "external-organisations";
		/// <summary>
		/// The external persons.
		/// </summary>
		public static string ExternalPersons = "external-persons";
		/// <summary>
		/// The fingerprints.
		/// </summary>
		public static string Fingerprints = "fingerprints";
		/// <summary>
		/// The impacts.
		/// </summary>
		public static string Impacts = "impacts";
		/// <summary>
		/// The journals.
		/// </summary>
		public static string Journals = "journals";
		/// <summary>
		/// The organisational units.
		/// </summary>
		public static string OrganisationalUnits = "organisational-units";
		/// <summary>
		/// The persons.
		/// </summary>
		public static string Persons = "persons";
		/// <summary>
		/// The press media.
		/// </summary>
		public static string PressMedia = "press-media";
		/// <summary>
		/// The prizes.
		/// </summary>
		public static string Prizes = "prizes";
		/// <summary>
		/// The projects.
		/// </summary>
		public static string Projects = "projects";
		/// <summary>
		/// The publishers.
		/// </summary>
		public static string Publishers = "publishers";
		/// <summary>
		/// The research outputs.
		/// </summary>
		public static string ResearchOutputs = "research-outputs";
		/// <summary>
		/// The semantic groups.
		/// </summary>
		public static string SemanticGroups = "semantic-groups";
		/// <summary>
		/// The thesauri.
		/// </summary>
		public static string Thesauri = "thesauri";

		/// <summary>
		/// Returns the endpoint for the specified family system name.
		/// </summary>
		/// <remarks>
		/// The changes endpoint yields family system names that do not match
		/// the name of the corresponding API endpoints. This method helps fix that.
		/// </remarks>
		/// <returns>The family.</returns>
		/// <param name="familySystemName">Family system name.</param>
		public static string FromFamily(string familySystemName){

			if (!familyToEndpoint.ContainsKey(familySystemName))
				return "unknown";

			return familyToEndpoint[familySystemName];
		}

		/// <summary>
		/// Maps family system names to endpoints.
		/// </summary>
		static Dictionary<string, string> familyToEndpoint = new Dictionary<string, string>()
		{
			{"Person", Persons},
			{"Organisation", OrganisationalUnits},
			{"Journal", Journals},
			{"Event", Events},
			{"Publisher", Publishers},
			{"ExternalPerson", ExternalPersons},
			{"ResearchOutput", ResearchOutputs}
		};

	}

}
