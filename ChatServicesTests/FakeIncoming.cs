using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ChatServicesTests
{
	public class FakeIncoming //: IIncomingWebRequestContext
	{
		public string Accept
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public long ContentLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ContentType
		{
			get
			{
				return "hello!";
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Method
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/*public System.UriTemplateMatch UriTemplateMatch
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}*/
		public string UserAgent
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
