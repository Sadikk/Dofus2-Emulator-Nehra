using NLog;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Alliances;
using Stump.Server.WorldServer.Game.Guilds;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Stump.Core.Extensions;
using Stump.Server.WorldServer.Handlers.Alliances;
using System;
using Stump.Core.Reflection;
using Stump.Server.WorldServer.Game.Prisms;

namespace Stump.Server.WorldServer.Game.Alliances
{
    public class Alliance
    {
        // FIELDS
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private bool m_isDirty;
        private readonly Dictionary<int, Guild> m_guilds = new Dictionary<int, Guild>();
        private readonly object m_lock = new object();

        // PROPERTIES
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
        public string Name
        {
            get
            {
                return this.Record.Name;
            }
            private set
            {
                this.Record.Name = value;
                this.IsDirty = true;
            }
        }
        public string Tag
        {
            get
            {
                return this.Record.Tag;
            }
            private set
            {
                this.Record.Tag = value;
                this.IsDirty = true;
            }
        }
        public AllianceRecord Record
        {
            get;
            private set;
        }
        public Guild Boss
        {
            get;
            private set;
        }
        public AllianceEmblem Emblem { get; protected set; }
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
        public ushort Members
        {
            get
            {
                return (ushort)this.m_guilds.Sum(entry => entry.Value.Members.Count);
            }
        }

        // CONSTRUCTORS
        public Alliance(int id, string name, string tag)
        {
            this.Record = new AllianceRecord();
            this.Id = id;
            this.Name = name;
            this.Tag = tag;
            this.Record.CreationDate = DateTime.Now;
            this.Emblem = new AllianceEmblem(this.Record)
            {
                BackgroundColor = Color.White,
                BackgroundShape = 1,
                SymbolColor = Color.Black,
                SymbolShape = 1
            };

            this.Record.IsNew = true;
            this.IsDirty = true;
        }
        public Alliance(AllianceRecord record)
        {
            this.Record = record;
            this.Emblem = new AllianceEmblem(this.Record);

            foreach (var item in Singleton<GuildManager>.Instance.GetGuildsByAlliance(this.Record))
            {
                item.SetAlliance(this);

                this.m_guilds.Add(item.Id, item);

                if (this.Record.Owner == item.Id)
                {
                    this.SetBoss(item);
                }
            }

            if (this.Boss == null)
            {
                if (this.m_guilds.Count == 0)
                {

                }
                else
                {
                    this.SetBoss(this.m_guilds.First().Value);
                }
            }
        }

        // METHODS
        public void AddPrism(PrismNpc prism)
        {

        }

        public bool TryAddGuild(Guild guild)
        {
            bool result;
            lock (this.m_lock)
            {
                //if (false) // TODO : je ne sais plus
                //{

                //    result = false;
                //}
                //else
                //{
                    this.m_guilds.Add(guild.Id, guild);
                    guild.SetAlliance(this);
                    if (this.m_guilds.Count == 1)
                    {
                        this.SetBoss(guild);
                    }
                    this.OnGuildAdded(guild);

                    result = true;
                //}
            }
            return result;
        }

        public Guild GetGuildById(uint id)
        {
            Guild guild;

            return this.m_guilds.TryGetValue((int)id, out guild) ? guild : null;
        }
        public AllianceInformations GetAllianceInformations()
        {
            return new AllianceInformations((uint)this.Id, this.Tag, this.Name, this.Emblem.GetNetworkGuildEmblem());
        }
        public AllianceFactSheetInformations GetAllianceFactSheetInformations()
        {
            return new AllianceFactSheetInformations((uint)this.Id, this.Tag, this.Name, this.Emblem.GetNetworkGuildEmblem(), this.Record.CreationDate.GetUnixTimeStamp());
        }

        public AllianceVersatileInformations GetAllianceVersatileInformations()
        {
            return new AllianceVersatileInformations((uint)this.Id, (ushort)this.m_guilds.Count, this.Members, 0);
        }

        public IEnumerable<GuildInsiderFactSheetInformations> GetGuildsInformations()
        {
            return (from guild in this.m_guilds
                    select new GuildInsiderFactSheetInformations(
                        (uint)guild.Value.Id,
                        guild.Value.Name,
                        guild.Value.Emblem.GetNetworkGuildEmblem(),
                        (uint)guild.Value.Boss.Id,
                        guild.Value.Level,
                        (ushort)guild.Value.Members.Count,
                        guild.Value.Boss.Name,
                        (ushort)guild.Value.Members.Count(entry => entry.IsConnected),
                        (sbyte)guild.Value.TaxCollectors.Count,
                        guild.Value.CreationDate.GetUnixTimeStamp(),
                        true));
        }
        public IEnumerable<PrismSubareaEmptyInfo> GetPrismsInformations()
        {
            return new PrismSubareaEmptyInfo[0];
        }

        public IEnumerable<GuildInAllianceInformations> GetGuildsInAllianceInformations()
        {
            return (from guild in this.m_guilds
                   select new GuildInAllianceInformations(
                       (uint)guild.Value.Id,
                       guild.Value.Name,
                       guild.Value.Emblem.GetNetworkGuildEmblem(),
                       guild.Value.Level,
                       (byte)guild.Value.Members.Count,
                       true));
        }

        protected virtual void OnGuildAdded(Guild guild)
        {
            foreach (var member in guild.Members)
            {
                if (member.IsConnected)
                {
                    AllianceHandler.SendAllianceJoinedMessage(member.Character.Client, this);
                }
            }
        }

        public void SetBoss(Guild guild)
        {
            this.Boss = guild;

            if (this.Record.Owner != this.Boss.Id)
            {
                this.Record.Owner = this.Boss.Id;
                this.IsDirty = true;
            }
        }
        
        public void Save(Stump.ORM.Database database)
        {
            if (this.Record.IsNew)
            {
                database.Insert(this.Record);
            }
            else
            {
                database.Update(this.Record);
            }
            this.IsDirty = false;
            this.Record.IsNew = false;
        }
    }
}
