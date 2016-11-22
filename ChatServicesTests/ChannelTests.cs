using NUnit.Framework;
using Moq;
using System.Linq;
using ChatRESTServices.Services;
using Models;
using System.ServiceModel.Web;
using System.Collections.Generic;

namespace ChatServicesTests
{
	[TestFixture()]
	public class ChannelTests
	{
		private Mock<IWebOperationContext> basicWebContext { get; set; }
		private System.Net.HttpStatusCode statusCode { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.basicWebContext = new Mock<IWebOperationContext> { DefaultValue = DefaultValue.Mock };
			this.basicWebContext.SetupSet(x => x.OutgoingResponse.StatusCode = It.IsAny<System.Net.HttpStatusCode>())
					   .Callback((System.Net.HttpStatusCode x) => this.statusCode = x);
		}

		[TearDown]
		public void CleanUp()
		{
			this.basicWebContext = null;
			this.statusCode = System.Net.HttpStatusCode.NotImplemented;
		}

		[Test()]
		public void CreateChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			Channel channel = new Channel { ChannelName = "test channel" };
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "the channel list should be empty on start up");

			service.CreateChannel(channel);
			Assert.AreEqual(1, (service as ChatService).Channels.Count(), "the chat channel list was not updated");
			Assert.IsNotNull((service as ChatService).Channels.FirstOrDefault().Subscribers, "the chat channel subscriber list was not initialised");
			Assert.AreEqual("test channel", (service as ChatService).Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(System.Net.HttpStatusCode.Created, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void CreateChannelAlreadyExists()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			Channel existingChannel = new Channel { ChannelName = "existing test channel" };
			(service as ChatService).Channels.Add(existingChannel);
			Assert.AreEqual(1, (service as ChatService).Channels.Count(), "the channel list should be primed to test dupes");

			service.CreateChannel(existingChannel);
			Assert.AreEqual(1, (service as ChatService).Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("existing test channel", (service as ChatService).Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(System.Net.HttpStatusCode.Conflict, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void GetAllAvailableChannels()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);

			var channels  =  service.GetAllChannels();
			Assert.AreEqual(@"[{""ChannelName"":""test channel 1"",""Subscribers"":[]},{""ChannelName"":""test channel 2"",""Subscribers"":[]}]", 
			                channels.ToString(), "the channels were not returned properly");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void GetChannelByName()
		{ 
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 3" });

			var channels = service.GetChannelByName("test channel 2");
			Assert.AreEqual(@"[{""ChannelName"":""test channel 2"",""Subscribers"":[]}]", channels.ToString(), "the channels were not returned properly");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void SubscribeUserToExistentChannel()
		{ 
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);
			service.SubscribeUserToChannel(new Channel { ChannelName = "test channel 2" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not properly subscribed to");
			Assert.AreEqual("test user 1", (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2")
			                .Subscribers.FirstOrDefault().UserName, "the user name was incorrect");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void SubscribeUserToExistentChannelAlreadySubscribed()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);
			(service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Add(new ChatUser { UserName = "test user 1" });
			service.SubscribeUserToChannel(new Channel { ChannelName = "test channel 2" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not properly subscribed to");
			Assert.AreEqual(System.Net.HttpStatusCode.Conflict, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void SubscribeUserToNonExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "make sure we have no channels currently");

			service.SubscribeUserToChannel(new Channel { ChannelName = "test channel 2" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the channel was not created by default");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void UnsubscribeUserFromExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);
			(service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 1").Subscribers.Add(new ChatUser { UserName = "test user 1" });
			(service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Add(new ChatUser { UserName = "test user 1" });

			service.UnsubscribeUserFromChannel(new Channel { ChannelName = "test channel 1" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(0, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 1").Subscribers.Count(), "the user was not unsubscribed");
			Assert.AreEqual(1, (service as ChatService).Channels.FirstOrDefault(x => x.ChannelName == "test channel 2").Subscribers.Count(), "the user was unsubscribed from the wrong channel!");

			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void UnsubscribeUserFromNonExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "make sure we have no channels currently");

			service.UnsubscribeUserFromChannel(new Channel { ChannelName = "test channel 1" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "make sure we don't create any new channels");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		[Test()]
		public void UnsubscribeUserNotSubscribedFromExistentChannel()
		{
			IChatService service = new ChatService(this.basicWebContext.Object);
			this.SetUpFakeChannels(service);

			service.UnsubscribeUserFromChannel(new Channel { ChannelName = "test channel 1" }, new ChatUser { UserName = "test user 1" });
			Assert.AreEqual(2, (service as ChatService).Channels.Count(), "make sure we don't create any new channels");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, this.statusCode, "the response status was not correct");
		}

		private void SetUpFakeChannels(IChatService service)
		{
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 1" });
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 2" });
		}
	}
}
