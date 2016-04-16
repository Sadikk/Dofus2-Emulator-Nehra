using Stump.Server.WorldServer.Database.Characters;

namespace Stump.Server.WorldServer.Game.Spells
{
	public class CharacterSpell : Spell
	{
		public CharacterSpellRecord Record
		{
			get;
			private set;
		}
		public CharacterSpell(CharacterSpellRecord record) : base(record)
		{
			this.Record = record;
		}
	}
}
