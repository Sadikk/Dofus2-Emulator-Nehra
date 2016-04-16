using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions
{
	public abstract class ConditionExpression
	{
		public static ConditionExpression Parse(string str)
		{
			ConditionParser conditionParser = new ConditionParser(str);
			return conditionParser.Parse();
		}
		public abstract bool Eval(Character character);
	}
}
