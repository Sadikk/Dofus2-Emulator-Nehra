using NLog;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.ORM;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC.Objects;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Accounts;
using Stump.Server.WorldServer.Database.Breeds;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Achievements;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Merchants;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Actors.Stats;
using Stump.Server.WorldServer.Game.Arenas;
using Stump.Server.WorldServer.Game.Breeds;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Game.Dialogs.Interactives;
using Stump.Server.WorldServer.Game.Dialogs.Merchants;
using Stump.Server.WorldServer.Game.Dialogs.Npcs;
using Stump.Server.WorldServer.Game.Exchanges;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Pathfinding;
using Stump.Server.WorldServer.Game.Notifications;
using Stump.Server.WorldServer.Game.Parties;
using Stump.Server.WorldServer.Game.Shortcuts;
using Stump.Server.WorldServer.Game.Social;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Characters;
using Stump.Server.WorldServer.Handlers.Context;
using Stump.Server.WorldServer.Handlers.Context.RolePlay;
using Stump.Server.WorldServer.Handlers.Guilds;
using Stump.Server.WorldServer.Handlers.Moderation;
using Stump.Server.WorldServer.Handlers.PvP;
using Stump.Server.WorldServer.Handlers.Titles;
using System;
using System.Drawing;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Characters
{
    public sealed class Character : Humanoid, ICommandsUser, IStatsOwner, IInventoryOwner
    {
        public delegate void LevelChangedHandler(Character character, byte currentLevel, int difference);
        public delegate void GradeChangedHandler(Character character, sbyte currentGrade, int difference);
        public delegate void CharacterContextChangedHandler(Character character, bool inFight);
        public delegate void CharacterFightEndedHandler(Character character, CharacterFighter fighter);
        public delegate void CharacterDiedHandler(Character character);

        private const int AURA_1_SKIN = 170;
        private const int AURA_2_SKIN = 171;
        private new readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly CharacterRecord m_record;
        private bool m_recordLoaded;
        private bool m_inWorld;
        private int m_earnKamasInMerchant;
        private IDialoger m_dialoger;
        private IDialog m_dialog;
        private readonly System.Collections.Generic.Dictionary<int, PartyInvitation> m_partyInvitations = new System.Collections.Generic.Dictionary<int, PartyInvitation>();
        private readonly int[] JAILS_MAPS = new int[]
		{
			105121026,
			105119744,
			105120002
		};
        private readonly int[][] JAILS_CELLS = new int[][]
		{
			new int[]
			{
				179,
				445,
				184,
				435
			},
			new int[]
			{
				314
			},
			new int[]
			{
				300
			}
		};
        private ObjectPosition m_spawnPoint;
        private Merchant m_merchantToSpawn;
        private readonly System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>> m_commandsError = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>>();
        private readonly System.Collections.Generic.List<System.Exception> m_errors = new System.Collections.Generic.List<System.Exception>();
        public event System.Action<Character> LoggedIn;
        public event System.Action<Character> LoggedOut;
        public event System.Action<Character> Saved;
        public event Action<Character, int> LifeRegened;
        public event Character.LevelChangedHandler LevelChanged;
        public event Character.GradeChangedHandler GradeChanged;
        public event Action<Character, bool> PvPToggled;
        public event Action<Character, AlignmentSideEnum> AligmenentSideChanged;
        public event Character.CharacterContextChangedHandler ContextChanged;
        public event Character.CharacterFightEndedHandler FightEnded;
        public event Character.CharacterDiedHandler Died;

        public WorldClient Client
        {
            get;
            private set;
        }
        public AccountData Account
        {
            get
            {
                return this.Client.Account;
            }
        }
        public UserGroup UserGroup
        {
            get
            {
                return this.Client.UserGroup;
            }
        }
        public object SaveSync
        {
            get;
            private set;
        }
        public object LoggoutSync
        {
            get;
            private set;
        }
        public override bool IsInWorld
        {
            get
            {
                return this.m_inWorld;
            }
        }
        public CharacterMerchantBag MerchantBag
        {
            get;
            private set;
        }
        public override string Name
        {
            get
            {
                return (this.UserGroup.Role < RoleEnum.Moderator) ? this.m_record.Name : string.Format("[{0}]", this.m_record.Name);
            }
            protected set
            {
                this.m_record.Name = value;
                base.Name = value;
            }
        }
        public override int Id
        {
            get
            {
                return this.m_record.Id;
            }
            protected set
            {
                this.m_record.Id = value;
                base.Id = value;
            }
        }
        public Inventory Inventory
        {
            get;
            private set;
        }
        public int Kamas
        {
            get
            {
                return this.Record.Kamas;
            }
            set
            {
                this.Record.Kamas = value;
            }
        }
        public override ICharacterContainer CharacterContainer
        {
            get
            {
                ICharacterContainer result;
                if (this.IsFighting())
                {
                    result = this.Fight;
                }
                else
                {
                    result = base.Map;
                }
                return result;
            }
        }
        public IDialoger Dialoger
        {
            get
            {
                return this.m_dialoger;
            }
            private set
            {
                this.m_dialoger = value;
                this.m_dialog = ((value != null) ? this.m_dialoger.Dialog : null);
            }
        }
        public IDialog Dialog
        {
            get
            {
                return this.m_dialog;
            }
            private set
            {
                this.m_dialog = value;
                if (this.m_dialog == null)
                {
                    this.m_dialoger = null;
                }
            }
        }
        public PlayerAchievement Achievement
        {
            get;
            private set;
        }
        public NpcShopDialogLogger NpcShopDialog
        {
            get
            {
                return this.Dialog as NpcShopDialogLogger;
            }
        }
        public ZaapDialog ZaapDialog
        {
            get
            {
                return this.Dialog as ZaapDialog;
            }
        }
        public MerchantShopDialog MerchantShopDialog
        {
            get
            {
                return this.Dialog as MerchantShopDialog;
            }
        }
        public RequestBox RequestBox
        {
            get;
            private set;
        }
        public Party Party
        {
            get;
            private set;
        }
        public ArenaParty ArenaParty
        {
            get;
            private set;
        }
        public ArenaInvitation ArenaInvitation
        {
            get;
            private set;
        }
        public ITrade Trade
        {
            get
            {
                return this.Dialog as ITrade;
            }
        }
        public PlayerTrade PlayerTrade
        {
            get
            {
                return this.Trade as PlayerTrade;
            }
        }
        public Trader Trader
        {
            get
            {
                return this.Dialoger as Trader;
            }
        }
        public System.Collections.ObjectModel.ReadOnlyCollection<ushort> Titles
        {
            get
            {
                return this.Record.Titles.AsReadOnly();
            }
        }
        public System.Collections.ObjectModel.ReadOnlyCollection<ushort> Ornaments
        {
            get
            {
                return this.Record.Ornaments.AsReadOnly();
            }
        }
        public ushort? SelectedTitle
        {
            get
            {
                return this.Record.TitleId;
            }
            private set
            {
                this.Record.TitleId = value;
            }
        }
        public ushort? SelectedOrnament
        {
            get
            {
                return this.Record.Ornament;
            }
            private set
            {
                this.Record.Ornament = value;
            }
        }
        public bool CustomLookActivated
        {
            get
            {
                return this.m_record.CustomLookActivated;
            }
            set
            {
                this.m_record.CustomLookActivated = value;
            }
        }
        public ActorLook CustomLook
        {
            get
            {
                return this.m_record.CustomEntityLook;
            }
            set
            {
                this.m_record.CustomEntityLook = value;
            }
        }
        public ActorLook RealLook
        {
            get
            {
                return this.m_record.EntityLook;
            }
            private set
            {
                this.m_record.EntityLook = value;
                base.Look = value;
            }
        }
        public override ActorLook Look
        {
            get
            {
                return (!this.CustomLookActivated || this.CustomLook == null) ? this.RealLook : this.CustomLook;
            }
        }
        public override SexTypeEnum Sex
        {
            get
            {
                return this.m_record.Sex;
            }
            protected set
            {
                this.m_record.Sex = value;
            }
        }
        public PlayableBreedEnum BreedId
        {
            get
            {
                return this.m_record.Breed;
            }
            private set
            {
                this.m_record.Breed = value;
                this.Breed = Singleton<BreedManager>.Instance.GetBreed(value);
            }
        }
        public Breed Breed
        {
            get;
            private set;
        }
        public Head Head
        {
            get;
            private set;
        }
        public bool Invisible
        {
            get;
            private set;
        }
        public bool IsAway
        {
            get;
            private set;
        }
        public byte Level
        {
            get;
            private set;
        }
        public long Experience
        {
            get
            {
                return this.m_record.Experience;
            }
            private set
            {
                this.m_record.Experience = value;
                if ((value >= this.UpperBoundExperience && this.Level < Singleton<ExperienceManager>.Instance.HighestCharacterLevel) || value < this.LowerBoundExperience)
                {
                    byte level = this.Level;
                    this.Level = Singleton<ExperienceManager>.Instance.GetCharacterLevel(this.m_record.Experience);
                    this.LowerBoundExperience = Singleton<ExperienceManager>.Instance.GetCharacterLevelExperience(this.Level);
                    this.UpperBoundExperience = Singleton<ExperienceManager>.Instance.GetCharacterNextLevelExperience(this.Level);
                    int difference = (int)(this.Level - level);

                    this.OnLevelChanged(this.Level, difference);
                }
            }
        }
        public long LowerBoundExperience
        {
            get;
            private set;
        }
        public long UpperBoundExperience
        {
            get;
            private set;
        }
        public ushort StatsPoints
        {
            get
            {
                return this.m_record.StatsPoints;
            }
            set
            {
                this.m_record.StatsPoints = value;
            }
        }
        public ushort SpellsPoints
        {
            get
            {
                return this.m_record.SpellsPoints;
            }
            set
            {
                this.m_record.SpellsPoints = value;
            }
        }
        public short EnergyMax
        {
            get
            {
                return this.m_record.EnergyMax;
            }
            set
            {
                this.m_record.EnergyMax = value;
            }
        }
        public short Energy
        {
            get
            {
                return this.m_record.Energy;
            }
            set
            {
                this.m_record.Energy = value;
            }
        }
        public int LifePoints
        {
            get
            {
                return this.Stats.Health.Total;
            }
        }
        public int MaxLifePoints
        {
            get
            {
                return this.Stats.Health.TotalMax;
            }
        }
        public SpellInventory Spells
        {
            get;
            private set;
        }
        public StatsFields Stats
        {
            get;
            private set;
        }
        public bool GodMode
        {
            get;
            private set;
        }
        public short PermanentAddedStrength
        {
            get
            {
                return this.m_record.PermanentAddedStrength;
            }
            set
            {
                this.m_record.PermanentAddedStrength = value;
            }
        }
        public short PermanentAddedChance
        {
            get
            {
                return this.m_record.PermanentAddedChance;
            }
            set
            {
                this.m_record.PermanentAddedChance = value;
            }
        }
        public short PermanentAddedVitality
        {
            get
            {
                return this.m_record.PermanentAddedVitality;
            }
            set
            {
                this.m_record.PermanentAddedVitality = value;
            }
        }
        public short PermanentAddedWisdom
        {
            get
            {
                return this.m_record.PermanentAddedWisdom;
            }
            set
            {
                this.m_record.PermanentAddedWisdom = value;
            }
        }
        public short PermanentAddedIntelligence
        {
            get
            {
                return this.m_record.PermanentAddedIntelligence;
            }
            set
            {
                this.m_record.PermanentAddedIntelligence = value;
            }
        }
        public short PermanentAddedAgility
        {
            get
            {
                return this.m_record.PermanentAddedAgility;
            }
            set
            {
                this.m_record.PermanentAddedAgility = value;
            }
        }
        public bool CanRestat
        {
            get
            {
                return this.m_record.CanRestat;
            }
            set
            {
                this.m_record.CanRestat = value;
            }
        }
        public Stump.Server.WorldServer.Game.Guilds.GuildMember GuildMember
        {
            get;
            set;
        }
        public Guild Guild
        {
            get
            {
                return (this.GuildMember != null) ? this.GuildMember.Guild : null;
            }
        }
        public bool WarnOnGuildConnection
        {
            get
            {
                return this.Record.WarnOnGuildConnection;
            }
            set
            {
                this.Record.WarnOnGuildConnection = value;
                GuildHandler.SendGuildMemberWarnOnConnectionStateMessage(this.Client, value);
            }
        }
        public AlignmentSideEnum AlignmentSide
        {
            get
            {
                return this.m_record.AlignmentSide;
            }
            private set
            {
                this.m_record.AlignmentSide = value;
            }
        }
        public sbyte AlignmentGrade
        {
            get;
            private set;
        }
        public sbyte AlignmentValue
        {
            get
            {
                return this.m_record.AlignmentValue;
            }
            private set
            {
                this.m_record.AlignmentValue = value;
            }
        }
        public ushort Honor
        {
            get
            {
                return this.m_record.Honor;
            }
            set
            {
                this.m_record.Honor = Convert.ToUInt16((value > 17500) ? 17500 : value);
                if (value >= this.UpperBoundHonor && this.AlignmentGrade < (sbyte)Singleton<ExperienceManager>.Instance.HighestGrade)
                {
                    sbyte alignmentGrade = this.AlignmentGrade;
                    this.AlignmentGrade = (sbyte)Singleton<ExperienceManager>.Instance.GetAlignementGrade(this.m_record.Honor);
                    this.LowerBoundHonor = Singleton<ExperienceManager>.Instance.GetAlignementGradeHonor((byte)this.AlignmentGrade);
                    this.UpperBoundHonor = Singleton<ExperienceManager>.Instance.GetAlignementNextGradeHonor((byte)this.AlignmentGrade);
                    int difference = (int)(this.AlignmentGrade - alignmentGrade);
                    this.OnGradeChanged(this.AlignmentGrade, difference);
                }
            }
        }
        public ushort LowerBoundHonor
        {
            get;
            private set;
        }
        public ushort UpperBoundHonor
        {
            get;
            private set;
        }
        public ushort Dishonor
        {
            get
            {
                return this.m_record.Dishonor;
            }
            private set
            {
                this.m_record.Dishonor = value;
            }
        }
        public int CharacterPower
        {
            get
            {
                return this.Id + this.Level;
            }
        }
        public bool PvPEnabled
        {
            get
            {
                return this.m_record.PvPEnabled;
            }
            private set
            {
                this.m_record.PvPEnabled = value;
                this.OnPvPToggled();
            }
        }
        public CharacterFighter Fighter
        {
            get;
            private set;
        }
        public FightSpectator Spectator
        {
            get;
            private set;
        }
        public FightPvT TaxCollectorDefendFight
        {
            get;
            private set;
        }
        public Fights.Fight Fight
        {
            get
            {
                return (this.Fighter == null) ? ((this.Spectator != null) ? this.Spectator.Fight : null) : this.Fighter.Fight;
            }
        }
        public FightTeam Team
        {
            get
            {
                return (this.Fighter != null) ? this.Fighter.Team : null;
            }
        }
        public ShortcutBar Shortcuts
        {
            get;
            private set;
        }
        public byte RegenSpeed
        {
            get;
            private set;
        }
        public System.DateTime? RegenStartTime
        {
            get;
            private set;
        }
        public ChatHistory ChatHistory
        {
            get;
            private set;
        }
        public System.DateTime? MuteUntil
        {
            get
            {
                return this.m_record.MuteUntil;
            }
            private set
            {
                this.m_record.MuteUntil = value;
            }
        }
        public bool AdminMessagesEnabled
        {
            get;
            set;
        }
        public System.Collections.Generic.List<Map> KnownZaaps
        {
            get
            {
                return this.Record.KnownZaaps;
            }
        }
        public FriendsBook FriendsBook
        {
            get;
            private set;
        }
        public bool IsLoggedIn
        {
            get;
            private set;
        }
        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Exception>> CommandsErrors
        {
            get
            {
                return this.m_commandsError;
            }
        }
        public System.Collections.Generic.List<System.Exception> Errors
        {
            get
            {
                return this.m_errors;
            }
        }
        internal CharacterRecord Record
        {
            get
            {
                return this.m_record;
            }
        }
        public Character(CharacterRecord record, WorldClient client)
        {
            this.m_record = record;
            this.Client = client;
            this.SaveSync = new object();
            this.LoggoutSync = new object();
            this.LoadRecord();
        }

        private void OnLoggedIn()
        {
            if (this.GuildMember != null)
            {
                this.GuildMember.OnCharacterConnected(this);
            }
            System.Action<Character> loggedIn = this.LoggedIn;
            if (loggedIn != null)
            {
                loggedIn(this);
            }
        }
        private void OnLoggedOut()
        {
            if (this.GuildMember != null)
            {
                this.GuildMember.OnCharacterDisconnected(this);
            }
            if (this.TaxCollectorDefendFight != null)
            {
                this.TaxCollectorDefendFight.RemoveDefender(this);
            }
            System.Action<Character> loggedOut = this.LoggedOut;
            if (loggedOut != null)
            {
                loggedOut(this);
            }
        }
        private void OnSaved()
        {
            System.Action<Character> saved = this.Saved;
            if (saved != null)
            {
                saved(this);
            }
        }
        private void OnLifeRegened(int regenedLife)
        {
            Action<Character, int> lifeRegened = this.LifeRegened;
            if (lifeRegened != null)
            {
                lifeRegened(this, regenedLife);
            }
        }
        public void SetDialoger(IDialoger dialoger)
        {
            this.Dialoger = dialoger;
        }
        public void SetDialog(IDialog dialog)
        {
            if (this.Dialog != null)
            {
                this.Dialog.Close();
            }
            this.Dialog = dialog;
        }
        public void CloseDialog(IDialog dialog)
        {
            if (this.Dialog == dialog)
            {
                this.Dialoger = null;
            }
        }
        public void ResetDialog()
        {
            this.Dialoger = null;
        }
        public void OpenRequestBox(RequestBox request)
        {
            this.RequestBox = request;
        }
        public void ResetRequestBox()
        {
            this.RequestBox = null;
        }
        public bool IsBusy()
        {
            return this.IsInRequest() || this.IsDialoging();
        }
        public bool IsDialoging()
        {
            return this.Dialog != null;
        }
        public bool IsInRequest()
        {
            return this.RequestBox != null;
        }
        public bool IsRequestSource()
        {
            return this.IsInRequest() && this.RequestBox.Source == this;
        }
        public bool IsRequestTarget()
        {
            return this.IsInRequest() && this.RequestBox.Target == this;
        }
        public bool IsTalkingWithNpc()
        {
            return this.Dialog is NpcDialog;
        }
        public bool IsInZaapDialog()
        {
            return this.Dialog is ZaapDialog;
        }
        public bool IsInMerchantDialog()
        {
            return this.Dialog is MerchantShopDialog;
        }
        public bool IsInParty()
        {
            return this.Party != null;
        }
        public bool IsPartyLeader()
        {
            return this.IsInParty() && this.Party.Leader == this;
        }
        public bool IsTrading()
        {
            return this.Trade != null;
        }
        public bool IsTradingWithPlayer()
        {
            return this.PlayerTrade != null;
        }
        public bool HasTitle(ushort title)
        {
            return this.Record.Titles.Contains(title);
        }
        public void AddTitle(ushort title)
        {
            if (!this.HasTitle(title))
            {
                this.Record.Titles.Add(title);
                TitleHandler.SendTitleGainedMessage(this.Client, (short)title);
            }
        }
        public bool RemoveTitle(ushort title)
        {
            bool result;
            if (result = this.Record.Titles.Remove(title))
            {
                TitleHandler.SendTitleLostMessage(this.Client, (short)title);
            }
            return result;
        }
        public bool SelectTitle(ushort title)
        {
            bool result;
            if (!this.HasTitle(title))
            {
                result = false;
            }
            else
            {
                this.SelectedTitle = new ushort?(title);
                TitleHandler.SendTitleSelectedMessage(this.Client, (short)title);
                this.RefreshActor();
                result = true;
            }
            return result;
        }
        public void ResetTitle()
        {
            this.SelectedTitle = null;
            TitleHandler.SendTitleSelectedMessage(this.Client, 0);
            this.RefreshActor();
        }
        public bool HasOrnament(ushort ornament)
        {
            return this.Record.Ornaments.Contains(ornament);
        }
        public void AddOrnament(ushort ornament)
        {
            if (!this.HasOrnament(ornament))
            {
                this.Record.Ornaments.Add(ornament);
            }
            TitleHandler.SendOrnamentGainedMessage(this.Client, (short)ornament);
        }
        public bool RemoveOrnament(ushort ornament)
        {
            bool result;
            if (result = this.Record.Ornaments.Remove(ornament))
            {
                TitleHandler.SendTitlesAndOrnamentsListMessage(this.Client, this);
            }
            return result;
        }
        public void RemoveAllOrnament()
        {
            this.Record.Ornaments.Clear();
            TitleHandler.SendTitlesAndOrnamentsListMessage(this.Client, this);
        }
        public bool SelectOrnament(ushort ornament)
        {
            bool result;
            if (!this.HasOrnament(ornament))
            {
                result = false;
            }
            else
            {
                this.SelectedOrnament = new ushort?(ornament);
                TitleHandler.SendOrnamentSelectedMessage(this.Client, (short)ornament);
                this.RefreshActor();
                result = true;
            }
            return result;
        }
        public void ResetOrnament()
        {
            this.SelectedOrnament = null;
            TitleHandler.SendOrnamentSelectedMessage(this.Client, 0);
            this.RefreshActor();
        }
        public bool ToggleAway()
        {
            this.IsAway = !this.IsAway;
            return this.IsAway;
        }
        public bool ToggleInvisibility(bool toggle)
        {
            this.Invisible = toggle;
            if (!this.IsInFight())
            {
                base.Map.Refresh(this);
            }
            this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, Convert.ToInt16(toggle ? 236 : 237), new object[0]);
            return this.Invisible;
        }
        public bool ToggleInvisibility()
        {
            return this.ToggleInvisibility(!this.Invisible);
        }
        public void UpdateLook(bool send = true)
        {
            System.Collections.Generic.List<short> list = new System.Collections.Generic.List<short>(this.Breed.GetLook(this.Sex, false).Skins);
            list.AddRange(this.Head.Skins);
            list.AddRange(this.Inventory.GetItemsSkins());
            this.RealLook.SetSkins(list.ToArray());
            short? petSkin = this.Inventory.GetPetSkin();
            if (petSkin.HasValue)
            {
                this.RealLook.SetPetSkin(petSkin.Value);
            }
            else
            {
                this.RealLook.RemovePets();
            }
            if (send)
            {
                this.RefreshActor();
            }
        }
        public void RefreshActor()
        {
            if (base.Map != null)
            {
                base.Map.Area.ExecuteInContext(delegate
                {
                    base.Map.Refresh(this);
                });
            }
        }
        public void LevelUp(byte levelAdded)
        {
            byte level;
            if (levelAdded + this.Level > Singleton<ExperienceManager>.Instance.HighestCharacterLevel)
            {
                level = Singleton<ExperienceManager>.Instance.HighestCharacterLevel;
            }
            else
            {
                level = Convert.ToByte(levelAdded + this.Level);
            }
            long characterLevelExperience = Singleton<ExperienceManager>.Instance.GetCharacterLevelExperience(level);
            this.Experience = characterLevelExperience;
        }
        public void LevelDown(byte levelRemoved)
        {
            byte level;
            if (this.Level - levelRemoved < 1)
            {
                level = 1;
            }
            else
            {
                level = Convert.ToByte(this.Level - levelRemoved);
            }
            long characterLevelExperience = Singleton<ExperienceManager>.Instance.GetCharacterLevelExperience(level);
            this.Experience = characterLevelExperience;
        }
        public void AddExperience(int amount)
        {
            this.Experience += (long)amount;
        }
        public void AddExperience(long amount)
        {
            this.Experience += amount;
        }
        public void AddExperience(double amount)
        {
            this.Experience += (long)amount;
        }
        private void OnLevelChanged(byte currentLevel, int difference)
        {
            if (difference > 0)
            {
                this.SpellsPoints += (ushort)difference;
                this.StatsPoints += (ushort)(difference * 5);
            }
            this.Stats.Health.Base += (int)((short)(difference * 5));
            this.Stats.Health.DamageTaken = 0;
            if (currentLevel >= 100 && (int)currentLevel - difference < 100)
            {
                this.Stats.AP.Base++;
                this.AddOrnament(13);
            }
            else
            {
                if (currentLevel < 100 && (int)currentLevel - difference >= 100)
                {
                    this.Stats.AP.Base--;
                    this.RemoveOrnament(13);
                }
            }
            if (currentLevel >= 160 && (int)currentLevel - difference < 160)
            {
                this.AddOrnament(14);
            }
            else
            {
                if (currentLevel < 160 && (int)currentLevel - difference >= 160)
                {
                    this.RemoveOrnament(14);
                }
            }
            if (currentLevel >= 200 && (int)currentLevel - difference < 200)
            {
                this.AddOrnament(15);
            }
            else
            {
                if (currentLevel < 200 && (int)currentLevel - difference >= 200)
                {
                    this.RemoveOrnament(15);
                }
            }
            foreach (BreedSpell current in this.Breed.Spells)
            {
                if (current.ObtainLevel > (int)currentLevel && this.Spells.HasSpell(current.Spell))
                {
                    this.Spells.UnLearnSpell(current.Spell);
                }
                else
                {
                    if (current.ObtainLevel <= (int)currentLevel && !this.Spells.HasSpell(current.Spell))
                    {
                        this.Spells.LearnSpell(current.Spell);
                        this.Shortcuts.AddSpellShortcut(this.Shortcuts.GetNextFreeSlot(ShortcutBarEnum.SPELL_SHORTCUT_BAR), (short)current.Spell);
                    }
                }
            }
            this.RefreshStats();
            CharacterHandler.SendCharacterLevelUpMessage(this.Client, currentLevel);
            CharacterHandler.SendCharacterLevelUpInformationMessage(base.Map.Clients, this, currentLevel);
            Character.LevelChangedHandler levelChanged = this.LevelChanged;
            if (levelChanged != null)
            {
                levelChanged(this, currentLevel, difference);
            }
        }
        public void RefreshStats()
        {
            if (this.IsRegenActive())
            {
                this.UpdateRegenedLife();
            }
            CharacterHandler.SendCharacterStatsListMessage(this.Client);
        }
        public void ToggleGodMode(bool state)
        {
            this.GodMode = state;
        }
        public bool IsGameMaster()
        {
            return this.UserGroup.IsGameMaster;
        }
        public void ChangeAlignementSide(AlignmentSideEnum side)
        {
            this.AlignmentSide = side;
            this.OnAligmenentSideChanged();
        }
        public void AddHonor(ushort amount)
        {
            this.Honor += amount;
        }
        public void SubHonor(ushort amount)
        {
            if (this.Honor - amount < 0)
            {
                this.Honor = 0;
            }
            else
            {
                this.Honor -= amount;
            }
        }
        public void AddDishonor(ushort amount)
        {
            this.Dishonor += amount;
        }
        public void SubDishonor(ushort amount)
        {
            if (this.Dishonor - amount < 0)
            {
                this.Dishonor = 0;
            }
            else
            {
                this.Dishonor -= amount;
            }
        }
        public void TogglePvPMode(bool state)
        {
            this.PvPEnabled = state;
        }
        private void OnGradeChanged(sbyte currentLevel, int difference)
        {
            base.Map.Refresh(this);
            this.RefreshStats();
            Character.GradeChangedHandler gradeChanged = this.GradeChanged;
            if (gradeChanged != null)
            {
                gradeChanged(this, currentLevel, difference);
            }
        }
        private void OnPvPToggled()
        {
            base.Map.Refresh(this);
            this.RefreshStats();
            Action<Character, bool> pvPToggled = this.PvPToggled;
            if (pvPToggled != null)
            {
                pvPToggled(this, this.PvPEnabled);
            }
        }
        private void OnAligmenentSideChanged()
        {
            base.Map.Refresh(this);
            this.RefreshStats();
            this.TogglePvPMode(false);
            this.Honor = 0;
            this.Dishonor = 0;
            Action<Character, AlignmentSideEnum> aligmenentSideChanged = this.AligmenentSideChanged;
            if (aligmenentSideChanged != null)
            {
                aligmenentSideChanged(this, this.AlignmentSide);
            }
        }
        public bool IsSpectator()
        {
            return this.Spectator != null;
        }
        public bool IsInFight()
        {
            return this.IsSpectator() || this.IsFighting();
        }
        public bool IsFighting()
        {
            return this.Fighter != null;
        }
        public void SetDefender(FightPvT fight)
        {
            this.TaxCollectorDefendFight = fight;
        }
        public void ResetDefender()
        {
            this.TaxCollectorDefendFight = null;
        }
        public void Mute(System.TimeSpan time, Character from)
        {
            this.MuteUntil = new System.DateTime?(System.DateTime.Now + time);
            this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 17, new object[]
			{
				from.Name,
				(int)time.TotalMinutes
			});
        }
        public void Mute(System.TimeSpan time)
        {
            this.MuteUntil = new System.DateTime?(System.DateTime.Now + time);
            this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 123, new object[]
			{
				(int)time.TotalSeconds
			});
        }
        public void UnMute()
        {
            this.MuteUntil = null;
        }
        public bool IsMuted()
        {
            return this.MuteUntil.HasValue && this.MuteUntil > System.DateTime.Now;
        }
        public System.TimeSpan GetMuteRemainingTime()
        {
            System.TimeSpan result;
            if (!this.MuteUntil.HasValue)
            {
                result = System.TimeSpan.MaxValue;
            }
            else
            {
                result = this.MuteUntil.Value - System.DateTime.Now;
            }
            return result;
        }
        public void SendConnectionMessages()
        {
            this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 89, new object[0]);
            if (this.Account.LastConnection.HasValue)
            {
                System.DateTime value = this.Account.LastConnection.Value;
                this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 193, new object[]
				{
					value.Year,
					value.Month,
					value.Day,
					value.Hour,
					value.Minute.ToString("00")
				});
            }
            if (this.m_earnKamasInMerchant > 0)
            {
                this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 45, new object[]
				{
					this.m_earnKamasInMerchant
				});
            }
        }
        public void SendServerMessage(string message)
        {
            BasicHandler.SendTextInformationMessage(this.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 0, new string[]
			{
				message
			});
        }
        public void SendServerMessage(string message, Color color)
        {
            this.SendServerMessage(string.Format("<font color=\"#{0}\">{1}</font>", color.ToArgb().ToString("X"), message));
        }
        public void SendInformationMessage(TextInformationTypeEnum msgType, short msgId, params object[] parameters)
        {
            BasicHandler.SendTextInformationMessage(this.Client, msgType, msgId, parameters);
        }
        public void SendSystemMessage(short msgId, bool hangUp, params object[] parameters)
        {
            BasicHandler.SendSystemMessageDisplayMessage(this.Client, hangUp, msgId, parameters);
        }
        public void OpenPopup(string message)
        {
            this.OpenPopup(message, "Server", 0);
        }
        public void OpenPopup(string message, string sender, byte lockDuration)
        {
            ModerationHandler.SendPopupWarningMessage(this.Client, message, sender, lockDuration);
        }

        public override void OnEnterMap(Map map)
        {
            ContextRoleplayHandler.SendCurrentMapMessage(this.Client, map.Id);
            if (map.Fights.Count > 0)
            {
                ContextRoleplayHandler.SendMapFightCountMessage(this.Client, (short)map.Fights.Count);
            }
            foreach (RolePlayActor current in map.Actors)
            {
                if (current.IsMoving())
                {
                    System.Collections.Generic.IEnumerable<short> serverPathKeys = current.MovementPath.GetServerPathKeys();
                    RolePlayActor actor = current;
                    ContextHandler.SendGameMapMovementMessage(this.Client, serverPathKeys, actor);
                    BasicHandler.SendBasicNoOperationMessage(this.Client);
                }
            }
            BasicHandler.SendBasicTimeMessage(this.Client);
            PvPHandler.SendUpdateMapPlayersAgressableStatusMessage(this.Client, map);
            if (map.Zaap != null && !this.KnownZaaps.Contains(map))
            {
                this.DiscoverZaap(map);
            }
            if (this.MustBeJailed() && !this.IsInJail())
            {
                this.TeleportToJail();
            }
            else
            {
                if (!this.MustBeJailed() && this.IsInJail())
                {
                    this.Teleport(this.Breed.GetStartPosition(), true);
                }
            }
            base.OnEnterMap(map);
        }
        public override bool CanMove()
        {
            return base.CanMove() && !this.IsDialoging();
        }
        public override bool StartMove(Path movementPath)
        {
            bool result;
            if (!this.IsFighting() && !this.MustBeJailed() && this.IsInJail())
            {
                this.Teleport(this.Breed.GetStartPosition(), true);
                result = false;
            }
            else
            {
                result = (this.IsFighting() ? this.Fighter.StartMove(movementPath) : base.StartMove(movementPath));
            }
            return result;
        }
        public override bool StopMove()
        {
            return this.IsFighting() ? this.Fighter.StopMove() : base.StopMove();
        }
        public override bool MoveInstant(ObjectPosition destination)
        {
            return this.IsFighting() ? this.Fighter.MoveInstant(destination) : base.MoveInstant(destination);
        }
        public override bool StopMove(ObjectPosition currentObjectPosition)
        {
            return this.IsFighting() ? this.Fighter.StopMove(currentObjectPosition) : base.StopMove(currentObjectPosition);
        }
        public override bool Teleport(MapNeighbour mapNeighbour)
        {
            bool result;
            if (!(result = base.Teleport(mapNeighbour)))
            {
                this.SendServerMessage("Unknown map transition");
            }
            return result;
        }

        public bool TeleportToJail()
        {
            AsyncRandom asyncRandom = new AsyncRandom();
            int num = asyncRandom.Next(0, this.JAILS_MAPS.Length);
            int num2 = asyncRandom.Next(0, this.JAILS_CELLS[num].Length);
            Map map = Singleton<World>.Instance.GetMap(this.JAILS_MAPS[num]);
            bool result;
            if (map == null)
            {
                this.logger.Error("Cannot find jail map {0}", this.JAILS_MAPS[num]);
                result = false;
            }
            else
            {
                Cell cell = map.Cells[this.JAILS_CELLS[num][num2]];
                this.Teleport(new ObjectPosition(map, cell), false);
                result = true;
            }
            return result;
        }
        public bool MustBeJailed()
        {
            return this.Client.Account.IsJailed && (!this.Client.Account.BanEndDate.HasValue || this.Client.Account.BanEndDate > System.DateTime.Now);
        }
        public bool IsInJail()
        {
            return this.JAILS_MAPS.Contains(base.Map.Id);
        }
        protected override void OnTeleported(ObjectPosition position)
        {
            base.OnTeleported(position);
            this.UpdateRegenedLife();
            if (this.Dialog != null)
            {
                this.Dialog.Close();
            }
        }
        public override bool CanChangeMap()
        {
            return base.CanChangeMap() && !this.IsFighting() && !this.Account.IsJailed;
        }
        public void DisplayNotification(string text, NotificationEnum notification = NotificationEnum.INFORMATION)
        {
            this.Client.Send(new NotificationByServerMessage((ushort)notification, new string[]
			{
				text
			}, true));
        }
        public void DisplayNotification(Notification notification)
        {
            notification.Display();
        }
        public void LeaveDialog()
        {
            if (this.IsInRequest())
            {
                this.CancelRequest();
            }
            if (this.IsDialoging())
            {
                this.Dialog.Close();
            }
        }
        public void ReplyToNpc(short replyId)
        {
            if (this.IsTalkingWithNpc())
            {
                ((NpcDialog)this.Dialog).Reply(replyId);
            }
        }
        public void AcceptRequest()
        {
            if (this.IsInRequest() && this.RequestBox.Target == this)
            {
                this.RequestBox.Accept();
            }
        }
        public void DenyRequest()
        {
            if (this.IsInRequest() && this.RequestBox.Target == this)
            {
                this.RequestBox.Deny();
            }
        }
        public void CancelRequest()
        {
            if (this.IsInRequest())
            {
                if (this.IsRequestSource())
                {
                    this.RequestBox.Cancel();
                }
                else
                {
                    if (this.IsRequestTarget())
                    {
                        this.DenyRequest();
                    }
                }
            }
        }

        public void Invite(Character target)
        {
            if (!this.IsInParty())
            {
                Party party = Singleton<PartyManager>.Instance.CreateClassicalParty(this);
                this.EnterParty(party);
            }
            if (this.Party.CanInvite(target) && !target.m_partyInvitations.ContainsKey(this.Party.Id))
            {
                PartyInvitation partyInvitation = new PartyInvitation(this.Party, this, target);
                target.m_partyInvitations.Add(this.Party.Id, partyInvitation);
                this.Party.AddGuest(target);
                partyInvitation.Display();
            }
        }
        public PartyInvitation GetInvitation(int id)
        {
            return this.m_partyInvitations.ContainsKey(id) ? this.m_partyInvitations[id] : null;
        }
        public PartyInvitation GetInvitation(uint id)
        {
            return this.m_partyInvitations.ContainsKey((int)id) ? this.m_partyInvitations[(int)id] : null;
        }
        public bool RemoveInvitation(PartyInvitation invitation)
        {
            return this.m_partyInvitations.Remove(invitation.Party.Id);
        }
        public void DenyAllInvitations()
        {
            System.Collections.Generic.KeyValuePair<int, PartyInvitation>[] array = this.m_partyInvitations.ToArray<System.Collections.Generic.KeyValuePair<int, PartyInvitation>>();
            for (int i = 0; i < array.Length; i++)
            {
                System.Collections.Generic.KeyValuePair<int, PartyInvitation> keyValuePair = array[i];
                keyValuePair.Value.Deny();
            }
        }
        public void EnterParty(Party party)
        {
            if (this.IsInParty())
            {
                this.LeaveParty();
            }
            if (this.m_partyInvitations.ContainsKey(party.Id))
            {
                this.m_partyInvitations.Remove(party.Id);
            }
            this.DenyAllInvitations();
            this.UpdateRegenedLife();
            this.Party = party;
            this.Party.MemberRemoved += new Party.MemberRemovedHandler(this.OnPartyMemberRemoved);
            this.Party.PartyDeleted += new System.Action<Party>(this.OnPartyDeleted);
            if (!party.IsMember(this) && !party.PromoteGuestToMember(this))
            {
                this.Party.MemberRemoved -= new Party.MemberRemovedHandler(this.OnPartyMemberRemoved);
                this.Party.PartyDeleted -= new System.Action<Party>(this.OnPartyDeleted);
                this.Party = null;
            }
        }
        public void LeaveParty()
        {
            if (this.IsInParty())
            {
                this.Party.MemberRemoved -= new Party.MemberRemovedHandler(this.OnPartyMemberRemoved);
                this.Party.PartyDeleted -= new System.Action<Party>(this.OnPartyDeleted);
                this.Party.RemoveMember(this);
                this.Party = null;
            }
        }
        private void OnPartyMemberRemoved(Party party, Character member, bool kicked)
        {
            if (member == this)
            {
                this.Party.MemberRemoved -= new Party.MemberRemovedHandler(this.OnPartyMemberRemoved);
                this.Party.PartyDeleted -= new System.Action<Party>(this.OnPartyDeleted);
                this.Party = null;
            }
        }
        private void OnPartyDeleted(Party party)
        {
            this.Party.MemberRemoved -= new Party.MemberRemovedHandler(this.OnPartyMemberRemoved);
            this.Party.PartyDeleted -= new System.Action<Party>(this.OnPartyDeleted);
            this.Party = null;
        }
        private void OnDied()
        {
            ObjectPosition objectPosition = this.GetSpawnPoint() ?? this.Breed.GetStartPosition();
            base.NextMap = objectPosition.Map;
            base.Cell = objectPosition.Cell;
            base.Direction = objectPosition.Direction;
            this.Stats.Health.DamageTaken = (int)((short)(this.Stats.Health.TotalMax - 1));
            Character.CharacterDiedHandler died = this.Died;
            if (died != null)
            {
                died(this);
            }
        }
        private void OnFightEnded(CharacterFighter fighter)
        {
            Character.CharacterFightEndedHandler fightEnded = this.FightEnded;
            if (fightEnded != null)
            {
                fightEnded(this, fighter);
            }
        }
        private void OnCharacterContextChanged(bool inFight)
        {
            Character.CharacterContextChangedHandler contextChanged = this.ContextChanged;
            if (contextChanged != null)
            {
                contextChanged(this, inFight);
            }
        }
        public FighterRefusedReasonEnum CanRequestFight(Character target)
        {
            FighterRefusedReasonEnum result;
            if (!target.IsInWorld || target.IsFighting() || target.IsSpectator() || target.IsBusy() || target.IsAway)
            {
                result = FighterRefusedReasonEnum.OPPONENT_OCCUPIED;
            }
            else
            {
                if (!this.IsInWorld || this.IsFighting() || this.IsSpectator() || this.IsBusy())
                {
                    result = FighterRefusedReasonEnum.IM_OCCUPIED;
                }
                else
                {
                    if (target == this)
                    {
                        result = FighterRefusedReasonEnum.FIGHT_MYSELF;
                    }
                    else
                    {
                        if (target.Map != base.Map || !base.Map.AllowFightChallenges)
                        {
                            result = FighterRefusedReasonEnum.WRONG_MAP;
                        }
                        else
                        {
                            result = FighterRefusedReasonEnum.FIGHTER_ACCEPTED;
                        }
                    }
                }
            }
            return result;
        }
        public FighterRefusedReasonEnum CanAgress(Character target)
        {
            FighterRefusedReasonEnum result;
            if (target == this)
            {
                result = FighterRefusedReasonEnum.FIGHT_MYSELF;
            }
            else
            {
                if (!target.PvPEnabled || !this.PvPEnabled)
                {
                    result = FighterRefusedReasonEnum.INSUFFICIENT_RIGHTS;
                }
                else
                {
                    if (!target.IsInWorld || target.IsFighting() || target.IsSpectator() || target.IsBusy())
                    {
                        result = FighterRefusedReasonEnum.OPPONENT_OCCUPIED;
                    }
                    else
                    {
                        if (!this.IsInWorld || this.IsFighting() || this.IsSpectator() || this.IsBusy())
                        {
                            result = FighterRefusedReasonEnum.IM_OCCUPIED;
                        }
                        else
                        {
                            if (this.AlignmentSide <= AlignmentSideEnum.ALIGNMENT_NEUTRAL || target.AlignmentSide <= AlignmentSideEnum.ALIGNMENT_NEUTRAL)
                            {
                                result = FighterRefusedReasonEnum.WRONG_ALIGNMENT;
                            }
                            else
                            {
                                if (target.AlignmentSide == this.AlignmentSide)
                                {
                                    result = FighterRefusedReasonEnum.WRONG_ALIGNMENT;
                                }
                                else
                                {
                                    if (target.Map != base.Map || !base.Map.AllowAggression)
                                    {
                                        result = FighterRefusedReasonEnum.WRONG_MAP;
                                    }
                                    else
                                    {
                                        if (target.Client.IP == this.Client.IP)
                                        {
                                            result = FighterRefusedReasonEnum.MULTIACCOUNT_NOT_ALLOWED;
                                        }
                                        else
                                        {
                                            if (this.Level - target.Level > 20)
                                            {
                                                result = FighterRefusedReasonEnum.INSUFFICIENT_RIGHTS;
                                            }
                                            else
                                            {
                                                result = FighterRefusedReasonEnum.FIGHTER_ACCEPTED;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        public FighterRefusedReasonEnum CanAttack(TaxCollectorNpc target)
        {
            FighterRefusedReasonEnum result;
            if (this.GuildMember != null && target.IsTaxCollectorOwner(this.GuildMember))
            {
                result = FighterRefusedReasonEnum.WRONG_GUILD;
            }
            else
            {
                if (target.IsBusy())
                {
                    result = FighterRefusedReasonEnum.OPPONENT_OCCUPIED;
                }
                else
                {
                    if (target.Map != base.Map)
                    {
                        result = FighterRefusedReasonEnum.WRONG_MAP;
                    }
                    else
                    {
                        result = FighterRefusedReasonEnum.FIGHTER_ACCEPTED;
                    }
                }
            }
            return result;
        }
        public CharacterFighter CreateFighter(FightTeam team)
        {
            CharacterFighter result;
            if (this.IsFighting() || this.IsSpectator() || !this.IsInWorld)
            {
                result = null;
            }
            else
            {
                base.NextMap = base.Map;
                base.Map.Leave(this);
                this.StopRegen();
                ContextHandler.SendGameContextDestroyMessage(this.Client);
                ContextHandler.SendGameContextCreateMessage(this.Client, 2);
                ContextHandler.SendGameFightStartingMessage(this.Client, team.Fight.FightType);
                this.Fighter = new CharacterFighter(this, team);
                this.OnCharacterContextChanged(true);
                result = this.Fighter;
            }
            return result;
        }
        public CharacterFighter SetFighter(CharacterFighter fighter)
        {
            this.Fighter = fighter;

            return this.Fighter;
        }
        public FightSpectator CreateSpectator(Fights.Fight fight)
        {
            FightSpectator result;
            if (this.IsFighting() || this.IsSpectator() || !this.IsInWorld)
            {
                result = null;
            }
            else
            {
                if (!fight.CanSpectatorJoin(this))
                {
                    result = null;
                }
                else
                {
                    base.NextMap = base.Map;
                    base.Map.Leave(this);
                    this.StopRegen();
                    ContextHandler.SendGameContextDestroyMessage(this.Client);
                    ContextHandler.SendGameContextCreateMessage(this.Client, 2);
                    ContextHandler.SendGameFightStartingMessage(this.Client, fight.FightType);
                    this.Spectator = new FightSpectator(this, fight);
                    this.OnCharacterContextChanged(true);
                    result = this.Spectator;
                }
            }
            return result;
        }
        public void RejoinMap()
        {
            if (this.IsFighting() || this.IsSpectator())
            {
                if (this.Fighter != null)
                {
                    this.OnFightEnded(this.Fighter);
                }
                if (this.GodMode)
                {
                    this.Stats.Health.DamageTaken = 0;
                }
                else
                {
                    if (this.Fighter != null && (this.Fighter.HasLeft() || this.Fight.Losers == this.Fighter.Team) && !(this.Fight is FightDuel))
                    {
                        this.OnDied();
                    }
                }
                this.Fighter = null;
                this.Spectator = null;
                ContextHandler.SendGameContextDestroyMessage(this.Client);
                ContextHandler.SendGameContextCreateMessage(this.Client, 1);
                this.RefreshStats();
                this.OnCharacterContextChanged(false);
                this.StartRegen();

                if (base.NextMap == null)
                {
                    base.NextMap = Singleton<World>.Instance.GetMap(this.Record.MapId);
                }
                base.NextMap.Area.ExecuteInContext(delegate
                {
                    base.LastMap = base.Map;
                    base.Map = base.NextMap;
                    base.NextMap.Enter(this);
                    base.LastMap = null;
                    base.NextMap = null;
                });
            }
        }
        public bool IsRegenActive()
        {
            return this.RegenStartTime.HasValue;
        }
        public void StartRegen()
        {
            this.StartRegen((byte)(20f / Rates.RegenRate));
        }
        public void StartRegen(byte timePerHp)
        {
            if (this.IsRegenActive())
            {
                this.StopRegen();
            }
            this.RegenStartTime = new System.DateTime?(System.DateTime.Now);
            this.RegenSpeed = timePerHp;
            CharacterHandler.SendLifePointsRegenBeginMessage(this.Client, this.RegenSpeed);
        }
        public void StopRegen()
        {
            if (this.IsRegenActive())
            {
                int num = (int)System.Math.Floor((System.DateTime.Now - this.RegenStartTime).Value.TotalSeconds / (double)((float)this.RegenSpeed / 10f));
                if (this.LifePoints + num > this.MaxLifePoints)
                {
                    num = this.MaxLifePoints - this.LifePoints;
                }
                if (num > 0)
                {
                    this.Stats.Health.DamageTaken -= (int)((short)num);
                }
                CharacterHandler.SendLifePointsRegenEndMessage(this.Client, num);
                this.RegenStartTime = null;
                this.RegenSpeed = 0;
                this.OnLifeRegened(num);
            }
        }
        public void UpdateRegenedLife()
        {
            if (this.IsRegenActive())
            {
                int num = (int)System.Math.Floor((System.DateTime.Now - this.RegenStartTime).Value.TotalSeconds / (double)((float)this.RegenSpeed / 10f));
                if (this.LifePoints + num > this.MaxLifePoints)
                {
                    num = this.MaxLifePoints - this.LifePoints;
                }
                if (num > 0)
                {
                    this.Stats.Health.DamageTaken -= (int)((short)num);
                    CharacterHandler.SendUpdateLifePointsMessage(this.Client);
                }
                this.RegenStartTime = new System.DateTime?(System.DateTime.Now);
                this.OnLifeRegened(num);
            }
        }
        public void DiscoverZaap(Map map)
        {
            if (!this.KnownZaaps.Contains(map))
            {
                this.KnownZaaps.Add(map);
            }
            BasicHandler.SendTextInformationMessage(this.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 24);
        }
        public void SetSpawnPoint(Map map)
        {
            this.Record.SpawnMap = map;
            this.m_spawnPoint = null;
            BasicHandler.SendTextInformationMessage(this.Client, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 6);
        }
        public ObjectPosition GetSpawnPoint()
        {
            ObjectPosition result;
            if (this.Record.SpawnMap == null)
            {
                result = this.Breed.GetStartPosition();
            }
            else
            {
                if (this.m_spawnPoint != null)
                {
                    result = this.m_spawnPoint;
                }
                else
                {
                    Map spawnMap = this.Record.SpawnMap;
                    if (spawnMap.Zaap == null)
                    {
                        result = new ObjectPosition(spawnMap, spawnMap.GetRandomFreeCell(false), base.Direction);
                    }
                    else
                    {
                        Cell randomAdjacentFreeCell = spawnMap.GetRandomAdjacentFreeCell(spawnMap.Zaap.Position.Point, false);
                        DirectionsEnum direction = spawnMap.Zaap.Position.Point.OrientationTo(new MapPoint(randomAdjacentFreeCell), true);
                        result = new ObjectPosition(spawnMap, randomAdjacentFreeCell, direction);
                    }
                }
            }
            return result;
        }
        public void PlayEmote(EmotesEnum emote)
        {
            short auraSkin = this.GetAuraSkin(emote);
            if (auraSkin != -1)
            {
                if (this.RealLook.AuraLook != null && this.RealLook.AuraLook.BonesID == auraSkin)
                {
                    this.RealLook.RemoveAuras();
                }
                else
                {
                    this.RealLook.SetAuraSkin(auraSkin);
                }
                this.RefreshActor();
            }
            ContextRoleplayHandler.SendEmotePlayMessage(base.Map.Clients, this, emote);
        }
        public short GetAuraSkin(EmotesEnum auraEmote)
        {
            short result;
            switch (auraEmote)
            {
                case EmotesEnum.EMOTE_AURA_DE_PUISSANCE:
                    result = 171;
                    break;
                case EmotesEnum.EMOTE_AURA_VAMPYRIQUE:
                    result = 170;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }
        public void ToggleAura(EmotesEnum emote, bool toggle)
        {
            short auraSkin = this.GetAuraSkin(emote);
            if (auraSkin != -1)
            {
                bool flag;
                if (!(flag = (this.RealLook.AuraLook == null || this.RealLook.AuraLook.BonesID != this.GetAuraSkin(emote))) && toggle)
                {
                    this.PlayEmote(emote);
                }
                else
                {
                    if (flag && !toggle)
                    {
                        this.PlayEmote(emote);
                    }
                }
            }
        }
        public bool CanEnableMerchantMode(bool sendError = true)
        {
            bool result;
            if (this.MerchantBag.Count == 0)
            {
                if (sendError)
                {
                    this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 23, new object[0]);
                }
                result = false;
            }
            else
            {
                if (!base.Map.AllowHumanVendor)
                {
                    if (sendError)
                    {
                        this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 237, new object[0]);
                    }
                    result = false;
                }
                else
                {
                    if (base.Map.IsMerchantLimitReached())
                    {
                        if (sendError)
                        {
                            this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 25, new object[]
							{
								Map.MaxMerchantsPerMap
							});
                        }
                        result = false;
                    }
                    else
                    {
                        if (!base.Map.IsCellFree(base.Cell.Id, this))
                        {
                            if (sendError)
                            {
                                this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 24, new object[0]);
                            }
                            result = false;
                        }
                        else
                        {
                            if (this.Kamas <= this.MerchantBag.GetMerchantTax())
                            {
                                if (sendError)
                                {
                                    this.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 76, new object[0]);
                                }
                                result = false;
                            }
                            else
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }
        public bool EnableMerchantMode()
        {
            bool result;
            if (!this.CanEnableMerchantMode(true))
            {
                result = false;
            }
            else
            {
                this.m_merchantToSpawn = new Merchant(this);
                this.Inventory.SubKamas(this.MerchantBag.GetMerchantTax());
                Singleton<MerchantManager>.Instance.AddMerchantSpawn(this.m_merchantToSpawn.Record, true);
                Singleton<MerchantManager>.Instance.ActiveMerchant(this.m_merchantToSpawn);
                this.Client.Disconnect();
                result = true;
            }
            return result;
        }
        private void CheckMerchantModeReconnection()
        {
            foreach (Merchant current in Singleton<MerchantManager>.Instance.UnActiveMerchantFromAccount(this.Client.WorldAccount))
            {
                current.Save();
                if (current.Record.CharacterId == this.Id)
                {
                    if (current.KamasEarned > 0u)
                    {
                        this.Inventory.AddKamas((int)current.KamasEarned);
                        this.m_earnKamasInMerchant = (int)current.KamasEarned;
                    }
                    this.MerchantBag.LoadMerchantBag(current.Bag);
                    Singleton<MerchantManager>.Instance.RemoveMerchantSpawn(current.Record, true);
                }
            }
            WorldMapMerchantRecord merchantSpawn = Singleton<MerchantManager>.Instance.GetMerchantSpawn(this.Id);
            if (merchantSpawn != null)
            {
                this.Inventory.AddKamas((int)merchantSpawn.KamasEarned);
                this.m_earnKamasInMerchant = (int)merchantSpawn.KamasEarned;
                Singleton<MerchantManager>.Instance.RemoveMerchantSpawn(merchantSpawn, true);
            }
        }

        public void LogIn()
        {
            if (!this.IsInWorld)
            {
                if (this.IsInFight())
                {
                    ContextRoleplayHandler.SendCurrentMapMessage(this.Client, this.Fighter.Map.Id);
                }
                else
                {
                    base.Map.Area.AddMessage(delegate
                    {
                        base.Map.Enter(this);
                        this.StartRegen();
                        if (this.Record.CreationDate == this.Record.LastUsage)
                        {
                            Client.Send(new CinematicMessage(10));
                        }
                    });
                }

                Singleton<World>.Instance.Enter(this);
                this.m_inWorld = true;
                this.SendServerMessage(Stump.Server.WorldServer.Settings.MOTD, Stump.Server.WorldServer.Settings.MOTDColor);
                this.IsLoggedIn = true;
                this.OnLoggedIn();
            }
        }
        public void LogOut()
        {
            if (base.Area == null)
            {
                ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(new Action(this.PerformLoggout));
            }
            else
            {
                base.Area.AddMessage(new Action(this.PerformLoggout));
            }
        }
        private void PerformLoggout()
        {
            lock (this.LoggoutSync)
            {
                this.IsLoggedIn = false;
                try
                {
                    this.OnLoggedOut();
                    if (this.IsInWorld)
                    {
                        this.DenyAllInvitations();
                        if (this.IsInRequest())
                        {
                            this.CancelRequest();
                        }
                        if (this.IsDialoging())
                        {
                            this.Dialog.Close();
                        }
                        if (this.IsInParty())
                        {
                            this.LeaveParty();
                        }
                        if (base.Map != null && base.Map.IsActor(this))
                        {
                            base.Map.Leave(this);
                        }
                        if (base.Map != null && this.m_merchantToSpawn != null)
                        {
                            base.Map.Enter(this.m_merchantToSpawn);
                        }
                        Singleton<World>.Instance.Leave(this);
                        this.m_inWorld = false;
                    }
                }
                catch (System.Exception argument)
                {
                    this.logger.Error<System.Exception>("Cannot perfom OnLoggout actions, but trying to Save character : {0}", argument);
                }
                finally
                {
                    ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(delegate
                    {
                        try
                        {
                            this.SaveNow();
                            this.UnLoadRecord();
                        }
                        finally
                        {
                            base.Delete();
                        }
                    });
                }
            }
        }
        public void SaveLater()
        {
            ServerBase<WorldServer>.Instance.IOTaskPool.AddMessage(new Action(this.SaveNow));
        }
        internal void SaveNow()
        {
            ServerBase<WorldServer>.Instance.IOTaskPool.EnsureContext();
            if (this.m_recordLoaded)
            {
                lock (this.SaveSync)
                {
                    using (Transaction transaction = ServerBase<WorldServer>.Instance.DBAccessor.Database.GetTransaction())
                    {
                        this.Inventory.Save();
                        this.MerchantBag.Save();
                        this.Spells.Save();
                        this.Shortcuts.Save();
                        this.FriendsBook.Save();
                        if (this.GuildMember != null && this.GuildMember.IsDirty)
                        {
                            this.GuildMember.Save(ServerBase<WorldServer>.Instance.DBAccessor.Database);
                        }
                        this.m_record.MapId = base.Map.Id;
                        this.m_record.CellId = base.Cell.Id;
                        this.m_record.Direction = base.Direction;
                        this.m_record.AP = this.Stats[PlayerFields.AP].Base;
                        this.m_record.MP = this.Stats[PlayerFields.MP].Base;
                        this.m_record.Strength = this.Stats[PlayerFields.Strength].Base;
                        this.m_record.Agility = this.Stats[PlayerFields.Agility].Base;
                        this.m_record.Chance = this.Stats[PlayerFields.Chance].Base;
                        this.m_record.Intelligence = this.Stats[PlayerFields.Intelligence].Base;
                        this.m_record.Wisdom = this.Stats[PlayerFields.Wisdom].Base;
                        this.m_record.Vitality = this.Stats[PlayerFields.Vitality].Base;
                        this.m_record.BaseHealth = this.Stats.Health.Base;
                        this.m_record.DamageTaken = this.Stats.Health.DamageTaken;
                        ServerBase<WorldServer>.Instance.DBAccessor.Database.Update(this.m_record);
                        transaction.Complete();
                    }
                }
                this.OnSaved();
            }
        }
        private void LoadRecord()
        {
            this.Breed = Singleton<BreedManager>.Instance.GetBreed(this.BreedId);
            this.Head = Singleton<BreedManager>.Instance.GetHead(this.Record.Head);
            Map map = Singleton<World>.Instance.GetMap(this.m_record.MapId);
            if (map == null)
            {
                map = Singleton<World>.Instance.GetMap(this.Breed.StartMap);
                this.m_record.CellId = this.Breed.StartCell;
                this.m_record.Direction = this.Breed.StartDirection;
            }
            this.Position = new ObjectPosition(map, map.Cells[(int)this.m_record.CellId], this.m_record.Direction);
            this.Stats = new StatsFields(this);
            this.Stats.Initialize(this.m_record);
            this.Level = Singleton<ExperienceManager>.Instance.GetCharacterLevel(this.m_record.Experience);
            this.LowerBoundExperience = Singleton<ExperienceManager>.Instance.GetCharacterLevelExperience(this.Level);
            this.UpperBoundExperience = Singleton<ExperienceManager>.Instance.GetCharacterNextLevelExperience(this.Level);
            this.AlignmentGrade = (sbyte)Singleton<ExperienceManager>.Instance.GetAlignementGrade(this.m_record.Honor);
            this.LowerBoundHonor = Singleton<ExperienceManager>.Instance.GetAlignementGradeHonor((byte)this.AlignmentGrade);
            this.UpperBoundHonor = Singleton<ExperienceManager>.Instance.GetAlignementNextGradeHonor((byte)this.AlignmentGrade);
            this.Inventory = new Inventory(this);
            this.Inventory.LoadInventory();
            this.Achievement = new PlayerAchievement(this);
            this.Achievement.LoadAchievements();
            this.UpdateLook(false);
            this.MerchantBag = new CharacterMerchantBag(this);
            this.CheckMerchantModeReconnection();
            this.MerchantBag.LoadMerchantBag();
            this.GuildMember = Singleton<GuildManager>.Instance.TryGetGuildMember(this.Id);
            this.Spells = new SpellInventory(this);
            this.Spells.LoadSpells();
            this.Shortcuts = new ShortcutBar(this);
            this.Shortcuts.Load();
            this.FriendsBook = new FriendsBook(this);
            this.FriendsBook.Load();
            this.ChatHistory = new ChatHistory(this);
            this.m_recordLoaded = true;
        }
        private void UnLoadRecord()
        {
            if (this.m_recordLoaded)
            {
                this.m_recordLoaded = false;
                if (base.Area != null)
                {
                    base.Area.ExecuteInContext(delegate
                    {
                        base.Dispose();
                    });
                }
                else
                {
                    base.Dispose();
                }
            }
        }
        public override GameContextActorInformations GetGameContextActorInformations(Character character)
        {
            return new GameRolePlayCharacterInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(), this.Name, this.GetHumanInformations(), this.Account.Id, this.GetActorAlignmentInformations());
        }
        public ActorAlignmentInformations GetActorAlignmentInformations()
        {
            //return new ActorAlignmentInformations((sbyte)(this.PvPEnabled ? this.AlignmentSide : AlignmentSideEnum.ALIGNMENT_NEUTRAL), Convert.ToSByte(this.PvPEnabled ? this.AlignmentValue : 0), Convert.ToSByte(this.PvPEnabled ? this.AlignmentGrade : 0), (uint)(this.CharacterPower));
            return new ActorAlignmentInformations(0, 0, 0, 0);
        }
        public ActorExtendedAlignmentInformations GetActorAlignmentExtendInformations()
        {
            return new ActorExtendedAlignmentInformations((sbyte)this.AlignmentSide, this.AlignmentValue, this.AlignmentGrade, (uint)this.CharacterPower, this.Honor, this.LowerBoundHonor, this.UpperBoundHonor, (sbyte)(this.PvPEnabled ? 1 : 0));
        }
        public CharacterBaseInformations GetCharacterBaseInformations()
        {
            return new CharacterBaseInformations((uint)this.Id, this.Level, this.Name, this.Look.GetEntityLook(), (sbyte)this.BreedId, this.Sex == SexTypeEnum.SEX_FEMALE);
        }
        public CharacterMinimalPlusLookInformations GetCharacterMinimalPlusLookInformations()
        {
            return new CharacterMinimalPlusLookInformations((uint)this.Id, this.Level, this.Name, this.Look.GetEntityLook());
        }
        public PartyInvitationMemberInformations GetPartyInvitationMemberInformations()
        {
            return new PartyInvitationMemberInformations((uint)this.Id, this.Level, this.Name, this.Look.GetEntityLook(), (sbyte)this.BreedId, this.Sex == SexTypeEnum.SEX_FEMALE, (short)base.Map.Position.X, (short)base.Map.Position.Y, base.Map.Id, (ushort)base.Map.SubArea.Id, Enumerable.Empty<PartyCompanionBaseInformations>());
        }
        public PartyMemberInformations GetPartyMemberInformations()
        {
            return new PartyMemberInformations((uint)this.Id, this.Level, this.Name, this.Look.GetEntityLook(), (sbyte)this.BreedId, this.Sex == SexTypeEnum.SEX_FEMALE, (uint)this.LifePoints, (uint)this.MaxLifePoints, (ushort)this.Stats[PlayerFields.Prospecting].Total, this.RegenSpeed, (ushort)this.Stats[PlayerFields.Initiative].Total, (sbyte)this.AlignmentSide, (short)base.Map.Position.X, (short)base.Map.Position.Y, base.Map.Id, (ushort)base.SubArea.Id, new PlayerStatus(), Enumerable.Empty<PartyCompanionMemberInformations>());
        }
        public PartyGuestInformations GetPartyGuestInformations(Party party)
        {
            PartyGuestInformations result;
            if (!this.m_partyInvitations.ContainsKey(party.Id))
            {
                result = new PartyGuestInformations();
            }
            else
            {
                PartyInvitation partyInvitation = this.m_partyInvitations[party.Id];
                result = new PartyGuestInformations(this.Id, partyInvitation.Source.Id, this.Name, this.Look.GetEntityLook(), (sbyte)this.BreedId, this.Sex == SexTypeEnum.SEX_FEMALE, new PlayerStatus(), Enumerable.Empty<PartyCompanionBaseInformations>());
            }
            return result;
        }
        public override HumanInformations GetHumanInformations()
        {
            HumanInformations humanInformations = base.GetHumanInformations();
            System.Collections.Generic.List<HumanOption> list = new System.Collections.Generic.List<HumanOption>();
            if (this.Guild != null)
            {
                list.Add(new HumanOptionGuild(this.Guild.GetGuildInformations()));
                if (this.Guild.Alliance != null)
                {
                    list.Add(new HumanOptionAlliance(this.Guild.Alliance.GetAllianceInformations(), (sbyte)AggressableStatusEnum.NON_AGGRESSABLE));
                }
            }
            ushort? num = this.SelectedTitle;
            if ((num.HasValue ? new int?((int)num.GetValueOrDefault()) : null).HasValue)
            {
                list.Add(new HumanOptionTitle((ushort)this.SelectedTitle.Value, string.Empty));
            }
            num = this.SelectedOrnament;
            if ((num.HasValue ? new int?((int)num.GetValueOrDefault()) : null).HasValue)
            {
                list.Add(new HumanOptionOrnament((ushort)this.SelectedOrnament.Value));
            }
            humanInformations.options = list;
            return humanInformations;
        }
        public override bool CanBeSee(WorldObject byObj)
        {
            return base.CanBeSee(byObj) && (byObj == this || !this.Invisible);
        }
        protected override void OnDisposed()
        {
            if (this.FriendsBook != null)
            {
                this.FriendsBook.Dispose();
            }
            if (this.Inventory != null)
            {
                this.Inventory.Dispose();
            }
            base.OnDisposed();
        }
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, this.Id);
        }

        internal void ChangeClient(WorldClient client)
        {
            this.Client = client;
        }
    }
}
