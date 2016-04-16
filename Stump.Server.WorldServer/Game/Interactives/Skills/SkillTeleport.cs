using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Database.Interactives;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Game.Interactives.Skills
{
	[Discriminator("Teleport", typeof(Skill), new System.Type[]
	{
		typeof(int),
		typeof(InteractiveSkillRecord),
		typeof(InteractiveObject)
	})]
	public class SkillTeleport : Skill
	{
		private bool m_mustRefreshPosition;
		private ObjectPosition m_position;
		public int MapId
		{
			get
			{
				return base.Record.GetParameter<int>(0u, false);
			}
		}
		public int CellId
		{
			get
			{
				return base.Record.GetParameter<int>(1u, false);
			}
		}
		public DirectionsEnum Direction
		{
			get
			{
				return (DirectionsEnum)base.Record.GetParameter<int>(2u, true);
			}
		}
		public SkillTeleport(int id, InteractiveSkillRecord record, InteractiveObject interactiveObject) : base(id, record, interactiveObject)
		{
		}
		public override bool IsEnabled(Character character)
		{
			return base.Record.IsConditionFilled(character);
		}
		public override void Execute(Character character)
		{
			character.Teleport(this.GetPosition(), true);
		}
		private void RefreshPosition()
		{
			Map map = Singleton<World>.Instance.GetMap(this.MapId);
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
	}
}
