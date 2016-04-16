using Stump.Core.Extensions;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Collections.Generic;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items
{
	public class ItemsCollection<T> : System.Collections.IEnumerable, System.Collections.Generic.IEnumerable<T> where T : IItem
	{
		public delegate void ItemAddedEventHandler(ItemsCollection<T> sender, T item);
		public delegate void ItemRemovedEventHandler(ItemsCollection<T> sender, T item);
		public delegate void ItemStackChangedEventHandler(ItemsCollection<T> sender, T item, int difference);
		public event ItemsCollection<T>.ItemAddedEventHandler ItemAdded;
		public event ItemsCollection<T>.ItemRemovedEventHandler ItemRemoved;
		public event ItemsCollection<T>.ItemStackChangedEventHandler ItemStackChanged;
		protected object Locker
		{
			get;
			set;
		}
		protected System.Collections.Generic.Dictionary<int, T> Items
		{
			get;
			set;
		}
		protected Queue<T> ItemsToDelete
		{
			get;
			set;
		}
		public int Count
		{
			get
			{
				return this.Items.Count;
			}
		}
		protected ItemsCollection()
		{
			this.Locker = new object();
			this.Items = new System.Collections.Generic.Dictionary<int, T>();
			this.ItemsToDelete = new Queue<T>();
		}
		public void NotifyItemAdded(T item)
		{
			this.OnItemAdded(item);
			ItemsCollection<T>.ItemAddedEventHandler itemAdded = this.ItemAdded;
			if (itemAdded != null)
			{
				itemAdded(this, item);
			}
		}
		protected virtual void OnItemAdded(T item)
		{
		}
		public void NotifyItemRemoved(T item)
		{
			this.OnItemRemoved(item);
			ItemsCollection<T>.ItemRemovedEventHandler itemRemoved = this.ItemRemoved;
			if (itemRemoved != null)
			{
				itemRemoved(this, item);
			}
		}
		protected virtual void OnItemRemoved(T item)
		{
		}
		public void NotifyItemStackChanged(T item, int difference)
		{
			this.OnItemStackChanged(item, difference);
			ItemsCollection<T>.ItemStackChangedEventHandler itemStackChanged = this.ItemStackChanged;
			if (itemStackChanged != null)
			{
				itemStackChanged(this, item, difference);
			}
		}
		protected virtual void OnItemStackChanged(T item, int difference)
		{
		}
		public virtual T AddItem(T item)
		{
			if (this.HasItem(item))
			{
				throw new System.Exception("Cannot add an item that is already in the collection");
			}
			T result;
			lock (this.Locker)
			{
				T t;
				if (this.IsStackable(item, out t))
				{
					this.StackItem(t, item.Stack);
					this.DeleteItem(item);
					result = t;
					return result;
				}
				this.Items.Add(item.Guid, item);
				this.NotifyItemAdded(item);
			}
			result = item;
			return result;
		}
		public virtual uint RemoveItem(T item, uint amount, bool delete = true)
		{
			uint result;
			if (!this.HasItem(item))
			{
				result = 0u;
			}
			else
			{
				if (item.Stack <= amount)
				{
					this.RemoveItem(item, delete);
					result = item.Stack;
				}
				else
				{
					this.UnStackItem(item, amount);
					result = amount;
				}
			}
			return result;
		}
		public virtual bool RemoveItem(T item, bool delete = true)
		{
			bool result;
			if (!this.HasItem(item))
			{
				result = false;
			}
			else
			{
				lock (this.Locker)
				{
					bool flag2 = this.Items.Remove(item.Guid);
					if (delete)
					{
						this.DeleteItem(item);
					}
					if (flag2)
					{
						this.NotifyItemRemoved(item);
					}
					result = flag2;
				}
			}
			return result;
		}
		protected virtual void DeleteItem(T item)
		{
			if (this.Items.ContainsKey(item.Guid))
			{
				this.Items.Remove(item.Guid);
				this.NotifyItemRemoved(item);
			}
			this.ItemsToDelete.Enqueue(item);
		}
		public virtual void StackItem(T item, uint amount)
		{
			item.Stack += amount;
			this.NotifyItemStackChanged(item, (int)amount);
		}
		public virtual void UnStackItem(T item, uint amount)
		{
			if (item.Stack - amount <= 0u)
			{
				this.RemoveItem(item, true);
			}
			else
			{
				item.Stack -= amount;
				this.NotifyItemStackChanged(item, (int)(-(int)((ulong)amount)));
			}
		}
		public void DeleteAll(bool notify = true)
		{
			if (notify)
			{
				foreach (T current in this)
				{
					this.NotifyItemRemoved(current);
				}
			}
			this.ItemsToDelete = new Queue<T>(this.ItemsToDelete.Concat(this.Items.Values));
			this.Items.Clear();
		}
		public virtual void Save()
		{
			lock (this.Locker)
			{
                Stump.ORM.Database database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
				using (System.Collections.Generic.Dictionary<int, T>.Enumerator enumerator = this.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						System.Collections.Generic.KeyValuePair<int, T> current = enumerator.Current;
						T value = current.Value;
						if (value.Record.IsNew)
						{
                            Stump.ORM.Database arg_74_0 = database;
							value = current.Value;
							arg_74_0.Insert(value.Record);
							value = current.Value;
							value.Record.IsNew = false;
						}
						else
						{
							value = current.Value;
							if (value.Record.IsDirty)
							{
                                Stump.ORM.Database arg_CF_0 = database;
								value = current.Value;
								arg_CF_0.Update(value.Record);
							}
						}
					}
					goto IL_111;
				}
				IL_F1:
				T t = this.ItemsToDelete.Dequeue();
				database.Delete(t.Record);
				IL_111:
				if (this.ItemsToDelete.Count > 0)
				{
					goto IL_F1;
				}
			}
		}
		public virtual bool IsStackable(T item, out T stackableWith)
		{
			T t;
			bool result;
			if ((t = this.TryGetItem(item.Template, item.Effects)) != null)
			{
				stackableWith = t;
				result = true;
			}
			else
			{
				stackableWith = default(T);
				result = false;
			}
			return result;
		}
		public bool HasItem(int guid)
		{
			return this.Items.ContainsKey(guid);
		}
		public bool HasItem(ItemTemplate template)
		{
			return this.Items.Any(delegate(System.Collections.Generic.KeyValuePair<int, T> entry)
			{
				T value = entry.Value;
				return value.Template.Id == template.Id;
			});
		}
		public bool HasItem(T item)
		{
			return this.HasItem(item.Guid);
		}
		public T TryGetItem(int guid)
		{
			return (!this.Items.ContainsKey(guid)) ? default(T) : this.Items[guid];
		}
		public T TryGetItem(ItemTemplate template)
		{
			System.Collections.Generic.IEnumerable<T> source = 
				from entry in this.Items.Values
				where entry.Template.Id == template.Id
				select entry;
			return source.FirstOrDefault<T>();
		}
		public T TryGetItem(ItemTemplate template, System.Collections.Generic.IEnumerable<EffectBase> effects)
		{
			System.Collections.Generic.IEnumerable<T> source = 
				from entry in this.Items.Values
				where entry.Template.Id == template.Id && effects.CompareEnumerable(entry.Effects)
				select entry;
			return source.FirstOrDefault<T>();
		}
		public System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return this.Items.Values.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
