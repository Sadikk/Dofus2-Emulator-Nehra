using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Handlers.Basic;

namespace Stump.Server.WorldServer.Game.Effects.Handlers.Usables
{
    [EffectHandler(EffectsEnum.Effect_AddHealth)]
    public class RestoreHp : UsableEffectHandler
    {
        public RestoreHp(EffectBase effect, Character target, BasePlayerItem item) : base(effect, target, item)
        {
        }

        public override bool Apply()
        {
            EffectInteger effectInteger =
                this.Effect.GenerateEffect(EffectGenerationContext.Item, EffectGenerationType.Normal) as EffectInteger;
            bool result;
            if (effectInteger == null)
            {
                result = false;
            }
            else
            {
                if (base.Target.Stats.Health.DamageTaken == 0)
                {
                    BasicHandler.SendTextInformationMessage(base.Target.Client,
                        TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 225);
                    result = false;
                }
                else
                {
                    int num = (int) ((long) effectInteger.Value*(long) ((ulong) base.NumberOfUses));
                    if (base.Target.Stats.Health.DamageTaken < num)
                    {
                        num = base.Target.Stats.Health.DamageTaken;
                    }
                    base.UsedItems = (uint) System.Math.Ceiling((double) num/(double) effectInteger.Value);
                    base.Target.Stats.Health.DamageTaken -= num;
                    base.Target.RefreshStats();
                    BasicHandler.SendTextInformationMessage(base.Target.Client,
                        TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 1, new object[]
                        {
                            num
                        });
                    base.Target.PlayEmote(EmotesEnum.EMOTE_MANGER);
                    result = true;
                }
            }
            return result;
        }
    }
}
