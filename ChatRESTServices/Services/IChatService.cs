using System.ServiceModel;
using System.ServiceModel.Web;

using Models;

namespace ChatRESTServices.Services
{
    [ServiceContract]
    public interface IChatService
    {
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Channels")]
		void CreateChannel(Channel channel);

		[OperationContract]
		[WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Channels")]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Channels")]
		string GetAllChannels();

		[OperationContract]
		[WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "ChannelSearch?channelName={channelName}")]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ChannelSearch?channelName={channelName}")]
		string GetChannelByName(string channelName);

		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
		           UriTemplate = "Subscribe?channel={channel}&user={user}")]
		void SubscribeUserToChannel(Channel channel, ChatUser user);

		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "UnSubscribe")]
		void UnsubscribeUserFromChannel(Channel channel, ChatUser user);
    }
}
