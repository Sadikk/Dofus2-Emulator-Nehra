using Stump.Core.Reflection;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Matching.Characters
{
	public class CharacterMatching : BaseMatching<Character>
	{
		public CharacterMatching(string pattern) : base(pattern)
		{
		}
		public CharacterMatching(string pattern, Character caller) : base(pattern, caller)
		{
		}
		protected override string GetName(Character obj)
		{
			return obj.Name;
		}
		protected override System.Collections.Generic.IEnumerable<Character> GetSource()
		{
			return Singleton<World>.Instance.GetCharacters();
		}
		protected override BaseCriteria<Character> GetCriteria(string pattern)
		{
			if (!pattern.StartsWith("map", System.StringComparison.InvariantCultureIgnoreCase))
			{
				throw new System.Exception(string.Format("Criterias for string '{0}' not found", pattern));
			}
			return new MapCriteria(this, pattern);
		}
		protected override Character[] GetByNames(string name)
		{
			Character character = Singleton<World>.Instance.GetCharacter(name);
			return (character != null) ? new Character[]
			{
				character
			} : new Character[0];
		}
		public override Character[] FindMatchs()
		{
			Character[] result;
			if (base.Pattern == "*")
			{
				if (base.Caller == null)
				{
					throw new System.Exception("No caller specified");
				}
				result = new Character[]
				{
					base.Caller
				};
			}
			else
			{
				if (base.Pattern.StartsWith("!"))
				{
					base.Pattern = base.Pattern.Remove(0, 1);
					System.Collections.Generic.List<Character> list = base.FindMatchs().ToList<Character>();
					list.Remove(base.Caller);
					result = list.ToArray();
				}
				else
				{
					result = base.FindMatchs();
				}
			}
			return result;
		}
	}
}
