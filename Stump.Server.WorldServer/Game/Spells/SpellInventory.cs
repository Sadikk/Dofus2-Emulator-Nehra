using Stump.Core.Reflection;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Inventory;
using System.Collections.Generic;
namespace Stump.Server.WorldServer.Game.Spells
{
	public class SpellInventory : System.Collections.IEnumerable, System.Collections.Generic.IEnumerable<CharacterSpell>
	{
		private readonly System.Collections.Generic.Dictionary<int, CharacterSpell> m_spells = new System.Collections.Generic.Dictionary<int, CharacterSpell>();
		private readonly Queue<CharacterSpellRecord> m_spellsToDelete = new Queue<CharacterSpellRecord>();
		private readonly object m_locker = new object();
		public Character Owner
		{
			get;
			private set;
		}
		public SpellInventory(Character owner)
		{
			this.Owner = owner;
		}
		internal void LoadSpells()
		{
			Stump.ORM.Database database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
			foreach (CharacterSpellRecord current in database.Query<CharacterSpellRecord>(string.Format(CharacterSpellRelator.FetchByOwner, this.Owner.Id), new object[0]))
			{
				CharacterSpell characterSpell = new CharacterSpell(current);
				this.m_spells.Add(characterSpell.Id, characterSpell);
			}
		}
		public CharacterSpell GetSpell(int id)
		{
			CharacterSpell characterSpell;
			CharacterSpell result;
			if (this.m_spells.TryGetValue(id, out characterSpell))
			{
				result = characterSpell;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public bool HasSpell(int id)
		{
			return this.m_spells.ContainsKey(id);
		}
		public bool HasSpell(CharacterSpell spell)
		{
			return this.m_spells.ContainsKey(spell.Id);
		}
		public System.Collections.Generic.IEnumerable<CharacterSpell> GetSpells()
		{
			return this.m_spells.Values;
		}
		public CharacterSpell LearnSpell(int id)
		{
			SpellTemplate spellTemplate = Singleton<SpellManager>.Instance.GetSpellTemplate(id);
			CharacterSpell result;
			if (spellTemplate == null)
			{
				result = null;
			}
			else
			{
				result = this.LearnSpell(spellTemplate);
			}
			return result;
		}
		public CharacterSpell LearnSpell(SpellTemplate template)
		{
			CharacterSpellRecord record = Singleton<SpellManager>.Instance.CreateSpellRecord(this.Owner.Record, template);
			CharacterSpell characterSpell = new CharacterSpell(record);
			this.m_spells.Add(characterSpell.Id, characterSpell);
			InventoryHandler.SendSpellUpgradeSuccessMessage(this.Owner.Client, characterSpell);
			return characterSpell;
		}
		public bool UnLearnSpell(int id)
		{
			CharacterSpell spell = this.GetSpell(id);
			bool result;
			if (spell == null)
			{
				result = true;
			}
			else
			{
				this.m_spells.Remove(id);
				this.m_spellsToDelete.Enqueue(spell.Record);
				if (spell.CurrentLevel > 1)
				{
					int num = 0;
					for (int i = 1; i < (int)spell.CurrentLevel; i++)
					{
						num += i;
					}
					Character expr_60 = this.Owner;
					expr_60.SpellsPoints += (ushort)num;
				}
				InventoryHandler.SendSpellListMessage(this.Owner.Client, true);
				result = true;
			}
			return result;
		}
		public bool UnLearnSpell(CharacterSpell spell)
		{
			return this.UnLearnSpell(spell.Id);
		}
		public bool UnLearnSpell(SpellTemplate spell)
		{
			return this.UnLearnSpell(spell.Id);
		}

        public bool CanBoostSpell(Spell spell, sbyte spellLevel, ref int neededSpellsPoints, bool send = true)
		{
			bool result;

			if (this.Owner.IsFighting())
			{
				if (send)
				{
					InventoryHandler.SendSpellUpgradeFailureMessage(this.Owner.Client);
				}
				result = false;
			}
			else
			{
				if (spell.CurrentLevel >= 6 || spell.CurrentLevel >= spellLevel)
				{
					if (send)
					{
						InventoryHandler.SendSpellUpgradeFailureMessage(this.Owner.Client);
					}
					result = false;
				}
				else
				{
                    neededSpellsPoints = ((spellLevel - 1) * (spellLevel) - (spell.CurrentLevel - 1)* spell.CurrentLevel) / 2;
                    if (this.Owner.SpellsPoints < neededSpellsPoints)
					{
						if (send)
						{
							InventoryHandler.SendSpellUpgradeFailureMessage(this.Owner.Client);
						}
						result = false;
					}
					else
					{
						if (spell.ByLevel[(int)(spell.CurrentLevel + 1)].MinPlayerLevel > (uint)this.Owner.Level)
						{
							if (send)
							{
								InventoryHandler.SendSpellUpgradeFailureMessage(this.Owner.Client);
							}
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		public bool BoostSpell(int id, sbyte spellLevel)
		{
            bool result;
			CharacterSpell spell = this.GetSpell(id);

			if (spell == null)
			{
				InventoryHandler.SendSpellUpgradeFailureMessage(this.Owner.Client);
				result = false;
			}
			else
			{
                int spellPoints = 0;
				if (!this.CanBoostSpell(spell, spellLevel, ref spellPoints, true))
				{
					result = false;
				}
				else
				{
                    this.Owner.SpellsPoints -= (ushort)spellPoints;
                    spell.CurrentLevel = (byte)spellLevel;
					InventoryHandler.SendSpellUpgradeSuccessMessage(this.Owner.Client, spell);
					result = true;
				}
			}

			return result;
		}
		public bool ForgetSpell(SpellTemplate spell)
		{
			return this.ForgetSpell(spell.Id);
		}
		public bool ForgetSpell(int id)
		{
			bool result;
			if (!this.HasSpell(id))
			{
				result = false;
			}
			else
			{
				CharacterSpell spell = this.GetSpell(id);
				result = this.ForgetSpell(spell);
			}
			return result;
		}
		public bool ForgetSpell(CharacterSpell spell)
		{
			bool result;
			if (!this.HasSpell(spell.Id))
			{
				result = false;
			}
			else
			{
				int num = 0;
				for (int i = 1; i < (int)spell.CurrentLevel; i++)
				{
					num += i;
				}
				spell.CurrentLevel = 1;
				Character expr_38 = this.Owner;
				expr_38.SpellsPoints += (ushort)num;
				InventoryHandler.SendSpellListMessage(this.Owner.Client, true);
				result = true;
			}
			return result;
		}
		public int ForgetAllSpells()
		{
			int num = 0;
			foreach (System.Collections.Generic.KeyValuePair<int, CharacterSpell> current in this.m_spells)
			{
				for (int i = 1; i < (int)current.Value.CurrentLevel; i++)
				{
					num += i;
				}
				current.Value.CurrentLevel = 1;
			}
			Character expr_63 = this.Owner;
			expr_63.SpellsPoints += (ushort)num;
			InventoryHandler.SendSpellListMessage(this.Owner.Client, true);
			this.Owner.RefreshStats();
			return num;
		}
		public void MoveSpell(int id, byte position)
		{
			CharacterSpell spell = this.GetSpell(id);
			if (spell != null)
			{
				this.Owner.Shortcuts.AddSpellShortcut((int)position, (short)id);
			}
		}
		public void Save()
		{
			lock (this.m_locker)
			{
                Stump.ORM.Database database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
				using (System.Collections.Generic.Dictionary<int, CharacterSpell>.Enumerator enumerator = this.m_spells.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						System.Collections.Generic.KeyValuePair<int, CharacterSpell> current = enumerator.Current;
						database.Save(current.Value.Record);
					}
					goto IL_79;
				}
				IL_65:
				CharacterSpellRecord poco = this.m_spellsToDelete.Dequeue();
				database.Delete(poco);
				IL_79:
				if (this.m_spellsToDelete.Count > 0)
				{
					goto IL_65;
				}
			}
		}
		public System.Collections.Generic.IEnumerator<CharacterSpell> GetEnumerator()
		{
			return this.m_spells.Values.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
