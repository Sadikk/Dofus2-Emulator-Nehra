using Stump.Core.Threading;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Handler;
using Stump.Server.BaseServer.Network;
using System.Linq;
using Stump.Server.WorldServer.Handlers;
using System.Collections.Generic;
namespace Stump.Server.WorldServer.Core.Network
{
	public class WorldPacketHandler : HandlerManager<WorldPacketHandler, WorldHandlerAttribute, WorldHandlerContainer, WorldClient>
	{
        public override void Dispatch(WorldClient client, Stump.DofusProtocol.Messages.Message message)
        {
            if (message is BasicPingMessage)
            {
                client.Send(new BasicPongMessage((message as BasicPingMessage).quiet));
            }
            else
            {
                List<MessageHandler> Handlers;
                if (this.m_handlers.TryGetValue(message.MessageId, out Handlers))
                {
                    if (!Handlers.Any(entry => entry.Container.CanHandleMessage(client, message.MessageId)))
                    {
                        this.m_logger.Warn(string.Concat(new object[]
						{
							client,
							" tried to send ",
							message,
							" but predicate didn't success"
						}));
                    }
                    else
                    {
                        IContextHandler contextHandler = this.GetContextHandler(Handlers.First().Attribute, client, message);
                        if (contextHandler != null)
                        {
                            foreach (var handler in Handlers)
                            {
                                contextHandler.AddMessage(new HandledMessage<WorldClient>(handler.Action, client, message));
                            }
                        }
                    }
                }
                else
                {
                    this.m_logger.Debug("Received Unknown packet : " + message);
                }
            }
        }
		public IContextHandler GetContextHandler(WorldHandlerAttribute attr, WorldClient client, Stump.DofusProtocol.Messages.Message message)
		{
			IContextHandler result;
			if (!attr.IsGamePacket)
			{
				result = ServerBase<WorldServer>.Instance.IOTaskPool;
			}
			else
			{
				if (client.Character == null || client.Account == null)
				{
					this.m_logger.Warn<WorldClient, Stump.DofusProtocol.Messages.Message>("Client {0} sent {1} before being logged", client, message);
					client.Disconnect();
					result = null;
				}
				else
				{
					if (client.Character.Area == null)
					{
						this.m_logger.Warn<WorldClient, Stump.DofusProtocol.Messages.Message>("Client {0} sent {1} while not in world", client, message);
						client.Disconnect();
						result = null;
					}
					else
					{
						result = client.Character.Area;
					}
				}
			}
			return result;
		}
	}
}
