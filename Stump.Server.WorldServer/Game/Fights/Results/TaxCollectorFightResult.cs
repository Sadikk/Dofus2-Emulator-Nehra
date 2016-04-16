using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.TaxCollector;

namespace Stump.Server.WorldServer.Game.Fights.Results
{
	public class TaxCollectorFightResult : IExperienceResult, IFightResult
	{
		public TaxCollectorNpc TaxCollector
		{
			get;
			private set;
		}
		public Fight Fight
		{
			get;
			private set;
		}
		public bool Alive
		{
			get
			{
				return true;
			}
		}
		public int Id
		{
			get
			{
				return this.TaxCollector.Id;
			}
		}
		public int Prospecting
		{
			get
			{
				return this.TaxCollector.Guild.TaxCollectorProspecting;
			}
		}
		public int Wisdom
		{
			get
			{
				return this.TaxCollector.Guild.TaxCollectorWisdom;
			}
		}
		public int Level
		{
			get
			{
				return (int)this.TaxCollector.Guild.Level;
			}
		}
		public FightLoot Loot
		{
			get;
			private set;
		}
		public int Experience
		{
			get;
			set;
		}
		public FightOutcomeEnum Outcome
		{
			get
			{
				return FightOutcomeEnum.RESULT_TAX;
			}
		}
		public TaxCollectorFightResult(TaxCollectorNpc taxCollector, Fight fight)
		{
			this.TaxCollector = taxCollector;
			this.Fight = fight;
			this.Loot = new FightLoot();
		}
		public bool CanLoot(FightTeam team)
		{
			return team is FightPlayerTeam;
		}
		public FightResultListEntry GetFightResultListEntry()
		{
            return new FightResultTaxCollectorListEntry((ushort)this.Outcome, 0, this.Loot.GetFightLoot(), this.Id, this.Alive, this.TaxCollector.Guild.Level, this.TaxCollector.Guild.GetBasicGuildInformations(), this.Experience);
		}
		public void Apply()
		{
			foreach (DroppedItem current in this.Loot.Items.Values)
			{
				ItemTemplate itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(current.ItemId);
				if (itemTemplate.Effects.Count > 0)
				{
					int num = 0;
					while ((long)num < (long)((ulong)current.Amount))
					{
						TaxCollectorItem item = Singleton<ItemManager>.Instance.CreateTaxCollectorItem(this.TaxCollector, current.ItemId, current.Amount);
						this.TaxCollector.Bag.AddItem(item);
						num++;
					}
				}
				else
				{
					TaxCollectorItem item = Singleton<ItemManager>.Instance.CreateTaxCollectorItem(this.TaxCollector, current.ItemId, current.Amount);
					this.TaxCollector.Bag.AddItem(item);
				}
			}
			this.TaxCollector.GatheredExperience += this.Experience;
			this.TaxCollector.GatheredKamas += this.Loot.Kamas;
		}
		public void AddEarnedExperience(int experience)
		{
			this.Experience += (int)((double)experience * 0.1);
		}
	}
}
