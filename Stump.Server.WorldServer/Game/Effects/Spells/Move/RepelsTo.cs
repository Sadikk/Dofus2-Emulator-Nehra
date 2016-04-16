using System;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
	[EffectHandler(EffectsEnum.Effect_RepelsTo)]
	public class RepelsTo : SpellEffectHandler
	{
		public RepelsTo(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
        public override bool Apply()
        {
            DirectionsEnum orientation = this.CastPoint.OrientationTo(this.TargetedPoint, true);
            FightActor target = this.Fight.GetFirstFighter<FightActor>((Predicate<FightActor>)(entry => (int)entry.Position.Cell.Id == (int)this.CastPoint.GetCellInDirection(orientation, (short)1).CellId));
            bool flag;
            if (target == null)
            {
                flag = false;
            }
            else
            {
                Cell startCell = target.Cell;
                Cell cell = this.TargetedCell;
                MapPoint[] cellsOnLineBetween = new MapPoint(startCell).GetCellsOnLineBetween(this.TargetedPoint);
                for (int index = 0; index < cellsOnLineBetween.Length; ++index)
                {
                    MapPoint mapPoint = cellsOnLineBetween[index];
                    if (!this.Fight.IsCellFree(this.Fight.Map.Cells[(int)mapPoint.CellId]))
                        cell = index <= 0 ? startCell : this.Fight.Map.Cells[(int)cellsOnLineBetween[index - 1].CellId];
                    if (this.Fight.ShouldTriggerOnMove(this.Fight.Map.Cells[(int)mapPoint.CellId]))
                    {
                        cell = this.Fight.Map.Cells[(int)mapPoint.CellId];
                        break;
                    }
                }
                target.Cell = cell;
                this.Fight.ForEach((Action<Character>)(entry => ActionsHandler.SendGameActionFightSlideMessage((IPacketReceiver)entry.Client, this.Caster, target, startCell.Id, target.Cell.Id)));
                flag = true;
            }
            return flag;
        }
	}
}
