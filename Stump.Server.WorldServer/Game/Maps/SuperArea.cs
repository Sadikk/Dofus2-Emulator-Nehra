using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.World;
using System.Drawing;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Maps
{
	public class SuperArea
	{
		private readonly System.Collections.Generic.List<Area> m_areas = new System.Collections.Generic.List<Area>();
		private readonly System.Collections.Generic.List<Map> m_maps = new System.Collections.Generic.List<Map>();
		private readonly System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>> m_mapsByPoint = new System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>>();
		private readonly System.Collections.Generic.List<SubArea> m_subAreas = new System.Collections.Generic.List<SubArea>();
		private readonly System.Collections.Generic.List<MonsterSpawn> m_monsterSpawns = new System.Collections.Generic.List<MonsterSpawn>();
		protected internal SuperAreaRecord m_record;
		public int Id
		{
			get
			{
				return this.m_record.Id;
			}
		}
		public string Name
		{
			get
			{
				return this.m_record.Name;
			}
		}
		public System.Collections.Generic.IEnumerable<Area> Areas
		{
			get
			{
				return this.m_areas;
			}
		}
		public System.Collections.Generic.IEnumerable<SubArea> SubAreas
		{
			get
			{
				return this.m_subAreas;
			}
		}
		public System.Collections.Generic.IEnumerable<Map> Maps
		{
			get
			{
				return this.m_maps;
			}
		}
		public System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>> MapsByPosition
		{
			get
			{
				return this.m_mapsByPoint;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<MonsterSpawn> MonsterSpawns
		{
			get
			{
				return this.m_monsterSpawns.AsReadOnly();
			}
		}
		public SuperArea(SuperAreaRecord record)
		{
			this.m_record = record;
		}
		internal void AddArea(Area area)
		{
			this.m_areas.Add(area);
			this.m_subAreas.AddRange(area.SubAreas);
			this.m_maps.AddRange(area.Maps);
			foreach (Map current in area.Maps)
			{
				if (!this.m_mapsByPoint.ContainsKey(current.Position))
				{
					this.m_mapsByPoint.Add(current.Position, new System.Collections.Generic.List<Map>());
				}
				this.m_mapsByPoint[current.Position].Add(current);
			}
			area.SuperArea = this;
		}
		internal void RemoveArea(Area area)
		{
			this.m_areas.Remove(area);
			this.m_subAreas.RemoveAll((SubArea entry) => area.SubAreas.Contains(entry));
			this.m_maps.RemoveAll(delegate(Map entry)
			{
				bool result;
				if (area.Maps.Contains(entry))
				{
					if (this.m_mapsByPoint.ContainsKey(entry.Position))
					{
						System.Collections.Generic.List<Map> list = this.m_mapsByPoint[entry.Position];
						list.Remove(entry);
						if (list.Count <= 0)
						{
							this.m_mapsByPoint.Remove(entry.Position);
						}
					}
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			});
			area.SuperArea = null;
		}
		public Map[] GetMaps(int x, int y)
		{
			return this.GetMaps(new Point(x, y));
		}
		public Map[] GetMaps(int x, int y, bool outdoor)
		{
			return this.GetMaps(new Point(x, y), outdoor);
		}
		public Map[] GetMaps(Point position)
		{
			Map[] result;
			if (!this.m_mapsByPoint.ContainsKey(position))
			{
				result = new Map[0];
			}
			else
			{
				result = this.m_mapsByPoint[position].ToArray();
			}
			return result;
		}
		public Map[] GetMaps(Point position, bool outdoor)
		{
			Map[] result;
			if (!this.m_mapsByPoint.ContainsKey(position))
			{
				result = new Map[0];
			}
			else
			{
				result = (
					from entry in this.m_mapsByPoint[position]
					where entry.Outdoor == outdoor
					select entry).ToArray<Map>();
			}
			return result;
		}
		public void AddMonsterSpawn(MonsterSpawn spawn)
		{
			this.m_monsterSpawns.Add(spawn);
			foreach (Area current in this.Areas)
			{
				current.AddMonsterSpawn(spawn);
			}
		}
		public void RemoveMonsterSpawn(MonsterSpawn spawn)
		{
			this.m_monsterSpawns.Remove(spawn);
			foreach (Area current in this.Areas)
			{
				current.RemoveMonsterSpawn(spawn);
			}
		}
	}
}
