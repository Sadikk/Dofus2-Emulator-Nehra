using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Enums.HomeMade;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Shield
{
    [EffectHandler(EffectsEnum.Effect_Shield)]
    class ShieldBuff : SpellEffectHandler
    {
        public ShieldBuff(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }

        public override bool Apply()
        {
            bool result;
            foreach (FightActor current in base.GetAffectedActors())
            {
                EffectInteger effectInteger = base.GenerateEffect();
                if (effectInteger == null)
                {
                    result = false;
                    return result;
                }
                base.AddStatBuff(current, effectInteger.Value, PlayerFields.Shield, true, CustomActionsEnum.Action_Shield_Buff);
            }
            result = true;
            return result;
        }
    }
}
