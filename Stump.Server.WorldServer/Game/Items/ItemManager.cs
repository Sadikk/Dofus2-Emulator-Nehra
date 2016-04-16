using NLog;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Shops;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Game.Items.Player.Custom;
using Stump.Server.WorldServer.Game.Items.TaxCollector;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
namespace Stump.Server.WorldServer.Game.Items
{
	public class ItemManager : DataManager<ItemManager>
	{
		private delegate BasePlayerItem PlayerItemConstructor(Character owner, PlayerItemRecord record);
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private System.Collections.Generic.Dictionary<int, ItemTemplate> m_itemTemplates = new System.Collections.Generic.Dictionary<int, ItemTemplate>();
		private System.Collections.Generic.Dictionary<int, LivingObjectRecord> m_livingObjects = new System.Collections.Generic.Dictionary<int, LivingObjectRecord>();
		private System.Collections.Generic.Dictionary<uint, ItemSetTemplate> m_itemsSets = new System.Collections.Generic.Dictionary<uint, ItemSetTemplate>();
		private System.Collections.Generic.Dictionary<int, ItemTypeRecord> m_itemTypes = new System.Collections.Generic.Dictionary<int, ItemTypeRecord>();
		private System.Collections.Generic.Dictionary<int, NpcItem> m_npcShopItems = new System.Collections.Generic.Dictionary<int, NpcItem>();
		private readonly System.Collections.Generic.Dictionary<ItemTypeEnum, ItemManager.PlayerItemConstructor> m_itemCtorByTypes = new System.Collections.Generic.Dictionary<ItemTypeEnum, ItemManager.PlayerItemConstructor>();
		private readonly System.Collections.Generic.Dictionary<EffectsEnum, ItemManager.PlayerItemConstructor> m_itemCtorByEffects = new System.Collections.Generic.Dictionary<EffectsEnum, ItemManager.PlayerItemConstructor>();

        [Initialization(InitializationPass.Fourth)]
        public override void Initialize()
        {
            this.m_itemTypes = base.Database.Query<ItemTypeRecord>(ItemTypeRecordRelator.FetchQuery, new object[0]).ToDictionary((ItemTypeRecord entry) => entry.Id);
            this.m_itemTemplates = base.Database.Query<ItemTemplate>(ItemTemplateRelator.FetchQuery, new object[0]).ToDictionary((ItemTemplate entry) => entry.Id);
            foreach (WeaponTemplate current in base.Database.Query<WeaponTemplate>(WeaponTemplateRelator.FetchQuery, new object[0]))
            {
                this.m_itemTemplates.Add(current.Id, current);
            }
            this.m_itemsSets = base.Database.Query<ItemSetTemplate>(ItemSetTemplateRelator.FetchQuery, new object[0]).ToDictionary((ItemSetTemplate entry) => entry.Id);
            this.m_npcShopItems = base.Database.Query<NpcItem>(NpcItemRelator.FetchQuery, new object[0]).ToDictionary((NpcItem entry) => entry.Id);
            this.m_livingObjects = base.Database.Query<LivingObjectRecord>(LivingObjectRelator.FetchQuery, new object[0]).ToDictionary((LivingObjectRecord entry) => entry.Id);
            this.InitializeItemCtors();
        }

