using NLog;
using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.Core.Timers;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.AI.Fights.Spells;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Interactives;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Database.World.Maps;
using Stump.Server.WorldServer.Game.Actors;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Interactives;
using Stump.Server.WorldServer.Game.Interactives.Skills;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Maps.Cells.Triggers;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Game.Maps.Spawns;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Context;
using Stump.Server.WorldServer.Handlers.Context.RolePlay;
using Stump.Server.WorldServer.Handlers.Interactives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Maps
{
	public class Map : WorldObjectsContext, ICharacterContainer
	{
        // FIELDS
		[Variable(true)]
		public static int MaxMerchantsPerMap;
		[Variable(true)]
		public static int AutoMoveActorMaxInverval;
		[Variable(true)]
		public static int AutoMoveActorMinInverval;

		private static readonly Logger logger;
		public static MapPoint[] PointsGrid;
		private readonly System.Collections.Generic.List<RolePlayActor> m_actors = new System.Collections.Generic.List<RolePlayActor>();
		private readonly ConcurrentDictionary<int, RolePlayActor> m_actorsMap = new ConcurrentDictionary<int, RolePlayActor>();
		private readonly ReversedUniqueIdProvider m_contextualIds = new ReversedUniqueIdProvider(0);
		private readonly System.Collections.Generic.List<Fight> m_fights = new System.Collections.Generic.List<Fight>();
		private readonly System.Collections.Generic.Dictionary<int, InteractiveObject> m_interactives = new System.Collections.Generic.Dictionary<int, InteractiveObject>();
		private readonly System.Collections.Generic.Dictionary<int, MapNeighbour> m_clientMapsAround = new System.Collections.Generic.Dictionary<int, MapNeighbour>();
		private readonly System.Collections.Generic.Dictionary<Cell, System.Collections.Generic.List<CellTrigger>> m_cellsTriggers = new System.Collections.Generic.Dictionary<Cell, System.Collections.Generic.List<CellTrigger>>();
		private readonly System.Collections.Generic.List<MonsterSpawn> m_monsterSpawns = new System.Collections.Generic.List<MonsterSpawn>();
		private TimedTimerEntry m_autoMoveTimer;
		private Map m_bottomNeighbour;
		private Map m_leftNeighbour;
		private Map m_rightNeighbour;
		private Map m_topNeighbour;
		private Cell[] m_redPlacement;
		private Cell[] m_bluePlacement;
		private Cell[] m_freeCells;
		private readonly System.Collections.Generic.List<SpawningPoolBase> m_spawningPools = new System.Collections.Generic.List<SpawningPoolBase>();
		private readonly WorldClientCollection m_clients = new WorldClientCollection();
		public event Action<Map, RolePlayActor> ActorEnter;
		public event Action<Map, RolePlayActor> ActorLeave;
		public event Action<Map, Fight> FightCreated;
		public event Action<Map, Fight> FightRemoved;
		public event Action<Map, InteractiveObject> InteractiveSpawned;
		public event Action<Map, InteractiveObject> InteractiveUnSpawned;
		public event Action<Map, Character, InteractiveObject, Skill> InteractiveUsed;
		public event Action<Map, Character, InteractiveObject, Skill> InteractiveUseEnded;

        // PROPERTIES
		public MapRecord Record
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
		public override Cell[] Cells
		{
			get
			{
				return this.Record.Cells;
			}
		}
		protected override IReadOnlyCollection<WorldObject> Objects
		{
			get
			{
				return this.Actors;
			}
		}
		public IReadOnlyCollection<RolePlayActor> Actors
		{
			get
			{
				return this.m_actors.AsReadOnly();
			}
		}
		public MapCellsInformationProvider CellsInfoProvider
		{
			get;
			private set;
		}
		public SubArea SubArea
		{
			get;
			internal set;
		}
		public Area Area
		{
			get
			{
				return this.SubArea.Area;
			}
		}
		public SuperArea SuperArea
		{
			get
			{
				return this.Area.SuperArea;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<SpawningPoolBase> SpawningPools
		{
			get
			{
				return this.m_spawningPools.AsReadOnly();
			}
		}
		public bool SpawnEnabled
		{
			get;
			private set;
		}
        public bool IsDungeonSpawn
        {
            get;
            private set;
        }
		public uint RelativeId
		{
			get
			{
				return this.Record.RelativeId;
			}
		}
		public int MapType
		{
			get
			{
				return this.Record.MapType;
			}
		}
		public Point Position
		{
			get
			{
				return this.Record.Position.Pos;
			}
		}
		public bool Outdoor
		{
			get
			{
				return this.Record.Outdoor;
			}
		}
		public int TopNeighbourId
		{
			get
			{
				return this.Record.TopNeighbourId;
			}
			set
			{
				this.Record.TopNeighbourId = value;
			}
		}
		public int Capabilities
		{
			get
			{
				return (this.Record.Position != null) ? this.Record.Position.Capabilities : 65535;
			}
		}
		public Map TopNeighbour
		{
			get
			{
                Map arg_2F_0;
				if (this.TopNeighbourId == -1)
				{
					arg_2F_0 = null;
				}
				else
				{
					if ((arg_2F_0 = this.m_topNeighbour) == null)
					{
						arg_2F_0 = (this.m_topNeighbour = Singleton<World>.Instance.GetMap(this.TopNeighbourId));
					}
				}
				return arg_2F_0;
			}
		}
		public int BottomNeighbourId
		{
			get
			{
				return this.Record.BottomNeighbourId;
			}
			set
			{
				this.Record.BottomNeighbourId = value;
			}
		}
		public Map BottomNeighbour
		{
			get
			{
                Map arg_2F_0;
				if (this.BottomNeighbourId == -1)
				{
					arg_2F_0 = null;
				}
				else
				{
					if ((arg_2F_0 = this.m_bottomNeighbour) == null)
					{
						arg_2F_0 = (this.m_bottomNeighbour = Singleton<World>.Instance.GetMap(this.BottomNeighbourId));
					}
				}
				return arg_2F_0;
			}
		}
		public int LeftNeighbourId
		{
			get
			{
				return this.Record.LeftNeighbourId;
			}
			set
			{
				this.Record.LeftNeighbourId = value;
			}
		}
		public Map LeftNeighbour
		{
			get
			{
                Map arg_2F_0;
				if (this.LeftNeighbourId == -1)
				{
					arg_2F_0 = null;
				}
				else
				{
					if ((arg_2F_0 = this.m_leftNeighbour) == null)
					{
						arg_2F_0 = (this.m_leftNeighbour = Singleton<World>.Instance.GetMap(this.LeftNeighbourId));
					}
				}
				return arg_2F_0;
			}
		}
		public int RightNeighbourId
		{
			get
			{
				return this.Record.RightNeighbourId;
			}
			set
			{
				this.Record.RightNeighbourId = value;
			}
		}
		public Map RightNeighbour
		{
			get
			{
                Map arg_2F_0;
				if (this.RightNeighbourId == -1)
				{
					arg_2F_0 = null;
				}
				else
				{
					if ((arg_2F_0 = this.m_rightNeighbour) == null)
					{
						arg_2F_0 = (this.m_rightNeighbour = Singleton<World>.Instance.GetMap(this.RightNeighbourId));
					}
				}
				return arg_2F_0;
			}
		}
		public int ShadowBonusOnEntities
		{
			get
			{
				return this.Record.ShadowBonusOnEntities;
			}
			set
			{
				this.Record.ShadowBonusOnEntities = value;
			}
		}
		public bool UseLowpassFilter
		{
			get
			{
				return this.Record.UseLowpassFilter;
			}
			set
			{
				this.Record.UseLowpassFilter = value;
			}
		}
		public bool UseReverb
		{
			get
			{
				return this.Record.UseReverb;
			}
			set
			{
				this.Record.UseReverb = value;
			}
		}
		public int PresetId
		{
			get
			{
				return this.Record.PresetId;
			}
		}
		public InteractiveObject Zaap
		{
			get;
			private set;
		}
		public bool IsMuted
		{
			get;
			private set;
		}
		public bool AllowChallenge
		{
			get
			{
				return (this.Capabilities & 1) != 0;
			}
		}
		public bool AllowAggression
		{
			get
			{
				return (this.Capabilities & 2) != 0;
			}
		}
		public bool AllowTeleportTo
		{
			get
			{
				return (this.Capabilities & 4) != 0;
			}
		}
		public bool AllowTeleportFrom
		{
			get
			{
				return (this.Capabilities & 8) != 0;
			}
		}
		public bool AllowExchangesBetweenPlayers
		{
			get
			{
				return (this.Capabilities & 16) != 0;
			}
		}
		public bool AllowHumanVendor
		{
			get
			{
				return (this.Capabilities & 32) != 0;
			}
		}
		public bool AllowCollector
		{
			get
			{
				return (this.Capabilities & 64) != 0;
			}
		}
		public bool AllowSoulCapture
		{
			get
			{
				return (this.Capabilities & 128) != 0;
			}
		}
		public bool AllowSoulSummon
		{
			get
			{
				return (this.Capabilities & 256) != 0;
			}
		}
		public bool AllowTavernRegen
		{
			get
			{
				return (this.Capabilities & 512) != 0;
			}
		}
		public bool AllowTombMode
		{
			get
			{
				return (this.Capabilities & 1024) != 0;
			}
		}
		public bool AllowTeleportEverywhere
		{
			get
			{
				return (this.Capabilities & 2048) != 0;
			}
		}
		public bool AllowFightChallenges
		{
			get
			{
				return (this.Capabilities & 4096) != 0;
			}
		}
		public TaxCollectorNpc TaxCollector
		{
			get;
			private set;
		}
		public int MonsterSpawnsCount
		{
			get
			{
				return this.m_monsterSpawns.Count;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<MonsterSpawn> MonsterSpawns
		{
			get
			{
				return this.m_monsterSpawns.AsReadOnly();
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<Fight> Fights
		{
			get
			{
				return this.m_fights.AsReadOnly();
			}
		}
		public WorldClientCollection Clients
		{
			get
			{
				return this.m_clients;
			}
		}
        
        // CONSTRUCTORS
        static Map()
        {
            Map.MaxMerchantsPerMap = 5;
            Map.AutoMoveActorMaxInverval = 40;
            Map.AutoMoveActorMinInverval = 20;
            Map.logger = LogManager.GetCurrentClassLogger();
            Map.PointsGrid = new MapPoint[560];
            short num = 0;
            while ((long)num < 560L)
            {
                Map.PointsGrid[(int)num] = new MapPoint(num);
                num += 1;
            }
        }
        public Map(MapRecord record)
        {
            this.Record = record;
            this.InitializeValidators();
            this.UpdateMapArrounds();
            this.UpdateCells();
            this.UpdateFightPlacements();
        }

        // METHODS
		protected virtual void OnActorEnter(RolePlayActor actor)
		{
			this.OnEnter(actor);
			Action<Map, RolePlayActor> actorEnter = this.ActorEnter;
			if (actorEnter != null)
			{
				actorEnter(this, actor);
			}
		}
		protected virtual void OnActorLeave(RolePlayActor actor)
		{
			this.OnLeave(actor);
			Action<Map, RolePlayActor> actorLeave = this.ActorLeave;
			if (actorLeave != null)
			{
				actorLeave(this, actor);
			}
		}
		protected virtual void OnFightCreated(Fight fight)
		{
			Action<Map, Fight> fightCreated = this.FightCreated;
			if (fightCreated != null)
			{
				fightCreated(this, fight);
			}
		}
		protected virtual void OnFightRemoved(Fight fight)
		{
			Action<Map, Fight> fightRemoved = this.FightRemoved;
			if (fightRemoved != null)
			{
				fightRemoved(this, fight);
			}
		}
		protected virtual void OnInteractiveUsed(Character user, InteractiveObject interactive, Skill skill)
		{
			InteractiveHandler.SendInteractiveUsedMessage(this.Clients, user, interactive, skill);
			Action<Map, Character, InteractiveObject, Skill> interactiveUsed = this.InteractiveUsed;
			if (interactiveUsed != null)
			{
				interactiveUsed(this, user, interactive, skill);
			}
		}
		protected virtual void OnInteractiveUseEnded(Character user, InteractiveObject interactive, Skill skill)
		{
			Action<Map, Character, InteractiveObject, Skill> interactiveUseEnded = this.InteractiveUseEnded;
			if (interactiveUseEnded != null)
			{
				interactiveUseEnded(this, user, interactive, skill);
			}
		}
        protected virtual void OnInteractiveSpawned(InteractiveObject interactive)
        {
            Action<Map, InteractiveObject> interactiveSpawned = this.InteractiveSpawned;
            if (interactiveSpawned != null)
            {
                interactiveSpawned(this, interactive);
            }
        }
        protected virtual void OnInteractiveUnSpawned(InteractiveObject interactive)
        {
            Action<Map, InteractiveObject> interactiveUnSpawned = this.InteractiveUnSpawned;
            if (interactiveUnSpawned != null)
            {
                interactiveUnSpawned(this, interactive);
            }
        }

		public void UpdateMapArrounds()
		{
			this.m_clientMapsAround.Clear();
			if (this.TopNeighbourId != -1 && !this.m_clientMapsAround.ContainsKey(this.TopNeighbourId))
			{
				this.m_clientMapsAround.Add(this.Record.ClientTopNeighbourId, MapNeighbour.Top);
			}
			if (this.BottomNeighbourId != -1 && !this.m_clientMapsAround.ContainsKey(this.BottomNeighbourId))
			{
				this.m_clientMapsAround.Add(this.Record.ClientBottomNeighbourId, MapNeighbour.Bottom);
			}
			if (this.LeftNeighbourId != -1 && !this.m_clientMapsAround.ContainsKey(this.LeftNeighbourId))
			{
				this.m_clientMapsAround.Add(this.Record.ClientLeftNeighbourId, MapNeighbour.Left);
			}
			if (this.RightNeighbourId != -1 && !this.m_clientMapsAround.ContainsKey(this.RightNeighbourId))
			{
				this.m_clientMapsAround.Add(this.Record.ClientRightNeighbourId, MapNeighbour.Right);
			}
		}
		public void UpdateFightPlacements()
		{
			if (this.Record.BlueFightCells.Length == 0 || this.Record.RedFightCells.Length == 0)
			{
				this.m_bluePlacement = new Cell[0];
				this.m_redPlacement = new Cell[0];
			}
			else
			{
				this.m_bluePlacement = (
					from entry in this.Record.BlueFightCells
					select this.Cells[(int)entry]).ToArray<Cell>();
				this.m_redPlacement = (
					from entry in this.Record.RedFightCells
					select this.Cells[(int)entry]).ToArray<Cell>();
			}
		}
		public void UpdateCells()
		{
			this.CellsInfoProvider = new MapCellsInformationProvider(this);
			this.m_freeCells = (
				from entry in this.Cells
				where this.CellsInfoProvider.IsCellWalkable(entry.Id)
				select entry).ToArray<Cell>();
		}
		public Npc SpawnNpc(NpcTemplate template, ObjectPosition position, ActorLook look)
		{
			if (position.Map != this)
			{
				throw new System.Exception("Try to spawn a npc on the wrong map");
			}
			int nextContextualId = this.GetNextContextualId();
			Npc npc = new Npc(nextContextualId, template, position, look);
			template.OnNpcSpawned(npc);
			this.Enter(npc);
			return npc;
		}
		public Npc SpawnNpc(NpcSpawn spawn)
		{
			ObjectPosition position = spawn.GetPosition();
			if (position.Map != this)
			{
				throw new System.Exception("Try to spawn a npc on the wrong map");
			}
			int nextContextualId = this.GetNextContextualId();
			Npc npc = new Npc(nextContextualId, spawn);
			spawn.Template.OnNpcSpawned(npc);
			this.Enter(npc);
			return npc;
		}
		public bool UnSpawnNpc(sbyte id)
		{
			Npc actor = this.GetActor<Npc>((int)id);
			bool result;
			if (actor == null)
			{
				result = false;
			}
			else
			{
				this.Leave(actor);
				result = true;
			}
			return result;
		}
		public void UnSpawnNpc(Npc npc)
		{
			if (this.GetActor<Npc>(npc.Id) != npc)
			{
				throw new System.Exception(string.Format("Npc with id {0} not found, cannot unspawn an unexistant npc", npc.Id));
			}
			this.Leave(npc);
		}
		public InteractiveObject SpawnInteractive(InteractiveSpawn spawn)
		{
			InteractiveObject interactiveObject = new InteractiveObject(spawn);
			if (interactiveObject.Template != null && interactiveObject.Template.Type == InteractiveTypeEnum.TYPE_ZAAP)
			{
				if (this.Zaap != null)
				{
					throw new System.Exception("Cannot add a second zaap on the map");
				}
				this.Zaap = interactiveObject;
			}
			InteractiveObject result;
			if (this.m_interactives.ContainsKey(interactiveObject.Id))
			{
				Map.logger.Error<int, int>("Interactive object {0} already exists on map {1}", interactiveObject.Id, this.Id);
				result = null;
			}
			else
			{
				this.m_interactives.Add(interactiveObject.Id, interactiveObject);
				this.Area.Enter(interactiveObject);
				this.OnInteractiveSpawned(interactiveObject);
				result = interactiveObject;
			}
			return result;
		}
		public void UnSpawnInteractive(InteractiveObject interactive)
		{
			if (interactive.Template != null && interactive.Template.Type == InteractiveTypeEnum.TYPE_ZAAP && this.Zaap != null)
			{
				this.Zaap = null;
			}
			interactive.Delete();
			this.m_interactives.Remove(interactive.Id);
			this.Area.Leave(interactive);
			this.OnInteractiveUnSpawned(interactive);
		}
		public bool UseInteractiveObject(Character character, int interactiveId, int skillId)
		{
			InteractiveObject interactiveObject = this.GetInteractiveObject(interactiveId);
			bool result;
			if (interactiveObject == null)
			{
				result = false;
			}
			else
			{
				Skill skill = interactiveObject.GetSkill(skillId);
				if (skill == null)
				{
					result = false;
				}
				else
				{
					if (skill.IsEnabled(character))
					{
						skill.Execute(character);
						this.OnInteractiveUsed(character, interactiveObject, skill);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}
		public bool NotifyInteractiveObjectUseEnded(Character character, int interactiveId, int skillId)
		{
			InteractiveObject interactiveObject = this.GetInteractiveObject(interactiveId);
			bool result;
			if (interactiveObject == null)
			{
				result = false;
			}
			else
			{
				Skill skill = interactiveObject.GetSkill(skillId);
				if (skill == null)
				{
					result = false;
				}
				else
				{
					skill.PostExecute(character);
					this.OnInteractiveUseEnded(character, interactiveObject, skill);
					result = true;
				}
			}
			return result;
		}
		public bool CanSpawnMonsters()
		{
			return this.m_bluePlacement.Length > 0 && this.m_redPlacement.Length > 0;
		}
		public void AddSpawningPool(SpawningPoolBase spawningPool)
		{
			this.m_spawningPools.Add(spawningPool);
		}
		public bool RemoveSpawningPool(SpawningPoolBase spawningPool)
		{
			spawningPool.StopAutoSpawn();
			return this.m_spawningPools.Remove(spawningPool);
		}
		public void ClearSpawningPools()
		{
			SpawningPoolBase[] array = this.SpawningPools.ToArray<SpawningPoolBase>();
			for (int i = 0; i < array.Length; i++)
			{
				SpawningPoolBase spawningPool = array[i];
				this.RemoveSpawningPool(spawningPool);
			}
		}
		public void EnableClassicalMonsterSpawns()
		{
			if (!this.SpawnEnabled && this.CanSpawnMonsters())
			{
				ClassicalSpawningPool[] array = this.SpawningPools.OfType<ClassicalSpawningPool>().ToArray<ClassicalSpawningPool>();
				if (array.Length == 0)
				{
					ClassicalSpawningPool classicalSpawningPool = new ClassicalSpawningPool(this, this.SubArea.GetMonsterSpawnInterval());
					this.AddSpawningPool(classicalSpawningPool);
					classicalSpawningPool.StartAutoSpawn();
				}
				ClassicalSpawningPool[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					ClassicalSpawningPool classicalSpawningPool = array2[i];
					classicalSpawningPool.StartAutoSpawn();
				}
				this.SpawnEnabled = true;
			}
		}
		public void DisableClassicalMonsterSpawns()
		{
			if (this.SpawnEnabled)
			{
				foreach (MonsterGroup current in 
					from actor in this.GetActors<MonsterGroup>()
					where actor.GetMonsters().All((Monster entry) => this.MonsterSpawns.Any((MonsterSpawn spawn) => spawn.MonsterId == entry.Template.Id))
					select actor)
				{
					this.Leave(current);
				}
				foreach (ClassicalSpawningPool current2 in 
					from spawningPool in this.SpawningPools.OfType<ClassicalSpawningPool>()
					where spawningPool.AutoSpawnEnabled
					select spawningPool)
				{
					current2.StopAutoSpawn();
				}
				this.SpawnEnabled = false;
			}
		}
		public void AddMonsterSpawn(MonsterSpawn spawn)
		{
			this.m_monsterSpawns.Add(spawn);
		}
		public void RemoveMonsterSpawn(MonsterSpawn spawn)
		{
			this.m_monsterSpawns.Remove(spawn);
		}
		public void RemoveMonsterSpawns(int monsterId)
		{
			this.m_monsterSpawns.RemoveAll((MonsterSpawn x) => x.MonsterId == monsterId);
		}
		public void AddMonsterDungeonSpawn(MonsterDungeonSpawn spawn)
		{
            this.IsDungeonSpawn = true;
			DungeonSpawningPool dungeonSpawningPool = this.m_spawningPools.FirstOrDefault((SpawningPoolBase entry) => entry is DungeonSpawningPool) as DungeonSpawningPool;
			if (dungeonSpawningPool == null)
			{
				this.AddSpawningPool(dungeonSpawningPool = new DungeonSpawningPool(this));
			}
			dungeonSpawningPool.AddSpawn(spawn);
			if (!dungeonSpawningPool.AutoSpawnEnabled)
			{
				dungeonSpawningPool.StartAutoSpawn();
			}
		}
		public void RemoveMonsterDungeonSpawn(MonsterDungeonSpawn spawn)
        {
            this.IsDungeonSpawn = false;
			DungeonSpawningPool dungeonSpawningPool = this.m_spawningPools.FirstOrDefault((SpawningPoolBase entry) => entry is DungeonSpawningPool) as DungeonSpawningPool;
			if (dungeonSpawningPool != null)
			{
				dungeonSpawningPool.RemoveSpawn(spawn);
				if (dungeonSpawningPool.SpawnsCount == 0)
				{
					dungeonSpawningPool.StopAutoSpawn();
				}
			}
		}
		public MonsterGroup GenerateRandomMonsterGroup()
		{
			return this.GenerateRandomMonsterGroup(this.SubArea.RollMonsterLengthLimit(8));
		}
		public MonsterGroup GenerateRandomMonsterGroup(int minLength, int maxLength)
		{
			if (minLength == maxLength)
			{
				this.GenerateRandomMonsterGroup(minLength);
			}
			return this.GenerateRandomMonsterGroup(new AsyncRandom().Next(minLength, maxLength + 1));
		}
		public MonsterGroup GenerateRandomMonsterGroup(int length)
		{
			AsyncRandom asyncRandom = new AsyncRandom();
			MonsterGroup result;
			if (this.MonsterSpawns.Count <= 0)
			{
				result = null;
			}
			else
			{
				double max = this.MonsterSpawns.Sum((MonsterSpawn entry) => entry.Frequency);
				MonsterGroup monsterGroup = new MonsterGroup(this.GetNextContextualId(), new ObjectPosition(this, this.GetRandomFreeCell(false), this.GetRandomDirection()));
				for (int i = 0; i < length; i++)
				{
					double num = asyncRandom.NextDouble(0.0, max);
					double num2 = 0.0;
					MonsterGrade monsterGrade = null;
					foreach (MonsterSpawn current in this.MonsterSpawns)
					{
						num2 += current.Frequency;
						if (num <= num2)
						{
							monsterGrade = Singleton<MonsterManager>.Instance.GetMonsterGrade(current.MonsterId, this.SubArea.RollMonsterGrade(current.MinGrade, current.MaxGrade));
                            if (monsterGrade != null && Map.CheckMonsterAI(monsterGrade))
							{
								break;
							}
						}
					}
					if (monsterGrade != null)
					{
						monsterGroup.AddMonster(new Monster(monsterGrade, monsterGroup));
					}
				}
				result = ((monsterGroup.Count() <= 0) ? null : monsterGroup);
			}
			return result;
		}
		private static bool CheckMonsterAI(MonsterGrade grade)
		{
			System.Collections.Generic.IEnumerable<SpellCategory> source = grade.Spells.Select(new Func<Spell, SpellCategory>(SpellIdentifier.GetSpellCategories));
			return source.Any((SpellCategory x) => (x & SpellCategory.Damages) != SpellCategory.None || x.HasFlag(SpellCategory.Healing));
		}
		public MonsterGroup SpawnMonsterGroup(MonsterGrade monster, ObjectPosition position)
		{
			if (position.Map != this)
			{
				throw new System.Exception("Try to spawn a monster group on the wrong map");
			}
			int nextContextualId = this.GetNextContextualId();
			MonsterGroup monsterGroup = new MonsterGroup(nextContextualId, position);
			monsterGroup.AddMonster(new Monster(monster, monsterGroup));
			this.Enter(monsterGroup);
			return monsterGroup;
		}
		public MonsterGroup SpawnMonsterGroup(System.Collections.Generic.IEnumerable<MonsterGrade> monsters, ObjectPosition position)
		{
			if (position.Map != this)
			{
				throw new System.Exception("Try to spawn a monster group on the wrong map");
			}
			int nextContextualId = this.GetNextContextualId();
			MonsterGroup monsterGroup = new MonsterGroup(nextContextualId, position);
			foreach (MonsterGrade current in monsters)
			{
				monsterGroup.AddMonster(new Monster(current, monsterGroup));
			}
			this.Enter(monsterGroup);
			return monsterGroup;
		}
		public bool UnSpawnMonsterGroup(sbyte id)
		{
			MonsterGroup actor = this.GetActor<MonsterGroup>((int)id);
			bool result;
			if (actor == null)
			{
				result = false;
			}
			else
			{
				this.Leave(actor);
				actor.Delete();
				result = true;
			}
			return result;
		}
		public int GetMonsterSpawnsCount()
		{
			return this.GetActors<MonsterGroup>().Count<MonsterGroup>();
		}
		public int GetMonsterSpawnsLimit()
		{
			return this.SubArea.SpawnsLimit;
		}
		private void MoveRandomlyActors()
		{
			foreach (RolePlayActor current in 
				from x in this.Actors
				where x is IAutoMovedEntity && (x as IAutoMovedEntity).NextMoveDate <= System.DateTime.Now
				select x)
			{
				Lozenge lozenge = new Lozenge(1, 4);
				Cell cell = lozenge.GetCells(current.Cell, this).Where((Cell entry) => entry.Walkable && !entry.NonWalkableDuringRP && entry.MapChangeData == 0).RandomElementOrDefault<Cell>();
				if (cell == null)
				{
					break;
				}
				Pathfinder pathfinder = new Pathfinder(this.CellsInfoProvider);
				Path path = pathfinder.FindPath(current.Cell.Id, cell.Id, false, -1);
				if (!path.IsEmpty())
				{
					current.StartMove(path);
				}
				(current as IAutoMovedEntity).NextMoveDate = System.DateTime.Now + System.TimeSpan.FromSeconds((double)new AsyncRandom().Next(Map.AutoMoveActorMinInverval, Map.AutoMoveActorMaxInverval + 1));
			}
		}
		public void AddTrigger(CellTrigger trigger)
		{
			if (!this.m_cellsTriggers.ContainsKey(trigger.Position.Cell))
			{
				this.m_cellsTriggers.Add(trigger.Position.Cell, new System.Collections.Generic.List<CellTrigger>());
			}
			this.m_cellsTriggers[trigger.Position.Cell].Add(trigger);
		}
		public void RemoveTrigger(CellTrigger trigger)
		{
			if (this.m_cellsTriggers.ContainsKey(trigger.Position.Cell))
			{
				this.m_cellsTriggers[trigger.Position.Cell].Remove(trigger);
			}
		}
		public void RemoveTriggers(Cell cell)
		{
			if (this.m_cellsTriggers.ContainsKey(cell))
			{
				this.m_cellsTriggers[cell].Clear();
			}
		}
		public System.Collections.Generic.IEnumerable<CellTrigger> GetTriggers(Cell cell)
		{
			System.Collections.Generic.IEnumerable<CellTrigger> result;
			if (!this.m_cellsTriggers.ContainsKey(cell))
			{
				result = Enumerable.Empty<CellTrigger>();
			}
			else
			{
				result = this.m_cellsTriggers[cell];
			}
			return result;
		}
		public System.Collections.Generic.IEnumerable<CellTrigger> GetTriggers()
		{
			return this.m_cellsTriggers.Values.SelectMany((System.Collections.Generic.List<CellTrigger> x) => x);
		}
		public bool ExecuteTrigger(CellTriggerType triggerType, Cell cell, Character character)
		{
			bool result = false;
		    foreach (CellTrigger current in this.GetTriggers(cell))
		    {
		        if (current.TriggerType == triggerType)
		        {
		            current.Apply(character);
		            result = true;
		        }
		    }
		    return result;
		}
		public short GetFightCount()
		{
			return (short)this.m_fights.Count;
		}
		public void AddFight(Fight fight)
		{
			if (!(fight.Map != this))
			{
				this.m_fights.Add(fight);
				ContextRoleplayHandler.SendMapFightCountMessage(this.Clients, (short)this.m_fights.Count);
				this.OnFightCreated(fight);
			}
		}
		public void RemoveFight(Fight fight)
		{
			this.m_fights.Remove(fight);
			ContextRoleplayHandler.SendMapFightCountMessage(this.Clients, (short)this.m_fights.Count);
			this.OnFightRemoved(fight);
		}
		public Cell[] GetBlueFightPlacement()
		{
			return this.m_bluePlacement;
		}
		public Cell[] GetRedFightPlacement()
		{
			return this.m_redPlacement;
		}
		public void Enter(RolePlayActor actor)
		{
			if (ServerBase<WorldServer>.Instance.IsInitialized)
			{
				this.Area.EnsureContext();
			}
			if (this.m_actors.Contains(actor))
			{
				Map.logger.Error<RolePlayActor>("Map already contains actor {0}", actor);
				this.Leave(actor);
			}
			if (this.m_actorsMap.ContainsKey(actor.Id))
			{
				Map.logger.Error("Map already contains actor {0}", actor.Id);
				this.Leave(actor.Id);
			}
			this.m_actors.Add(actor);
			this.m_actorsMap.TryAdd(actor.Id, actor);
			this.OnActorEnter(actor);
		}
		public void Leave(RolePlayActor actor)
		{
			if (ServerBase<WorldServer>.Instance.IsInitialized)
			{
				this.Area.EnsureContext();
			}
			if (this.m_actors.Remove(actor))
			{
				RolePlayActor rolePlayActor;
				if (this.m_actorsMap.TryRemove(actor.Id, out rolePlayActor) && rolePlayActor != actor)
				{
					Map.logger.Error("Did not removed the expected actor !!");
				}
				this.OnActorLeave(actor);
			}
		}
		public void Leave(int actorId)
		{
			if (ServerBase<WorldServer>.Instance.IsInitialized)
			{
				this.Area.EnsureContext();
			}
			RolePlayActor rolePlayActor;
			if (this.m_actorsMap.TryRemove(actorId, out rolePlayActor) && this.m_actors.Remove(rolePlayActor))
			{
				this.OnActorLeave(rolePlayActor);
			}
		}
		public void Refresh(RolePlayActor actor)
		{
			if (ServerBase<WorldServer>.Instance.IsInitialized)
			{
				this.Area.EnsureContext();
			}
			if (this.IsActor(actor))
			{
				this.ForEach(delegate(Character x)
				{
					if (actor.CanBeSee(x))
					{
						ContextRoleplayHandler.SendGameRolePlayShowActorMessage(x.Client, x, actor);
					}
					else
					{
						ContextHandler.SendGameContextRemoveElementMessage(x.Client, actor);
					}
				});
			}
		}
		private void OnEnter(RolePlayActor actor)
		{
			if (actor.HasChangedZone())
			{
				this.Area.Enter(actor);
			}
			actor.StartMoving += new Action<ContextActor, Path>(this.OnActorStartMoving);
			actor.StopMoving += new Action<ContextActor, Path, bool>(this.OnActorStopMoving);
			Character character = actor as Character;
			if (character != null)
			{
				this.Clients.Add(character.Client);
			}
			if (actor is TaxCollectorNpc)
			{
				if (this.TaxCollector != null)
				{
					Map.logger.Error("There is already a Tax Collector on that map ({0}).", this.Id);
					this.Leave(actor);
					return;
				}
				this.TaxCollector = (actor as TaxCollectorNpc);
			}
			if (actor is IAutoMovedEntity)
			{
                (actor as IAutoMovedEntity).NextMoveDate = DateTime.Now + TimeSpan.FromSeconds(new AsyncRandom().Next(AutoMoveActorMinInverval, AutoMoveActorMaxInverval + 1));
                if (m_autoMoveTimer == null)
                {
                    m_autoMoveTimer = Area.CallPeriodically((int)TimeSpan.FromSeconds(new AsyncRandom().Next(AutoMoveActorMinInverval, AutoMoveActorMaxInverval)).TotalMilliseconds, MoveRandomlyActors);
                }
			}
			this.ForEach(delegate(Character x)
			{
				if (actor.CanBeSee(x))
				{
					ContextRoleplayHandler.SendGameRolePlayShowActorMessage(x.Client, x, actor);
				}
			});
			actor.OnEnterMap(this);
		}
		private void OnLeave(RolePlayActor actor)
		{
			if (actor == this.TaxCollector)
			{
				this.TaxCollector = null;
			}
			if (actor.IsGonnaChangeZone())
			{
				this.Area.Leave(actor);
			}
			actor.StartMoving -= new Action<ContextActor, Path>(this.OnActorStartMoving);
			actor.StopMoving -= new Action<ContextActor, Path, bool>(this.OnActorStopMoving);
			Character character = actor as Character;
			if (character != null)
			{
				this.Clients.Remove(character.Client);
			}
			ContextHandler.SendGameContextRemoveElementMessage(this.Clients, actor);
			if (actor is IContextDependant)
			{
				this.FreeContextualId((int)((sbyte)actor.Id));
			}
			if (this.m_autoMoveTimer != null && !this.Actors.OfType<IAutoMovedEntity>().Any<IAutoMovedEntity>())
			{
				this.m_autoMoveTimer.Dispose();
				this.m_autoMoveTimer = null;
			}
			actor.OnLeaveMap(this);
		}
		private void OnActorStartMoving(ContextActor actor, Path path)
		{
			System.Collections.Generic.IEnumerable<short> serverPathKeys = path.GetServerPathKeys();
			ContextHandler.SendGameMapMovementMessage(this.Clients, serverPathKeys, actor);
			BasicHandler.SendBasicNoOperationMessage(this.Clients);
		}
		private void OnActorStopMoving(ContextActor actor, Path path, bool canceled)
		{
            
            Character character = actor as Character;
            if (character != null)
            {
                this.ExecuteTrigger(CellTriggerType.END_MOVE_ON, actor.Cell, character);
            }

            if (character.Direction == DirectionsEnum.DIRECTION_SOUTH)
            {
                if (character.Level >= 200)
                {
                    character.ToggleAura(EmotesEnum.EMOTE_AURA_VAMPYRIQUE, true);
                }
                else if (character.Level >= 100)
                {
                    character.ToggleAura(EmotesEnum.EMOTE_AURA_DE_PUISSANCE, true);
                }
            }
		}
		public System.Collections.Generic.IEnumerable<Character> GetAllCharacters()
		{
			return this.GetActors<Character>();
		}
		public void ForEach(System.Action<Character> action)
		{
			foreach (Character current in this.GetAllCharacters())
			{
				action(current);
			}
		}
		public int GetNextContextualId()
		{
			int result;
			lock (this.m_contextualIds)
			{
				result = this.m_contextualIds.Pop();
			}
			return result;
		}
		public void FreeContextualId(int id)
		{
			lock (this.m_contextualIds)
			{
				this.m_contextualIds.Push(id);
			}
		}
		public bool IsActor(int id)
		{
			return this.m_actorsMap.ContainsKey(id);
		}
		public bool IsActor(RolePlayActor actor)
		{
			return this.IsActor(actor.Id);
		}
		public bool IsCellFree(short cell)
		{
			return this.Objects.All((WorldObject x) => x.Cell.Id != cell);
		}
		public bool IsCellFree(short cell, WorldObject exclude)
		{
			return exclude != null && this.Objects.All((WorldObject x) => x == exclude || x.Cell.Id != cell);
		}
		public T GetActor<T>(int id) where T : RolePlayActor
		{
			RolePlayActor rolePlayActor;
			T result;
			if (this.m_actorsMap.TryGetValue(id, out rolePlayActor))
			{
				result = (rolePlayActor as T);
			}
			else
			{
				result = default(T);
			}
			return result;
		}
		public T GetActor<T>(System.Predicate<T> predicate) where T : RolePlayActor
		{
			return this.m_actors.OfType<T>().FirstOrDefault((T entry) => predicate(entry));
		}
		public System.Collections.Generic.IEnumerable<T> GetActors<T>()
		{
			return this.m_actors.OfType<T>();
		}
		public System.Collections.Generic.IEnumerable<T> GetActors<T>(System.Predicate<T> predicate)
		{
			return 
				from entry in this.m_actors.OfType<T>()
				where predicate(entry)
				select entry;
		}
		public Cell GetCell(int id)
		{
			return this.Cells[id];
		}
		public Cell GetCell(int x, int y)
		{
			return this.Cells[(int)((System.UIntPtr)MapPoint.CoordToCellId(x, y))];
		}
		public Cell GetCell(Point pos)
		{
			return this.GetCell(pos.X, pos.Y);
		}
		public InteractiveObject GetInteractiveObject(int id)
		{
			return this.m_interactives[id];
		}
		public System.Collections.Generic.IEnumerable<InteractiveObject> GetInteractiveObjects()
		{
			return this.m_interactives.Values;
		}
		public ObjectPosition GetRandomFreePosition(bool actorFree = false)
		{
			return new ObjectPosition(this, this.GetRandomFreeCell(actorFree), this.GetRandomDirection());
		}
		public DirectionsEnum GetRandomDirection()
		{
			System.Array values = System.Enum.GetValues(typeof(DirectionsEnum));
			AsyncRandom asyncRandom = new AsyncRandom();
			return (DirectionsEnum)values.GetValue(asyncRandom.Next(0, values.Length));
		}
		public Cell GetRandomFreeCell(bool actorFree = false)
		{
			AsyncRandom asyncRandom = new AsyncRandom();
			Cell result;
			if (!actorFree)
			{
				result = this.m_freeCells[asyncRandom.Next(0, this.m_freeCells.Length)];
			}
			else
			{
				System.Collections.Generic.IEnumerable<short> excludedCells = 
					from entry in this.GetActors<RolePlayActor>()
					select entry.Cell.Id;
				Cell[] array = (
					from entry in this.m_freeCells
					where !excludedCells.Contains(entry.Id)
					select entry).ToArray<Cell>();
				result = array[asyncRandom.Next(0, array.Length)];
			}
			return result;
		}
		public Cell GetRandomAdjacentFreeCell(MapPoint cell, bool actorFree = false)
		{
			Cell result;
			if (actorFree)
			{
				System.Collections.Generic.IEnumerable<short> excludedCells = 
					from entry in this.GetActors<RolePlayActor>()
					select entry.Cell.Id;
				System.Collections.Generic.IEnumerable<MapPoint> adjacentCells = cell.GetAdjacentCells((short entry) => this.CellsInfoProvider.IsCellWalkable(entry) && !excludedCells.Contains(entry));
				MapPoint mapPoint = adjacentCells.RandomElementOrDefault<MapPoint>();
				result = ((mapPoint != null) ? this.Cells[(int)mapPoint.CellId] : null);
			}
			else
			{
				System.Collections.Generic.IEnumerable<MapPoint> adjacentCells = cell.GetAdjacentCells(new Func<short, bool>(this.CellsInfoProvider.IsCellWalkable));
				MapPoint mapPoint = adjacentCells.RandomElementOrDefault<MapPoint>();
				result = ((mapPoint != null) ? this.Cells[(int)mapPoint.CellId] : null);
			}
			return result;
		}
		public Map GetNeighbouringMap(MapNeighbour mapNeighbour)
		{
			Map result;
			switch (mapNeighbour)
			{
			case MapNeighbour.Right:
				result = this.RightNeighbour;
				break;
			case MapNeighbour.Top:
				result = this.TopNeighbour;
				break;
			case MapNeighbour.Left:
				result = this.LeftNeighbour;
				break;
			case MapNeighbour.Bottom:
				result = this.BottomNeighbour;
				break;
			default:
				throw new System.ArgumentException("mapNeighbour");
			}
			return result;
		}
		public MapNeighbour GetClientMapRelativePosition(int mapid)
		{
			return (!this.m_clientMapsAround.ContainsKey(mapid)) ? MapNeighbour.None : this.m_clientMapsAround[mapid];
		}
		public short GetCellAfterChangeMap(short currentCell, MapNeighbour mapneighbour)
		{
			short result;
			switch (mapneighbour)
			{
			case MapNeighbour.Right:
				result =  Convert.ToInt16(currentCell - 13);
				break;
			case MapNeighbour.Top:
				result =  Convert.ToInt16(currentCell + 532);
				break;
			case MapNeighbour.Left:
				result =  Convert.ToInt16(currentCell + 13);
				break;
			case MapNeighbour.Bottom:
				result =  Convert.ToInt16(currentCell - 532);
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}
		public bool Equals(Map other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || object.Equals(other.Id, this.Id));
		}
		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj.GetType() == typeof(Map) && this.Equals((Map)obj)));
		}
		public override int GetHashCode()
		{
			return (this.Record != null) ? this.Record.GetHashCode() : 0;
		}
		public static bool operator ==(Map left, Map right)
		{
			return object.Equals(left, right);
		}
		public static bool operator !=(Map left, Map right)
		{
			return !object.Equals(left, right);
		}
		private void InitializeValidators()
		{
		}
		public MapComplementaryInformationsDataMessage GetMapComplementaryInformationsDataMessage(Character character)
		{
			return new MapComplementaryInformationsDataMessage((ushort)this.SubArea.Id, this.Id, new HouseInformations[0], 
				from entry in this.m_actors
				where entry.CanBeSee(character)
				select entry.GetGameContextActorInformations(character) as GameRolePlayActorInformations, this.m_interactives.Where((System.Collections.Generic.KeyValuePair<int, InteractiveObject> entry) => entry.Value.CanBeSee(character)).Select((System.Collections.Generic.KeyValuePair<int, InteractiveObject> entry) => entry.Value.GetInteractiveElement(character)), new StatedElement[0], new MapObstacle[0], this.m_fights.Where((Fight entry) => entry.BladesVisible).Select((Fight entry) => entry.GetFightCommonInformations()));
		}
		public bool IsMerchantLimitReached()
		{
			return this.m_actors.OfType<Merchant>().Count((Merchant x) => !x.IsBagEmpty()) >= Map.MaxMerchantsPerMap;
		}
		public bool ToggleMute()
		{
			return this.IsMuted = !this.IsMuted;
		}
	}
}
