using System.ServiceModel;
using System.ServiceModel.Web;

using Models;

namespace ChatRESTServices.Services
{
    [ServiceContract]
    public interface IChatService
    {
		[OperationContract]
		[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Channels")]
		void CreateChannel(Channel channel);

		[OperationContract]
		[WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Channels")]
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Channels")]
		string GetAllChannels(Channel channel);
    }
}
