using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Dialogs.Merchants;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants
{
	public class Merchant : NamedActor
	{
		public const short BAG_SKIN = 237;
		private readonly WorldMapMerchantRecord m_record;
		private readonly System.Collections.Generic.List<MerchantShopDialog> m_openedDialogs = new System.Collections.Generic.List<MerchantShopDialog>();
		private bool m_isRecordDirty;
		public WorldMapMerchantRecord Record
		{
			get
			{
				return this.m_record;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<MerchantShopDialog> OpenDialogs
		{
			get
			{
				return this.m_openedDialogs.AsReadOnly();
			}
		}
		public override int Id
		{
			get
			{
				return this.m_record.CharacterId;
			}
			protected set
			{
				this.m_record.CharacterId = value;
			}
		}
		public override ObjectPosition Position
		{
			get;
			protected set;
		}
		public MerchantBag Bag
		{
			get;
			protected set;
		}
		public override ActorLook Look
		{
			get
			{
				return this.m_record.EntityLook;
			}
			set
			{
				this.m_record.EntityLook = value;
			}
		}
		public override string Name
		{
			get
			{
				return this.m_record.Name;
			}
		}
		public uint KamasEarned
		{
			get
			{
				return this.m_record.KamasEarned;
			}
			set
			{
				this.m_record.KamasEarned = value;
			}
		}
		public bool IsRecordDirty
		{
			get
			{
				return this.m_isRecordDirty || this.Bag.IsDirty;
			}
			set
			{
				this.m_isRecordDirty = value;
			}
		}
		public Merchant(Character character)
		{
			ActorLook actorLook = character.Look.Clone();
			actorLook.RemoveAuras();
			actorLook.AddSubLook(new SubActorLook(0, SubEntityBindingPointCategoryEnum.HOOK_POINT_CATEGORY_MERCHANT_BAG, new ActorLook
			{
				BonesID = 237
			}));
			this.m_record = new WorldMapMerchantRecord
			{
				CharacterId = character.Id,
				AccountId = character.Account.Id,
				Name = character.Name,
				Map = character.Map,
				Cell = (int)character.Cell.Id,
				Direction = (int)character.Direction,
				EntityLook = actorLook,
				IsActive = true,
				MerchantSince = System.DateTime.Now
			};
			this.Bag = new MerchantBag(this, character.MerchantBag);
			this.Position = character.Position.Clone();
		}
		public Merchant(WorldMapMerchantRecord record)
		{
			this.m_record = record;
			this.Bag = new MerchantBag(this);
			if (record.Map == null)
			{
				throw new System.Exception("Merchant's map not found");
			}
			this.Position = new ObjectPosition(record.Map, record.Map.Cells[this.m_record.Cell], (DirectionsEnum)this.m_record.Direction);
		}
		protected override void OnDisposed()
		{
			this.m_record.IsActive = false;
			MerchantShopDialog[] array = this.OpenDialogs.ToArray<MerchantShopDialog>();
			for (int i = 0; i < array.Length; i++)
			{
				MerchantShopDialog merchantShopDialog = array[i];
				merchantShopDialog.Close();
			}
			base.OnDisposed();
		}
		public override bool CanBeSee(WorldObject byObj)
		{
			return base.CanBeSee(byObj) && !this.IsBagEmpty();
		}
		public bool IsBagEmpty()
		{
			return this.Bag.Count == 0;
		}
		public void LoadRecord()
		{
			this.Bag.LoadRecord();
		}
		public void Save()
		{
			if (this.Bag.IsDirty)
			{
				this.Bag.Save();
			}
			ServerBase<WorldServer>.Instance.DBAccessor.Database.Update(this.m_record);
		}
		public bool IsMerchantOwner(WorldAccount account)
		{
			return account.Id == this.m_record.AccountId;
		}
		public void OnDialogOpened(MerchantShopDialog dialog)
		{
			this.m_openedDialogs.Add(dialog);
		}
		public void OnDialogClosed(MerchantShopDialog dialog)
		{
			this.m_openedDialogs.Remove(dialog);
		}
		public override GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return new GameRolePlayMerchantInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(), this.Name, 0, Enumerable.Empty<HumanOption>());
		}
		public override string ToString()
		{
			return string.Format("{0} ({1})", this.Name, this.Id);
		}
	}
}
