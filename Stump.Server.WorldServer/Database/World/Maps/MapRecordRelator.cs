namespace Stump.Server.WorldServer.Database.World.Maps
{
	public class MapRecordRelator
	{
		public static string FetchQuery = "SELECT * FROM world_maps INNER JOIN world_maps_positions ON world_maps_positions.Id = world_maps.Id";
		public MapRecord Map(MapRecord map, MapPositionRecord position)
		{
			map.Position = position;
			position.Map = map;
			return map;
		}
	}
}
