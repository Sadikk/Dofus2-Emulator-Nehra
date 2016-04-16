using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.Startup;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Accounts.Startup
{
	public class StartupActionItem
	{
		private ItemTemplate m_itemTemplate;
		public StartupActionItemRecord m_record;
		public StartupActionItemRecord Record
		{
			get
			{
				return this.m_record;
			}
		}
		public ItemTemplate ItemTemplate
		{
			get
			{
				ItemTemplate arg_28_0;
				if ((arg_28_0 = this.m_itemTemplate) == null)
				{
					arg_28_0 = (this.m_itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(this.m_record.ItemTemplate));
				}
				return arg_28_0;
			}
			set
			{
				this.m_itemTemplate = value;
				this.m_record.ItemTemplate = value.Id;
			}
		}
		public uint Amount
		{
			get
			{
				return this.m_record.Amount;
			}
			set
			{
				this.m_record.Amount = value;
			}
		}
		public bool MaxEffects
		{
			get
			{
				return this.m_record.MaxEffects;
			}
			set
			{
				this.m_record.MaxEffects = value;
			}
		}
		public StartupActionItem(StartupActionItemRecord record)
		{
			this.m_record = record;
		}
		public void GiveTo(CharacterRecord record)
		{
			System.Collections.Generic.List<EffectBase> effects = Singleton<ItemManager>.Instance.GenerateItemEffects(this.ItemTemplate, this.MaxEffects);
			PlayerItemRecord poco = new PlayerItemRecord
			{
				Id = AutoAssignedRecord<PlayerItemRecord>.PopNextId(),
				OwnerId = record.Id,
				Template = this.ItemTemplate,
				Stack = this.Amount,
				Position = CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED,
				Effects = effects
			};
			ServerBase<WorldServer>.Instance.DBAccessor.Database.Insert(poco);
		}
		public ObjectItemInformationWithQuantity GetObjectItemInformationWithQuantity()
		{
			return new ObjectItemInformationWithQuantity((ushort)this.ItemTemplate.Id, (
				from entry in this.ItemTemplate.Effects
				select entry.GetObjectEffect()).ToArray<ObjectEffect>(), (uint)this.Amount);
		}
	}
}
