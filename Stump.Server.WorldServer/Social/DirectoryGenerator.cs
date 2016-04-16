using Stump.Core.Attributes;
using Stump.Core.Threading;
using Stump.Server.BaseServer.Initialization;
using System.Threading.Tasks;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;
using System.IO;
using System.Collections.Generic;
using System;
using Stump.Core.Reflection;
using Stump.Server.WorldServer.Game.Alliances;
using Stump.Server.WorldServer.Game.Guilds;

namespace Social
{
    /// <summary>
    /// Classe servant à la génération des fichiers permettant au client d'afficher les données de l'annuaire.
    /// </summary>
    public static class DirectoryGenerator
    {
        private static readonly Dictionary<Type, FileStream> _streams = new Dictionary<Type, FileStream>();

        private static readonly object _lock = new object();

        [Variable]
        public static int NextDirectoryActualization = 1000 * 60 * 15;

        [Initialization(InitializationPass.Last)]
        public static void Initialize()
        {
            DirectoryGenerator.InitializeStreams(typeof(AllianceListMessage), typeof(AllianceVersatileInfoListMessage), 
                typeof(GuildListMessage), typeof(GuildVersatileInfoListMessage));

            DirectoryGenerator.Generate();
        }

        private static void Generate()
        {
            lock (DirectoryGenerator._lock)
            {
                DirectoryGenerator.Generate<AllianceListMessage>(DirectoryGenerator.GenerateAllianceList);
                DirectoryGenerator.Generate<AllianceVersatileInfoListMessage>(DirectoryGenerator.GenerateAllianceVersatileList);

                DirectoryGenerator.Generate<GuildListMessage>(DirectoryGenerator.GenerateGuildList);
                DirectoryGenerator.Generate<GuildVersatileInfoListMessage>(DirectoryGenerator.GenerateGuildVersatileList);
            }

            Task.Factory.StartNewDelayed(DirectoryGenerator.NextDirectoryActualization, DirectoryGenerator.Generate);
        }

        private static void Generate<T>(Action<ICustomDataOutput> action)
        {
            var stream = DirectoryGenerator._streams[typeof(T)];
            var writer = new CustomDataWriter(stream);
            stream.Lock(0, long.MaxValue);
            try
            {
                action(writer);

                stream.Flush();
            }
            finally
            {
                stream.Unlock(0, long.MaxValue);
            }
        }

        private static void GenerateAllianceList(ICustomDataOutput data)
        {
            var message = new AllianceListMessage(Singleton<AllianceManager>.Instance.GetAlliancesFactSheetInformations());

            message.Pack(data);
        }
        private static void GenerateAllianceVersatileList(ICustomDataOutput data)
        {
            var message = new AllianceVersatileInfoListMessage(Singleton<AllianceManager>.Instance.GetAlliancesVersatileInformations());

            message.Pack(data);
        }
        private static void GenerateGuildList(ICustomDataOutput data)
        {
            var message = new GuildListMessage(Singleton<GuildManager>.Instance.GetGuildsListInformations());

            message.Pack(data);
        }
        private static void GenerateGuildVersatileList(ICustomDataOutput data)
        {
            var message = new GuildVersatileInfoListMessage(Singleton<GuildManager>.Instance.GetGuildsVersatileInformations());

            message.Pack(data);
        }


        private static void InitializeStreams(params Type[] types)
        {
            foreach (var item in types)
            {
                DirectoryGenerator._streams.Add(item, new FileStream(DirectoryGenerator.GetFileName(item.Name), FileMode.Create, FileAccess.Write));
            }
        }

        private static string GetFileName(string type)
        {
            return string.Format("{0}.{1}.data", type, 1); // TODO : server id
        }
    }
}
