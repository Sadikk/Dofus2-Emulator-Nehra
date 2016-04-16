using System;

namespace Stump.Server.WorldServer.Game.Fights.Triggers
{
    [Flags]
	public enum TriggerTypeFlag
	{
		NEVER = 0x00,
		TURN_BEGIN = 0x01,
		TURN_END = 0x02,
		MOVE = 0x04
	}
}
