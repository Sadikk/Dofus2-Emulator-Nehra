using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.Startup;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Accounts.Startup
{
	public class StartupAction
	{
		private readonly StartupActionRecord m_record;
		public StartupActionRecord Record
		{
			get
			{
				return this.m_record;
			}
		}
		public int Id
		{
			get
			{
				return this.m_record.Id;
			}
			set
			{
				this.m_record.Id = value;
			}
		}
		public string Title
		{
			get
			{
				return this.m_record.Title;
			}
			set
			{
				this.m_record.Title = value;
			}
		}
		public string Text
		{
			get
			{
				return this.m_record.Text;
			}
			set
			{
				this.m_record.Text = value;
			}
		}
		public string DescUrl
		{
			get
			{
				return this.m_record.DescUrl;
			}
			set
			{
				this.m_record.DescUrl = value;
			}
		}
		public string PictureUrl
		{
			get
			{
				return this.m_record.PictureUrl;
			}
			set
			{
				this.m_record.PictureUrl = value;
			}
		}
		public StartupActionItem[] Items
		{
			get;
			private set;
		}
		public StartupAction(StartupActionRecord record)
		{
			this.m_record = record;
			this.Items = (
				from entry in record.Items
				select new StartupActionItem(entry)).ToArray<StartupActionItem>();
		}
		public void GiveGiftTo(CharacterRecord character)
		{
			StartupActionItem[] items = this.Items;
			for (int i = 0; i < items.Length; i++)
			{
				StartupActionItem startupActionItem = items[i];
				startupActionItem.GiveTo(character);
			}
		}
		public StartupActionAddObject GetStartupActionAddObject()
		{
			return new StartupActionAddObject(this.Id, this.Title, this.Text, this.DescUrl, this.PictureUrl, (
				from entry in this.Items
				select entry.GetObjectItemInformationWithQuantity()).ToArray<ObjectItemInformationWithQuantity>());
		}
	}
}
