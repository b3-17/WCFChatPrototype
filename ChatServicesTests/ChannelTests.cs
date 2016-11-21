using NUnit.Framework;
using Moq;
using System.Linq;
using ChatRESTServices.Services;
using Models;
using System.ServiceModel.Web;

namespace ChatServicesTests
{
	[TestFixture()]
	public class ChannelTests
	{
		private Mock<IWebOperationContext> basicWebContext { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.basicWebContext = new Mock<IWebOperationContext> { DefaultValue = DefaultValue.Mock };
		}

		[TearDown]
		public void CleanUp()
		{
			this.basicWebContext = null;
		}

		[Test()]
		public void CreateChannel()
		{
			System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.NotImplemented;
			this.basicWebContext.SetupSet(x => x.OutgoingResponse.StatusCode = It.IsAny<System.Net.HttpStatusCode>())
					   .Callback((System.Net.HttpStatusCode x) => statusCode = x);

			IChatService service = new ChatService(this.basicWebContext.Object);
			Channel channel = new Channel { ChannelName = "test channel" };
			Assert.AreEqual(0, (service as ChatService).Channels.Count(), "the channel list should be empty on start up");

			service.CreateChannel(channel);
			Assert.AreEqual(1, (service as ChatService).Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("test channel", (service as ChatService).Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(System.Net.HttpStatusCode.Created, statusCode, "the response status was not correct");
		}

		[Test()]
		public void CreateChannelAlreadyExists()
		{
			System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.NotImplemented;
			this.basicWebContext.SetupSet(x => x.OutgoingResponse.StatusCode = It.IsAny<System.Net.HttpStatusCode>())
					   .Callback((System.Net.HttpStatusCode x) => statusCode = x);

			IChatService service = new ChatService(this.basicWebContext.Object);
			Channel existingChannel = new Channel { ChannelName = "existing test channel" };
			(service as ChatService).Channels.Add(existingChannel);
			Assert.AreEqual(1, (service as ChatService).Channels.Count(), "the channel list should be primed to test dupes");

			service.CreateChannel(existingChannel);
			Assert.AreEqual(1, (service as ChatService).Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("existing test channel", (service as ChatService).Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
			Assert.AreEqual(System.Net.HttpStatusCode.Conflict, statusCode, "the response status was not correct");
		}

		[Test()]
		public void GetAllAvailableChannels()
		{
			System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.NotImplemented;
			this.basicWebContext.SetupSet(x => x.OutgoingResponse.StatusCode = It.IsAny<System.Net.HttpStatusCode>())
					   .Callback((System.Net.HttpStatusCode x) => statusCode = x);

			IChatService service = new ChatService(this.basicWebContext.Object);
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 1" });
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 2" });

			var channels  =  service.GetAllChannels();
			Assert.AreEqual(@"[{""ChannelName"":""test channel 1""},{""ChannelName"":""test channel 2""}]", channels.ToString(), "the channels were not returned properly");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, statusCode, "the response status was not correct");
		}

		[Test()]
		public void GetChannelByName()
		{ 
			System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.NotImplemented;
			this.basicWebContext.SetupSet(x => x.OutgoingResponse.StatusCode = It.IsAny<System.Net.HttpStatusCode>())
					   .Callback((System.Net.HttpStatusCode x) => statusCode = x);

			IChatService service = new ChatService(this.basicWebContext.Object);
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 1" });
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 2" });
			(service as ChatService).Channels.Add(new Channel { ChannelName = "test channel 3" });

			var channels = service.GetChannelByName(new Channel { ChannelName = "test channel 2"});
			Assert.AreEqual(@"[{""ChannelName"":""test channel 2""}]", channels.ToString(), "the channels were not returned properly");
			Assert.AreEqual(System.Net.HttpStatusCode.OK, statusCode, "the response status was not correct");
		}
	}
}
