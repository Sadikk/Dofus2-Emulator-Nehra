using Stump.DofusProtocol.Messages;
using System;
namespace Stump.Server.BaseServer.Network
{
	public interface IPacketReceiver
	{
		void Send(Message message);
	}
}
