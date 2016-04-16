using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Game.Effects.Handlers.Usables
{
	public abstract class UsableEffectHandler : EffectHandler
	{
		public Character Target
		{
			get;
			protected set;
		}
		public BasePlayerItem Item
		{
			get;
			protected set;
		}
		public Cell TargetCell
		{
			get;
			set;
		}
		public uint NumberOfUses
		{
			get;
			set;
		}
		public uint UsedItems
		{
			get;
			protected set;
		}
		protected UsableEffectHandler(EffectBase effect, Character target, BasePlayerItem item) : base(effect)
		{
			this.Target = target;
			this.Item = item;
			this.NumberOfUses = 1u;
		}
	}
}
