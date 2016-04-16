using System;
namespace Stump.Server.BaseServer.IPC
{
	public delegate void RequestCallbackDelegate<in T>(T callbackMessage) where T : IPCMessage;
}
