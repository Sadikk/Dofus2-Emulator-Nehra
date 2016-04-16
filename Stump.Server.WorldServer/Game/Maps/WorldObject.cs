using NLog;
using Stump.Core.Collections;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Game.Maps
{
	public abstract class WorldObject : System.IDisposable
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
		internal readonly LockFreeQueue<IMessage> m_messageQueue = new LockFreeQueue<IMessage>();
		public abstract int Id
		{
			get;
			protected set;
		}
		public System.DateTime CreationTime
		{
			get;
			protected set;
		}
		public System.DateTime LastUpdateTime
		{
			get;
			protected internal set;
		}
		public virtual ObjectPosition Position
		{
			get;
			protected set;
		}
		public Cell Cell
		{
			get
			{
				return (this.Position != null) ? this.Position.Cell : null;
			}
			set
			{
				this.Position.Cell = value;
			}
		}
		public DirectionsEnum Direction
		{
			get
			{
				return (this.Position != null) ? this.Position.Direction : DirectionsEnum.DIRECTION_EAST;
			}
			set
			{
				this.Position.Direction = value;
			}
		}
		public Map LastMap
		{
			get;
			internal set;
		}
		public Map NextMap
		{
			get;
			internal set;
		}
		public Map Map
		{
			get
			{
				return (this.Position != null) ? this.Position.Map : null;
			}
			set
			{
				this.Position.Map = value;
			}
		}
		public SubArea SubArea
		{
			get
			{
				return (this.Position == null || !(this.Position.Map != null)) ? null : this.Position.Map.SubArea;
			}
		}
		public Area Area
		{
			get
			{
				return (this.Position == null || !(this.Position.Map != null)) ? null : this.Position.Map.Area;
			}
		}
		public SuperArea SuperArea
		{
			get
			{
				return (this.Position == null || !(this.Position.Map != null)) ? null : this.Position.Map.SuperArea;
			}
		}
		public virtual bool IsInWorld
		{
			get
			{
				return this.Position != null && this.Map != null && this.Area != null;
			}
		}
		public bool IsTeleporting
		{
			get;
			internal set;
		}
		public bool IsDeleted
		{
			get;
			protected set;
		}
		public bool IsDisposed
		{
			get;
			protected set;
		}
		public virtual bool BlockSight
		{
			get
			{
				return true;
			}
		}
		public LockFreeQueue<IMessage> MessageQueue
		{
			get
			{
				return this.m_messageQueue;
			}
		}
		public virtual IContextHandler Context
		{
			get
			{
				return this.Area;
			}
		}
		protected WorldObject()
		{
			this.CreationTime = System.DateTime.Now;
		}
		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				this.IsDisposed = true;
				this.OnDisposed();
			}
		}
		protected virtual void OnDisposed()
		{
			this.Position = null;
		}
		public void Delete()
		{
			this.Dispose();
		}
		public bool IsGonnaChangeZone()
		{
			return this.NextMap == null || this.NextMap.Area.Id != this.Area.Id;
		}
		public bool HasChangedZone()
		{
			return this.LastMap == null || this.LastMap.Area.Id != this.Area.Id;
		}
		public virtual bool CanBeSee(WorldObject byObj)
		{
			return byObj != null && !this.IsDeleted && !this.IsDisposed && byObj.Map != null && byObj.Map == this.Map;
		}
		public virtual bool CanSee(WorldObject obj)
		{
			return obj != null && !obj.IsDeleted && !obj.IsDisposed && obj.CanBeSee(this);
		}
	}
}
