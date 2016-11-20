using System;
using System.Runtime.Serialization;

namespace Models
{
	[DataContract]
	public class Channel
	{
		[DataMember]
		public string ChannelName { get; set; }
	}
}
