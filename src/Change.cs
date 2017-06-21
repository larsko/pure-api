using System;
namespace PureAPI
{
	/// <summary>
	/// A content change operation in Pure.
	/// </summary>
	public class Change
	{
		/// <summary>
		/// The type of change, i.e. addition, update or deletion
		/// </summary>
		/// <value>The type of the change.</value>
		public string ChangeType { get; set; }
		/// <summary>
		/// UUID of the affected content.
		/// </summary>
		/// <value>The UUID.</value>
		public string UUID { get; set; }
		/// <summary>
		/// The endpoint of the associated content.
		/// </summary>
		/// <value>The endpoint.</value>
		public string Endpoint { get; set; }
		/// <summary>
		/// The version of the content.
		/// </summary>
		/// <value>The version.</value>
		public short Version { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PureAPI.Change"/> class.
		/// </summary>
		/// <param name="changeType">Change type.</param>
		/// <param name="uuid">UUID.</param>
		/// <param name="family">Family.</param>
		/// <param name="version">Version.</param>
		public Change(string changeType, string uuid, string family, short version)
		{
			ChangeType = changeType;
			UUID = uuid;
			Endpoint = ContentType.FromFamily(family); // change the family to endpoint.
			Version = version;
		}

		/// <summary>
		/// Gets the get URL of the content. Use this to request the content.
		/// </summary>
		/// <value>The resource URL.</value>
		public string ResourceUrl => $"{Endpoint}/{UUID}";
	}
}
