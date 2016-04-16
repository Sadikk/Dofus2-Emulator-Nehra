using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay
{
	public abstract class RolePlayActor : ContextActor
	{
		public event Action<RolePlayActor, Map> EnterMap;
		public event Action<RolePlayActor, Map> LeaveMap;
        public event Action<RolePlayActor, SubArea> ChangeSubArea;
		public event Action<ContextActor, ObjectPosition> Teleported;

		public virtual void OnEnterMap(Map map)
		{
            if (base.LastMap == null || base.LastMap.SubArea != map.SubArea)
            {
                this.OnChangeSubArea(map.SubArea);
            }

			Action<RolePlayActor, Map> enterMap = this.EnterMap;
			if (enterMap != null)
			{
				enterMap(this, map);
			}
		}
		public virtual void OnLeaveMap(Map map)
		{
			Action<RolePlayActor, Map> leaveMap = this.LeaveMap;
			if (leaveMap != null)
			{
				leaveMap(this, map);
			}
		}

        public virtual void OnChangeSubArea(SubArea subArea)
        {
            Action<RolePlayActor, SubArea> changeSubArea = this.ChangeSubArea;
            if (changeSubArea != null)
            {
                changeSubArea(this, subArea);
            }
        }
		public override GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return new GameRolePlayActorInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations());
		}

		protected virtual void OnTeleported(ObjectPosition position)
		{
			Action<ContextActor, ObjectPosition> teleported = this.Teleported;
			if (teleported != null)
			{
				teleported(this, position);
			}
		}

		protected override void OnInstantMoved(Cell cell)
		{
            base.Map.Clients.Send(new TeleportOnSameMapMessage(this.Id, (ushort)cell.Id));
			base.OnInstantMoved(cell);
		}

		public virtual bool Teleport(MapNeighbour mapNeighbour)
		{
			Map neighbouringMap = this.Position.Map.GetNeighbouringMap(mapNeighbour);
			bool result;
			if (neighbouringMap == null)
			{
				result = false;
			}
			else
			{
				short cellAfterChangeMap = this.Position.Map.GetCellAfterChangeMap(this.Position.Cell.Id, mapNeighbour);
				if (cellAfterChangeMap < 0 || cellAfterChangeMap >= 560)
				{
					throw new System.Exception(string.Format("Cell {0} out of range, current={1} neighbour={2}", cellAfterChangeMap, base.Cell.Id, mapNeighbour));
				}
				ObjectPosition destination = new ObjectPosition(neighbouringMap, cellAfterChangeMap, this.Position.Direction);
				result = this.Teleport(destination, true);
			}
			return result;
		}
		public virtual bool Teleport(Map map, Cell cell)
		{
			return this.Teleport(new ObjectPosition(map, cell), true);
		}
		public virtual bool Teleport(ObjectPosition destination, bool performCheck = true)
		{
			if (this.IsMoving())
			{
				this.StopMove();
			}
			bool result;
			if (!this.CanChangeMap() && performCheck)
			{
				result = false;
			}
			else
			{
				if (this.Position.Map == destination.Map)
				{
					result = this.MoveInstant(destination);
				}
				else
				{
					base.NextMap = destination.Map;
					base.LastMap = base.Map;
					this.Position.Map.Leave(this);
					base.NextMap.Area.ExecuteInContext(delegate
					{
						this.Position = destination.Clone();
						this.Position.Map.Enter(this);
						this.NextMap = null;
						this.LastMap = null;
						this.OnTeleported(this.Position);
					});
					result = true;
				}
			}
			return result;
		}

		public virtual bool CanChangeMap()
		{
			return base.Map != null && base.Map.IsActor(this);
		}

		protected override void OnDisposed()
		{
			if (base.Map != null && base.Map.IsActor(this))
			{
				base.Map.Leave(this);
			}
			base.OnDisposed();
		}
	}
}
