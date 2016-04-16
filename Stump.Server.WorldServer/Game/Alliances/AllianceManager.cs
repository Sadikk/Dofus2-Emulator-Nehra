using Stump.Core.Pool;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database;
using Stump.Server.WorldServer.Database.Alliances;
using Stump.Server.WorldServer.Database.Symbols;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Alliances
{
    public class AllianceManager : DataManager<AllianceManager>, ISaveable
    {
        // FIELDS
        public const int ALLIOGEMME_ID = 14290;

        private UniqueIdProvider m_idProvider;
        private Dictionary<int, Alliance> m_alliances;
        private Dictionary<int, EmblemRecord> m_emblems;
        private readonly Stack<Alliance> m_alliancesToDelete = new Stack<Alliance>();
        private readonly object m_lock = new object();

        // PROPERTIES

        // CONSTRUCTORS

        // METHODS
        [Initialization(InitializationPass.Seventh)]
        public override void Initialize()
        {
            this.m_emblems = base.Database.Query<EmblemRecord>(EmblemRelator.FetchQuery, new object[0]).ToDictionary((EmblemRecord x) => x.Id);
            this.m_alliances = (from temp in base.Database.Query<AllianceRecord>(AllianceRelator.FetchQuery, new object[0]).ToList<AllianceRecord>()
                                select new Alliance(temp)).ToDictionary(entry => entry.Id);

            if (!this.m_alliances.Any())
            {
                this.m_idProvider = new UniqueIdProvider(1);
            }
            else
            {
                this.m_idProvider = new UniqueIdProvider((
                    from temp in this.m_alliances
                    select temp.Value.Id).Max());
            }

            Singleton<World>.Instance.RegisterSaveableInstance(this);
        }

        public SocialGroupCreationResultEnum CreateAlliance(Character character, string allianceName, string allianceTag, Stump.DofusProtocol.Types.GuildEmblem emblem)
        {
            SocialGroupCreationResultEnum result;
            if (this.DoesNameExist(allianceName))
            {
                result = SocialGroupCreationResultEnum.SOCIAL_GROUP_CREATE_ERROR_NAME_ALREADY_EXISTS;
            }
            else
            {
                if (this.DoesTagExist(allianceTag))
                {
                    result = SocialGroupCreationResultEnum.SOCIAL_GROUP_CREATE_ERROR_TAG_ALREADY_EXISTS;
                }
                else
                {
                    if (this.DoesEmblemExist(emblem))
                    {
                        result = SocialGroupCreationResultEnum.SOCIAL_GROUP_CREATE_ERROR_EMBLEM_ALREADY_EXISTS;
                    }
                    else
                    {
                        var basePlayerItem = character.Inventory.TryGetItem(Singleton<ItemManager>.Instance.TryGetTemplate(AllianceManager.ALLIOGEMME_ID));
                        if (basePlayerItem == null)
                        {
                            result = SocialGroupCreationResultEnum.SOCIAL_GROUP_CREATE_ERROR_REQUIREMENT_UNMET;
                        }
                        else
                        {
                            character.Inventory.RemoveItem(basePlayerItem, 1, true);
                            character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 22, new object[] { 1, basePlayerItem.Template.Id });

                            var alliance = this.CreateAlliance(allianceName, allianceTag);
                            if (alliance == null)
                            {
                                result = SocialGroupCreationResultEnum.SOCIAL_GROUP_CREATE_ERROR_CANCEL;
                            }
                            else
                            {
                                alliance.Emblem.ChangeEmblem(emblem);
                                if (!alliance.TryAddGuild(character.Guild))
                                {
                                    this.DeleteAlliance(alliance);
                                    result = SocialGroupCreationResultEnum.SOCIAL_GROUP_CREATE_ERROR_CANCEL;
                                }
                                else
                                {
                                    character.RefreshActor();
                                    result = SocialGroupCreationResultEnum.SOCIAL_GROUP_CREATE_OK;
                                }
                            }
                        }
                    }

                }
            }

            return result;
        }

        public Alliance CreateAlliance(string name, string tag)
        {
            Alliance result;

            lock (this.m_lock)
            {
                var alliance = new Alliance(this.m_idProvider.Pop(), name, tag);
                this.m_alliances.Add(alliance.Id, alliance);

                result = alliance;
            }

            return result;
        }

        public Alliance TryGetAlliance(int id)
        {
            Alliance result;
            lock (this.m_lock)
            {
                Alliance alliance;

                result = (this.m_alliances.TryGetValue(id, out alliance) ? alliance : null);
            }
            return result;
        }
        public Alliance TryGetAlliance(string name)
        {
            Alliance result;
            lock (this.m_lock)
            {
                result = this.m_alliances.FirstOrDefault(entry => string.Equals(entry.Value.Name, name, StringComparison.CurrentCultureIgnoreCase)).Value;
            }
            return result;
        }

        public EmblemRecord TryGetEmblem(int id)
        {
            EmblemRecord record;

            return this.m_emblems.TryGetValue(id, out record) ? record : null;
        }

        public bool DeleteAlliance(Alliance alliance)
        {
            bool result;
            lock (this.m_lock)
            {
                // TODO : clean alliance
                this.m_alliances.Remove(alliance.Id);
                this.m_alliancesToDelete.Push(alliance);
                result = true;
            }
            return result;
        }

        public bool DoesNameExist(string name)
        {
            return this.m_alliances.Any(pair => string.Equals(pair.Value.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }
        public bool DoesTagExist(string tag)
        {
            return this.m_alliances.Any(pair => string.Equals(pair.Value.Tag, tag, StringComparison.CurrentCultureIgnoreCase));
        }
        public bool DoesEmblemExist(Stump.DofusProtocol.Types.GuildEmblem emblem)
        {
            return this.m_alliances.Any(entry => entry.Value.Emblem.DoesEmblemMatch(emblem));
        }

        public IEnumerable<AllianceFactSheetInformations> GetAlliancesFactSheetInformations()
        {
            lock (this.m_lock)
            {
                return this.m_alliances.Values
                    .Select(entry => entry.GetAllianceFactSheetInformations());
            }
        }
        public IEnumerable<AllianceVersatileInformations> GetAlliancesVersatileInformations()
        {
            lock (this.m_lock)
            {
                return this.m_alliances.Values
                    .Select(entry => entry.GetAllianceVersatileInformations());
            }
        }

        public void Save()
        {
            lock (this.m_lock)
            {
                Alliance temp;
                using (var enumerator = (
                    from alliance in this.m_alliances.Values
                    where alliance.IsDirty
                    select alliance).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        temp = enumerator.Current;

                        temp.Save(base.Database);
                    }
                }

                while (this.m_alliancesToDelete.Count > 0)
                {
                    temp = this.m_alliancesToDelete.Pop();

                    base.Database.Delete(temp.Record);
                }
            }

        }
    }
}
