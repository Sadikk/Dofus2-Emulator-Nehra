using Stump.ORM;
using Stump.Server.WorldServer.Database.Characters;

namespace Stump.Server.WorldServer.Database.Shortcuts
{
	public abstract class Shortcut : ISaveIntercepter
	{
		private int m_id;
		private int m_ownerId;
		private int m_slot;
		public int Id
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
				this.IsDirty = true;
			}
		}
		public int OwnerId
		{
			get
			{
				return this.m_ownerId;
			}
			set
			{
				this.m_ownerId = value;
				this.IsDirty = true;
			}
		}
		public int Slot
		{
			get
			{
				return this.m_slot;
			}
			set
			{
				this.m_slot = value;
				this.IsDirty = true;
			}
		}
		[Ignore]
		public bool IsDirty
		{
			get;
			set;
		}
		[Ignore]
		public bool IsNew
		{
			get;
			set;
		}
		protected Shortcut()
		{
		}
		protected Shortcut(CharacterRecord owner, int slot)
		{
			this.OwnerId = owner.Id;
			this.Slot = slot;
			this.IsNew = true;
		}
		public abstract Stump.DofusProtocol.Types.Shortcut GetNetworkShortcut();
		public void BeforeSave(bool insert)
		{
			this.IsDirty = false;
		}
	}
}
