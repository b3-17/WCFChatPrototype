using System;
using System.Runtime.Serialization;

namespace Models
{
	[DataContract(Namespace = "")]
	public class Channel
	{
		[DataMember]
		public string ChannelName { get; set; }
	}
}
