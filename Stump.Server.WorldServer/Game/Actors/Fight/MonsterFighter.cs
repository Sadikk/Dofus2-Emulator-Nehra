using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Actors.Stats;
using Stump.Server.WorldServer.Game.Fights.Results;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Formulas;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public sealed class MonsterFighter : AIFighter
	{
		private readonly System.Collections.Generic.Dictionary<DroppableItem, int> m_dropsCount = new System.Collections.Generic.Dictionary<DroppableItem, int>();
		private readonly StatsFields m_stats;
		public Monster Monster
		{
			get;
			private set;
		}
		public override string Name
		{
			get
			{
				return this.Monster.Template.Name;
			}
		}
		public override ObjectPosition MapPosition
		{
			get
			{
				return this.Monster.Group.Position;
			}
		}
		public override short Level
		{
			get
			{
				return (short)this.Monster.Grade.Level;
			}
		}
		public override StatsFields Stats
		{
			get
			{
				return this.m_stats;
			}
		}
		public MonsterFighter(FightTeam team, Monster monster) : base(team, monster.Grade.Spells.ToArray(), monster.Grade.MonsterId)
		{
			this.Id = (int)base.Fight.GetNextContextualId();
			this.Monster = monster;
			this.Look = monster.Look.Clone();
			this.m_stats = new StatsFields(this);
			this.m_stats.Initialize(this.Monster.Grade);
			Cell cell;
			base.Fight.FindRandomFreeCell(this, out cell, false);
			this.Position = new ObjectPosition(monster.Group.Map, cell, monster.Group.Direction);
		}
		public override int GetTackledAP()
		{
			return 0;
		}
		public override int GetTackledMP()
		{
			return 0;
		}
		public override uint GetDroppedKamas()
		{
			AsyncRandom asyncRandom = new AsyncRandom();
			return (uint)asyncRandom.Next(this.Monster.Template.MinDroppedKamas, this.Monster.Template.MaxDroppedKamas + 1);
		}
		public override int GetGivenExperience()
		{
			return this.Monster.Grade.GradeXp;
		}
        public override System.Collections.Generic.IEnumerable<DroppedItem> RollLoot(IFightResult looter, int challengeBonus)
		{
			System.Collections.Generic.IEnumerable<DroppedItem> result;
			if (!base.IsDead())
			{
				result = new DroppedItem[0];
			}
			else
			{
				AsyncRandom asyncRandom = new AsyncRandom();
				System.Collections.Generic.List<DroppedItem> list = new System.Collections.Generic.List<DroppedItem>();
				int prospectingSum = base.OpposedTeam.GetAllFighters<CharacterFighter>().Sum((CharacterFighter entry) => entry.Stats[PlayerFields.Prospecting].Total);

                foreach (DroppableItem current in 
					from droppableItem in this.Monster.Template.DroppableItems
					where prospectingSum >= droppableItem.ProspectingLock
					select droppableItem)
				{
					int num = 0;
					while (num < current.RollsCounter && (current.DropLimit <= 0 || !this.m_dropsCount.ContainsKey(current) || this.m_dropsCount[current] < current.DropLimit))
					{
						double num2 = (double)asyncRandom.Next(0, 100) + asyncRandom.NextDouble();
                        double num3 = FightFormulas.AdjustDropChance(looter, current, this.Monster, this.Fight.GetFightBonus());
						if (num3 >= num2)
						{
							list.Add(new DroppedItem((int)current.ItemId, 1u));
							if (!this.m_dropsCount.ContainsKey(current))
							{
								this.m_dropsCount.Add(current, 1);
							}
							else
							{
								System.Collections.Generic.Dictionary<DroppableItem, int> dropsCount;
								DroppableItem key;
								(dropsCount = this.m_dropsCount)[key = current] = dropsCount[key] + 1;
							}
						}
						num++;
					}
				}
				result = list;
			}
			return result;
		}
		public override GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return this.GetGameFightFighterInformations();
		}
		public override GameFightFighterInformations GetGameFightFighterInformations(WorldClient client = null)
		{
			return new GameFightMonsterInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(client), base.Team.Id,0, base.IsAlive(), this.GetGameFightMinimalStats(client), Enumerable.Empty<ushort>(), (ushort)this.Monster.Template.Id, (sbyte)this.Monster.Grade.GradeId);
		}
		public override FightTeamMemberInformations GetFightTeamMemberInformations()
		{
			return new FightTeamMemberMonsterInformations(this.Id, this.Monster.Template.Id, (sbyte)this.Monster.Grade.GradeId);
		}
        public override GameFightFighterLightInformations GetGameFightFighterLightInformations()
        {
            return new GameFightFighterMonsterLightInformations(false, IsAlive(), Id, 0, (ushort)Level, 0, (ushort)Monster.Template.Id);
        }
		public override string GetMapRunningFighterName()
		{
			return this.Monster.Template.Id.ToString();
		}
		public override string ToString()
		{
			return this.Monster.ToString();
		}
		protected override void OnDisposed()
		{
			base.OnDisposed();
			if (!this.Monster.Group.IsDisposed)
			{
				this.Monster.Group.Delete();
			}
		}
	}
}
