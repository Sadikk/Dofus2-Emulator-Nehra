using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Database.Npcs.Replies
{
	[Discriminator("DeleteItem", typeof(NpcReply), new System.Type[]
	{
		typeof(NpcReplyRecord)
	})]
	public class DeleteItemReply : NpcReply
	{
		private ItemTemplate m_itemTemplate;
		public int ItemId
		{
			get
			{
				return base.Record.GetParameter<int>(0u, false);
			}
			set
			{
				base.Record.SetParameter<int>(0u, value);
			}
		}
		public ItemTemplate Item
		{
			get
			{
				ItemTemplate arg_23_0;
				if ((arg_23_0 = this.m_itemTemplate) == null)
				{
					arg_23_0 = (this.m_itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(this.ItemId));
				}
				return arg_23_0;
			}
			set
			{
				this.m_itemTemplate = value;
				this.ItemId = value.Id;
			}
		}
		public uint Amount
		{
			get
			{
				return base.Record.GetParameter<uint>(1u, false);
			}
			set
			{
				base.Record.SetParameter<uint>(1u, value);
			}
		}
		public DeleteItemReply(NpcReplyRecord record) : base(record)
		{
		}
		public override bool Execute(Npc npc, Character character)
		{
			bool result;
			if (!base.Execute(npc, character))
			{
				result = false;
			}
			else
			{
				BasePlayerItem basePlayerItem = character.Inventory.TryGetItem(this.Item);
				if (basePlayerItem == null)
				{
					result = false;
				}
				else
				{
					character.Inventory.RemoveItem(basePlayerItem, this.Amount, true);
					character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 22, new object[]
					{
						this.Amount,
						basePlayerItem.Template.Id
					});
					result = true;
				}
			}
			return result;
		}
	}
}
