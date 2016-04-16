using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Database.Npcs.Replies
{
	[Discriminator("Teleport", typeof(NpcReply), new System.Type[]
	{
		typeof(NpcReplyRecord)
	})]
	public class TeleportReply : NpcReply
	{
		private int m_cellId;
		private DirectionsEnum m_direction;
		private int m_mapId;
		private bool m_mustRefreshPosition;
		private ObjectPosition m_position;
		public int MapId
		{
			get
			{
				return base.Record.GetParameter<int>(0u, false);
			}
			set
			{
				base.Record.SetParameter<int>(0u, value);
				this.m_mustRefreshPosition = true;
			}
		}
		public int CellId
		{
			get
			{
				return base.Record.GetParameter<int>(1u, false);
			}
			set
			{
				base.Record.SetParameter<int>(1u, value);
				this.m_mustRefreshPosition = true;
			}
		}
		public DirectionsEnum Direction
		{
			get
			{
				return (DirectionsEnum)base.Record.GetParameter<int>(2u, false);
			}
			set
			{
				base.Record.SetParameter<int>(2u, (int)value);
				this.m_mustRefreshPosition = true;
			}
		}
		public TeleportReply()
		{
			base.Record.Type = "Teleport";
		}
		public TeleportReply(NpcReplyRecord record) : base(record)
		{
		}
		private void RefreshPosition()
		{
            Map map = Singleton<Game.World>.Instance.GetMap(this.MapId);
			if (map == null)
			{
				throw new System.Exception(string.Format("Cannot load SkillTeleport id={0}, map {1} isn't found", base.Id, this.MapId));
			}
			Cell cell = map.Cells[this.CellId];
			this.m_position = new ObjectPosition(map, cell, this.Direction);
		}
		public ObjectPosition GetPosition()
		{
			if (this.m_position == null || this.m_mustRefreshPosition)
			{
				this.RefreshPosition();
			}
			this.m_mustRefreshPosition = false;
			return this.m_position;
		}
		public override bool Execute(Npc npc, Character character)
		{
			return base.Execute(npc, character) && character.Teleport(this.GetPosition(), true);
		}
	}
}
