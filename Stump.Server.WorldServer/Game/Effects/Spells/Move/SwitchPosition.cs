using System.Linq;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
	[EffectHandler(EffectsEnum.Effect_SwitchPosition)]
	public class SwitchPosition : SpellEffectHandler
	{
		public SwitchPosition(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			FightActor fightActor = base.GetAffectedActors().FirstOrDefault<FightActor>();
			if (fightActor != null)
			{
				base.Caster.ExchangePositions(fightActor);
			}
			return true;
		}
	}
}
