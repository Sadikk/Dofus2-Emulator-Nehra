using Stump.Server.AuthServer;
using System;
using System.Diagnostics;
using System.Threading;

namespace Stump.GUI.AuthConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AuthServer server = new AuthServer();
            if (!Debugger.IsAttached)
            {
                try
                {
                    Program.StartServer(ref server);
                }
                catch (Exception e)
                {
                    server.HandleCrashException(e);
                }
                finally
                {
                    server.Shutdown();
                }
            }
            else
            {
                Program.StartServer(ref server);
            }
        }

        private static void StartServer(ref AuthServer server)
        {
            server.Initialize();
            server.Start();
            GC.Collect();

            while (true)
            {
                Thread.Sleep(5000);
            }
        }
    }
}