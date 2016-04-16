using Stump.DofusProtocol.Enums;

namespace Stump.Server.WorldServer.Game.Effects.Handlers
{
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
	public class EffectHandlerAttribute : System.Attribute
	{
		public EffectsEnum Effect
		{
			get;
			private set;
		}
		public EffectHandlerAttribute(EffectsEnum effect)
		{
			this.Effect = effect;
		}
	}
}
