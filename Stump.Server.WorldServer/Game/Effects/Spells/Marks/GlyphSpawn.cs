using System.Drawing;
using NLog;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Marks
{
	[EffectHandler(EffectsEnum.Effect_Glyph_402), EffectHandler(EffectsEnum.Effect_Glyph)]
	public class GlyphSpawn : SpellEffectHandler
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public GlyphSpawn(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			Spell spell = new Spell((int)base.Dice.DiceNum, (byte)base.Dice.DiceFace);
			bool result;
		    if (spell.Template == null || !spell.ByLevel.ContainsKey((int) base.Dice.DiceFace))
		    {
		        GlyphSpawn.logger.Error<short, short, int>(
		            "Cannot find glyph spell id = {0}, level = {1}. Casted Spell = {2}", base.Dice.DiceNum, base.Dice.DiceFace,
		            base.Spell.Id);
		        result = false;
		    }
		    else
		    {
		        Glyph trigger = (base.EffectZone.ShapeType == SpellShapeEnum.Q)
		            ? new Glyph((short) base.Fight.PopNextTriggerId(), base.Caster, base.Spell, base.TargetedCell, base.Dice,
		                spell,
		                GameActionMarkCellsTypeEnum.CELLS_CROSS, (byte) this.Effect.ZoneSize,
		                GlyphSpawn.GetGlyphColorBySpell(base.Spell))
		            : new Glyph((short) base.Fight.PopNextTriggerId(), base.Caster, base.Spell, base.TargetedCell, base.Dice,
		                spell,
		                (byte) this.Effect.ZoneSize, GlyphSpawn.GetGlyphColorBySpell(base.Spell));

		        base.Fight.AddTrigger(trigger);
		        result = true;
		    }
		    return result;
		}
		private static Color GetGlyphColorBySpell(Spell spell)
		{
			int arg_06_0 = spell.Id;
			return Color.Red;
		}
	}
}
