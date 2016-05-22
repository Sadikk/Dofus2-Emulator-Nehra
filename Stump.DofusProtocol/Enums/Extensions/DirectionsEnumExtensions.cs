using System;

namespace Stump.DofusProtocol.Enums.Extensions
{         
	public static class DirectionsEnumExtensions
	{
		public static DirectionsEnum GetOpposedDirection(this DirectionsEnum direction)
		{
			return (DirectionsEnum)Math.Abs(direction - DirectionsEnum.DIRECTION_WEST);
		}

        public static DirectionsEnum GetPullableDirection(this DirectionsEnum direction)
        {
            switch(direction)
            {
                case DirectionsEnum.DIRECTION_EAST:
                    return DirectionsEnum.DIRECTION_NORTH_EAST;
                case DirectionsEnum.DIRECTION_WEST:
                    return DirectionsEnum.DIRECTION_NORTH_WEST;
                case DirectionsEnum.DIRECTION_NORTH:
                    return DirectionsEnum.DIRECTION_SOUTH_EAST;
                case DirectionsEnum.DIRECTION_SOUTH:
                    return DirectionsEnum.DIRECTION_SOUTH_WEST;
                default:
                    return direction;
            }
        }
	}
}
