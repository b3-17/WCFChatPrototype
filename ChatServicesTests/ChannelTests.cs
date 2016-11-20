using NUnit.Framework;
using System;
using System.Linq;
using Services;
using Models;

namespace ChatServicesTests
{
	[TestFixture()]
	public class ChannelTests
	{
		[TestFixtureSetUp]
		public void SetUp()
		{ 
		
		}

		[TestFixtureTearDown]
		public void CleanUp()
		{ 
		
		}

		[Test()]
		public void CreateChannel()
		{
			IChatService service = new ChatService();
			Channel channel = new Channel { ChannelName = "test channel" };
			Assert.AreEqual(0, service.Channels.Count(), "the channel list should be empty on start up");
			service.CreateChannel(channel);

			Assert.IsTrue(1, service.Channels.Count(), "the chat channel list was not updated");
			Assert.AreEqual("test channel", service.Channels.FirstOrDefault().ChannelName, "the channel name was incorrect");
		}
	}
}
