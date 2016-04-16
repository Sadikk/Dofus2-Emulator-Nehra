using Stump.DofusProtocol.Enums;

namespace Stump.Server.WorldServer.Game.Spells.Casts
{
	public class SpellCastHandlerAttribute : System.Attribute
	{
		public int Spell
		{
			get;
			set;
		}
		public SpellCastHandlerAttribute(int spellId)
		{
			this.Spell = spellId;
		}
		public SpellCastHandlerAttribute(SpellIdEnum spellId)
		{
			this.Spell = (int)spellId;
		}
	}
}
