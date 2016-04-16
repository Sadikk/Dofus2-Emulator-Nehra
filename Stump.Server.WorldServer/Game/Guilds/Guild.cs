using NLog;
using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Guilds;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using Stump.Server.WorldServer.Game.Alliances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Guilds;
using Stump.Server.WorldServer.Handlers.TaxCollector;
using System;
using System.Drawing;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Guilds
{
	public class Guild
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private static readonly double[][] XP_PER_GAP = new double[][]
		{
			new double[]
			{
				0.0,
				10.0
			},
			new double[]
			{
				10.0,
				8.0
			},
			new double[]
			{
				20.0,
				6.0
			},
			new double[]
			{
				30.0,
				4.0
			},
			new double[]
			{
				40.0,
				3.0
			},
			new double[]
			{
				50.0,
				2.0
			},
			new double[]
			{
				60.0,
				1.5
			},
			new double[]
			{
				70.0,
				1.0
			}
		};
		public static readonly ushort[] TAX_COLLECTOR_SPELLS = new ushort[]
		{
			458,
			457,
			456,
			455,
			462,
			460,
			459,
			451,
			453,
			454,
			452,
			461
		};
		[Variable(true)]
		public static int MaxMembersNumber = 50;
		[Variable(true)]
		public static int MaxGuildXP = 300000;
		private readonly System.Collections.Generic.List<GuildMember> m_members = new System.Collections.Generic.List<GuildMember>();
		private readonly WorldClientCollection m_clients = new WorldClientCollection();
		private readonly System.Collections.Generic.List<TaxCollectorNpc> m_taxCollectors = new System.Collections.Generic.List<TaxCollectorNpc>();
		private readonly Spell[] m_spells = new Spell[Guild.TAX_COLLECTOR_SPELLS.Length];
		private bool m_isDirty;
        private Alliance m_alliance;
		private readonly object m_lock = new object();

		public System.Collections.ObjectModel.ReadOnlyCollection<GuildMember> Members
		{
			get
			{
				return this.m_members.AsReadOnly();
			}
		}
		public WorldClientCollection Clients
		{
			get
			{
				return this.m_clients;
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<TaxCollectorNpc> TaxCollectors
		{
			get
			{
				return this.m_taxCollectors.AsReadOnly();
			}
		}
		public GuildRecord Record
		{
			get;
			set;
		}
		public int Id
		{
			get
			{
				return this.Record.Id;
			}
			private set
			{
				this.Record.Id = value;
			}
		}
		public GuildMember Boss
		{
			get;
			private set;
		}
		public long Experience
		{
			get
			{
				return this.Record.Experience;
			}
			protected set
			{
				this.Record.Experience = value;
				this.IsDirty = true;
			}
		}
		public uint Boost
		{
			get
			{
				return this.Record.Boost;
			}
			protected set
			{
				this.Record.Boost = value;
				this.IsDirty = true;
			}
		}
		public int TaxCollectorProspecting
		{
			get
			{
				return this.Record.Prospecting;
			}
			protected set
			{
				this.Record.Prospecting = value;
				this.IsDirty = true;
			}
		}
		public int TaxCollectorWisdom
		{
			get
			{
				return this.Record.Wisdom;
			}
			protected set
			{
				this.Record.Wisdom = value;
				this.IsDirty = true;
			}
		}
		public int TaxCollectorPods
		{
			get
			{
				return this.Record.Pods;
			}
			protected set
			{
				this.Record.Pods = value;
				this.IsDirty = true;
			}
		}
		public int TaxCollectorHealth
		{
			get
			{
				return TaxCollectorNpc.BaseHealth + (int)(20 * this.Level);
			}
		}
		public int TaxCollectorDamageBonuses
		{
			get
			{
				return (int)this.Level;
			}
		}
		public int MaxTaxCollectors
		{
			get
			{
				return this.Record.MaxTaxCollectors;
			}
			protected set
			{
				this.Record.MaxTaxCollectors = value;
				this.IsDirty = true;
			}
		}
		public long ExperienceLevelFloor
		{
			get;
			protected set;
		}
		public long ExperienceNextLevelFloor
		{
			get;
			protected set;
		}
		public System.DateTime CreationDate
		{
			get
			{
				return this.Record.CreationDate;
			}
		}
		public string Name
		{
			get
			{
				return this.Record.Name;
			}
			protected set
			{
				this.Record.Name = value;
				this.IsDirty = true;
			}
		}
		public GuildEmblem Emblem
		{
			get;
			protected set;
		}
		public byte Level
		{
			get;
			protected set;
		}
		public bool IsDirty
		{
			get
			{
				return this.m_isDirty || this.Emblem.IsDirty;
			}
			set
			{
				this.m_isDirty = value;
				if (!value)
				{
					this.Emblem.IsDirty = false;
				}
			}
		}
        public Alliance Alliance
        {
            get
            {
                return this.m_alliance;
            }
            private set
            {
                this.m_alliance = value;
                this.Record.AllianceId = (this.m_alliance == null ? new int?() : this.m_alliance.Id);
                this.IsDirty = true;
            }
        }

		public Guild(int id, string name)
		{
			this.Record = new GuildRecord();
			this.Id = id;
            this.Alliance = null;
			this.Name = name;
			this.Level = 1;
			this.Boost = 0u;
			this.TaxCollectorProspecting = 100;
			this.TaxCollectorWisdom = 0;
			this.TaxCollectorPods = 1000;
			this.MaxTaxCollectors = 1;
			this.ExperienceLevelFloor = 0L;
			this.ExperienceNextLevelFloor = Singleton<ExperienceManager>.Instance.GetGuildNextLevelExperience(this.Level);
			this.Record.CreationDate = System.DateTime.Now;
			this.Record.IsNew = true;
			this.Emblem = new GuildEmblem(this.Record)
			{
				BackgroundColor = Color.White,
				BackgroundShape = 1,
				SymbolColor = Color.Black,
				SymbolShape = 1
			};
			this.IsDirty = true;
		}
		public Guild(GuildRecord record, System.Collections.Generic.IEnumerable<GuildMember> members)
		{
			this.Record = record;
			this.m_members.AddRange(members);
			this.Level = Singleton<ExperienceManager>.Instance.GetGuildLevel(this.Experience);
			this.ExperienceLevelFloor = Singleton<ExperienceManager>.Instance.GetGuildLevelExperience(this.Level);
			this.ExperienceNextLevelFloor = Singleton<ExperienceManager>.Instance.GetGuildNextLevelExperience(this.Level);
			this.Emblem = new GuildEmblem(this.Record);
			foreach (GuildMember guildMember in this.m_members)
			{
				if (guildMember.IsBoss)
				{
					if (this.Boss != null)
					{
						Guild.logger.Error<int, string>("There is at least two boss in guild {0} ({1})", this.Id, this.Name);
					}
					this.Boss = guildMember;
				}
				this.BindMemberEvents(guildMember);
				guildMember.BindGuild(this);
			}
			if (this.m_members.Count == 0)
			{
				Guild.logger.Error<int, string>("Guild {0} ({1}) is empty", this.Id, this.Name);
			}
			else
			{
				if (this.Boss == null)
				{
					GuildMember guildMember = this.m_members.First<GuildMember>();
					this.SetBoss(guildMember);
					Guild.logger.Error<int, string>("There is at no boss in guild {0} ({1}) -> Promote new Boss", this.Id, this.Name);
				}
			}
			int num = 0;
			while (num < record.Spells.Length && num < Guild.TAX_COLLECTOR_SPELLS.Length)
			{
				if (record.Spells[num] != 0)
				{
					this.m_spells[num] = new Spell((int)Guild.TAX_COLLECTOR_SPELLS[num], (byte)record.Spells[num]);
				}
				num++;
			}
		}

		public void AddTaxCollector(TaxCollectorNpc taxCollector)
		{
			this.m_taxCollectors.Add(taxCollector);
			TaxCollectorHandler.SendTaxCollectorMovementAddMessage(taxCollector.Guild.Clients, taxCollector);
		}
		public void RemoveTaxCollector(TaxCollectorNpc taxCollector)
		{
			this.m_taxCollectors.Remove(taxCollector);
			Singleton<TaxCollectorManager>.Instance.RemoveTaxCollectorSpawn(taxCollector, true);
			TaxCollectorHandler.SendTaxCollectorMovementRemoveMessage(taxCollector.Guild.Clients, taxCollector);
		}
		public void RemoveTaxCollectors()
		{
			TaxCollectorNpc[] array = this.m_taxCollectors.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				TaxCollectorNpc taxCollectorNpc = array[i];
				this.m_taxCollectors.Remove(taxCollectorNpc);
				Singleton<TaxCollectorManager>.Instance.RemoveTaxCollectorSpawn(taxCollectorNpc, true);
			}
		}
		public long AdjustGivenExperience(Character giver, long amount)
		{
			int num = (int)(giver.Level - this.Level);
			long result;
			for (int i = Guild.XP_PER_GAP.Length - 1; i >= 0; i--)
			{
				if ((double)num > Guild.XP_PER_GAP[i][0])
				{
					result = (long)((double)amount * Guild.XP_PER_GAP[i][1] * 0.01);
					return result;
				}
			}
			result = (long)((double)amount * Guild.XP_PER_GAP[0][1] * 0.01);
			return result;
		}
		public void AddXP(long experience)
		{
			lock (this.m_lock)
			{
				this.Experience += experience;
				byte guildLevel = Singleton<ExperienceManager>.Instance.GetGuildLevel(this.Experience);
				if (guildLevel != this.Level)
				{
					if (guildLevel > this.Level)
					{
						this.Boost += (uint)((guildLevel - this.Level) * 5);
					}
					this.Level = guildLevel;
					this.OnLevelChanged();
				}
			}
		}
		public void SetXP(long experience)
		{
			lock (this.m_lock)
			{
				this.Experience = experience;
				byte guildLevel = Singleton<ExperienceManager>.Instance.GetGuildLevel(this.Experience);
				if (guildLevel != this.Level)
				{
					this.Level = guildLevel;
					this.OnLevelChanged();
				}
			}
		}
		public bool UpgradeTaxCollectorPods()
		{
			bool result;
			lock (this.m_lock)
			{
				if (this.TaxCollectorPods >= 5000)
				{
					result = false;
				}
				else
				{
					if (this.Boost <= 0u)
					{
						result = false;
					}
					else
					{
						this.Boost -= 1u;
						this.TaxCollectorPods += 20;
						if (this.TaxCollectorPods > 5000)
						{
							this.TaxCollectorPods = 5000;
						}
						result = true;
					}
				}
			}
			return result;
		}
		public bool UpgradeTaxCollectorProspecting()
		{
			bool result;
			lock (this.m_lock)
			{
				if (this.TaxCollectorProspecting >= 500)
				{
					result = false;
				}
				else
				{
					if (this.Boost <= 0u)
					{
						result = false;
					}
					else
					{
						this.Boost -= 1u;
						this.TaxCollectorProspecting++;
						if (this.TaxCollectorProspecting > 500)
						{
							this.TaxCollectorProspecting = 500;
						}
						result = true;
					}
				}
			}
			return result;
		}
		public bool UpgradeTaxCollectorWisdom()
		{
			bool result;
			lock (this.m_lock)
			{
				if (this.TaxCollectorWisdom >= 400)
				{
					result = false;
				}
				else
				{
					if (this.Boost <= 0u)
					{
						result = false;
					}
					else
					{
						this.Boost -= 1u;
						this.TaxCollectorWisdom++;
						if (this.TaxCollectorWisdom > 400)
						{
							this.TaxCollectorWisdom = 400;
						}
						result = true;
					}
				}
			}
			return result;
		}
		public bool UpgradeMaxTaxCollectors()
		{
			bool result;
			lock (this.m_lock)
			{
				if (this.MaxTaxCollectors >= 50)
				{
					result = false;
				}
				else
				{
					if (this.Boost < 10u)
					{
						result = false;
					}
					else
					{
						this.Boost -= 10u;
						this.MaxTaxCollectors++;
						if (this.MaxTaxCollectors > 50)
						{
							this.MaxTaxCollectors = 50;
						}
						result = true;
					}
				}
			}
			return result;
		}
		public bool UpgradeSpell(int spellId)
		{
			bool result;
			lock (this.m_lock)
			{
				int num = System.Array.IndexOf<ushort>(Guild.TAX_COLLECTOR_SPELLS, (ushort)spellId);
				if (num == -1)
				{
					result = false;
				}
				else
				{
					if (this.Boost < 5u)
					{
						result = false;
					}
					else
					{
						Spell spell = this.m_spells[num];
						if (spell == null)
						{
							SpellTemplate spellTemplate = Singleton<SpellManager>.Instance.GetSpellTemplate(spellId);
							if (spellTemplate == null)
							{
								Guild.logger.Error("Cannot boost tax collector spell {0}, template not found", spellId);
								result = false;
								return result;
							}
							this.m_spells[num] = new Spell(spellTemplate, 1);
						}
						else
						{
							if (!spell.BoostSpell())
							{
								result = false;
								return result;
							}
						}
						this.Boost -= 5u;
						result = true;
					}
				}
			}
			return result;
		}
		public bool UnBoostSpell(int spellId)
		{
			bool result;
			lock (this.m_lock)
			{
				int num = System.Array.IndexOf<ushort>(Guild.TAX_COLLECTOR_SPELLS, (ushort)spellId);
				if (num == -1)
				{
					result = false;
				}
				else
				{
					Spell spell = this.m_spells[num];
					if (spell == null)
					{
						result = false;
					}
					else
					{
						if (!spell.UnBoostSpell())
						{
							result = false;
						}
						else
						{
							this.Boost += 5u;
							result = true;
						}
					}
				}
			}
			return result;
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<Spell> GetTaxCollectorSpells()
		{
			return (
				from x in this.m_spells
				where x != null
				select x).ToList<Spell>().AsReadOnly();
		}
		public int[] GetTaxCollectorSpellsLevels()
		{
			return (
				from x in this.m_spells
				select (int)((x == null) ? 0 : x.CurrentLevel)).ToArray<int>();
		}
		public void SetBoss(GuildMember guildMember)
		{
			lock (this.m_lock)
			{
				if (guildMember.Guild == this)
				{
					if (this.Boss != guildMember)
					{
						if (this.Boss != null)
						{
							this.Boss.RankId = 0;
							this.Boss.Rights = GuildRightsBitEnum.GUILD_RIGHT_NONE;
							if (this.m_members.Count > 1)
							{
								BasicHandler.SendTextInformationMessage(this.m_clients, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 199, new string[]
								{
									guildMember.Name,
									this.Boss.Name,
									this.Name
								});
							}
							this.UpdateMember(this.Boss);
						}
						this.Boss = guildMember;
						this.Boss.RankId = 1;
						this.Boss.Rights = GuildRightsBitEnum.GUILD_RIGHT_BOSS;
						this.UpdateMember(this.Boss);
					}
				}
			}
		}
        public void SetAlliance(Alliance alliance)
        {
            lock (this.m_lock)
            {
                this.m_alliance = alliance;
            }
        }
		public bool KickMember(GuildMember kickedMember, bool kicked)
		{
			bool result;
			lock (this.m_lock)
			{
				if (kickedMember.IsBoss && this.m_members.Count > 1)
				{
					result = false;
				}
				else
				{
					if (!this.RemoveMember(kickedMember))
					{
						result = false;
					}
					else
					{
						foreach (WorldClient current in this.m_clients)
						{
							GuildHandler.SendGuildMemberLeavingMessage(current, kickedMember, true);
						}
						if (kickedMember.IsBoss && this.m_members.Count == 0)
						{
							Singleton<GuildManager>.Instance.DeleteGuild(kickedMember.Guild);
						}
						result = true;
					}
				}
			}
			return result;
		}
		public bool KickMember(Character kicker, GuildMember kickedMember)
		{
			bool result;
			lock (this.m_lock)
			{
				if (kicker.Guild != kickedMember.Guild)
				{
					result = false;
				}
				else
				{
					if (kicker.GuildMember != kickedMember && (!kicker.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_BAN_MEMBERS) || kickedMember.IsBoss))
					{
						result = false;
					}
					else
					{
						if (kicker.GuildMember.Id != kickedMember.Id)
						{
							kicker.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 177, new object[]
							{
								kickedMember.Name
							});
						}
						result = this.KickMember(kickedMember, kickedMember.Id == kicker.GuildMember.Id);
					}
				}
			}
			return result;
		}
		public bool ChangeParameters(GuildMember member, short rank, byte xpPercent, uint rights)
		{
			bool result;
			lock (this.m_lock)
			{
				if (rank == 1)
				{
					this.SetBoss(member);
				}
				else
				{
					member.RankId = rank;
					member.Rights = (GuildRightsBitEnum)rights;
				}
				member.GivenPercent = xpPercent;
				this.UpdateMember(member);
				if (member.IsConnected)
				{
					GuildHandler.SendGuildMembershipMessage(member.Character.Client, member);
				}
				result = true;
			}
			return result;
		}
		public bool ChangeParameters(Character modifier, GuildMember member, short rank, byte xpPercent, uint rights)
		{
			bool result;
			lock (this.m_lock)
			{
				if (modifier.Guild != member.Guild)
				{
					result = false;
				}
				else
				{
					if (modifier.GuildMember != member && modifier.GuildMember.IsBoss && rank == 1)
					{
						this.SetBoss(member);
					}
					else
					{
						if (modifier.GuildMember == member || !member.IsBoss)
						{
							if (modifier.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_MANAGE_RANKS) && rank >= 0 && rank <= 35)
							{
								member.RankId = rank;
							}
							if (modifier.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_MANAGE_RIGHTS))
							{
								member.Rights = (GuildRightsBitEnum)rights;
							}
						}
					}
					if (modifier.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_MANAGE_XP_CONTRIBUTION) || (modifier.GuildMember == member && modifier.GuildMember.HasRight(GuildRightsBitEnum.GUILD_RIGHT_MANAGE_MY_XP_CONTRIBUTION)))
					{
                        member.GivenPercent = Convert.ToByte((xpPercent < 90) ? xpPercent : 90);
					}
					this.UpdateMember(member);
					if (member.IsConnected)
					{
						GuildHandler.SendGuildMembershipMessage(member.Character.Client, member);
					}
					result = true;
				}
			}
			return result;
		}
        public void Save(Stump.ORM.Database database)
		{
			this.Record.Spells = this.GetTaxCollectorSpellsLevels();
			if (this.Record.IsNew)
			{
				database.Insert(this.Record);
			}
			else
			{
				database.Update(this.Record);
			}
			foreach (GuildMember current in 
				from x in this.Members
				where !x.IsConnected && x.IsDirty
				select x)
			{
				current.Save(database);
			}
			this.IsDirty = false;
			this.Record.IsNew = false;
		}
		protected void UpdateMember(GuildMember member)
		{
			GuildHandler.SendGuildInformationsMemberUpdateMessage(this.m_clients, member);
		}
		public bool CanAddMember()
		{
			return this.m_members.Count < Guild.MaxMembersNumber;
		}
		public GuildMember TryGetMember(int id)
		{
			return this.m_members.FirstOrDefault((GuildMember x) => x.Id == id);
		}
		public bool TryAddMember(Character character)
		{
			GuildMember guildMember;
			return this.TryAddMember(character, out guildMember);
		}
		public bool TryAddMember(Character character, out GuildMember member)
		{
			bool result;
			lock (this.m_lock)
			{
				if (!this.CanAddMember())
				{
					member = null;
					result = false;
				}
				else
				{
					member = new GuildMember(this, character);
					this.m_members.Add(member);
					character.GuildMember = member;
					this.m_clients.Add(character.Client);
					if (this.m_members.Count == 1)
					{
						this.SetBoss(member);
					}
					this.OnMemberAdded(member);
					result = true;
				}
			}
			return result;
		}
		public bool RemoveMember(GuildMember member)
		{
			bool result;
			lock (this.m_lock)
			{
				if (member == null || !this.m_members.Contains(member))
				{
					result = false;
				}
				else
				{
					this.m_members.Remove(member);
					if (member.IsConnected)
					{
						this.m_clients.Remove(member.Character.Client);
					}
					this.OnMemberRemoved(member);
					result = true;
				}
			}
			return result;
		}
		protected virtual void OnMemberAdded(GuildMember member)
		{
			this.BindMemberEvents(member);
			Singleton<GuildManager>.Instance.RegisterGuildMember(member);
			if (member.IsConnected)
			{
				GuildHandler.SendGuildJoinedMessage(member.Character.Client, member);
				GuildHandler.SendGuildInformationsMembersMessage(member.Character.Client, this);
				GuildHandler.SendGuildInformationsGeneralMessage(member.Character.Client, this);
				member.Character.RefreshActor();
			}
			this.UpdateMember(member);
		}
		protected virtual void OnMemberRemoved(GuildMember member)
		{
			Singleton<GuildManager>.Instance.DeleteGuildMember(member);
			this.UnBindMemberEvents(member);
			if (member.IsConnected)
			{
				Character character = member.Character;
				character.GuildMember = null;
				character.RefreshActor();
				character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 176, new object[0]);
				GuildHandler.SendGuildLeftMessage(character.Client);
			}
		}
		protected virtual void OnLevelChanged()
		{
			this.ExperienceLevelFloor = Singleton<ExperienceManager>.Instance.GetGuildLevelExperience(this.Level);
			this.ExperienceNextLevelFloor = Singleton<ExperienceManager>.Instance.GetGuildNextLevelExperience(this.Level);
			BasicHandler.SendTextInformationMessage(this.m_clients, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 208, new object[]
			{
				this.Level
			});
			this.m_clients.Send(new GuildLevelUpMessage(this.Level));
		}
		private void OnMemberConnected(GuildMember member)
		{
			BasicHandler.SendTextInformationMessage(this.m_clients, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 224, new string[]
			{
				member.Character.Name
			});
			this.m_clients.Add(member.Character.Client);
			this.UpdateMember(member);
            this.m_clients.Send(new GuildMemberOnlineStatusMessage((uint)member.Id, true));
		}
		private void OnMemberDisconnected(GuildMember member, Character character)
		{
			this.m_clients.Remove(character.Client);
			this.UpdateMember(member);
            this.m_clients.Send(new GuildMemberOnlineStatusMessage((uint)member.Id, false));
			this.m_clients.Send(new GuildMemberLeavingMessage(false, member.Id));
		}
		private void BindMemberEvents(GuildMember member)
		{
			member.Connected += new System.Action<GuildMember>(this.OnMemberConnected);
			member.Disconnected += new Action<GuildMember, Character>(this.OnMemberDisconnected);
		}
		private void UnBindMemberEvents(GuildMember member)
		{
			member.Connected -= new System.Action<GuildMember>(this.OnMemberConnected);
			member.Disconnected -= new Action<GuildMember, Character>(this.OnMemberDisconnected);
		}
		public GuildInformations GetGuildInformations()
		{
			return new GuildInformations((uint)this.Id, this.Name, this.Emblem.GetNetworkGuildEmblem());
		}
		public BasicGuildInformations GetBasicGuildInformations()
		{
            return new BasicGuildInformations((uint)this.Id, this.Name);
		}

        public GuildVersatileInformations GetGuildVersatileInformations()
        {
            if (this.Alliance != null)
            {
                return new GuildInAllianceVersatileInformations((uint)this.Id, (uint)this.Boss.Id, this.Level, (byte)this.Members.Count, (uint)this.Alliance.Id);
            }
            else
            {
                return new GuildVersatileInformations((uint)this.Id, (uint)this.Boss.Id, this.Level, (byte)this.Members.Count);
            }
        }
    }
}
