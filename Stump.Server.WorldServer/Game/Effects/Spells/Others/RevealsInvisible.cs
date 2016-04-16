using System.Linq;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Context;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Others
{
	[EffectHandler(EffectsEnum.Effect_RevealsInvisible)]
	public class RevealsInvisible : SpellEffectHandler
	{
		public RevealsInvisible(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			Cell[] arg_06_0 = base.AffectedCells;
			System.Collections.Generic.IEnumerable<Trap> enumerable = 
				from entry in base.Fight.GetTriggers().OfType<Trap>()
				where entry.VisibleState == GameActionFightInvisibilityStateEnum.INVISIBLE && base.Caster.IsEnnemyWith(entry.Caster) && entry.Shapes.Any((MarkShape subentry) => base.AffectedCells.Contains(subentry.Cell))
				select entry;
			foreach (Trap current in enumerable)
			{
				current.VisibleState = GameActionFightInvisibilityStateEnum.DETECTED;
				ContextHandler.SendGameActionFightMarkCellsMessage(base.Fight.Clients, current, true);
			}
			foreach (FightActor current2 in 
				from target in base.GetAffectedActors()
				where target.VisibleState == GameActionFightInvisibilityStateEnum.INVISIBLE && target.IsEnnemyWith(base.Caster)
				select target)
			{
				current2.SetInvisibilityState(GameActionFightInvisibilityStateEnum.DETECTED);
			}
			return true;
		}
	}
}
