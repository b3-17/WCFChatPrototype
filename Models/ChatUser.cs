using System;
using System.Runtime.Serialization;

namespace Models
{
	[DataContract(Namespace = "")]
	public class ChatUser
	{
		[DataMember]
		public string UserName { get; set; }
	}
}
