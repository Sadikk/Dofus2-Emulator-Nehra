using Stump.Server.BaseServer.Handler;

namespace Stump.Server.WorldServer.Handlers
{
	public class WorldHandlerAttribute : HandlerAttribute
	{
		public bool ShouldBeLogged
		{
			get;
			set;
		}
		public bool IsGamePacket
		{
			get;
			set;
		}
		public bool IgnorePredicate
		{
			get;
			set;
		}
		public WorldHandlerAttribute(uint messageId) : base(messageId)
		{
			this.IsGamePacket = true;
			this.ShouldBeLogged = true;
		}
		public WorldHandlerAttribute(uint messageId, bool isGamePacket, bool requiresLogin) : base(messageId)
		{
			this.IsGamePacket = isGamePacket;
			this.ShouldBeLogged = requiresLogin;
		}
	}
}
