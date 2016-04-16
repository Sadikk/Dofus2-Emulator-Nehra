using Stump.DofusProtocol.Enums;

namespace Stump.Server.WorldServer.Game.Items.Player.Custom
{
	public class ItemHasEffectAttribute : System.Attribute
	{
		public EffectsEnum Effect
		{
			get;
			set;
		}
		public ItemHasEffectAttribute(EffectsEnum effect)
		{
			this.Effect = effect;
		}
	}
}
