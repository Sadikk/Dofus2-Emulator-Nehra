using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Characters;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Characters
{
	public class ExperienceManager : DataManager<ExperienceManager>
	{
		private readonly System.Collections.Generic.Dictionary<byte, ExperienceTableEntry> m_records = new System.Collections.Generic.Dictionary<byte, ExperienceTableEntry>();
		private System.Collections.Generic.KeyValuePair<byte, ExperienceTableEntry> m_highestCharacterLevel;
		private System.Collections.Generic.KeyValuePair<byte, ExperienceTableEntry> m_highestGrade;
		private System.Collections.Generic.KeyValuePair<byte, ExperienceTableEntry> m_highestGuildLevel;

		public byte HighestCharacterLevel
		{
			get
			{
				return this.m_highestCharacterLevel.Key;
			}
		}
		public byte HighestGuildLevel
		{
			get
			{
				return this.m_highestGuildLevel.Key;
			}
		}
		public byte HighestGrade
		{
			get
			{
				return this.m_highestGrade.Key;
			}
		}

        [Initialization(InitializationPass.Fourth)]
        public override void Initialize()
        {
            foreach (ExperienceTableEntry current in base.Database.Query<ExperienceTableEntry>(ExperienceTableRelator.FetchQuery, new object[0]))
            {
                if (current.Level > 200)
                {
                    throw new System.Exception("Level cannot exceed 200 (protocol constraint)");
                }
                this.m_records.Add((byte)current.Level, current);
            }

            this.m_highestCharacterLevel = this.m_records.OrderByDescending(entry => entry.Value.CharacterExp).FirstOrDefault();
            this.m_highestGrade = this.m_records.OrderByDescending(entry => entry.Value.AlignmentHonor).FirstOrDefault();
            this.m_highestGuildLevel = this.m_records.OrderByDescending(entry => entry.Value.GuildExp).FirstOrDefault();
        }

		public long GetCharacterLevelExperience(byte level)
		{
			if (!this.m_records.ContainsKey(level))
			{
				throw new System.Exception("Level " + level + " not found");
			}
			long? characterExp = this.m_records[level].CharacterExp;
			if (!characterExp.HasValue)
			{
				throw new System.Exception("Character level " + level + " is not defined");
			}
			return characterExp.Value;
		}
		public long GetCharacterNextLevelExperience(byte level)
		{
			long result;
			if (this.m_records.ContainsKey(Convert.ToByte(level + 1)))
			{
				long? characterExp = this.m_records[Convert.ToByte(level + 1)].CharacterExp;
				if (!characterExp.HasValue)
				{
					throw new System.Exception("Character level " + level + " is not defined");
				}
				result = characterExp.Value;
			}
			else
			{
				result = long.MaxValue;
			}
			return result;
		}
		public byte GetCharacterLevel(long experience)
		{
			byte result;
			try
			{
				if (experience >= this.m_highestCharacterLevel.Value.CharacterExp)
				{
					result = this.m_highestCharacterLevel.Key;
				}
				else
				{
					result = Convert.ToByte(this.m_records.First((System.Collections.Generic.KeyValuePair<byte, ExperienceTableEntry> entry) => entry.Value.CharacterExp > experience).Key - 1);
				}
			}
			catch (System.InvalidOperationException innerException)
			{
				throw new System.Exception(string.Format("Experience {0} isn't bind to a character level", experience), innerException);
			}
			return result;
		}
		public ushort GetAlignementGradeHonor(byte grade)
		{
			if (!this.m_records.ContainsKey(grade))
			{
				throw new System.Exception("Grade " + grade + " not found");
			}
			ushort? alignmentHonor = this.m_records[grade].AlignmentHonor;
			if (!alignmentHonor.HasValue)
			{
				throw new System.Exception("Grade " + grade + " is not defined");
			}
			return alignmentHonor.Value;
		}
		public ushort GetAlignementNextGradeHonor(byte grade)
		{
			ushort result;
			if (!this.m_records.ContainsKey(Convert.ToByte(grade + 1)))
			{
				result = 17500;
			}
			else
			{
				ushort? alignmentHonor = this.m_records[Convert.ToByte(grade + 1)].AlignmentHonor;
				result =  Convert.ToUInt16((!alignmentHonor.HasValue) ? 17500 : alignmentHonor.Value);
			}
			return result;
		}
		public byte GetAlignementGrade(ushort honor)
		{
			byte result;
			try
			{
				if (honor >= this.m_highestGrade.Value.AlignmentHonor)
				{
					result = this.m_highestGrade.Key;
				}
				else
				{
					result = Convert.ToByte(this.m_records.First((System.Collections.Generic.KeyValuePair<byte, ExperienceTableEntry> entry) => entry.Value.AlignmentHonor > honor).Key - 1);
				}
			}
			catch (System.InvalidOperationException innerException)
			{
				throw new System.Exception(string.Format("Honor {0} isn't bind to a grade", honor), innerException);
			}
			return result;
		}
		public long GetGuildLevelExperience(byte level)
		{
			if (!this.m_records.ContainsKey(level))
			{
				throw new System.Exception("Level " + level + " not found");
			}
			long? guildExp = this.m_records[level].GuildExp;
			if (!guildExp.HasValue)
			{
				throw new System.Exception("Guild level " + level + " is not defined");
			}
			return guildExp.Value;
		}
		public long GetGuildNextLevelExperience(byte level)
		{
			long result;
			if (this.m_records.ContainsKey(Convert.ToByte(level + 1)))
			{
				long? guildExp = this.m_records[Convert.ToByte(level + 1)].GuildExp;
				if (!guildExp.HasValue)
				{
					throw new System.Exception("Guild level " + level + " is not defined");
				}
				result = guildExp.Value;
			}
			else
			{
				result = 9223372036854775807L;
			}
			return result;
		}
		public byte GetGuildLevel(long experience)
		{
			byte result;
			try
			{
				if (experience >= this.m_highestGuildLevel.Value.GuildExp)
				{
					result = this.m_highestGuildLevel.Key;
				}
				else
				{
                    result = Convert.ToByte(this.m_records.First((System.Collections.Generic.KeyValuePair<byte, ExperienceTableEntry> entry) => entry.Value.GuildExp > experience).Key - 1);
				}
			}
			catch (System.InvalidOperationException innerException)
			{
				throw new System.Exception(string.Format("Experience {0} isn't bind to a guild level", experience), innerException);
			}
			return result;
		}
	}
}
