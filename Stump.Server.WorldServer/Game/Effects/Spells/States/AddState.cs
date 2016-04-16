using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.States
{
	[EffectHandler(EffectsEnum.Effect_AddState)]
	public class AddState : SpellEffectHandler
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public AddState(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			bool result;
			foreach (FightActor current in base.GetAffectedActors())
			{
				SpellState spellState = Singleton<SpellManager>.Instance.GetSpellState((uint)base.Dice.Value);
				if (spellState == null)
				{
					AddState.logger.Error<short>("Spell state {0} not found", base.Dice.Value);
					result = false;
					return result;
				}
				base.AddStateBuff(current, true, spellState);
			}
			result = true;
			return result;
		}
	}
}
