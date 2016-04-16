using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Database.World.Triggers;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Maps.Cells.Triggers
{
	[Discriminator("Teleport", typeof(CellTrigger), new System.Type[]
	{
		typeof(CellTriggerRecord)
	})]
	public class TeleportTrigger : CellTrigger
	{
		private short? m_destinationCellId;
		private int? m_destinationMapId;
		private ObjectPosition m_destinationPosition;
		private bool m_mustRefreshDestinationPosition;
		public short DestinationCellId
		{
			get
			{
				short? destinationCellId = this.m_destinationCellId;
				short arg_3C_0;
				if (!destinationCellId.HasValue)
				{
					short? num = this.m_destinationCellId = new short?(base.Record.GetParameter<short>(0u, false));
					arg_3C_0 = num.Value;
				}
				else
				{
					arg_3C_0 = destinationCellId.GetValueOrDefault();
				}
				return arg_3C_0;
			}
			set
			{
				base.Record.SetParameter<short>(0u, value);
				this.m_destinationCellId = new short?(value);
				this.m_mustRefreshDestinationPosition = true;
			}
		}
		public int DestinationMapId
		{
			get
			{
				int? destinationMapId = this.m_destinationMapId;
				int arg_3C_0;
				if (!destinationMapId.HasValue)
				{
					int? num = this.m_destinationMapId = new int?(base.Record.GetParameter<int>(1u, false));
					arg_3C_0 = num.Value;
				}
				else
				{
					arg_3C_0 = destinationMapId.GetValueOrDefault();
				}
				return arg_3C_0;
			}
			set
			{
				this.m_destinationMapId = new int?(value);
				this.m_mustRefreshDestinationPosition = true;
			}
		}
		public TeleportTrigger(CellTriggerRecord record) : base(record)
		{
		}
		private void RefreshPosition()
		{
			Map map = Singleton<World>.Instance.GetMap(this.DestinationMapId);
			if (map == null)
			{
				throw new System.Exception(string.Format("Cannot load CellTeleport id={0}, DestinationMapId {1} isn't found", base.Record.Id, this.DestinationMapId));
			}
			Cell cell = map.Cells[(int)this.DestinationCellId];
			this.m_destinationPosition = new ObjectPosition(map, cell, DirectionsEnum.DIRECTION_EAST);
		}
		public ObjectPosition GetDestinationPosition()
		{
			if (this.m_destinationPosition == null || this.m_mustRefreshDestinationPosition)
			{
				this.RefreshPosition();
			}
			this.m_mustRefreshDestinationPosition = false;
			return this.m_destinationPosition;
		}
		public override void Apply(Character character)
		{
			ObjectPosition destinationPosition = this.GetDestinationPosition();
			character.Teleport(destinationPosition.Map, destinationPosition.Cell);
		}
	}
}
