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
    [EffectHandler(EffectsEnum.Effect_SubState)]
    public class SubState : SpellEffectHandler
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public SubState(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
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
                    SubState.logger.Error<short>("Spell state {0} not found", base.Dice.Value);
                    result = false;
                    return result;
                }
                base.RemoveStateBuff(current, spellState.Id);
            }
            result = true;
            return result;
        }
    }
}
