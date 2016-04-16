namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	[System.Flags]
	public enum SpellCategory
	{
		Healing = 1,
		Teleport = 2,
		Summoning = 4,
		Buff = 8,
		DamagesWater = 16,
		DamagesEarth = 32,
		DamagesAir = 64,
		DamagesFire = 128,
		DamagesNeutral = 256,
		Curse = 512,
		Damages = 496,
		None = 0,
		All = 511
	}
}
