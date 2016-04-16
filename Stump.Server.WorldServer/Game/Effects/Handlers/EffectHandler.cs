using Stump.Server.WorldServer.Game.Effects.Instances;

namespace Stump.Server.WorldServer.Game.Effects.Handlers
{
	public abstract class EffectHandler
	{
		public virtual EffectBase Effect
		{
			get;
			private set;
		}
		public double Efficiency
		{
			get;
			set;
		}
		protected EffectHandler(EffectBase effect)
		{
			this.Effect = effect;
			this.Efficiency = 1.0;
		}
		public abstract bool Apply();
	}
}
