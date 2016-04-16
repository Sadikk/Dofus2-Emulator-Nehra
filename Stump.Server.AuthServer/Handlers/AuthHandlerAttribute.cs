using Stump.Server.BaseServer.Handler;
using System;
namespace Stump.Server.AuthServer.Handlers
{
	public class AuthHandlerAttribute : HandlerAttribute
	{
		public AuthHandlerAttribute(uint messageId) : base(messageId)
		{
		}
	}
}
