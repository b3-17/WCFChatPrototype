using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
	[DataContract(Namespace = "")]
	public class Channel
	{
		public Channel()
		{
			this.Subscribers = new List<ChatUser>();
		}

		[DataMember]
		public string ChannelName { get; set; }
		[DataMember]
		public List<ChatUser> Subscribers { get; set; }
	}
}
