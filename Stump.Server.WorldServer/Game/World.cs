using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Database.Interactives;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Database.World.Maps;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Interactives;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Triggers;
using Stump.Server.WorldServer.Game.Maps.Spawns;
using Stump.Server.WorldServer.Game.Prisms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Stump.Server.WorldServer.Game
{
	public class World : DataManager<World>
	{
        // FIELDS
		private readonly Logger logger = LogManager.GetCurrentClassLogger();
		private readonly ConcurrentDictionary<int, Character> m_charactersById = new ConcurrentDictionary<int, Character>(System.Environment.ProcessorCount, ClientManager.MaxConcurrentConnections);
		private readonly ConcurrentDictionary<string, Character> m_charactersByName = new ConcurrentDictionary<string, Character>(System.Environment.ProcessorCount, ClientManager.MaxConcurrentConnections);
		private readonly ConcurrentDictionary<int, WorldAccount> m_connectedAccounts = new ConcurrentDictionary<int, WorldAccount>();
		private System.Collections.Generic.Dictionary<int, Area> m_areas = new System.Collections.Generic.Dictionary<int, Area>();
		private int m_characterCount;
		private System.Collections.Generic.Dictionary<int, Map> m_maps = new System.Collections.Generic.Dictionary<int, Map>();
		private System.Collections.Generic.Dictionary<int, SubArea> m_subAreas = new System.Collections.Generic.Dictionary<int, SubArea>();
		private System.Collections.Generic.Dictionary<int, SuperArea> m_superAreas = new System.Collections.Generic.Dictionary<int, SuperArea>();
		private readonly object m_saveLock = new object();
		private readonly ConcurrentBag<ISaveable> m_saveablesInstances = new ConcurrentBag<ISaveable>();
		private bool m_spacesLoaded;
		private bool m_spacesSpawned;
		private readonly System.Collections.Generic.List<Area> m_pausedAreas = new System.Collections.Generic.List<Area>();
		public event System.Action<Character> CharacterJoined;
		public event System.Action<Character> CharacterLeft;

        // PROPERTIES
		public int CharacterCount
		{
			get
			{
				return this.m_characterCount;
			}
		}
		public object SaveLock
		{
			get
			{
				return this.m_saveLock;
			}
		}

        // CONSTRUCTORS

        // METHODS
        [Initialization(InitializationPass.Seventh)]
        public override void Initialize()
        {
            this.LoadSpaces();

            this.SpawnSpaces();
        }
        
		public void LoadSpaces()
		{
			if (this.m_spacesLoaded)
			{
				this.UnSetLinks();
			}
			this.m_spacesLoaded = true;
			this.logger.Info("Load maps...");
            var map = Database.Query<MapRecord>("SELECT * FROM world_maps").Count();
			this.m_maps = base.Database.Query<MapRecord, MapPositionRecord, MapRecord>(new Func<MapRecord, MapPositionRecord, MapRecord>(new MapRecordRelator().Map), MapRecordRelator.FetchQuery, new object[0]).ToDictionary((MapRecord entry) => entry.Id, (MapRecord entry) => new Map(entry));
			this.logger.Info("Load sub areas...");
			this.m_subAreas = base.Database.Query<SubAreaRecord>(SubAreaRecordRelator.FetchQuery, new object[0]).ToDictionary((SubAreaRecord entry) => entry.Id, (SubAreaRecord entry) => new SubArea(entry));
			this.logger.Info("Load areas...");
			this.m_areas = base.Database.Query<AreaRecord>(AreaRecordRelator.FetchQuery, new object[0]).ToDictionary((AreaRecord entry) => entry.Id, (AreaRecord entry) => new Area(entry));
			this.logger.Info("Load super areas...");
			this.m_superAreas = base.Database.Query<SuperAreaRecord>(SuperAreaRecordRelator.FetchQuery, new object[0]).ToDictionary((SuperAreaRecord entry) => entry.Id, (SuperAreaRecord entry) => new SuperArea(entry));
			this.SetLinks();
		}

		private void SetLinks()
		{
            foreach (Map map in this.m_maps.Values)
            {
                SubArea subarea;
                if (map.Record.Position != null && this.m_subAreas.TryGetValue(map.Record.Position.SubAreaId, out subarea))
                {
                    subarea.AddMap(map);
                }
            }
            foreach (SubArea subarea in this.m_subAreas.Values)
            {
                Area area;
                if (this.m_areas.TryGetValue(subarea.Record.AreaId, out area))
                {
                    area.AddSubArea(subarea);
                }
            }
            foreach (Area area in this.m_areas.Values)
            {
                SuperArea superArea;
                if (this.m_superAreas.TryGetValue(area.Record.SuperAreaId, out superArea))
                {
                    superArea.AddArea(area);
                }
            }
		}
		
        private void UnSetLinks()
		{
            foreach (Map map in this.m_maps.Values)
            {
                SubArea subarea;
                if (map.Record.Position != null && this.m_subAreas.TryGetValue(map.Record.Position.SubAreaId, out subarea))
                {
                    subarea.RemoveMap(map);
				}
			}
            foreach (SubArea subarea in this.m_subAreas.Values)
            {
                Area area;
                if (this.m_areas.TryGetValue(subarea.Record.AreaId, out area))
                {
                    area.RemoveSubArea(subarea);
				}
			}
            foreach (Area area in this.m_areas.Values)
            {
                SuperArea superArea;
                if (this.m_superAreas.TryGetValue(area.Record.SuperAreaId, out superArea))
                {
                    superArea.RemoveArea(area);
				}
			}
		}

        #region UnSpawning methods

        private void UnSpawnMerchants()
        {
            foreach (Merchant current in Singleton<MerchantManager>.Instance.Merchants)
            {
                Singleton<MerchantManager>.Instance.UnActiveMerchant(current);
                current.Map.Leave(current);
            }
        }

        public void UnSpawnSpaces()
        {
            foreach (Map current in this.m_maps.Values)
            {
                InteractiveObject[] array = current.GetInteractiveObjects().ToArray<InteractiveObject>();
                InteractiveObject[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    InteractiveObject interactive = array2[i];
                    current.UnSpawnInteractive(interactive);
                }
                SpawningPoolBase[] array3 = current.SpawningPools.ToArray<SpawningPoolBase>();
                for (int i = 0; i < array3.Length; i++)
                {
                    SpawningPoolBase spawningPool = array3[i];
                    current.RemoveSpawningPool(spawningPool);
                }
                CellTrigger[] array4 = current.GetTriggers().ToArray<CellTrigger>();
                CellTrigger[] array5 = array4;
                for (int i = 0; i < array5.Length; i++)
                {
                    CellTrigger trigger = array5[i];
                    current.RemoveTrigger(trigger);
                }
            }
            foreach (System.Collections.Generic.KeyValuePair<int, SubArea> current2 in this.m_subAreas)
            {
                foreach (MonsterSpawn current3 in current2.Value.MonsterSpawns)
                {
                    current2.Value.RemoveMonsterSpawn(current3);
                }
            }
        }

        #endregion

        #region Spawning methods

        private static void SpawnMerchants()
        {
            foreach (Merchant current in
                from spawn in Singleton<MerchantManager>.Instance.GetMerchantSpawns()
                where spawn.Map != null
                select new Merchant(spawn))
            {
                current.LoadRecord();
                Singleton<MerchantManager>.Instance.ActiveMerchant(current);
                current.Map.Enter(current);
            }
        }
        private static void SpawnCellTriggers()
        {
            foreach (CellTrigger current in
                from cellTrigger in Singleton<CellTriggerManager>.Instance.GetCellTriggers()
                select cellTrigger.GenerateTrigger())
            {
                current.Position.Map.AddTrigger(current);
            }
        }
        private static void SpawnNpcs()
        {
            foreach (NpcSpawn current in Singleton<NpcManager>.Instance.GetNpcSpawns())
            {
                ObjectPosition position = current.GetPosition();
                position.Map.SpawnNpc(current);
            }
        }

        private void SpawnInteractives()
        {
            foreach (InteractiveSpawn current in Singleton<InteractiveManager>.Instance.GetInteractiveSpawns())
            {
                Map map = current.GetMap();
                if (map == null)
                {
                    this.logger.Error<int, int>("Cannot spawn interactive id={0} : map {1} doesn't exist", current.Id, current.MapId);
                }
                else
                {
                    map.SpawnInteractive(current);
                }
            }
        }
        private void SpawnMonsters()
        {
            MonsterSpawn[] monsterSpawns = Singleton<MonsterManager>.Instance.GetMonsterSpawns();
            for (int i = 0; i < monsterSpawns.Length; i++)
            {
                MonsterSpawn monsterSpawn = monsterSpawns[i];
                if (monsterSpawn.Map != null)
                {
                    if (monsterSpawn.SubArea == null)
                    {
                        monsterSpawn.Map.AddMonsterSpawn(monsterSpawn);
                    }
                    else
                    {
                        if (!Singleton<MonsterManager>.Instance.GetMonsterDisableSpawns(monsterSpawn.MonsterId, monsterSpawn.SubArea.Id))
                        {
                            monsterSpawn.Map.AddMonsterSpawn(monsterSpawn);
                        }
                    }
                }
                else
                {
                    if (monsterSpawn.SubArea != null && !Singleton<MonsterManager>.Instance.GetMonsterDisableSpawns(monsterSpawn.MonsterId, monsterSpawn.SubArea.Id))
                    {
                        monsterSpawn.SubArea.AddMonsterSpawn(monsterSpawn);
                    }
                }
            }
            foreach (MonsterDungeonSpawn current in
                from spawn in Singleton<MonsterManager>.Instance.GetMonsterDungeonsSpawns()
                where spawn.Map != null
                select spawn)
            {
                current.Map.AddMonsterDungeonSpawn(current);
            }
            foreach (System.Collections.Generic.KeyValuePair<int, Map> current2 in
                from map in this.m_maps
                where !map.Value.SpawnEnabled && !map.Value.IsDungeonSpawn && map.Value.MonsterSpawnsCount > 0
                select map)
            {
                current2.Value.EnableClassicalMonsterSpawns();
            }
        }

        public void SpawnTaxCollectors()
        {
            this.logger.Info("Spawn TaxCollectors ...");
            foreach (TaxCollectorNpc current in
                from spawn in Singleton<TaxCollectorManager>.Instance.GetTaxCollectorSpawns()
                where spawn.Map != null
                select new TaxCollectorNpc(spawn, spawn.Map.GetNextContextualId()))
            {
                current.Guild.AddTaxCollector(current);
                current.Map.Enter(current);
            }
        }
        public void SpawnPrisms()
        {
            this.logger.Info("Spawn Prisms ...");
            foreach (var current in
                from spawn in Singleton<PrismManager>.Instance.GetPrismSpawns()
                where spawn.Map != null
                select new PrismNpc(spawn, spawn.Map.GetNextContextualId()))
            {
                current.Alliance.AddPrism(current);
                current.Map.Enter(current);
            }
        }
        public void SpawnSpaces()
        {
            if (this.m_spacesSpawned)
            {
                this.UnSpawnSpaces();
            }
            this.m_spacesSpawned = true;
            this.logger.Info("Spawn npcs ...");
            World.SpawnNpcs();
            this.logger.Info("Spawn interactives ...");
            this.SpawnInteractives();
            this.logger.Info("Spawn cell triggers ...");
            World.SpawnCellTriggers();
            this.logger.Info("Spawn monsters ...");
            this.SpawnMonsters();
            this.logger.Info("Spawn merchants ...");
            World.SpawnMerchants();
        }

        #endregion

        #region Get methods

        public Map GetMap(int id)
        {
            Map result;
            this.m_maps.TryGetValue(id, out result);
            return result;
        }
        public Map GetMap(int x, int y, bool outdoor = true)
        {
            return this.m_maps.Values.FirstOrDefault((Map entry) => entry.Position.X == x && entry.Position.Y == y && entry.Outdoor == outdoor);
        }

        public IEnumerable<Map> GetMaps()
        {
            return this.m_maps.Values;
        }
        public Map[] GetMaps(int x, int y)
        {
            return (
                from entry in this.m_maps.Values
                where entry.Position.X == x && entry.Position.Y == y
                select entry).ToArray<Map>();
        }
        public Map[] GetMaps(int x, int y, bool outdoor)
        {
            return (
                from entry in this.m_maps.Values
                where entry.Position.X == x && entry.Position.Y == y && entry.Outdoor == outdoor
                select entry).ToArray<Map>();
        }
        public Map[] GetMaps(Map reference, int x, int y)
        {
            Map[] maps = reference.SubArea.GetMaps(x, y);
            Map[] result;
            if (maps.Length > 0)
            {
                result = maps;
            }
            else
            {
                maps = reference.Area.GetMaps(x, y);
                if (maps.Length > 0)
                {
                    result = maps;
                }
                else
                {
                    maps = reference.SuperArea.GetMaps(x, y);
                    result = ((maps.Length > 0) ? maps : new Map[0]);
                }
            }
            return result;
        }
        public Map[] GetMaps(Map reference, int x, int y, bool outdoor)
        {
            Map[] maps = reference.SubArea.GetMaps(x, y, outdoor);
            Map[] result;
            if (maps.Length > 0)
            {
                result = maps;
            }
            else
            {
                maps = reference.Area.GetMaps(x, y, outdoor);
                if (maps.Length > 0)
                {
                    result = maps;
                }
                else
                {
                    maps = reference.SuperArea.GetMaps(x, y, outdoor);
                    result = ((maps.Length > 0) ? maps : new Map[0]);
                }
            }
            return result;
        }

        public SubArea GetSubArea(int id)
        {
            SubArea result;
            this.m_subAreas.TryGetValue(id, out result);
            return result;
        }
        public SubArea GetSubArea(string name)
        {
            return this.m_subAreas.Values.FirstOrDefault((SubArea entry) => entry.Record.Name == name);
        }

        public Area GetArea(int id)
        {
            Area result;
            this.m_areas.TryGetValue(id, out result);
            return result;
        }
        public Area GetArea(string name)
        {
            return this.m_areas.Values.FirstOrDefault((Area entry) => entry.Name == name);
        }

        public SuperArea GetSuperArea(int id)
        {
            SuperArea result;
            this.m_superAreas.TryGetValue(id, out result);
            return result;
        }
        public SuperArea GetSuperArea(string name)
        {
            return this.m_superAreas.Values.FirstOrDefault((SuperArea entry) => entry.Name == name);
        }

        public WorldAccount GetConnectedAccount(int id)
        {
            WorldAccount worldAccount;
            return this.m_connectedAccounts.TryGetValue(id, out worldAccount) ? worldAccount : null;
        }

        public Character GetCharacter(int id)
        {
            Character character;
            return this.m_charactersById.TryGetValue(id, out character) ? character : null;
        }
        public Character GetCharacter(string name)
        {
            Character character;
            return this.m_charactersByName.TryGetValue(name, out character) ? character : null;
        }
        public Character GetCharacter(Predicate<Character> predicate)
        {
            return this.m_charactersById.FirstOrDefault((System.Collections.Generic.KeyValuePair<int, Character> k) => predicate(k.Value)).Value;
        }

        public Character GetCharacterByPattern(string pattern)
        {
            Character result;
            if (pattern[0] != '*')
            {
                result = this.GetCharacter(pattern);
            }
            else
            {
                string name = pattern.Remove(0, 1);
                result = (
                    from entry in Singleton<ClientManager>.Instance.FindAll<WorldClient>((WorldClient entry) => entry.Account.Login == name)
                    select entry.Character).SingleOrDefault<Character>();
            }
            return result;
        }

        public IEnumerable<Character> GetCharacters()
        {
            return this.m_charactersById.Values;
        }
        public IEnumerable<Character> GetCharacters(Predicate<Character> predicate)
        {
            return
                from k in this.m_charactersById.Values
                where predicate(k)
                select k;
        }

        public Character GetCharacterByPattern(Character caller, string pattern)
        {
            return (pattern == "*") ? caller : this.GetCharacterByPattern(pattern);
        }

        #endregion

		public void Enter(Character character)
		{
			if (this.m_charactersById.ContainsKey(character.Id))
			{
				this.Leave(character);
			}
			if (this.m_charactersById.TryAdd(character.Id, character) && this.m_charactersByName.TryAdd(character.Name, character))
			{
				System.Threading.Interlocked.Increment(ref this.m_characterCount);
				this.OnCharacterEntered(character);
			}
			else
			{
				this.logger.Error<Character>("Cannot add character {0} to the World", character);
			}
			if (!this.m_connectedAccounts.ContainsKey(character.Account.Id))
			{
				this.m_connectedAccounts.TryAdd(character.Account.Id, character.Client.WorldAccount);
			}
		}
		public void Leave(Character character)
		{
			Character character2;
			if (this.m_charactersById.TryRemove(character.Id, out character2) && this.m_charactersByName.TryRemove(character.Name, out character2))
			{
				System.Threading.Interlocked.Decrement(ref this.m_characterCount);
				this.OnCharacterLeft(character);
			}
			else
			{
				this.logger.Error<Character>("Cannot remove character {0} from the World", character);
			}
			WorldAccount worldAccount;
			this.m_connectedAccounts.TryRemove(character.Account.Id, out worldAccount);
		}

		public bool IsConnected(int id)
		{
			return this.m_charactersById.ContainsKey(id);
		}
		public bool IsConnected(string name)
		{
			return this.m_charactersByName.ContainsKey(name);
		}

		public bool IsAccountConnected(int id)
		{
			return this.m_connectedAccounts.ContainsKey(id);
		}
		
		public void ForEachCharacter(Action<Character> action)
		{
			foreach (System.Collections.Generic.KeyValuePair<int, Character> current in this.m_charactersById)
			{
				action(current.Value);
			}
		}
		public void ForEachCharacter(Predicate<Character> predicate, Action<Character> action)
		{
			foreach (System.Collections.Generic.KeyValuePair<int, Character> current in 
				from k in this.m_charactersById
				where predicate(k.Value)
				select k)
			{
				action(current.Value);
			}
		}
		
        public void SendAnnounce(string announce)
		{
			ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
			{
				this.ForEachCharacter(delegate(Character character)
				{
					character.SendServerMessage(announce);
				});
			});
		}
		public void SendAnnounce(string announce, Color color)
		{
			ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
			{
				this.ForEachCharacter(delegate(Character character)
				{
					character.SendServerMessage(announce, color);
				});
			});
		}
		public void SendAnnounce(TextInformationTypeEnum type, short messageId, params object[] parameters)
		{
			ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
			{
				this.ForEachCharacter(delegate(Character character)
				{
					character.SendInformationMessage(type, messageId, parameters);
				});
			});
		}
		
        public void RegisterSaveableInstance(ISaveable instance)
		{
			this.m_saveablesInstances.Add(instance);
		}

        private void OnCharacterEntered(Character character)
        {
            System.Action<Character> characterJoined = this.CharacterJoined;
            if (characterJoined != null)
            {
                characterJoined(character);
            }
        }
        private void OnCharacterLeft(Character character)
        {
            System.Action<Character> characterLeft = this.CharacterLeft;
            if (characterLeft != null)
            {
                characterLeft(character);
            }
        }

        public void Save()
		{
			lock (this.SaveLock)
			{
				this.logger.Info("Saving world ...");
				this.SendAnnounce(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 164, new object[0]);
				Stopwatch stopwatch = Stopwatch.StartNew();
				WorldClient[] array = Singleton<ClientManager>.Instance.FindAll<WorldClient>();
				WorldClient[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					WorldClient worldClient = array2[i];
					try
					{
						if (worldClient.Character != null)
						{
							worldClient.Character.SaveNow();
						}
					}
					catch (System.Exception argument)
					{
						this.logger.Error<WorldClient, System.Exception>("Cannot save {0} : {1}", worldClient, argument);
					}
				}
				foreach (ISaveable current in this.m_saveablesInstances)
				{
					try
					{
						current.Save();
					}
					catch (System.Exception argument)
					{
						this.logger.Error<ISaveable, System.Exception>("Cannot save {0} : {1}", current, argument);
					}
				}
				this.SendAnnounce(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 165, new object[0]);
				this.logger.Info("World server saved ! ({0} ms)", stopwatch.ElapsedMilliseconds);
			}
		}
        public void Stop(bool wait = false)
		{
			foreach (System.Collections.Generic.KeyValuePair<int, Area> current in this.m_areas)
			{
				current.Value.Stop(wait);
			}
		}
        public void Pause()
		{
			this.logger.Info("World Paused !!");
			foreach (System.Collections.Generic.KeyValuePair<int, Area> current in 
				from x in this.m_areas
				where x.Value.IsRunning
				select x)
			{
				if (current.Value.IsInContext)
				{
					throw new System.Exception("Has to be called from another thread !!");
				}
				current.Value.Stop(true);
				this.m_pausedAreas.Add(current.Value);
			}
			if (ServerBase<WorldServer>.Instance.IOTaskPool.IsInContext)
			{
				throw new System.Exception("Has to be called from another thread !!");
			}
			ServerBase<WorldServer>.Instance.IOTaskPool.Stop(true);
		}
        public void Resume()
		{
			this.logger.Info("World Resumed");
			foreach (Area current in this.m_pausedAreas)
			{
				current.Start();
			}
			this.m_pausedAreas.Clear();
			ServerBase<WorldServer>.Instance.IOTaskPool.Start();
		}
	}
}
