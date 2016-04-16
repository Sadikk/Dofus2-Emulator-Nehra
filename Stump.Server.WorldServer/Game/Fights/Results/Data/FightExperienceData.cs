using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Fights.Results.Data
{
	public class FightExperienceData : FightResultAdditionalData
	{
		public bool ShowExperience
		{
			get;
			set;
		}
		public bool ShowExperienceLevelFloor
		{
			get;
			set;
		}
		public bool ShowExperienceNextLevelFloor
		{
			get;
			set;
		}
		public bool ShowExperienceFightDelta
		{
			get;
			set;
		}
		public bool ShowExperienceForGuild
		{
			get;
			set;
		}
		public bool ShowExperienceForMount
		{
			get;
			set;
		}
		public bool IsIncarnationExperience
		{
			get;
			set;
		}
		public int ExperienceFightDelta
		{
			get;
			set;
		}
		public int ExperienceForGuild
		{
			get;
			set;
		}
		public int ExperienceForMount
		{
			get;
			set;
		}
		public FightExperienceData(Character character) : base(character)
		{
		}
		public override Stump.DofusProtocol.Types.FightResultAdditionalData GetFightResultAdditionalData()
		{
            return new FightResultExperienceData(
                this.ShowExperience, 
                this.ShowExperienceLevelFloor, 
                this.ShowExperienceNextLevelFloor, 
                this.ShowExperienceFightDelta, 
                this.ShowExperienceForGuild,
                this.ShowExperienceForMount, 
                this.IsIncarnationExperience,
                (ulong)base.Character.Experience,
                this.ShowExperienceLevelFloor ? (ulong)base.Character.LowerBoundExperience : 0,
                this.ShowExperienceNextLevelFloor ? (ulong)base.Character.UpperBoundExperience : (ulong)base.Character.Experience, 
                this.ExperienceFightDelta, (uint)this.ExperienceForGuild,
                (uint)this.ExperienceForMount, 
                1);
		}
		public override void Apply()
		{
			base.Character.AddExperience(this.ExperienceFightDelta);
			if (base.Character.Guild != null && this.ExperienceForGuild > 0)
			{
				base.Character.GuildMember.AddXP((long)this.ExperienceForGuild);
			}
		}
	}
}
