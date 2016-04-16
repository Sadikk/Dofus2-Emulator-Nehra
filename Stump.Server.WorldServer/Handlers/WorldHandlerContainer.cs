using Stump.Server.BaseServer.Handler;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Handlers
{
	public abstract class WorldHandlerContainer : IHandlerContainer
	{
		protected System.Collections.Generic.Dictionary<uint, System.Predicate<WorldClient>> Predicates = new System.Collections.Generic.Dictionary<uint, System.Predicate<WorldClient>>();
		protected void Predicate(uint messageId, System.Predicate<WorldClient> predicate)
		{
			this.Predicates.Add(messageId, predicate);
		}
		public bool CanHandleMessage(BaseClient client, uint messageId)
		{
			return !this.Predicates.ContainsKey(messageId) || (client is WorldClient && this.Predicates[messageId](client as WorldClient));
		}
	}
}
