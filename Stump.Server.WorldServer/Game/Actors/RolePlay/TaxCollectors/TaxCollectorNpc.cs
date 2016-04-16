using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer;
using Stump.Server.WorldServer.Database.I18n;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Game.Dialogs.TaxCollector;
using Stump.Server.WorldServer.Game.Exchanges;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Game.Items.TaxCollector;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Handlers.Context;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors
{
	public class TaxCollectorNpc : NamedActor, IAutoMovedEntity, IContextDependant, IInteractNpc
	{
		public const int TAXCOLLECTOR_BONES = 714;
		[Variable]
		public static int BaseAP = 6;
		[Variable]
		public static int BaseMP = 5;
		[Variable]
		public static int BaseHealth = 3000;
		[Variable]
		public static int BaseResistance = 25;
		private readonly WorldMapTaxCollectorRecord m_record;
		private readonly System.Collections.Generic.List<IDialog> m_openedDialogs = new System.Collections.Generic.List<IDialog>();
		private string m_name;
		private ActorLook m_look;
		private readonly int m_contextId;
		public WorldMapTaxCollectorRecord Record
		{
			get
			{
				return this.m_record;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<IDialog> OpenDialogs
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
				return this.m_contextId;
			}
		}
		public int GlobalId
		{
			get
			{
				return this.m_record.Id;
			}
			protected set
			{
				this.m_record.Id = value;
			}
		}
		public override string Name
		{
			get
			{
				string arg_3D_0;
				if ((arg_3D_0 = this.m_name) == null)
				{
					arg_3D_0 = (this.m_name = string.Format("{0} {1}", Singleton<TextManager>.Instance.GetText((int)this.FirstNameId), Singleton<TextManager>.Instance.GetText((int)this.LastNameId)));
				}
				return arg_3D_0;
			}
		}
		public byte Level
		{
			get
			{
				return this.Guild.Level;
			}
		}
		public Guild Guild
		{
			get;
			protected set;
		}
		public TaxCollectorBag Bag
		{
			get;
			protected set;
		}
		public sealed override ObjectPosition Position
		{
			get;
			protected set;
		}
		public override ActorLook Look
		{
			get
			{
				return this.m_look ?? this.RefreshLook();
			}
		}
		public short FirstNameId
		{
			get
			{
				return this.m_record.FirstNameId;
			}
			protected set
			{
				this.m_record.FirstNameId = value;
				this.m_name = null;
			}
		}
		public short LastNameId
		{
			get
			{
				return this.m_record.LastNameId;
			}
			protected set
			{
				this.m_record.LastNameId = value;
				this.m_name = null;
			}
		}
		public int GatheredExperience
		{
			get
			{
				return this.m_record.GatheredExperience;
			}
			set
			{
				this.m_record.GatheredExperience = value;
				this.IsRecordDirty = true;
			}
		}
		public int GatheredKamas
		{
			get
			{
				return this.m_record.GatheredKamas;
			}
			set
			{
				this.m_record.GatheredKamas = value;
				this.IsRecordDirty = true;
			}
		}
		public int AttacksCount
		{
			get
			{
				return this.m_record.AttacksCount;
			}
			set
			{
				this.m_record.AttacksCount = value;
				this.IsRecordDirty = true;
			}
		}
		public TaxCollectorFighter Fighter
		{
			get;
			private set;
		}
		public bool IsFighting
		{
			get
			{
				return this.Fighter != null;
			}
		}
		public System.DateTime NextMoveDate
		{
			get;
			set;
		}
		public System.DateTime LastMoveDate
		{
			get;
			private set;
		}
		public bool IsRecordDirty
		{
			get;
			private set;
		}
		public TaxCollectorNpc(int globalId, int contextId, ObjectPosition position, Guild guild, string callerName)
		{
			AsyncRandom asyncRandom = new AsyncRandom();
			this.m_contextId = contextId;
			this.Position = position;
			this.Guild = guild;
			this.Bag = new TaxCollectorBag(this);
			this.m_record = new WorldMapTaxCollectorRecord
			{
				Id = globalId,
				Map = this.Position.Map,
				Cell = (int)this.Position.Cell.Id,
				Direction = (int)this.Position.Direction,
				FirstNameId = (short)asyncRandom.Next(1, 154),
				LastNameId = (short)asyncRandom.Next(1, 253),
				GuildId = guild.Id,
				CallerName = callerName,
				Date = System.DateTime.Now
			};
			this.IsRecordDirty = true;
		}
		public TaxCollectorNpc(WorldMapTaxCollectorRecord record, int contextId)
		{
			this.m_record = record;
			this.m_contextId = contextId;
			this.Bag = new TaxCollectorBag(this);
			if (!record.MapId.HasValue)
			{
				throw new System.Exception("TaxCollector's map not found");
			}
			this.Position = new ObjectPosition(record.Map, record.Map.Cells[this.m_record.Cell], (DirectionsEnum)this.m_record.Direction);
			this.Guild = Singleton<GuildManager>.Instance.TryGetGuild(this.Record.GuildId);
			this.LoadRecord();
		}

		public ActorLook RefreshLook()
		{
			this.m_look = new ActorLook
			{
				BonesID = 714
			};
			if (this.Guild.Emblem.Template != null)
			{
				this.m_look.AddSkin((short)this.Guild.Emblem.Template.SkinId);
			}
            this.m_look.AddColor(7, this.Guild.Emblem.BackgroundColor);
            this.m_look.AddColor(8, this.Guild.Emblem.SymbolColor);

			return this.m_look;
		}
		public void InteractWith(NpcActionTypeEnum actionType, Character dialoguer)
		{
			if (this.CanInteractWith(actionType, dialoguer))
			{
				TaxCollectorInfoDialog taxCollectorInfoDialog = new TaxCollectorInfoDialog(dialoguer, this);
				taxCollectorInfoDialog.Open();
			}
		}
		public bool CanInteractWith(NpcActionTypeEnum action, Character dialoguer)
		{
			return this.CanBeSee(dialoguer) && action == NpcActionTypeEnum.ACTION_TALK;
		}
		public void OnDialogOpened(IDialog dialog)
		{
			this.m_openedDialogs.Add(dialog);
		}
		public void OnDialogClosed(IDialog dialog)
		{
			this.m_openedDialogs.Remove(dialog);
		}
		public void CloseAllDialogs()
		{
			IDialog[] array = this.OpenDialogs.ToArray<IDialog>();
			for (int i = 0; i < array.Length; i++)
			{
				IDialog dialog = array[i];
				dialog.Close();
			}
			this.m_openedDialogs.Clear();
		}
		public override bool StartMove(Path movementPath)
		{
			bool result;
			if (!this.CanMove() || movementPath.IsEmpty())
			{
				result = false;
			}
			else
			{
				this.Position = movementPath.EndPathPosition;
				System.Collections.Generic.IEnumerable<short> keys = movementPath.GetServerPathKeys();
				base.Map.ForEach(delegate(Character entry)
				{
					ContextHandler.SendGameMapMovementMessage(entry.Client, keys, this);
				});
				this.StopMove();
				this.LastMoveDate = System.DateTime.Now;
				result = true;
			}
			return result;
		}
		public TaxCollectorFighter CreateFighter(FightTeam team)
		{
			if (this.IsFighting)
			{
				throw new System.Exception("Tax collector is already fighting !");
			}
			this.Fighter = new TaxCollectorFighter(team, this);
			base.Map.Refresh(this);
			this.CloseAllDialogs();
			return this.Fighter;
		}
		public void RejoinMap()
		{
			if (this.IsFighting)
			{
				this.Fighter = null;
				base.Map.Refresh(this);
				this.AttacksCount++;
			}
		}
		public override bool CanBeSee(WorldObject byObj)
		{
			return base.CanBeSee(byObj) && !this.IsFighting;
		}
		public bool CanGatherLoots()
		{
			return !this.IsFighting;
		}
		public void LoadRecord()
		{
			this.Bag.LoadRecord();
		}
		public void Save()
		{
			ServerBase<WorldServer>.Instance.IOTaskPool.EnsureContext();
			if (this.Bag.IsDirty)
			{
				this.Bag.Save();
			}
			ServerBase<WorldServer>.Instance.DBAccessor.Database.Update(this.m_record);
		}
		public bool IsBagEmpty()
		{
			return this.Bag.Count == 0;
		}
		public bool IsTaxCollectorOwner(Stump.Server.WorldServer.Game.Guilds.GuildMember member)
		{
			return member.Guild.Id == this.m_record.GuildId;
		}
		public bool IsBusy()
		{
			return this.OpenDialogs.Any((IDialog x) => x is TaxCollectorTrade);
		}

		protected override void OnDisposed()
		{
			this.CloseAllDialogs();
			this.Guild.RemoveTaxCollector(this);
			base.OnDisposed();
		}

        public TaxCollectorStaticInformations GetStaticInformations()
        {
            return new TaxCollectorStaticInformations((ushort)FirstNameId, (ushort)LastNameId, Guild.GetGuildInformations());
        }
        public override GameContextActorInformations GetGameContextActorInformations(Character character)
        {
            return new GameRolePlayTaxCollectorInformations((int)Id, Look.GetEntityLook(), GetEntityDispositionInformations(), GetStaticInformations(), Guild.Level, (character == null || character.CanAttack(this) == FighterRefusedReasonEnum.FIGHTER_ACCEPTED) ? 0 : 1);
        }
        public TaxCollectorInformations GetNetworkTaxCollector()
        {
            return new TaxCollectorInformations(GlobalId, (ushort)FirstNameId, (ushort)LastNameId, GetAdditionalTaxCollectorInformations(), (short)Position.Map.Position.X, (short)Position.Map.Position.Y, (ushort)Position.Map.SubArea.Id, (sbyte)TaxCollectorStateEnum.STATE_COLLECTING, Look.GetEntityLook(), GetComplementaryInfos());
        }

        private IEnumerable<TaxCollectorComplementaryInformations> GetComplementaryInfos()
        {
            yield return new TaxCollectorGuildInformations(Guild.GetBasicGuildInformations());
            yield return new TaxCollectorLootInformations((uint)GatheredKamas, (ulong)GatheredExperience, (uint)Bag.BagWeight, (uint)Bag.BagValue);
        }
        
		public AdditionalTaxCollectorInformations GetAdditionalTaxCollectorInformations()
		{
			return new AdditionalTaxCollectorInformations(this.Record.CallerName, this.Record.Date.GetUnixTimeStamp());
		}
		public TaxCollectorBasicInformations GetTaxCollectorBasicInformations()
		{
            return new TaxCollectorBasicInformations((ushort)this.FirstNameId, (ushort)this.LastNameId, (short)this.Position.Map.Position.X, (short)this.Position.Map.Position.Y, this.Position.Map.Id, (ushort)this.Position.Map.SubArea.Id);
		}
		public ExchangeGuildTaxCollectorGetMessage GetExchangeGuildTaxCollector()
		{
			return new ExchangeGuildTaxCollectorGetMessage(this.Name, (short)this.Position.Map.Position.X, (short)this.Position.Map.Position.Y, this.Position.Map.Id, (ushort)this.Position.Map.SubArea.Id, this.Record.CallerName, (double)this.GatheredExperience,
                from x in this.Bag
				select x.GetObjectItemQuantity());
		}
		public StorageInventoryContentMessage GetStorageInventoryContent()
		{
			return new StorageInventoryContentMessage(
				from x in this.Bag
				select x.GetObjectItem(), (uint)this.GatheredKamas);
		}
	}
}
