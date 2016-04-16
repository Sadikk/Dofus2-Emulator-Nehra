using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Fights.Results.Data;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Handlers.Characters;

namespace Stump.Server.WorldServer.Game.Fights.Results
{
	public class FightPlayerResult : FightResult<CharacterFighter>, IExperienceResult, IPvpResult
	{
		public Character Character
		{
			get
			{
				return base.Fighter.Character;
			}
		}
		public new byte Level
		{
			get
			{
				return this.Character.Level;
			}
		}
		public FightExperienceData ExperienceData
		{
			get;
			private set;
		}
		public FightPvpData PvpData
		{
			get;
			private set;
		}
		public FightPlayerResult(CharacterFighter fighter, FightOutcomeEnum outcome, FightLoot loot) : base(fighter, outcome, loot)
		{
		}
		public override bool CanLoot(FightTeam team)
		{
			return base.Fighter.Team == team;
		}
		public override FightResultListEntry GetFightResultListEntry()
		{
			System.Collections.Generic.List<Stump.DofusProtocol.Types.FightResultAdditionalData> list = new System.Collections.Generic.List<Stump.DofusProtocol.Types.FightResultAdditionalData>();
			if (this.ExperienceData != null)
			{
				list.Add(this.ExperienceData.GetFightResultAdditionalData());
			}
			if (this.PvpData != null)
			{
				list.Add(this.PvpData.GetFightResultAdditionalData());
			}
			return new FightResultPlayerListEntry((ushort)base.Outcome, 0, base.Loot.GetFightLoot(), base.Id, base.Alive, this.Level, list);
		}
		public override void Apply()
		{
			this.Character.Inventory.AddKamas(base.Loot.Kamas);
			foreach (DroppedItem current in base.Loot.Items.Values)
			{
				ItemTemplate itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(current.ItemId);
                if (itemTemplate != null)
                {
                    if (itemTemplate.Effects.Count > 0)
                    {
                        int num = 0;
                        while ((long)num < (long)((ulong)current.Amount))
                        {
                            BasePlayerItem item = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Character, current.ItemId, 1u, false);
                            this.Character.Inventory.AddItem(item);
                            num++;
                        }
                    }
                    else
                    {
                        BasePlayerItem item = Singleton<ItemManager>.Instance.CreatePlayerItem(this.Character, current.ItemId, current.Amount, false);
                        this.Character.Inventory.AddItem(item);
                    }
                }
			}
			if (this.ExperienceData != null)
			{
				this.ExperienceData.Apply();
			}
			if (this.PvpData != null)
			{
				this.PvpData.Apply();
			}
			CharacterHandler.SendCharacterStatsListMessage(this.Character.Client);
		}
		public void AddEarnedExperience(int experience)
		{
			if (!base.Fighter.HasLeft())
			{
				if (this.ExperienceData == null)
				{
					this.ExperienceData = new FightExperienceData(this.Character);
				}
				if (this.Character.GuildMember != null && this.Character.GuildMember.GivenPercent > 0)
				{
					int num = (int)((double)experience * ((double)this.Character.GuildMember.GivenPercent * 0.01));
					int num2 = (int)this.Character.Guild.AdjustGivenExperience(this.Character, (long)num);
					num2 = ((num2 > Guild.MaxGuildXP) ? Guild.MaxGuildXP : num2);
                    experience = (int)(experience * (100.0 - this.Character.GuildMember.GivenPercent) * 0.01);

					if (num2 > 0)
					{
						this.ExperienceData.ShowExperienceForGuild = true;
						this.ExperienceData.ExperienceForGuild += num2;
					}
				}
				this.ExperienceData.ShowExperienceFightDelta = true;
				this.ExperienceData.ShowExperience = true;
				this.ExperienceData.ShowExperienceLevelFloor = this.Character.Level != 200;
                this.ExperienceData.ShowExperienceNextLevelFloor = this.Character.Level != 200;
				this.ExperienceData.ExperienceFightDelta += experience;
			}
		}
		public void SetEarnedHonor(short honor, short dishonor)
		{
			if (this.PvpData == null)
			{
				this.PvpData = new FightPvpData(this.Character);
			}
			this.PvpData.HonorDelta = honor;
			this.PvpData.DishonorDelta = dishonor;
			this.PvpData.Honor = this.Character.Honor;
			this.PvpData.Dishonor = this.Character.Dishonor;
			this.PvpData.Grade = (byte)this.Character.AlignmentGrade;
			this.PvpData.MinHonorForGrade = this.Character.LowerBoundHonor;
			this.PvpData.MaxHonorForGrade = this.Character.UpperBoundHonor;
		}
	}
}
