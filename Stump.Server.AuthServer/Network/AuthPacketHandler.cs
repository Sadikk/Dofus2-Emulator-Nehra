using Stump.DofusProtocol.Messages;
using Stump.Server.AuthServer.Handlers;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Handler;
using Stump.Server.BaseServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.AuthServer.Network
{
    public class AuthPacketHandler : HandlerManager<AuthPacketHandler, AuthHandlerAttribute, AuthHandlerContainer, AuthClient>
    {
        public override void Dispatch(AuthClient client, Message message)
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
                        foreach (var handler in Handlers)
                        {
                            AuthServer.Instance.IOTaskPool.AddMessage(new HandledMessage<AuthClient>(handler.Action, client, message));
                        }
                    }
                }
                else
                {
                    this.m_logger.Debug("Received Unknown packet : " + message);
                }
            }
        }
    }
}
