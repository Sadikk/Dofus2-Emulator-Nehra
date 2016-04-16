using Stump.Server.BaseServer.Network;
using System;
namespace Stump.Server.BaseServer.Handler
{
	public interface IHandlerContainer
	{
		bool CanHandleMessage(BaseClient client, uint messageId);
	}
}
