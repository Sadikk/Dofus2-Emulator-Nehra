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
    [EffectHandler(EffectsEnum.Effect_AddPercentShield)]
    class ShieldPercentBuff : SpellEffectHandler
    {
        public ShieldPercentBuff(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
        {
        }
        public override bool Apply()
        {
            bool result;
            //Following code shouldn't be there.
            //If there is any reason to this code : remove commentary.
            //switch (Spell.Id)
            //{
            //    case (int)SpellIdEnum.Trance:
            //        base.Targets = SpellTargetType.ALLY_ALL;
            //        break;
            //}
            foreach (FightActor current in base.GetAffectedActors())
            {
                EffectInteger effectInteger = base.GenerateEffect();
                if (effectInteger == null || this.Effect.Duration <= 0)
                {
                    return false;
                }

                double num = (double)Caster.Stats.Health.TotalMax * ((double)effectInteger.Value / 100.0);
                base.AddStatBuff(current, (short)num, PlayerFields.Shield, true, CustomActionsEnum.Action_Shield_Buff);
            }
            result = true;
            return result;
        }
    }
}
