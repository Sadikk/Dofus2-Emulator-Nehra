namespace Stump.Server.WorldServer.Game.Maps.Pathfinding
{
	public class NodesComparer : System.Collections.Generic.IComparer<short>
	{
		private readonly PathNode[] m_matrix;
		private readonly bool m_orderByDescending;
		public NodesComparer(PathNode[] matrix, bool orderByDescending)
		{
			this.m_matrix = matrix;
			this.m_orderByDescending = orderByDescending;
		}
		public int Compare(short a, short b)
		{
			int result;
			if (this.m_matrix[(int)a].F > this.m_matrix[(int)b].F)
			{
				result = (this.m_orderByDescending ? -1 : 1);
			}
			else
			{
				if (this.m_matrix[(int)a].F < this.m_matrix[(int)b].F)
				{
					result = (this.m_orderByDescending ? 1 : -1);
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}
	}
}
