using NLog;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Basic;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Marks
{
	[EffectHandler(EffectsEnum.Effect_Trap)]
	public class TrapSpawn : SpellEffectHandler
	{
		private readonly Logger logger = LogManager.GetCurrentClassLogger();
		public TrapSpawn(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			Spell spell = new Spell((int)base.Dice.DiceNum, (byte)base.Dice.DiceFace);
			bool result;
			if (spell.Template == null || !spell.ByLevel.ContainsKey((int)base.Dice.DiceFace))
			{
				this.logger.Error<short, short, int>("Cannot find trap spell id = {0}, level = {1}. Casted Spell = {2}", base.Dice.DiceNum, base.Dice.DiceFace, base.Spell.Id);
				result = false;
			}
			else
			{
			    if (base.Fight.GetTriggers(base.TargetedCell).Length > 0)
			    {
			        if (base.Caster is CharacterFighter)
			        {
			            BasicHandler.SendTextInformationMessage((base.Caster as CharacterFighter).Character.Client,
			                TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 229);
			        }
			        result = false;
			    }
			    else
			    {
			        Trap trigger = (base.EffectZone.ShapeType == SpellShapeEnum.Q)
			            ? new Trap((short)base.Fight.PopNextTriggerId(), base.Caster, base.Spell, base.TargetedCell, 
                            base.Dice, spell, GameActionMarkCellsTypeEnum.CELLS_CROSS, (byte) this.Effect.ZoneSize)
			            : new Trap((short)base.Fight.PopNextTriggerId(), base.Caster, base.Spell, base.TargetedCell, 
                            base.Dice, spell, (byte) this.Effect.ZoneSize);

			        base.Fight.AddTrigger(trigger);
			        result = true;
			    }
			}
			return result;
		}
		public override bool RequireSilentCast()
		{
			return true;
		}
	}
}
