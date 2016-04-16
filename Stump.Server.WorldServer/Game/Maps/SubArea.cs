using Stump.Core.Threading;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Maps.Spawns;
using System.Drawing;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Maps
{
	public class SubArea
	{
        // FIELDS
		public static readonly System.Collections.Generic.Dictionary<Difficulty, double[]> MonsterGroupLengthProb = new System.Collections.Generic.Dictionary<Difficulty, double[]>
		{

			{
				Difficulty.VeryEasy,
				new double[]
				{
					2.2,
					2.0,
					1.6,
					0.2,
					0.1,
					0.0,
					0.0,
					0.0
				}
			},

			{
				Difficulty.Easy,
				new double[]
				{
					1.8,
					2.0,
					1.8,
					1.1,
					0.4,
					0.2,
					0.0,
					0.0
				}
			},

			{
				Difficulty.Normal,
				new double[]
				{
					1.3,
					1.5,
					1.5,
					1.1,
					0.6,
					0.4,
					0.0,
					0.0
				}
			},

			{
				Difficulty.Hard,
				new double[]
				{
					0.8,
					1.0,
					1.5,
					1.9,
					1.3,
					0.9,
					0.5,
					0.1
				}
			},

			{
				Difficulty.VeryHard,
				new double[]
				{
					0.2,
					0.6,
					1.2,
					2.2,
					1.9,
					1.3,
					1.1,
					0.6
				}
			},

			{
				Difficulty.Insane,
				new double[]
				{
					0.1,
					0.3,
					0.6,
					1.0,
					1.7,
					2.2,
					1.7,
					1.5
				}
			}
		};
		public static readonly System.Collections.Generic.Dictionary<Difficulty, double[]> MonsterGradeProb = new System.Collections.Generic.Dictionary<Difficulty, double[]>
		{

			{
				Difficulty.VeryEasy,
				new double[]
				{
					3.0,
					2.0,
					1.0,
					0.8,
					0.5
				}
			},

			{
				Difficulty.Easy,
				new double[]
				{
					2.5,
					1.5,
					1.5,
					0.9,
					0.6
				}
			},

			{
				Difficulty.Normal,
				new double[]
				{
					1.0,
					1.0,
					1.0,
					1.0,
					1.0
				}
			},

			{
				Difficulty.Hard,
				new double[]
				{
					0.6,
					0.8,
					1.0,
					1.2,
					1.2
				}
			},

			{
				Difficulty.VeryHard,
				new double[]
				{
					0.4,
					0.6,
					0.8,
					1.0,
					1.2
				}
			},

			{
				Difficulty.Insane,
				new double[]
				{
					0.1,
					0.4,
					0.6,
					1.0,
					2.0
				}
			}
		};
		public static readonly System.Collections.Generic.Dictionary<Difficulty, uint> MonsterSpawnInterval = new System.Collections.Generic.Dictionary<Difficulty, uint>
		{

			{
				Difficulty.VeryEasy,
				60u
			},

			{
				Difficulty.Easy,
				90u
			},

			{
				Difficulty.Normal,
				120u
			},

			{
				Difficulty.Hard,
				160u
			},

			{
				Difficulty.VeryHard,
				180u
			},

			{
				Difficulty.Insane,
				220u
			}
		};
		private readonly System.Collections.Generic.List<Map> m_maps = new System.Collections.Generic.List<Map>();
		private readonly System.Collections.Generic.List<MonsterSpawn> m_monsterSpawns = new System.Collections.Generic.List<MonsterSpawn>();
		private readonly System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>> m_mapsByPoint = new System.Collections.Generic.Dictionary<Point, System.Collections.Generic.List<Map>>();

        // PROPERTIES
		public SubAreaRecord Record
		{
			get;
			private set;
		}
		public int Id
		{
			get
			{
				return this.Record.Id;
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
		public Area Area
		{
			get;
			internal set;
		}
		public SuperArea SuperArea
		{
			get
			{
				return this.Area.SuperArea;
			}
		}
		public Difficulty Difficulty
		{
			get
			{
				return this.Record.Difficulty;
			}
			set
			{
				this.Record.Difficulty = value;
			}
		}
		public int SpawnsLimit
		{
			get
			{
				return this.Record.SpawnsLimit;
			}
			set
			{
				this.Record.SpawnsLimit = value;
			}
		}
		public uint? CustomSpawnInterval
		{
			get
			{
				return this.Record.CustomSpawnInterval;
			}
			set
			{
				this.Record.CustomSpawnInterval = value;
				this.RefreshMapsSpawnInterval();
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<MonsterSpawn> MonsterSpawns
		{
			get
			{
				return this.m_monsterSpawns.AsReadOnly();
			}
		}

        // CONSTRUCTORS
		public SubArea(SubAreaRecord record)
		{
			this.Record = record;
		}
		
        // METHODS
        internal void AddMap(Map map)
		{
			this.m_maps.Add(map);
			if (!this.m_mapsByPoint.ContainsKey(map.Position))
			{
				this.m_mapsByPoint.Add(map.Position, new System.Collections.Generic.List<Map>());
			}
			this.m_mapsByPoint[map.Position].Add(map);
			map.SubArea = this;
		}
		internal void RemoveMap(Map map)
		{
			this.m_maps.Remove(map);
			if (this.m_mapsByPoint.ContainsKey(map.Position))
			{
				System.Collections.Generic.List<Map> list = this.m_mapsByPoint[map.Position];
				list.Remove(map);
				if (list.Count <= 0)
				{
					this.m_mapsByPoint.Remove(map.Position);
				}
			}
			map.SubArea = null;
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
			foreach (Map current in this.Maps)
			{
				current.AddMonsterSpawn(spawn);
			}
		}
		public void RemoveMonsterSpawn(MonsterSpawn spawn)
		{
			this.m_monsterSpawns.Remove(spawn);
			foreach (Map current in this.Maps)
			{
				current.RemoveMonsterSpawn(spawn);
			}
		}
		public int RollMonsterLengthLimit(int imposedLimit = 8)
		{
			Difficulty key = this.Difficulty;
			if (!SubArea.MonsterGroupLengthProb.ContainsKey(key))
			{
				key = Difficulty.Normal;
			}
			double[] array = SubArea.MonsterGroupLengthProb[key].Take(imposedLimit).ToArray<double>();
			double max = array.Sum();
			AsyncRandom asyncRandom = new AsyncRandom();
			double num = asyncRandom.NextDouble(0.0, max);
			double num2 = 0.0;
			int result;
			for (int i = 0; i < array.Length; i++)
			{
				num2 += array[i];
				if (num <= num2)
				{
					result = i + 1;
					return result;
				}
			}
			result = 1;
			return result;
		}
		public int RollMonsterGrade(int minGrade, int maxGrade)
		{
			Difficulty key = this.Difficulty;
			if (!SubArea.MonsterGroupLengthProb.ContainsKey(key))
			{
				key = Difficulty.Normal;
			}
			double[] array = SubArea.MonsterGroupLengthProb[key].Skip(minGrade - 1).Take(maxGrade - minGrade + 1).ToArray<double>();
			double max = array.Sum();
			AsyncRandom asyncRandom = new AsyncRandom();
			double num = asyncRandom.NextDouble(0.0, max);
			double num2 = 0.0;
			int result;
			for (int i = 0; i < array.Length; i++)
			{
				num2 += array[i];
				if (num <= num2)
				{
					if (i >= array.Length - 1 && maxGrade > array.Length)
					{
						int num3 = asyncRandom.Next(0, maxGrade - array.Length + 1);
						result = i + num3 + 1;
					}
					else
					{
						result = i + 1;
					}
					return result;
				}
			}
			result = 1;
			return result;
		}
		public int GetMonsterSpawnInterval()
		{
			Difficulty key = this.Difficulty;
			if (!SubArea.MonsterSpawnInterval.ContainsKey(key))
			{
				key = Difficulty.Normal;
			}
			int result;
			if (this.Record.CustomSpawnInterval.HasValue)
			{
				result = (int)this.Record.CustomSpawnInterval.Value;
			}
			else
			{
				result = (int)SubArea.MonsterSpawnInterval[key];
			}
			return result;
		}
		private void RefreshMapsSpawnInterval()
		{
			foreach (Map current in this.Maps)
			{
				foreach (SpawningPoolBase current2 in current.SpawningPools)
				{
					current2.SetTimer(this.GetMonsterSpawnInterval());
				}
			}
		}
	}
}
