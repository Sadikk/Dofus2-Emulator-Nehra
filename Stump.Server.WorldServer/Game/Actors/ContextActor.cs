using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Handlers.Chat;
using System;
namespace Stump.Server.WorldServer.Game.Actors
{
	public abstract class ContextActor : WorldObject
	{
		private ObjectPosition m_position;
        private ObjectPosition m_lastPosition;
        private bool m_isMoving;

		public event Action<ContextActor, Path> StartMoving;
		public event Action<ContextActor, Path, bool> StopMoving;
		public event Action<ContextActor, Cell> InstantMoved;
		public event Action<ContextActor, ObjectPosition> PositionChanged;

		public override int Id
		{
			get;
			protected set;
		}
		public virtual ActorLook Look
		{
			get;
			set;
		}
		public virtual ICharacterContainer CharacterContainer
		{
			get
			{
				return this.Position.Map;
			}
		}
		public override ObjectPosition Position
		{
			get
			{
				return this.m_position;
			}
			protected set
			{
				if (this.m_position != null)
				{
					this.m_position.PositionChanged -= new System.Action<ObjectPosition>(this.OnPositionChanged);
				}
				this.m_position = value;
				this.OnPositionChanged(this.m_position);
				if (this.m_position != null)
				{
					this.m_position.PositionChanged += new System.Action<ObjectPosition>(this.OnPositionChanged);
				}
			}
		}
		public Path MovementPath
		{
			get;
			private set;
		}

		public virtual EntityDispositionInformations GetEntityDispositionInformations()
		{
			return new EntityDispositionInformations(base.Cell.Id, (sbyte)base.Direction);
		}
		public virtual GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return new GameContextActorInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations());
		}
		public virtual IdentifiedEntityDispositionInformations GetIdentifiedEntityDispositionInformations()
		{
			return new IdentifiedEntityDispositionInformations(base.Cell.Id, (sbyte)base.Direction, this.Id);
		}
		public void DisplaySmiley(sbyte smileyId)
		{
			this.CharacterContainer.ForEach(delegate(Character entry)
			{
				ChatHandler.SendChatSmileyMessage(entry.Client, this, smileyId);
			});
		}
		protected virtual void OnStartMoving(Path path)
		{
			Action<ContextActor, Path> startMoving = this.StartMoving;
			if (startMoving != null)
			{
				startMoving(this, path);
			}
		}
		protected virtual void OnStopMoving(Path path, bool canceled)
		{
			Action<ContextActor, Path, bool> stopMoving = this.StopMoving;
			if (stopMoving != null)
			{
				stopMoving(this, path, canceled);
			}
		}
		protected virtual void OnInstantMoved(Cell cell)
		{
			Action<ContextActor, Cell> instantMoved = this.InstantMoved;
			if (instantMoved != null)
			{
				instantMoved(this, cell);
			}
		}
		protected virtual void OnPositionChanged(ObjectPosition position)
		{
			Action<ContextActor, ObjectPosition> positionChanged = this.PositionChanged;
			if (positionChanged != null)
			{
				positionChanged(this, position);
			}
		}
		public virtual bool IsMoving()
		{
			return this.m_isMoving && this.MovementPath != null;
		}
		public virtual bool CanMove()
		{
			return base.Map != null && !this.IsMoving();
		}
		public ObjectPosition GetPositionBeforeMove()
		{
			ObjectPosition result;
			if (this.m_lastPosition != null)
			{
				result = this.m_lastPosition;
			}
			else
			{
				result = this.Position;
			}
			return result;
		}

		public virtual bool StartMove(Path movementPath)
		{
			bool result;
			if (!this.CanMove())
			{
				result = false;
			}
			else
			{
				this.m_isMoving = true;
				this.MovementPath = movementPath;
				this.OnStartMoving(movementPath);
				result = true;
			}
			return result;
		}
		public virtual bool MoveInstant(ObjectPosition destination)
		{
			bool result;
			if (!this.CanMove())
			{
				result = true;
			}
			else
			{
				this.m_lastPosition = this.Position;
				this.Position = destination;
				this.OnInstantMoved(destination.Cell);
				result = true;
			}
			return result;
		}

		public virtual bool StopMove()
		{
			bool result;
			if (!this.IsMoving())
			{
				result = false;
			}
			else
			{
				this.m_lastPosition = this.Position;
				this.Position = this.MovementPath.EndPathPosition;
				this.m_isMoving = false;
				this.OnStopMoving(this.MovementPath, false);
				this.MovementPath = null;
				result = true;
			}
			return result;
		}
		public virtual bool StopMove(ObjectPosition currentObjectPosition)
		{
			bool result;
			if (!this.IsMoving() || !this.MovementPath.Contains(currentObjectPosition.Cell.Id))
			{
				result = false;
			}
			else
			{
				this.m_lastPosition = this.Position;
				this.Position = currentObjectPosition;
				this.m_isMoving = false;
				this.OnStopMoving(this.MovementPath, true);
				this.MovementPath = null;
				result = true;
			}
			return result;
		}
	}
}
