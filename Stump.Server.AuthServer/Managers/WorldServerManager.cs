using NLog;
using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Handlers.Connection;
using Stump.Server.AuthServer.IPC;
using Stump.Server.AuthServer.Network;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.IPC.Objects;
using Stump.Server.BaseServer.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Stump.Server.AuthServer.Managers
{
    public class WorldServerManager : DataManager<WorldServerManager>
    {
        [Variable(true)]
        public static int WorldServerTimeout = 20;

        [Variable(true)]
        public static int PingCheckInterval = 2000;

        [Variable(true)]
        public static bool CheckPassword;

        [Variable(true)]
        public static List<string> AllowedServerIps = new List<string>
		{
			"127.0.0.1"
		};

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private ConcurrentDictionary<int, WorldServer> m_realmlist;

        public event Action<WorldServer> ServerAdded;

        public event Action<WorldServer> ServerRemoved;

        public event Action<WorldServer> ServerStateChanged;

        private void OnServerAdded(WorldServer server)
        {
            Singleton<ClientManager>.Instance.FindAll<AuthClient>((AuthClient entry) => entry.LookingOfServers).ForEach(delegate(AuthClient entry)
            {
                ConnectionHandler.SendServerStatusUpdateMessage(entry, server);
            });
            Action<WorldServer> serverAdded = this.ServerAdded;
            if (serverAdded != null)
            {
                serverAdded(server);
            }
        }

        private void OnServerRemoved(WorldServer server)
        {
            Singleton<ClientManager>.Instance.FindAll<AuthClient>((AuthClient entry) => entry.LookingOfServers).ForEach(delegate(AuthClient entry)
            {
                ConnectionHandler.SendServerStatusUpdateMessage(entry, server);
            });
            Action<WorldServer> serverRemoved = this.ServerRemoved;
            if (serverRemoved != null)
            {
                serverRemoved(server);
            }
        }

        private void OnServerStateChanged(WorldServer server)
        {
            Singleton<ClientManager>.Instance.FindAll<AuthClient>((AuthClient entry) => entry.LookingOfServers).ForEach(delegate(AuthClient entry)
            {
                ConnectionHandler.SendServerStatusUpdateMessage(entry, server);
            });
            Action<WorldServer> serverStateChanged = this.ServerStateChanged;
            if (serverStateChanged != null)
            {
                serverStateChanged(server);
            }
        }

        public override void Initialize()
        {
            IEnumerable<WorldServer> source = base.Database.Query<WorldServer>(WorldServerRelator.FetchQuery, new object[0]);
            this.m_realmlist = new ConcurrentDictionary<int, WorldServer>(source.ToDictionary((WorldServer entry) => entry.Id));
            foreach (KeyValuePair<int, WorldServer> current in this.m_realmlist)
            {
                current.Value.SetOffline();
                base.Database.Update(current.Value);
            }
        }

        public WorldServer CreateWorld(WorldServerData worldServerData)
        {
            WorldServer worldServer = new WorldServer
            {
                Id = worldServerData.Id,
                Name = worldServerData.Name,
                RequireSubscription = worldServerData.RequireSubscription,
                RequiredRole = worldServerData.RequiredRole,
                CharCapacity = worldServerData.Capacity,
                ServerSelectable = true,
                Address = worldServerData.Address,
                Port = worldServerData.Port
            };
            if (!this.m_realmlist.TryAdd(worldServer.Id, worldServer))
            {
                throw new Exception("Server already registered");
            }
            base.Database.Insert(worldServer);
            WorldServerManager.logger.Info(string.Format("World {0} created", worldServerData.Name));
            return worldServer;
        }

        public void UpdateWorld(WorldServer record, WorldServerData worldServerData, bool save = true)
        {
            if (record.Id != worldServerData.Id)
            {
                throw new Exception("Ids don't match");
            }
            record.Name = worldServerData.Name;
            record.Address = worldServerData.Address;
            record.Port = worldServerData.Port;
            record.CharCapacity = worldServerData.Capacity;
            record.RequiredRole = worldServerData.RequiredRole;
            record.RequireSubscription = worldServerData.RequireSubscription;
            if (save)
            {
                base.Database.Update(record);
            }
        }

        public WorldServer RequestConnection(IPCClient client, WorldServerData world)
        {
            if (!this.IsIPAllowed(client.Address))
            {
                WorldServerManager.logger.Error<int, IPAddress>("Server <Id : {0}> ({1}) Try to connect self on the authenfication server but its ip is not allowed (see WorldServerManager.AllowedServerIps)", world.Id, client.Address);
                throw new Exception(string.Format("Ip {0} not allow by auth. server", client.Address));
            }
            WorldServer worldServer;
            if (!this.m_realmlist.ContainsKey(world.Id))
            {
                worldServer = this.CreateWorld(world);
            }
            else
            {
                worldServer = this.m_realmlist[world.Id];
                this.UpdateWorld(worldServer, world, false);
            }
            worldServer.SetOnline(client);
            base.Database.Update(worldServer);
            WorldServerManager.logger.Info<string, int, string>("Registered World : \"{0}\" <Id : {1}> <{2}>", world.Name, world.Id, world.Address);
            this.OnServerAdded(worldServer);
            return worldServer;
        }

        public bool IsIPAllowed(IPAddress ip)
        {
            return WorldServerManager.AllowedServerIps.Select(new Func<string, IPAddressRange>(IPAddressRange.Parse)).Any((IPAddressRange x) => x.Match(ip));
        }

        public WorldServer GetServerById(int id)
        {
            return this.m_realmlist.ContainsKey(id) ? this.m_realmlist[id] : null;
        }

        public bool CanAccessToWorld(AuthClient client, WorldServer world)
        {
            return world != null && world.Status == ServerStatusEnum.ONLINE && client.Account.Role >= world.RequiredRole && world.CharsCount < world.CharCapacity && (!world.RequireSubscription || client.Account.SubscriptionEnd <= DateTime.Now);
        }

        public bool CanAccessToWorld(AuthClient client, int worldId)
        {
            WorldServer serverById = this.GetServerById(worldId);
            return serverById != null && serverById.Status == ServerStatusEnum.ONLINE && client.Account.Role >= serverById.RequiredRole && serverById.CharsCount < serverById.CharCapacity && (!serverById.RequireSubscription || client.Account.SubscriptionEnd <= DateTime.Now);
        }

        public void ChangeWorldState(WorldServer server, ServerStatusEnum state, bool save = true)
        {
            server.Status = state;
            this.OnServerStateChanged(server);
            if (save)
            {
                base.Database.Update(server);
            }
        }

        public IEnumerable<GameServerInformations> GetServersInformationArray(AuthClient client)
        {
            return
                from world in this.m_realmlist.Values
                select this.GetServerInformation(client, world);
        }

        public GameServerInformations GetServerInformation(AuthClient client, WorldServer world)
        {
            return new GameServerInformations((ushort)world.Id, (sbyte)world.Status, (sbyte)world.Completion, world.ServerSelectable, client.Account.GetCharactersCountByWorld(world.Id), 0xFFFFFd);
        }

        public bool HasWorld(int id)
        {
            return this.m_realmlist.ContainsKey(id);
        }

        public void RemoveWorld(WorldServerData world)
        {
            WorldServer serverById = this.GetServerById(world.Id);
            if (serverById != null && serverById.Connected)
            {
                serverById.SetOffline();
                base.Database.Update(serverById);
                this.OnServerRemoved(this.m_realmlist[world.Id]);
                WorldServerManager.logger.Info<string, int, string>("Unregistered \"{0}\" <Id : {1}> <{2}>", world.Name, world.Id, world.Address);
            }
        }

        public void RemoveWorld(WorldServer world)
        {
            WorldServer serverById = this.GetServerById(world.Id);
            if (serverById != null && serverById.Connected)
            {
                serverById.SetOffline();
                base.Database.Update(serverById);
                this.OnServerRemoved(this.m_realmlist[world.Id]);
                WorldServerManager.logger.Info<string, int, string>("Unregistered \"{0}\" <Id : {1}> <{2}>", world.Name, world.Id, world.Address);
            }
        }
    }
}
