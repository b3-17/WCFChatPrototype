using System.Collections.Generic;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using System.Linq;
using Models;

namespace ChatRESTServices.Services
{
	public class ChatService : IChatService
	{
		public List<Channel> Channels { get; set; }
		private IWebOperationContext context { get; set; }

		public ChatService(IWebOperationContext webContext)
		{
			this.Channels = new List<Channel>();
			this.context = webContext;
		}

		public ChatService()
			:this(new WebOperationContextWrapper(WebOperationContext.Current))
		{

		}

		public void CreateChannel(Channel channel)
		{
			this.Channels.Add(channel);
			this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Created;
		}

		public string GetAllChannels(Channel channel)
		{
			JavaScriptSerializer serialiser = new JavaScriptSerializer();
			return serialiser.Serialize(this.Channels);
		}
	}
}