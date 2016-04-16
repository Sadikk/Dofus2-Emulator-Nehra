using MongoDB.Bson;
using Stump.Core.Reflection;
using Stump.Server.BaseServer.Logging;
using Stump.Server.WorldServer.Database.Items.Shops;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Dialogs.Npcs
{
	public class NpcShopDialogLogger : NpcShopDialog
	{
		public NpcShopDialogLogger(Character character, Npc npc, System.Collections.Generic.IEnumerable<NpcItem> items) : base(character, npc, items)
		{
			base.Character = character;
			base.Npc = npc;
			base.Items = items;
			base.CanSell = true;
		}
		public NpcShopDialogLogger(Character character, Npc npc, System.Collections.Generic.IEnumerable<NpcItem> items, ItemTemplate token) : base(character, npc, items, token)
		{
			base.Character = character;
			base.Npc = npc;
			base.Items = items;
			base.Token = token;
			base.CanSell = true;
		}
		public override bool BuyItem(int itemId, uint amount)
		{
			bool result;
			if (!base.BuyItem(itemId, amount))
			{
				result = false;
			}
			else
			{
				NpcItem npcItem = base.Items.FirstOrDefault((NpcItem entry) => entry.Item.Id == itemId);
				BsonDocument document = new BsonDocument
				{

					{
						"AcctId",
						base.Character.Account.Id
					},

					{
						"CharacterId",
						base.Character.Id
					},

					{
						"ItemId",
						npcItem.ItemId
					},

					{
						"Amount",
						(long)((ulong)amount)
					},

					{
						"FinalPrice",
						npcItem.Price * amount
					},

					{
						"IsToken",
						base.Token != null
					},

					{
						"Date",
						System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture)
					}
				};
				Singleton<MongoLogger>.Instance.Insert("NpcShopBuy", document);
				result = true;
			}
			return result;
		}
	}
}
