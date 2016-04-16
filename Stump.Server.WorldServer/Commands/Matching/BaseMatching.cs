using Stump.Core.Extensions;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Linq;
namespace Stump.Server.WorldServer.Commands.Matching
{
	public abstract class BaseMatching<T>
	{
		public string Pattern
		{
			get;
			protected set;
		}
		public Character Caller
		{
			get;
			private set;
		}
		public BaseMatching(string pattern)
		{
			this.Pattern = pattern;
		}
		public BaseMatching(string pattern, Character caller) : this(pattern)
		{
			this.Caller = caller;
		}
		protected abstract string GetName(T obj);
		protected abstract System.Collections.Generic.IEnumerable<T> GetSource();
		protected abstract BaseCriteria<T> GetCriteria(string pattern);
		protected virtual T[] GetByNames(string name)
		{
			return (
				from x in this.GetSource()
				where this.GetName(x).Equals(this.Pattern, System.StringComparison.InvariantCultureIgnoreCase)
				select x).ToArray<T>();
		}
		public virtual T[] FindMatchs()
		{
			System.Collections.Generic.List<T> list = new System.Collections.Generic.List<T>();
			T[] result;
			if (this.Pattern.StartsWith("{"))
			{
				if (!this.Pattern.EndsWith("}"))
				{
					throw new System.Exception("Unexcepted token. Enters special criterias between { }");
				}
				string text = this.Pattern.Substring(1, this.Pattern.Length - 2);
				string[] array = text.SplitAdvanced("&&", "\"");
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					list.AddRange(this.GetCriteria(text).GetMatchings());
				}
				result = list.ToArray();
			}
			else
			{
				if (this.Pattern.StartsWith("*"))
				{
					if (this.Pattern.EndsWith("*"))
					{
						string name = this.Pattern.Substring(1, this.Pattern.Length - 2);
						result = (
							from x in this.GetSource()
							where this.GetName(x).IndexOf(name, System.StringComparison.InvariantCultureIgnoreCase) != -1
							select x).ToArray<T>();
					}
					else
					{
						string name = this.Pattern.Substring(1, this.Pattern.Length - 1);
						result = (
							from x in this.GetSource()
							where this.GetName(x).EndsWith(name, System.StringComparison.InvariantCultureIgnoreCase)
							select x).ToArray<T>();
					}
				}
				else
				{
					if (this.Pattern.EndsWith("*"))
					{
						string name = this.Pattern.Substring(0, this.Pattern.Length - 1);
						result = (
							from x in this.GetSource()
							where this.GetName(x).StartsWith(name, System.StringComparison.InvariantCultureIgnoreCase)
							select x).ToArray<T>();
					}
					else
					{
						result = this.GetByNames(this.Pattern);
					}
				}
			}
			return result;
		}
	}
}
