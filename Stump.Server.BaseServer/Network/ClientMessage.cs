using Stump.DofusProtocol.Messages;
using System;
namespace Stump.Server.BaseServer.Network
{
	public class ClientMessage
	{
		private readonly BaseClient m_client;
		private readonly Message m_message;
		public BaseClient Client
		{
			get
			{
				return this.m_client;
			}
		}
		public Message Message
		{
			get
			{
				return this.m_message;
			}
		}
		public ClientMessage(BaseClient client, Message message)
		{
			this.m_client = client;
			this.m_message = message;
		}
	}
}
