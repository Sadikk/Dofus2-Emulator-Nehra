using System.Drawing;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Marks
{
	[EffectHandler(EffectsEnum.Effect_DamageSharing)]
	public class DamageSharing : SpellEffectHandler
	{
	    public DamageSharing(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical)
	        : base(effect, caster, spell, targetedCell, critical)
	    {
	    }

	    public override bool Apply()
	    {
	        FractionGlyph trigger = new FractionGlyph((short) base.Fight.PopNextTriggerId(), base.Caster, base.Spell,
	            base.TargetedCell, base.Dice, (byte) this.Effect.ZoneSize, Color.White);

	        base.Fight.AddTrigger(trigger);
	        return true;
	    }
	}
}
