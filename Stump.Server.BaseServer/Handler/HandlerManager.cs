using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace Stump.Server.BaseServer.Handler
{
	public class HandlerManager<THandler, TAttribute, TContainer, TClient> : Singleton<THandler> where THandler : HandlerManager<THandler, TAttribute, TContainer, TClient> where TAttribute : HandlerAttribute where TContainer : IHandlerContainer where TClient : BaseClient
	{
		protected class MessageHandler
		{
			public TContainer Container
			{
				get;
				private set;
			}
			public TAttribute Attribute
			{
				get;
				private set;
			}
			public Action<object, TClient, Message> Action
			{
				get;
				private set;
			}
			public MessageHandler(TContainer container, TAttribute handlerAttribute, Action<object, TClient, Message> action)
			{
				this.Container = container;
				this.Attribute = handlerAttribute;
				this.Action = action;
			}
		}
		protected readonly Logger m_logger = LogManager.GetCurrentClassLogger();
		protected readonly Dictionary<uint, List<HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler>> m_handlers = new Dictionary<uint, List<HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler>>();
		public void RegisterAll(Assembly asm)
		{
			Type[] types = asm.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				this.Register(type);
			}
		}
		public void Register(Type type)
		{
			if (!type.IsAbstract && (type.GetInterfaces().Contains(typeof(TContainer)) || type.IsSubclassOf(typeof(TContainer))))
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
				TContainer handlerContainer;
				try
				{
					handlerContainer = (TContainer)((object)Activator.CreateInstance(type, true));
				}
				catch (Exception ex)
				{
					throw new Exception("Unable to create HandlerContainer " + type.Name + ".\n " + ex.Message);
				}
				MethodInfo[] array = methods;
				for (int i = 0; i < array.Length; i++)
				{
					MethodInfo methodInfo = array[i];
					TAttribute[] array2 = methodInfo.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];
					if (array2 != null && array2.Length != 0)
					{
						try
						{
							if (methodInfo.GetParameters().Count((ParameterInfo entry) => entry.ParameterType.IsSubclassOf(typeof(Message)) || entry.ParameterType == typeof(TClient) || entry.ParameterType.IsSubclassOf(typeof(TClient))) != 2)
							{
								throw new ArgumentException("Incorrect delegate parameters");
							}
							Action<object, TClient, Message> target = methodInfo.CreateDelegate(new Type[]
							{
								typeof(TClient),
								typeof(Message)
							}) as Action<object, TClient, Message>;
							TAttribute[] array3 = array2;
							for (int j = 0; j < array3.Length; j++)
							{
								TAttribute handlerAttribute = array3[j];
								this.RegisterHandler(handlerAttribute.MessageId, handlerContainer, handlerAttribute, target);
							}
						}
						catch (Exception ex)
						{
							string text = type.FullName + "." + methodInfo.Name;
							throw new Exception(string.Concat(new string[]
							{
								"Unable to register PacketHandler ",
								text,
								".\n Make sure its arguments are: ",
								typeof(TClient).FullName,
								", ",
								typeof(Message).FullName,
								".\n",
								ex.Message
							}));
						}
					}
				}
			}
		}
		private void RegisterHandler(uint messageId, TContainer handlerContainer, TAttribute handlerAttribute, Action<object, TClient, Message> target)
		{
			if (!this.m_handlers.ContainsKey(messageId))
			{
				this.m_handlers.Add(messageId, new List<HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler>());
			}
			this.m_handlers[messageId].Add((target != null) ? new HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler(handlerContainer, handlerAttribute, target) : new HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler(handlerContainer, handlerAttribute, null));
		}
		public bool IsRegister(uint messageId)
		{
			return this.m_handlers.ContainsKey(messageId);
		}
		public virtual void Dispatch(TClient client, Message message)
		{
			List<HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler> list;
			if (this.m_handlers.TryGetValue(message.MessageId, out list))
			{
				try
				{
					using (List<HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler handler = enumerator.Current;
							TContainer container = handler.Container;
							if (!container.CanHandleMessage(client, message.MessageId))
							{
								this.m_logger.Warn(string.Concat(new object[]
								{
									client,
									" tried to send ",
									message,
									" but predicate didn't success"
								}));
								break;
							}
							ServerBase.InstanceAsBase.IOTaskPool.AddMessage(delegate
							{
								handler.Action(null, client, message);
							});
						}
					}
					return;
				}
				catch (Exception arg)
				{
					this.m_logger.Error(string.Format("[Handler : {0}] Force disconnection of client {1} : {2}", message, client, arg));
					client.Disconnect();
					return;
				}
			}
			this.m_logger.Debug("Received Unknown packet : " + message);
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("****HandlerManager*****");
			stringBuilder.AppendLine("Available Handlers Count : " + this.m_handlers.Count);
			foreach (KeyValuePair<uint, List<HandlerManager<THandler, TAttribute, TContainer, TClient>.MessageHandler>> current in this.m_handlers)
			{
				stringBuilder.AppendLine(current.Key.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
