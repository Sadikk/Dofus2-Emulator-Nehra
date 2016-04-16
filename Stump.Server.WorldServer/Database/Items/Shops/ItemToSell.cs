using Stump.Core.Reflection;
using Stump.DofusProtocol.Types;
using Stump.ORM;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Items;

namespace Stump.Server.WorldServer.Database.Items.Shops
{
	public abstract class ItemToSell
	{
		private ItemTemplate m_item;
		public int Id
		{
			get;
			set;
		}
		public int ItemId
		{
			get;
			set;
		}
		[Ignore]
		public ItemTemplate Item
		{
			get
			{
				ItemTemplate arg_23_0;
				if ((arg_23_0 = this.m_item) == null)
				{
					arg_23_0 = (this.m_item = Singleton<ItemManager>.Instance.TryGetTemplate(this.ItemId));
				}
				return arg_23_0;
			}
			set
			{
				this.m_item = value;
				this.ItemId = value.Id;
			}
		}
		public abstract Item GetNetworkItem();
	}
}
