using Stump.Server.WorldServer;
using System;
using System.Diagnostics;
using System.Threading;

namespace Stump.GUI.WorldConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            WorldServer server = new WorldServer();
            if (!Debugger.IsAttached)
            {
                try
                {
                    server.Initialize();
                    server.Start();
                    GC.Collect();
                    while (true)
                    {
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    server.HandleCrashException(e);
                }
                finally
                {
                    server.Shutdown();
                }
                return;
            }
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
