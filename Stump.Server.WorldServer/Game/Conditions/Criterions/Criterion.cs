namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public abstract class Criterion : ConditionExpression
	{
		public ComparaisonOperatorEnum Operator
		{
			get;
			set;
		}
		public string Literal
		{
			get;
			set;
		}
		public static ComparaisonOperatorEnum? TryGetOperator(char c)
		{
			ComparaisonOperatorEnum? result;
			if (c != '!')
			{
				switch (c)
				{
				case '<':
					result = new ComparaisonOperatorEnum?(ComparaisonOperatorEnum.INFERIOR);
					break;
				case '=':
					result = new ComparaisonOperatorEnum?(ComparaisonOperatorEnum.EQUALS);
					break;
				case '>':
					result = new ComparaisonOperatorEnum?(ComparaisonOperatorEnum.SUPERIOR);
					break;
				default:
					if (c != '~')
					{
						result = null;
					}
					else
					{
						result = new ComparaisonOperatorEnum?(ComparaisonOperatorEnum.LIKE);
					}
					break;
				}
			}
			else
			{
				result = new ComparaisonOperatorEnum?(ComparaisonOperatorEnum.INEQUALS);
			}
			return result;
		}
		public static char GetOperatorChar(ComparaisonOperatorEnum op)
		{
			char result;
			switch (op)
			{
			case ComparaisonOperatorEnum.EQUALS:
				result = '=';
				break;
			case ComparaisonOperatorEnum.INEQUALS:
				result = '!';
				break;
			case ComparaisonOperatorEnum.SUPERIOR:
				result = '>';
				break;
			case ComparaisonOperatorEnum.INFERIOR:
				result = '<';
				break;
			case ComparaisonOperatorEnum.LIKE:
				result = '~';
				break;
			case ComparaisonOperatorEnum.STARTWITH:
				result = 's';
				break;
			case ComparaisonOperatorEnum.STARTWITHLIKE:
				result = 'S';
				break;
			case ComparaisonOperatorEnum.ENDWITH:
				result = 'e';
				break;
			case ComparaisonOperatorEnum.ENDWITHLIKE:
				result = 'E';
				break;
			case ComparaisonOperatorEnum.VALID:
				result = 'v';
				break;
			case ComparaisonOperatorEnum.INVALID:
				result = 'i';
				break;
			case ComparaisonOperatorEnum.UNKNOWN_1:
				result = '#';
				break;
			case ComparaisonOperatorEnum.UNKNOWN_2:
				result = '/';
				break;
			case ComparaisonOperatorEnum.UNKNOWN_3:
				result = 'X';
				break;
			default:
				throw new System.Exception(string.Format("{0} is not a valid comparaison operator", op));
			}
			return result;
		}
		public static Criterion CreateCriterionByName(string name)
		{
			Criterion result;
            if (!StatsCriterion.IsStatsIdentifier(name))
            {
                switch (name)
                {
                    case "PX":
                        result = new AdminRightsCriterion();
                        return result;
                    case "Pa":
                        result = new AlignementLevelCriterion();
                        return result;
                    case "Ps":
                        result = new AlignmentCriterion();
                        return result;
                    case "PU":
                        result = new BonesCriterion();
                        return result;
                    case "PG":
                        result = new BreedCriterion();
                        return result;
                    case "PE":
                        result = new EmoteCriterion();
                        return result;
                    case "Pb":
                        result = new FriendListCriterion();
                        return result;
                    case "Pg":
                        result = new GiftCriterion();
                        return result;
                    case "PO":
                        result = new HasItemCriterion();
                        return result;
                    case "PJ":
                        result = new JobCriterion();
                        return result;
                    case "PK":
                        result = new KamaCriterion();
                        return result;
                    case "PL":
                        result = new LevelCriterion();
                        return result;
                    case "MK":
                        result = new MapCharactersCriterion();
                        return result;
                    case "PR":
                        result = new MariedCriterion();
                        return result;
                    case "P¨Q":
                        result = new MaxRankCriterion();
                        return result;
                    case "SG":
                        result = new MonthCriterion();
                        return result;
                    case "PN":
                        result = new NameCriterion();
                        return result;
                    case "Pe":
                        result = new PreniumAccountCriterion();
                        return result;
                    case "PP":
                    case "Pp":
                        result = new PvpRankCriterion();
                        return result;
                    case "Qa":
                        result = new QuestActiveCriterion();
                        return result;
                    case "Qf":
                        result = new QuestDoneCriterion();
                        return result;
                    case "Qc":
                        result = new QuestStartableCriterion();
                        return result;
                    case "Pq":
                        result = new RankCriterion();
                        return result;
                    case "Pf":
                        result = new RideCriterion();
                        return result;
                    case "SI":
                        result = new ServerCriterion();
                        return result;
                    case "PS":
                        result = new SexCriterion();
                        return result;
                    case "Pi":
                    case "PI":
                        result = new SkillCriterion();
                        return result;
                    case "PA":
                        result = new SoulStoneCriterion();
                        return result;
                    case "Pr":
                        result = new SpecializationCriterion();
                        return result;
                    case "Sc":
                        result = new StaticCriterion();
                        return result;
                    case "PB":
                        result = new SubAreaCriterion();
                        return result;
                    case "PZ":
                        result = new SubscribeCriterion();
                        return result;
                    case "BI":
                        result = new UnusableCriterion();
                        return result;
                    case "PW":
                        result = new WeightCriterion();
                        return result;
                    case "Px":
                        result = new GuildRightsCriterion();
                        return result;
                    case "Ox":
                        result = new AllianceRightsCriterion();
                        return result;
                    case "Oc":
                        result = new IgnoreCriterion();
                        return result;
                }
                throw new System.Exception(string.Format("Criterion {0} doesn't not exist or not handled", name));
            }
			result = new StatsCriterion(name);
			return result;
		}
		public abstract void Build();
		protected bool Compare(object obj, object comparand)
		{
			bool result;
			switch (this.Operator)
			{
			case ComparaisonOperatorEnum.EQUALS:
				result = obj.Equals(comparand);
				break;
			case ComparaisonOperatorEnum.INEQUALS:
				result = !obj.Equals(comparand);
				break;
			default:
				throw new System.NotImplementedException(string.Format("Cannot use {0} comparator on objects {1} and {2}", this.Operator, obj, comparand));
			}
			return result;
		}
		protected bool Compare<T>(System.IComparable<T> obj, T comparand)
		{
			int num = obj.CompareTo(comparand);
			bool result;
			switch (this.Operator)
			{
			case ComparaisonOperatorEnum.EQUALS:
				result = (num == 0);
				break;
			case ComparaisonOperatorEnum.INEQUALS:
				result = (num != 0);
				break;
			case ComparaisonOperatorEnum.SUPERIOR:
				result = (num > 0);
				break;
			case ComparaisonOperatorEnum.INFERIOR:
				result = (num < 0);
				break;
			default:
				throw new System.NotImplementedException(string.Format("Cannot use {0} comparator on IComparable {1} and {2}", this.Operator, obj, comparand));
			}
			return result;
		}
		protected bool Compare(string str, string comparand)
		{
			switch (this.Operator)
			{
			case ComparaisonOperatorEnum.EQUALS:
			{
				bool result = str == comparand;
				return result;
			}
			case ComparaisonOperatorEnum.INEQUALS:
			{
				bool result = str != comparand;
				return result;
			}
			case ComparaisonOperatorEnum.LIKE:
			{
				bool result = str.Equals(comparand, System.StringComparison.InvariantCultureIgnoreCase);
				return result;
			}
			case ComparaisonOperatorEnum.STARTWITH:
			{
				bool result = str.StartsWith(comparand);
				return result;
			}
			case ComparaisonOperatorEnum.STARTWITHLIKE:
			{
				bool result = str.StartsWith(comparand, System.StringComparison.InvariantCultureIgnoreCase);
				return result;
			}
			case ComparaisonOperatorEnum.ENDWITH:
			{
				bool result = str.EndsWith(comparand);
				return result;
			}
			case ComparaisonOperatorEnum.ENDWITHLIKE:
			{
				bool result = str.EndsWith(comparand, System.StringComparison.InvariantCultureIgnoreCase);
				return result;
			}
			}
			throw new System.NotImplementedException(string.Format("Cannot use {0} comparator on strings '{1}' and '{2}'", this.Operator, str, comparand));
		}
		protected string FormatToString(string identifier)
		{
			return string.Format("{0}{1}{2}", identifier, Criterion.GetOperatorChar(this.Operator), this.Literal);
		}
	}
}
