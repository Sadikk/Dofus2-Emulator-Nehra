using Stump.Core.Reflection;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Matching.Characters
{
	public class MapCriteria : BaseCriteria<Character>
	{
		public MapCriteria(BaseMatching<Character> matching, string pattern) : base(matching, pattern)
		{
		}
		public override Character[] GetMatchings()
		{
			int num = 0;
			Character[] result;
			if (base.Pattern[0 + 1] == ':')
			{
				num++;
				string s = base.Pattern.Substring(num, base.Pattern.IndexOf("}", num) - num);
				int num2;
				if (int.TryParse(s, out num2))
				{
					throw new System.Exception("Invalid token. Did you mean {map} or {map:#} ? (# is a map id)");
				}
				Map map = Singleton<World>.Instance.GetMap(num2);
				if (map == null)
				{
					throw new System.Exception(string.Format("Map {0} not found", num2));
				}
				result = map.GetAllCharacters().ToArray<Character>();
			}
			else
			{
				if (!base.Pattern.Equals("map", System.StringComparison.InvariantCultureIgnoreCase))
				{
					throw new System.Exception("Invalid token. Did you mean {map} or {map:#} ? (# is a map id)");
				}
				if (base.Matching.Caller == null)
				{
					throw new System.Exception("Caller not specified, cannot retrieve current map");
				}
				result = base.Matching.Caller.Map.GetAllCharacters().ToArray<Character>();
			}
			return result;
		}
	}
}
