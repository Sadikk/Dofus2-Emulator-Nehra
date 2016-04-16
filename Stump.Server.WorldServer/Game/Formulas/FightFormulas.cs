using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Fights.Results;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Formulas
{
	public class FightFormulas
	{
		public static readonly double[] GroupCoefficients = new double[]
		{
			1.0,
			1.1,
			1.5,
			2.3,
			3.1,
			3.6,
			4.2,
			4.7
		};
		public static event Func<IFightResult, int, int> WinXpModifier;
		public static event Func<IFightResult, int, int> WinKamasModifier;
		public static event Func<IFightResult, DroppableItem, double, double> DropRateModifier;
		public static int InvokeWinXpModifier(IFightResult looter, int xp)
		{
			Func<IFightResult, int, int> winXpModifier = FightFormulas.WinXpModifier;
			return (winXpModifier != null) ? winXpModifier(looter, xp) : xp;
		}
		public static int InvokeWinKamasModifier(IFightResult looter, int kamas)
		{
			Func<IFightResult, int, int> winKamasModifier = FightFormulas.WinKamasModifier;
			return (winKamasModifier != null) ? winKamasModifier(looter, kamas) : kamas;
		}
		public static double InvokeDropRateModifier(IFightResult looter, DroppableItem item, double rate)
		{
			Func<IFightResult, DroppableItem, double, double> dropRateModifier = FightFormulas.DropRateModifier;
			return (dropRateModifier != null) ? dropRateModifier(looter, item, rate) : rate;
		}
		public static int CalculateWinExp(IFightResult fighter, System.Collections.Generic.IEnumerable<FightActor> alliesResults, System.Collections.Generic.IEnumerable<FightActor> droppersResults)
		{
			MonsterFighter[] monsters = droppersResults as MonsterFighter[];

            FightActor[] droppers = (monsters != null) ? ((FightActor[])monsters) : droppersResults.ToArray<FightActor>();
			FightActor[] allies = (alliesResults as FightActor[]) ?? alliesResults.ToArray<FightActor>();

			int result;
            if (!droppers.Any<FightActor>() || !allies.Any<FightActor>())
			{
				result = 0;
			}
			else
			{
                int sumPlayersLevel = allies.Sum((FightActor entry) => (int)entry.Level);
                short maxPlayerLevel = allies.Max((FightActor entry) => entry.Level);
                int sumDroppersLevel = droppers.Sum((FightActor entry) => (int)entry.Level);
                short maxDropperLevel = droppers.Max((FightActor entry) => entry.Level);
                int sumGivenExperience = droppers.Sum((FightActor entry) => entry.GetGivenExperience());

				double num4 = 1.0;
                if (sumPlayersLevel - 5 > sumDroppersLevel)
				{
                    num4 = (double)sumDroppersLevel / (double)sumPlayersLevel;
				}
				else
				{
                    if (sumPlayersLevel + 10 < sumDroppersLevel)
					{
                        num4 = (double)(sumPlayersLevel + 10) / (double)sumDroppersLevel;
					}
				}
                double num5 = System.Math.Min((double)fighter.Level, System.Math.Truncate(2.5 * (double)maxDropperLevel)) / (double)sumPlayersLevel * 100.0;
				int num6 = (
                    from entry in allies
					where entry.Level >= maxPlayerLevel / 3
					select entry).Sum((FightActor entry) => 1);
				if (num6 <= 0)
				{
					num6 = 1;
				}
                double num7 = System.Math.Truncate(num5 / 100.0 * System.Math.Truncate((double)sumGivenExperience * FightFormulas.GroupCoefficients[num6 - 1] * num4));
                double num8 = (fighter.Fight.GetFightBonus() <= 0) ? 1.0 : (1.0 + (double)fighter.Fight.GetFightBonus() / 100.0);
				int xp = (int)System.Math.Truncate(System.Math.Truncate(num7 * (double)(100 + fighter.Wisdom) / 100.0) * num8 * (double)Rates.XpRate);
				result = FightFormulas.InvokeWinXpModifier(fighter, xp);
			}
			return result;
		}
		public static int AdjustDroppedKamas(IFightResult looter, int teamPP, long baseKamas)
		{
			int prospecting = looter.Prospecting;
            double num = (looter.Fight.GetFightBonus() <= 0) ? 1.0 : (1.0 + (double)looter.Fight.GetFightBonus() / 100.0);
			int kamas = (int)((double)baseKamas * ((double)prospecting / (double)teamPP) * num * (double)Rates.KamasRate);

			return FightFormulas.InvokeWinKamasModifier(looter, kamas);
		}

		public static double AdjustDropChance(IFightResult looter, DroppableItem item, Monster dropper, int bonus)
		{
            double rate = item.GetDropRate((int)dropper.Grade.GradeId) * ((double)looter.Prospecting / 100.0) * ((double)bonus / 100.0 + 1.0) * (double)Rates.DropsRate;

			return FightFormulas.InvokeDropRateModifier(looter, item, rate);
		}
	}
}
