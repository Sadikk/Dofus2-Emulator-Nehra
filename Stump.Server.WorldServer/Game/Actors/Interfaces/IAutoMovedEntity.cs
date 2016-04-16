using Stump.Server.WorldServer.Game.Maps.Pathfinding;

namespace Stump.Server.WorldServer.Game.Actors.Interfaces
{
	public interface IAutoMovedEntity
	{
		System.DateTime NextMoveDate
		{
			get;
			set;
		}
		System.DateTime LastMoveDate
		{
			get;
		}
		bool StartMove(Path path);
	}
}
