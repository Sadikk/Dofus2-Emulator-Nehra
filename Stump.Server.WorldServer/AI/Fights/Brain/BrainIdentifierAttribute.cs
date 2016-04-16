namespace Stump.Server.WorldServer.AI.Fights.Brain
{
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class BrainIdentifierAttribute : System.Attribute
	{
		public int[] Identifiers
		{
			get;
			set;
		}
		public BrainIdentifierAttribute(params int[] identifiers)
		{
			this.Identifiers = identifiers;
		}
	}
}