        public BasePlayerItem CreatePlayerItem(Character owner, int id, uint amount, bool maxEffects = false)
		{
			if (!this.m_itemTemplates.ContainsKey(id))
			{
				throw new System.Exception(string.Format("Template id '{0}' doesn't exist", id));
			}
			return this.CreatePlayerItem(owner, this.m_itemTemplates[id], amount, maxEffects);
		}
		public BasePlayerItem CreatePlayerItem(Character owner, ItemTemplate template, uint amount, bool maxEffects = false)
		{
			return this.CreatePlayerItem(owner, template, amount, this.GenerateItemEffects(template, maxEffects));
		}
		public BasePlayerItem CreatePlayerItem(Character owner, IItem item)
		{
			return this.CreatePlayerItem(owner, item.Template, item.Stack, item.Effects.Clone<EffectBase>());
		}
		public BasePlayerItem CreatePlayerItem(Character owner, IItem item, uint amount)
		{
			return this.CreatePlayerItem(owner, item.Template, amount, item.Effects.Clone<EffectBase>());
		}
		public BasePlayerItem CreatePlayerItem(Character owner, ItemTemplate template, uint amount, System.Collections.Generic.List<EffectBase> effects)
		{
			int id = AutoAssignedRecord<PlayerItemRecord>.PopNextId();
			PlayerItemRecord record = new PlayerItemRecord
			{
				Id = id,
				OwnerId = owner.Id,
				Template = template,
				Stack = amount,
				Position = CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED,
				Effects = effects,
				IsNew = true
			};
			return this.CreateItemInstance(owner, record);
		}
		public BasePlayerItem RecreateItemInstance(BasePlayerItem item)
		{
			return this.CreateItemInstance(item.Owner, item.Record);
		}
		public BasePlayerItem LoadPlayerItem(Character owner, PlayerItemRecord record)
		{
			return this.CreateItemInstance(owner, record);
		}
		private BasePlayerItem CreateItemInstance(Character character, PlayerItemRecord record)
		{
			ItemManager.PlayerItemConstructor ctor = null;
			BasePlayerItem result;
			if (record.Effects.Any((EffectBase effect) => this.m_itemCtorByEffects.TryGetValue(effect.EffectId, out ctor)))
			{
				result = ctor(character, record);
			}
			else
			{
				result = (this.m_itemCtorByTypes.TryGetValue((ItemTypeEnum)record.Template.Type.Id, out ctor) ? ctor(character, record) : new DefaultItem(character, record));
			}
			return result;
		}
		public MerchantItem CreateMerchantItem(BasePlayerItem item, uint quantity, uint price)
		{
			int guid = AutoAssignedRecord<PlayerItemRecord>.PopNextId();
			return new MerchantItem(item.Owner, guid, item.Template, item.Effects, quantity, price);
		}
		public TaxCollectorItem CreateTaxCollectorItem(TaxCollectorNpc owner, ItemTemplate template, uint amount)
		{
			int id = AutoAssignedRecord<TaxCollectorItemRecord>.PopNextId();
			TaxCollectorItemRecord record = new TaxCollectorItemRecord
			{
				Id = id,
				OwnerId = owner.Id,
				Template = template,
				Stack = amount,
				Effects = this.GenerateItemEffects(template, false),
				IsNew = true
			};
			return new TaxCollectorItem(record);
		}
		public TaxCollectorItem CreateTaxCollectorItem(TaxCollectorNpc owner, int id, uint amount)
		{
			if (!this.m_itemTemplates.ContainsKey(id))
			{
				throw new System.Exception(string.Format("Template id '{0}' doesn't exist", id));
			}
			return this.CreateTaxCollectorItem(owner, this.m_itemTemplates[id], amount);
		}
		public System.Collections.Generic.List<EffectBase> GenerateItemEffects(ItemTemplate template, bool max = false)
		{
			System.Collections.Generic.List<EffectBase> source = (
				from effect in template.Effects
                where Enum.IsDefined(typeof(EffectsEnum), effect.EffectId) // TODO : check
				select Singleton<EffectManager>.Instance.IsUnRandomableWeaponEffect(effect.EffectId) ? effect : effect.GenerateEffect(EffectGenerationContext.Item, max ? EffectGenerationType.MaxEffects : EffectGenerationType.Normal)).ToList<EffectBase>();
			return source.ToList<EffectBase>();
		}

