using System.Collections.Generic;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using System.Linq;
using Models;
using System;

namespace ChatRESTServices.Services
{
	public class ChatService : IChatService
	{
		public List<Channel> Channels { get; set; }
		private IWebOperationContext context { get; set; }
		private JavaScriptSerializer serialiser { get; set; }

		public ChatService(IWebOperationContext webContext)
		{
			this.Channels = new List<Channel>();
			this.context = webContext;
			this.serialiser = new JavaScriptSerializer();
		}

		public ChatService()
			: this(new WebOperationContextWrapper(WebOperationContext.Current))
		{

		}

		public void CreateChannel(Channel channel)
		{
			if (this.Channels.Count(x => x.ChannelName == channel.ChannelName) == 0)
			{
				channel.Subscribers = new List<ChatUser>();
				this.Channels.Add(channel);
				this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Created;
			}
			else
				this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
		}

		public string GetAllChannels()
		{
			this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
			return this.serialiser.Serialize(this.Channels);
		}

		public string GetChannelByName(string channelName)
		{
			this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
			return this.serialiser.Serialize(this.Channels.Where(x => x.ChannelName == channelName));
		}

		public void SubscribeUserToChannel(Channel channel, ChatUser user)
		{
			if (this.Channels.Count(x => x.ChannelName == channel.ChannelName) == 0)
				this.Channels.Add(channel);

			if (this.Channels.FirstOrDefault(x => x.ChannelName == channel.ChannelName).Subscribers.Count(x => x.UserName == user.UserName) == 0)
			{
				this.Channels.FirstOrDefault(x => x.ChannelName == channel.ChannelName).Subscribers.Add(user);
				this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
			}
			else
				this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Conflict;
		}

		public void UnsubscribeUserFromChannel(Channel channel, ChatUser user)
		{
			if (this.Channels.Count(x => x.ChannelName == channel.ChannelName) > 0)
			{
				Channel targetChannel = this.Channels.FirstOrDefault(x => x.ChannelName == channel.ChannelName);

				if (targetChannel.Subscribers.Count(x => x.UserName == user.UserName) > 0)
				{
					ChatUser unsubUser = targetChannel.Subscribers.FirstOrDefault(x => x.UserName == user.UserName);
					targetChannel.Subscribers.Remove(unsubUser);
				}
			}
			this.context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
		}
	}
}