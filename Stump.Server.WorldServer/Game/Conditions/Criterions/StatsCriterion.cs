using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class StatsCriterion : Criterion
	{
		private static readonly System.Collections.Generic.Dictionary<string, PlayerFields> CriterionsBinds = new System.Collections.Generic.Dictionary<string, PlayerFields>
		{

			{
				"CA",
				PlayerFields.Agility
			},

			{
				"CC",
				PlayerFields.Chance
			},

			{
				"CS",
				PlayerFields.Strength
			},

			{
				"CI",
				PlayerFields.Intelligence
			},

			{
				"CW",
				PlayerFields.Wisdom
			},

			{
				"CV",
				PlayerFields.Vitality
			},

			{
				"CL",
				PlayerFields.Health
			},

			{
				"CM",
				PlayerFields.MP
			},

			{
				"CP",
				PlayerFields.AP
			},

			{
				"Ct",
				PlayerFields.TackleEvade
			},

			{
				"CT",
				PlayerFields.TackleBlock
			}
		};
		private static readonly System.Collections.Generic.Dictionary<string, PlayerFields> CriterionsStatsBaseBinds = new System.Collections.Generic.Dictionary<string, PlayerFields>
		{

			{
				"Ca",
				PlayerFields.Agility
			},

			{
				"Cc",
				PlayerFields.Chance
			},

			{
				"Cs",
				PlayerFields.Strength
			},

			{
				"Ci",
				PlayerFields.Intelligence
			},

			{
				"Cw",
				PlayerFields.Wisdom
			},

			{
				"Cv",
				PlayerFields.Vitality
			}
		};
		private static readonly string[] ExtraCriterions = new string[]
		{
			"Ce",
			"CE",
			"CD",
			"CH"
		};
		public string Identifier
		{
			get;
			private set;
		}
		public PlayerFields? Field
		{
			get;
			set;
		}
		public bool Base
		{
			get;
			set;
		}
		public int Comparand
		{
			get;
			set;
		}
		public StatsCriterion(string identifier)
		{
			this.Identifier = identifier;
			this.Field = null;
		}
		public static bool IsStatsIdentifier(string identifier)
		{
			return StatsCriterion.CriterionsBinds.ContainsKey(identifier) || StatsCriterion.CriterionsStatsBaseBinds.ContainsKey(identifier) || StatsCriterion.ExtraCriterions.Any((string entry) => entry == identifier);
		}
		public override bool Eval(Character character)
		{
			bool result;
			if (!this.Field.HasValue)
			{
				string identifier = this.Identifier;
				if (identifier != null)
				{
					if (identifier == "Ce")
					{
						result = base.Compare<short>(character.Energy, (short)this.Comparand);
						return result;
					}
					if (identifier == "CE")
					{
						result = base.Compare<short>(character.EnergyMax, (short)this.Comparand);
						return result;
					}
					if (identifier == "CD")
					{
						result = true;
						return result;
					}
					if (identifier == "CH")
					{
						result = true;
						return result;
					}
				}
				throw new System.Exception(string.Format("Cannot eval StatsCriterion {0}, {1} is not a stats identifier", this, this.Identifier));
			}
			result = base.Compare<int>(this.Base ? character.Stats[this.Field.Value].Base : character.Stats[this.Field.Value].Total, this.Comparand);
			return result;
		}
		public override void Build()
		{
			if (StatsCriterion.CriterionsBinds.ContainsKey(this.Identifier))
			{
				this.Field = new PlayerFields?(StatsCriterion.CriterionsBinds[this.Identifier]);
			}
			else
			{
				if (StatsCriterion.CriterionsStatsBaseBinds.ContainsKey(this.Identifier))
				{
					this.Field = new PlayerFields?(StatsCriterion.CriterionsStatsBaseBinds[this.Identifier]);
					this.Base = true;
				}
				else
				{
					if (StatsCriterion.ExtraCriterions.All((string entry) => entry != this.Identifier))
					{
						throw new System.Exception(string.Format("Cannot build StatsCriterion, {0} is not a stats identifier", this.Identifier));
					}
				}
			}
			int comparand;
			if (!int.TryParse(base.Literal, out comparand))
			{
				throw new System.Exception(string.Format("Cannot build StatsCriterion, {0} is not a valid integer", base.Literal));
			}
			this.Comparand = comparand;
		}
		public override string ToString()
		{
			return base.FormatToString(this.Identifier);
		}
	}
}
