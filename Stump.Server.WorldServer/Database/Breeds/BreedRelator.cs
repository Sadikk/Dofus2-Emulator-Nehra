namespace Stump.Server.WorldServer.Database.Breeds
{
	public class BreedRelator
	{
		public static string FetchQuery = "SELECT * FROM breeds LEFT JOIN breeds_items ON breeds_items.BreedId = breeds.id LEFT JOIN breeds_spells ON breeds_spells.BreedId = breeds.id";
		private Breed m_current;
		public Breed Map(Breed breed, BreedItem item, BreedSpell spell)
		{
			Breed result;
			if (breed == null)
			{
				result = this.m_current;
			}
			else
			{
				if (this.m_current != null && this.m_current.Id == breed.Id)
				{
					if (item.Id != 0)
					{
						this.m_current.Items.Add(item);
					}
					if (spell.Id != 0)
					{
						this.m_current.Spells.Add(spell);
					}
					result = null;
				}
				else
				{
					Breed current = this.m_current;
					this.m_current = breed;
					if (item.Id != 0)
					{
						this.m_current.Items.Add(item);
					}
					if (spell.Id != 0)
					{
						this.m_current.Spells.Add(spell);
					}
					result = current;
				}
			}
			return result;
		}
	}
}
