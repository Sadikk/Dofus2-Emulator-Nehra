using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.Fight;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Fights
{
	public class TimeLine
	{
		internal System.Collections.Generic.List<FightActor> Fighters
		{
			get;
			private set;
		}

        // PROPERTIES
		public Fight Fight
		{
			get;
			private set;
		}
		public FightActor Current
		{
			get
			{
				return (this.Index == -1 || this.Index >= this.Fighters.Count) ? null : this.Fighters[this.Index];
			}
		}
		public int Index
		{
			get;
			private set;
		}
		public int Count
		{
			get
			{
				return this.Fighters.Count;
			}
		}
		public short RoundNumber
		{
			get;
			private set;
		}
		public bool NewRound
		{
			get;
			private set;
		}

        // CONSTRUCTORS
		public TimeLine(Fight fight)
		{
            this.Fight = fight;
			this.Fighters = new System.Collections.Generic.List<FightActor>();
		}

        // METHODS
		public bool RemoveFighter(FightActor fighter)
		{
			bool result;
			if (!this.Fighters.Contains(fighter))
			{
				result = false;
			}
			else
			{
				int num = this.Fighters.IndexOf(fighter);
				this.Fighters.Remove(fighter);
				if (num <= this.Index && num > 0)
				{
					this.Index--;
				}
				result = true;
			}
			return result;
		}
		public bool InsertFighter(FightActor fighter, int index)
		{
			bool result;
			if (index > this.Fighters.Count)
			{
				result = false;
			}
			else
			{
				this.Fighters.Insert(index, fighter);
				if (index <= this.Index)
				{
					this.Index++;
				}
				result = true;
			}
			return result;
		}
		public bool SelectNextFighter()
		{
			bool result;
			if (this.Fighters.Count == 0)
			{
				this.Index = -1;
				result = false;
			}
			else
			{
				int num = 0;
				int num2 = (this.Index + 1 < this.Fighters.Count) ? (this.Index + 1) : 0;
				if (num2 == 0)
				{
					this.RoundNumber += 1;
					this.NewRound = true;
				}
				else
				{
					this.NewRound = false;
				}
				while (!this.Fighters[num2].CanPlay() && num < this.Fighters.Count)
				{
					num2 = ((num2 + 1 < this.Fighters.Count) ? (num2 + 1) : 0);
					if (num2 == 0)
					{
						this.RoundNumber += 1;
						this.NewRound = true;
					}
					num++;
				}
				if (!this.Fighters[num2].CanPlay())
				{
					this.Index = -1;
					result = false;
				}
				else
				{
					this.Index = num2;
					result = true;
				}
			}
			return result;
		}
		public void OrderLine()
		{
			IOrderedEnumerable<FightActor> orderedEnumerable = 
				from entry in this.Fight.RedTeam.GetAllFighters()
				orderby entry.Stats[PlayerFields.Initiative].Total descending
				select entry;
			IOrderedEnumerable<FightActor> orderedEnumerable2 = 
				from entry in this.Fight.BlueTeam.GetAllFighters()
				orderby entry.Stats[PlayerFields.Initiative].Total descending
				select entry;
			bool flag = orderedEnumerable.First<FightActor>().Stats[PlayerFields.Initiative].Total > orderedEnumerable2.First<FightActor>().Stats[PlayerFields.Initiative].Total;
			System.Collections.Generic.IEnumerator<FightActor> enumerator = orderedEnumerable.GetEnumerator();
			System.Collections.Generic.IEnumerator<FightActor> enumerator2 = orderedEnumerable2.GetEnumerator();
			System.Collections.Generic.List<FightActor> list = new System.Collections.Generic.List<FightActor>();
			bool flag2;
			bool flag3;
			while ((flag2 = enumerator.MoveNext()) | (flag3 = enumerator2.MoveNext()))
			{
				if (flag)
				{
					if (flag2)
					{
						list.Add(enumerator.Current);
					}
					if (flag3)
					{
						list.Add(enumerator2.Current);
					}
				}
				else
				{
					if (flag3)
					{
						list.Add(enumerator2.Current);
					}
					if (flag2)
					{
						list.Add(enumerator.Current);
					}
				}
			}
			this.Fighters = list;
			this.Index = -1;
		}
	}
}
