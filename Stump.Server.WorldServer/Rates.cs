using Stump.Core.Attributes;

namespace Stump.Server.WorldServer
{
	[System.Serializable]
	public static class Rates
	{
		[Variable(true)]
		public static float RegenRate = 1f;
		[Variable(true)]
		public static float XpRate = 1f;
		[Variable(true)]
		public static float KamasRate = 1f;
		[Variable(true)]
		public static float DropsRate = 1f;
	}
}
