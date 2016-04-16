using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database.Shortcuts;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Handlers.Shortcuts;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Shortcuts
{
	public class ShortcutBar
	{
		public const int MaxSlot = 40;
		private readonly object m_locker = new object();
		private readonly Queue<Stump.Server.WorldServer.Database.Shortcuts.Shortcut> m_shortcutsToDelete = new Queue<Stump.Server.WorldServer.Database.Shortcuts.Shortcut>();
		private System.Collections.Generic.Dictionary<int, SpellShortcut> m_spellShortcuts = new System.Collections.Generic.Dictionary<int, SpellShortcut>();
		private System.Collections.Generic.Dictionary<int, ItemShortcut> m_itemShortcuts = new System.Collections.Generic.Dictionary<int, ItemShortcut>();
		public Character Owner
		{
			get;
			private set;
		}
		public ShortcutBar(Character owner)
		{
			this.Owner = owner;
		}
		internal void Load()
		{
            Stump.ORM.Database database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
			this.m_spellShortcuts = database.Query<SpellShortcut>(string.Format(SpellShortcutRelator.FetchByOwner, this.Owner.Id), new object[0]).ToDictionary((SpellShortcut x) => x.Slot);
			this.m_itemShortcuts = database.Query<ItemShortcut>(string.Format(ItemShortcutRelator.FetchByOwner, this.Owner.Id), new object[0]).ToDictionary((ItemShortcut x) => x.Slot);
		}
		public void AddShortcut(ShortcutBarEnum barType, Stump.DofusProtocol.Types.Shortcut shortcut)
		{
			if (shortcut is ShortcutSpell && barType == ShortcutBarEnum.SPELL_SHORTCUT_BAR)
			{
				this.AddSpellShortcut(shortcut.slot,(short) (shortcut as ShortcutSpell).spellId);
			}
			else
			{
				if (shortcut is ShortcutObjectItem && barType == ShortcutBarEnum.GENERAL_SHORTCUT_BAR)
				{
					this.AddItemShortcut(shortcut.slot, this.Owner.Inventory.TryGetItem((shortcut as ShortcutObjectItem).itemUID));
				}
				else
				{
					ShortcutHandler.SendShortcutBarAddErrorMessage(this.Owner.Client);
				}
			}
		}
		public void AddSpellShortcut(int slot, short spellId)
		{
			if (!this.IsSlotFree(slot, ShortcutBarEnum.SPELL_SHORTCUT_BAR))
			{
				this.RemoveShortcut(ShortcutBarEnum.SPELL_SHORTCUT_BAR, slot);
			}
			SpellShortcut spellShortcut = new SpellShortcut(this.Owner.Record, slot, spellId);
			this.m_spellShortcuts.Add(slot, spellShortcut);
			ShortcutHandler.SendShortcutBarRefreshMessage(this.Owner.Client, ShortcutBarEnum.SPELL_SHORTCUT_BAR, spellShortcut);
		}
		public void AddItemShortcut(int slot, BasePlayerItem item)
		{
			if (!this.IsSlotFree(slot, ShortcutBarEnum.GENERAL_SHORTCUT_BAR))
			{
				this.RemoveShortcut(ShortcutBarEnum.GENERAL_SHORTCUT_BAR, slot);
			}
			ItemShortcut itemShortcut = new ItemShortcut(this.Owner.Record, slot, item.Template.Id, item.Guid);
			this.m_itemShortcuts.Add(slot, itemShortcut);
			ShortcutHandler.SendShortcutBarRefreshMessage(this.Owner.Client, ShortcutBarEnum.GENERAL_SHORTCUT_BAR, itemShortcut);
		}
		public void SwapShortcuts(ShortcutBarEnum barType, int slot, int newSlot)
		{
			if (!this.IsSlotFree(slot, barType))
			{
				Stump.Server.WorldServer.Database.Shortcuts.Shortcut shortcut = this.GetShortcut(barType, slot);
				Stump.Server.WorldServer.Database.Shortcuts.Shortcut shortcut2 = this.GetShortcut(barType, newSlot);
				this.RemoveInternal(shortcut);
				this.RemoveInternal(shortcut2);
				if (shortcut2 != null)
				{
					shortcut2.Slot = slot;
					this.AddInternal(shortcut2);
					ShortcutHandler.SendShortcutBarRefreshMessage(this.Owner.Client, barType, shortcut2);
				}
				else
				{
					ShortcutHandler.SendShortcutBarRemovedMessage(this.Owner.Client, barType, slot);
				}
				shortcut.Slot = newSlot;
				this.AddInternal(shortcut);
				ShortcutHandler.SendShortcutBarRefreshMessage(this.Owner.Client, barType, shortcut);
			}
		}
		public void RemoveShortcut(ShortcutBarEnum barType, int slot)
		{
			Stump.Server.WorldServer.Database.Shortcuts.Shortcut shortcut = this.GetShortcut(barType, slot);
			if (shortcut != null)
			{
				if (barType == ShortcutBarEnum.SPELL_SHORTCUT_BAR)
				{
					this.m_spellShortcuts.Remove(slot);
				}
				else
				{
					if (barType == ShortcutBarEnum.GENERAL_SHORTCUT_BAR)
					{
						this.m_itemShortcuts.Remove(slot);
					}
				}
				this.m_shortcutsToDelete.Enqueue(shortcut);
				ShortcutHandler.SendShortcutBarRemovedMessage(this.Owner.Client, barType, slot);
			}
		}
		private void AddInternal(Stump.Server.WorldServer.Database.Shortcuts.Shortcut shortcut)
		{
			if (shortcut is SpellShortcut)
			{
				this.m_spellShortcuts.Add(shortcut.Slot, (SpellShortcut)shortcut);
			}
			else
			{
				if (shortcut is ItemShortcut)
				{
					this.m_itemShortcuts.Add(shortcut.Slot, (ItemShortcut)shortcut);
				}
			}
		}
		private bool RemoveInternal(Stump.Server.WorldServer.Database.Shortcuts.Shortcut shortcut)
		{
			bool result;
			if (shortcut is SpellShortcut)
			{
				result = this.m_spellShortcuts.Remove(shortcut.Slot);
			}
			else
			{
				result = (shortcut is ItemShortcut && this.m_itemShortcuts.Remove(shortcut.Slot));
			}
			return result;
		}
		public int GetNextFreeSlot(ShortcutBarEnum barType)
		{
			int result;
			for (int i = 0; i < 40; i++)
			{
				if (this.IsSlotFree(i, barType))
				{
					result = i;
					return result;
				}
			}
			result = 40;
			return result;
		}
		public bool IsSlotFree(int slot, ShortcutBarEnum barType)
		{
			bool result;
			if (barType == ShortcutBarEnum.SPELL_SHORTCUT_BAR)
			{
				result = !this.m_spellShortcuts.ContainsKey(slot);
			}
			else
			{
				result = (barType != ShortcutBarEnum.GENERAL_SHORTCUT_BAR || !this.m_itemShortcuts.ContainsKey(slot));
			}
			return result;
		}
		public Stump.Server.WorldServer.Database.Shortcuts.Shortcut GetShortcut(ShortcutBarEnum barType, int slot)
		{
			Stump.Server.WorldServer.Database.Shortcuts.Shortcut result;
			switch (barType)
			{
			case ShortcutBarEnum.GENERAL_SHORTCUT_BAR:
				result = this.GetItemShortcut(slot);
				break;
			case ShortcutBarEnum.SPELL_SHORTCUT_BAR:
				result = this.GetSpellShortcut(slot);
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
		public System.Collections.Generic.IEnumerable<Stump.Server.WorldServer.Database.Shortcuts.Shortcut> GetShortcuts(ShortcutBarEnum barType)
		{
			System.Collections.Generic.IEnumerable<Stump.Server.WorldServer.Database.Shortcuts.Shortcut> result;
			switch (barType)
			{
			case ShortcutBarEnum.GENERAL_SHORTCUT_BAR:
				result = this.m_itemShortcuts.Values;
				break;
			case ShortcutBarEnum.SPELL_SHORTCUT_BAR:
				result = this.m_spellShortcuts.Values;
				break;
			default:
				result = new Stump.Server.WorldServer.Database.Shortcuts.Shortcut[0];
				break;
			}
			return result;
		}
		public SpellShortcut GetSpellShortcut(int slot)
		{
			SpellShortcut spellShortcut;
			return this.m_spellShortcuts.TryGetValue(slot, out spellShortcut) ? spellShortcut : null;
		}
		public ItemShortcut GetItemShortcut(int slot)
		{
			ItemShortcut itemShortcut;
			return this.m_itemShortcuts.TryGetValue(slot, out itemShortcut) ? itemShortcut : null;
		}
		public void Save()
		{
			lock (this.m_locker)
			{
                Stump.ORM.Database database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
				foreach (System.Collections.Generic.KeyValuePair<int, ItemShortcut> current in this.m_itemShortcuts)
				{
					if (current.Value.IsDirty || current.Value.IsNew)
					{
						database.Save(current.Value);
					}
				}
				using (System.Collections.Generic.Dictionary<int, SpellShortcut>.Enumerator enumerator2 = this.m_spellShortcuts.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						System.Collections.Generic.KeyValuePair<int, SpellShortcut> current2 = enumerator2.Current;
						if (current2.Value.IsDirty || current2.Value.IsNew)
						{
							database.Save(current2.Value);
						}
					}
					goto IL_FD;
				}
				IL_E3:
				Stump.Server.WorldServer.Database.Shortcuts.Shortcut shortcut = this.m_shortcutsToDelete.Dequeue();
				if (shortcut != null)
				{
					database.Delete(shortcut);
				}
				IL_FD:
				if (this.m_shortcutsToDelete.Count > 0)
				{
					goto IL_E3;
				}
			}
		}
	}
}
