using NLog;
using Stump.Core.Attributes;
using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.Core.Timers;
using Stump.DofusProtocol.Enums;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Network;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
namespace Stump.Server.AuthServer.Managers
{
    public class AccountManager : DataManager<AccountManager>
    {
        [Variable]
        public static List<PlayableBreedEnum> AvailableBreeds = new List<PlayableBreedEnum>
		{
			PlayableBreedEnum.Feca,
			PlayableBreedEnum.Osamodas,
			PlayableBreedEnum.Enutrof,
			PlayableBreedEnum.Sram,
			PlayableBreedEnum.Xelor,
			PlayableBreedEnum.Ecaflip,
			PlayableBreedEnum.Eniripsa,
			PlayableBreedEnum.Iop,
			PlayableBreedEnum.Cra,
			PlayableBreedEnum.Sadida,
			PlayableBreedEnum.Sacrieur,
			PlayableBreedEnum.Pandawa,
			PlayableBreedEnum.Roublard,
			PlayableBreedEnum.Zobal
		};

        [Variable]
        public static int CacheTimeout = 300;

        [Variable]
        public static int IpBanRefreshTime = 600;

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<int, Tuple<DateTime, Account>> m_accountsCache = new Dictionary<int, Tuple<DateTime, Account>>();
        private List<IpBan> m_ipBans = new List<IpBan>();
        private SimpleTimerEntry m_timer;
        private SimpleTimerEntry m_bansTimer;

        public override void Initialize()
        {
            base.Initialize();
            this.m_timer = ServerBase<AuthServer>.Instance.IOTaskPool.CallPeriodically((AccountManager.CacheTimeout * 60) / 4, new Action(this.TimerTick));
            this.m_ipBans = base.Database.Fetch<IpBan>(IpBanRelator.FetchQuery, new object[0]);
        }

        public override void TearDown()
        {
            ServerBase<AuthServer>.Instance.IOTaskPool.CancelSimpleTimer(this.m_timer);
            ServerBase<AuthServer>.Instance.IOTaskPool.CancelSimpleTimer(this.m_bansTimer);
        }

        private void TimerTick()
        {
            List<int> list = new List<int>();
            foreach (Tuple<DateTime, Account> current in this.m_accountsCache.Values)
            {
                if (current.Item1 >= DateTime.Now)
                {
                    list.Add(current.Item2.Id);
                }
            }
            foreach (int current2 in list)
            {
                this.m_accountsCache.Remove(current2);
            }
        }

        private void RefreshIpBans()
        {
            lock (this.m_ipBans)
            {
                this.m_ipBans.Clear();
                this.m_ipBans.AddRange(base.Database.Query<IpBan>(IpBanRelator.FetchQuery, new object[0]));
            }
        }

        public Account FindAccountById(int id)
        {
            return base.Database.Query<Account, WorldCharacter, Account>(new Func<Account, WorldCharacter, Account>(new AccountRelator().Map), string.Format(AccountRelator.FindAccountById, id), new object[0]).SingleOrDefault<Account>();
        }

        public Account FindAccountByLogin(string login)
        {
            return base.Database.Query<Account, WorldCharacter, Account>(new Func<Account, WorldCharacter, Account>(new AccountRelator().Map), AccountRelator.FindAccountByLogin, new object[]
			{
				login
			}).SingleOrDefault<Account>();
        }

        public Account FindAccountByNickname(string nickname)
        {
            return base.Database.Query<Account, WorldCharacter, Account>(new Func<Account, WorldCharacter, Account>(new AccountRelator().Map), AccountRelator.FindAccountByNickname, new object[]
			{
				nickname
			}).SingleOrDefault<Account>();
        }

        public IpBan FindIpBan(string ip)
        {
            IpBan result;
            lock (this.m_ipBans)
            {
                result = this.m_ipBans.FirstOrDefault((IpBan x) => x.IPAsString == ip);
            }
            return result;
        }

        public IpBan FindMatchingIpBan(string ipStr)
        {
            bool flag = false;
            IpBan result;
            List<IpBan> ipBans = new List<IpBan>();
            try
            {
                Monitor.Enter(ipBans = this.m_ipBans, ref flag);
                IPAddress ip = IPAddress.Parse(ipStr);
                IEnumerable<IpBan> source =
                    from entry in this.m_ipBans
                    where entry.Match(ip)
                    select entry;
                result = (
                    from entry in source
                    orderby entry.GetRemainingTime() descending
                    select entry).FirstOrDefault<IpBan>();
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(ipBans);
                }
            }
            return result;
        }

        public void CacheAccount(Account account)
        {
            if (this.m_accountsCache.ContainsKey(account.Id))
            {
                this.m_accountsCache[account.Id] = Tuple.Create<DateTime, Account>(DateTime.Now + TimeSpan.FromSeconds((double)AccountManager.CacheTimeout), account);
            }
            else
            {
                this.m_accountsCache.Add(account.Id, Tuple.Create<DateTime, Account>(DateTime.Now + TimeSpan.FromSeconds((double)AccountManager.CacheTimeout), account));
            }
        }

