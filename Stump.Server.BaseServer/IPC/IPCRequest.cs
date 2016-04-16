using Stump.Core.Timers;
using System;
namespace Stump.Server.BaseServer.IPC
{
	public class IPCRequest<T> : IIPCRequest where T : IPCMessage
	{
		public IPCMessage RequestMessage
		{
			get;
			set;
		}
		public Guid Guid
		{
			get;
			set;
		}
		public TimerEntry TimeoutTimer
		{
			get;
			set;
		}
		public RequestCallbackDelegate<T> Callback
		{
			get;
			set;
		}
		public RequestCallbackErrorDelegate ErrorCallback
		{
			get;
			set;
		}
		public RequestCallbackDefaultDelegate DefaultCallback
		{
			get;
			set;
		}
		public IPCRequest(IPCMessage requestMessage, Guid guid, RequestCallbackDelegate<T> callback, RequestCallbackErrorDelegate errorCallback, RequestCallbackDefaultDelegate defaultCallback, TimerEntry timeoutTimer)
		{
			this.RequestMessage = requestMessage;
			this.Guid = guid;
			this.Callback = callback;
			this.ErrorCallback = errorCallback;
			this.DefaultCallback = defaultCallback;
			this.TimeoutTimer = timeoutTimer;
		}
		public bool ProcessMessage(IPCMessage message)
		{
			this.TimeoutTimer.Stop();
			if (message is T)
			{
				this.Callback(message as T);
			}
			else
			{
				if (message is IPCErrorMessage)
				{
					this.ErrorCallback(message as IPCErrorMessage);
				}
				else
				{
					this.DefaultCallback(message);
				}
			}
			return true;
		}
	}
}
