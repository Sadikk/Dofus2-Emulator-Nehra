using System.Linq;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
	[EffectHandler(EffectsEnum.Effect_Rollback)]
	public class Rollback : SpellEffectHandler
	{
		public Rollback(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			System.Collections.Generic.IEnumerable<FightActor> allFighters = base.Fight.GetAllFighters((FightActor f) => f.IsAlive());
			foreach (FightActor current in allFighters)
			{
				Cell cell = current.FightStartPosition.Cell;
				FightActor oneFighter = base.Fight.GetOneFighter(cell);
				if (oneFighter != null)
				{
					this.MoveOldFighter(oneFighter);
				}
				current.Position.Cell = cell;
				ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(base.Fight.Clients, base.Caster, current, cell);
			}
			return true;
		}
		private void MoveOldFighter(FightActor oldFighter)
		{
			MapPoint mapPoint = oldFighter.Position.Point.GetAdjacentCells((short c) => base.Fight.IsCellFree(base.Map.Cells[(int)c])).FirstOrDefault<MapPoint>();
			if (mapPoint != null)
			{
				oldFighter.Position.Cell = base.Map.Cells[(int)mapPoint.CellId];
			}
			else
			{
				oldFighter.Die();
			}
		}
	}
}