        public void UnCacheAccount(Account account)
        {
            this.m_accountsCache.Remove(account.Id);
        }

        public Account FindCachedAccountByTicket(string ticket)
        {
            Tuple<DateTime, Account>[] array = (
                from entry in this.m_accountsCache.Values
                where entry.Item2.Ticket == ticket
                select entry).ToArray<Tuple<DateTime, Account>>();
            Account result;
            if (array.Count<Tuple<DateTime, Account>>() > 1)
            {
                Tuple<DateTime, Account>[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    Tuple<DateTime, Account> tuple = array2[i];
                    tuple.Item2.Ticket = string.Empty;
                    this.UnCacheAccount(tuple.Item2);
                }
                result = null;
            }
            else
            {
                Tuple<DateTime, Account> tuple2 = array.SingleOrDefault<Tuple<DateTime, Account>>();
                result = ((tuple2 != null) ? tuple2.Item2 : null);
            }

            return result;
        }

        public bool LoginExists(string login)
        {
            return base.Database.ExecuteScalar<bool>("SELECT EXISTS(SELECT 1 FROM accounts WHERE Login=@0)", new object[]
			{
				login
			});
        }

        public bool NicknameExists(string nickname)
        {
            return base.Database.ExecuteScalar<bool>("SELECT EXISTS(SELECT 1 FROM accounts WHERE Nickname=@0)", new object[]
			{
				nickname
			});
        }

        public bool CreateAccount(Account account)
        {
            bool result;
            if (this.LoginExists(account.Login))
            {
                result = false;
            }
            else
            {
                base.Database.Insert(account);
                result = true;
            }
            return result;
        }

        public bool DeleteAccount(Account account)
        {
            base.Database.Delete(account);
            return true;
        }

        public WorldCharacter CreateAccountCharacter(Account account, WorldServer world, int characterId)
        {
            WorldCharacter result;
            if (account.WorldCharacters.Any((WorldCharacter entry) => entry.CharacterId == characterId))
            {
                result = null;
            }
            else
            {
                WorldCharacter worldCharacter = new WorldCharacter
                {
                    AccountId = account.Id,
                    WorldId = world.Id,
                    CharacterId = characterId
                };
                account.WorldCharacters.Add(worldCharacter);
                base.Database.Insert(worldCharacter);
                result = worldCharacter;
            }
            return result;
        }

        public bool DeleteAccountCharacter(Account account, WorldServer world, int characterId)
        {
            bool result;
            if (base.Database.Execute(string.Format("DELETE FROM worlds_characters WHERE AccountId={0} AND CharacterId={1} AND WorldId={2}", account.Id, characterId, world.Id), new object[0]) <= 0)
            {
                result = false;
            }
            else
            {
                this.CreateDeletedCharacter(account, world, characterId);
                account.WorldCharacters.RemoveAll((WorldCharacter x) => x.CharacterId == characterId && x.WorldId == world.Id);
                result = true;
            }
            return result;
        }

        public bool AddAccountCharacter(Account account, WorldServer world, int characterId)
        {
            this.CreateAccountCharacter(account, world, characterId);
            return true;
        }

        public WorldCharacterDeleted CreateDeletedCharacter(Account account, WorldServer world, int characterId)
        {
            WorldCharacterDeleted worldCharacterDeleted = new WorldCharacterDeleted
            {
                AccountId = account.Id,
                WorldId = world.Id,
                CharacterId = characterId
            };
            base.Database.Insert(worldCharacterDeleted);
            return worldCharacterDeleted;
        }

        public bool DeleteDeletedCharacter(WorldCharacterDeleted deletedCharacter)
        {
            bool result;
            if (deletedCharacter == null)
            {
                result = false;
            }
            else
            {
                base.Database.Delete(deletedCharacter);
                result = true;
            }
            return result;
        }

        public void DisconnectClientsUsingAccount(Account account)
        {
            AuthClient[] array = ServerBase<AuthServer>.Instance.FindClients((AuthClient entry) => entry.Account != null && entry.Account.Id == account.Id).ToArray<AuthClient>();
            AuthClient[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                AuthClient authClient = array2[i];
                authClient.Disconnect();
            }
            if (account.LastConnectionWorld.HasValue)
            {
                WorldServer serverById = Singleton<WorldServerManager>.Instance.GetServerById(account.LastConnectionWorld.Value);
                if (serverById != null && serverById.Connected && serverById.IPCClient != null)
                {
                    serverById.IPCClient.SendRequest<DisconnectedClientMessage>(new DisconnectClientMessage(account.Id)
                    , delegate(DisconnectedClientMessage msg)
                    {
                    }, delegate(IPCErrorMessage error)
                    {
                    });
                }
            }
        }
    }
}
