using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Database.Items.Shops;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Dialogs.Npcs;
using Stump.Server.WorldServer.Game.Items;
using System.ComponentModel;
namespace Stump.Server.WorldServer.Database.Npcs.Actions
{
	[Discriminator("Shop", typeof(NpcActionDatabase), new System.Type[]
	{
		typeof(NpcActionRecord)
	})]
	public class NpcBuySellAction : NpcActionDatabase
	{
		public const string Discriminator = "Shop";
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private System.Collections.Generic.List<NpcItem> m_items;
		private ItemTemplate m_token;
		public System.Collections.Generic.List<NpcItem> Items
		{
			get
			{
				System.Collections.Generic.List<NpcItem> arg_28_0;
				if ((arg_28_0 = this.m_items) == null)
				{
					arg_28_0 = (this.m_items = Singleton<ItemManager>.Instance.GetNpcShopItems(base.Record.Id));
				}
				return arg_28_0;
			}
		}
		public int TokenId
		{
			get
			{
				return base.Record.GetParameter<int>(0u, true);
			}
			set
			{
				base.Record.SetParameter<int>(0u, value);
			}
		}
		public ItemTemplate Token
		{
			get
			{
				if (this.TokenId <= 0)
				{
					return null;
				}
				else
				{
					if (this.m_token == null)
					{
						this.m_token = Singleton<ItemManager>.Instance.TryGetTemplate(this.TokenId);
					}
				}
                return m_token;
			}
			set
			{
				this.m_token = value;
				this.TokenId = ((value == null) ? 0 : this.m_token.Id);
			}
		}
		[DefaultValue(1)]
		public bool CanSell
		{
			get
			{
				return base.Record.GetParameter<bool>(1u, true);
			}
			set
			{
				base.Record.SetParameter<bool>(1u, value);
			}
		}
		public bool MaxStats
		{
			get
			{
				return base.Record.GetParameter<bool>(2u, true);
			}
			set
			{
				base.Record.SetParameter<bool>(2u, value);
			}
		}
		public override NpcActionTypeEnum ActionType
		{
			get
			{
				return NpcActionTypeEnum.ACTION_BUY_SELL;
			}
		}
		public NpcBuySellAction(NpcActionRecord record) : base(record)
		{
		}
		public override void Execute(Npc npc, Character character)
		{
			NpcShopDialogLogger npcShopDialogLogger = new NpcShopDialogLogger(character, npc, this.Items, this.Token)
			{
				CanSell = this.CanSell,
				MaxStats = this.MaxStats
			};
			npcShopDialogLogger.Open();
		}
	}
}