		private void InitializeItemCtors()
		{
			foreach (System.Type current in 
				from x in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
				where typeof(BasePlayerItem).IsAssignableFrom(x)
				select x)
			{
				ItemTypeAttribute customAttribute = current.GetCustomAttribute<ItemTypeAttribute>();
				if (customAttribute != null)
				{
					if (this.m_itemCtorByTypes.ContainsKey(customAttribute.ItemType))
					{
						ItemManager.logger.Error<ItemTypeEnum>("Item Constructor with Type {0} defined twice or more !", customAttribute.ItemType);
						continue;
					}
					this.m_itemCtorByTypes.Add(customAttribute.ItemType, current.GetConstructor(new System.Type[]
					{
						typeof(Character),
						typeof(PlayerItemRecord)
					}).CreateDelegate<ItemManager.PlayerItemConstructor>());
				}
				ItemHasEffectAttribute customAttribute2 = current.GetCustomAttribute<ItemHasEffectAttribute>();
				if (customAttribute2 != null)
				{
					if (this.m_itemCtorByEffects.ContainsKey(customAttribute2.Effect))
					{
						ItemManager.logger.Error<EffectsEnum>("Item Constructor with Effect {0} defined twice or more !", customAttribute2.Effect);
					}
					else
					{
						this.m_itemCtorByEffects.Add(customAttribute2.Effect, current.GetConstructor(new System.Type[]
						{
							typeof(Character),
							typeof(PlayerItemRecord)
						}).CreateDelegate<ItemManager.PlayerItemConstructor>());
					}
				}
			}
		}
		public void AddItemConstructor(System.Type type)
		{
			ItemTypeAttribute customAttribute = type.GetCustomAttribute<ItemTypeAttribute>();
			if (customAttribute == null)
			{
				ItemManager.logger.Error<System.Type>("Item Constructor {0} has no attribute !", type);
			}
			else
			{
				if (this.m_itemCtorByTypes.ContainsKey(customAttribute.ItemType))
				{
					ItemManager.logger.Error<ItemTypeEnum>("Item Constructor with Type {0} defined twice or more !", customAttribute.ItemType);
				}
				else
				{
					this.m_itemCtorByTypes.Add(customAttribute.ItemType, type.GetConstructor(new System.Type[]
					{
						typeof(Character),
						typeof(PlayerItemRecord)
					}).CreateDelegate<ItemManager.PlayerItemConstructor>());
				}
			}
		}
		public System.Collections.Generic.IEnumerable<ItemTemplate> GetTemplates()
		{
			return this.m_itemTemplates.Values;
		}
		public ItemTemplate TryGetTemplate(int id)
		{
			return (!this.m_itemTemplates.ContainsKey(id)) ? null : this.m_itemTemplates[id];
		}
		public ItemTemplate TryGetTemplate(string name, bool ignorecase)
		{
			return this.m_itemTemplates.Values.FirstOrDefault((ItemTemplate entry) => entry.Name.Equals(name, ignorecase ? System.StringComparison.InvariantCultureIgnoreCase : System.StringComparison.InvariantCulture));
		}
		public ItemSetTemplate TryGetItemSetTemplate(uint id)
		{
			return (!this.m_itemsSets.ContainsKey(id)) ? null : this.m_itemsSets[id];
		}
		public ItemSetTemplate TryGetItemSetTemplate(string name, bool ignorecase)
		{
			return this.m_itemsSets.Values.FirstOrDefault((ItemSetTemplate entry) => entry.Name.Equals(name, ignorecase ? System.StringComparison.InvariantCultureIgnoreCase : System.StringComparison.InvariantCulture));
		}
		public System.Collections.Generic.List<NpcItem> GetNpcShopItems(uint id)
		{
			return (
				from entry in this.m_npcShopItems.Values
				where (long)entry.NpcShopId == (long)((ulong)id)
				select entry).ToList<NpcItem>();
		}
		public ItemTypeRecord TryGetItemType(int id)
		{
			return (!this.m_itemTypes.ContainsKey(id)) ? null : this.m_itemTypes[id];
		}
		public System.Collections.Generic.List<PlayerItemRecord> FindPlayerItems(int ownerId)
		{
			return base.Database.Fetch<PlayerItemRecord>(string.Format(PlayerItemRelator.FetchByOwner, ownerId), new object[0]);
		}
		public System.Collections.Generic.List<PlayerMerchantItemRecord> FindPlayerMerchantItems(int ownerId)
		{
			return base.Database.Fetch<PlayerMerchantItemRecord>(string.Format(PlayerMerchantItemRelator.FetchByOwner, ownerId), new object[0]);
		}
		public System.Collections.Generic.List<TaxCollectorItemRecord> FindTaxCollectorItems(int ownerId)
		{
			return base.Database.Fetch<TaxCollectorItemRecord>(string.Format(TaxCollectorItemRelator.FetchByOwner, ownerId), new object[0]);
		}
		public System.Collections.Generic.IEnumerable<ItemTemplate> GetItemsByPattern(string pattern, System.Collections.Generic.IEnumerable<ItemTemplate> list)
		{
			System.Collections.Generic.IEnumerable<ItemTemplate> result;
			if (pattern == "*")
			{
				result = list;
			}
			else
			{
				bool ignorecase = pattern[0] == '@';
				if (ignorecase)
				{
					pattern = pattern.Remove(0, 1);
				}
				int outvalue;
				if (!ignorecase && int.TryParse(pattern, out outvalue))
				{
					result = 
						from entry in list
						where entry.Id == outvalue
						select entry;
				}
				else
				{
					pattern = pattern.Replace("*", "[\\w\\d\\s_]*");
					result = 
						from entry in list
						where Regex.Match(entry.Name, pattern, ignorecase ? RegexOptions.IgnoreCase : RegexOptions.None).Success
						select entry;
				}
			}
			return result;
		}
		public System.Collections.Generic.IEnumerable<ItemTemplate> GetItemsByPattern(string pattern)
		{
			return this.GetItemsByPattern(pattern, this.m_itemTemplates.Values);
		}
		public System.Collections.Generic.IEnumerable<BasePlayerItem> GetItemsByPattern(string pattern, System.Collections.Generic.IEnumerable<BasePlayerItem> list)
		{
			System.Collections.Generic.IEnumerable<BasePlayerItem> result;
			if (pattern == "*")
			{
				result = list;
			}
			else
			{
				bool ignorecase = pattern[0] == '@';
				if (ignorecase)
				{
					pattern = pattern.Remove(0, 1);
				}
				int outvalue;
				if (!ignorecase && int.TryParse(pattern, out outvalue))
				{
					result = 
						from entry in list
						where entry.Template.Id == outvalue
						select entry;
				}
				else
				{
					pattern = pattern.Replace("*", "[\\w\\d\\s_]*");
					result = 
						from entry in list
						where Regex.Match(entry.Template.Name, pattern, ignorecase ? RegexOptions.IgnoreCase : RegexOptions.None).Success
						select entry;
				}
			}
			return result;
		}
		public void AddItemTemplate(ItemTemplate template)
		{
			this.m_itemTemplates.Add(template.Id, template);
			base.Database.Insert(template);
		}
		public LivingObjectRecord TryGetLivingObjectRecord(int id)
		{
			LivingObjectRecord livingObjectRecord;
			return (!this.m_livingObjects.TryGetValue(id, out livingObjectRecord)) ? null : livingObjectRecord;
		}
	}
}
