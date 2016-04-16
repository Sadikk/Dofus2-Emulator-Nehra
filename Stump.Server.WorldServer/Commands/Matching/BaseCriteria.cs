namespace Stump.Server.WorldServer.Commands.Matching
{
	public abstract class BaseCriteria<T>
	{
		public BaseMatching<T> Matching
		{
			get;
			protected set;
		}
		public string Pattern
		{
			get;
			protected set;
		}
		public BaseCriteria(BaseMatching<T> matching, string pattern)
		{
			this.Matching = matching;
			this.Pattern = pattern;
		}
		public abstract T[] GetMatchings();
	}
}
