using NLog;
using Stump.Core.Timers;
using Stump.Server.BaseServer.IPC.Messages;
using System;
using System.Collections.Generic;
namespace Stump.Server.BaseServer.IPC
{
	public abstract class IPCEntity
	{
		public delegate void IPCMessageHandler(IPCMessage message);
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private Dictionary<Guid, IIPCRequest> m_requests = new Dictionary<Guid, IIPCRequest>();
		protected abstract int RequestTimeout
		{
			get;
		}
		public abstract void Send(IPCMessage message);
		protected abstract TimerEntry RegisterTimer(Action<int> action, int timeout);
		protected abstract void ProcessRequest(IPCMessage request);
		protected abstract void ProcessAnswer(IIPCRequest request, IPCMessage answer);
		protected virtual void ProcessMessage(IPCMessage message)
		{
			if (message.RequestGuid == Guid.Empty)
			{
				this.ProcessRequest(message);
			}
			IIPCRequest iIPCRequest = this.TryGetRequest(message.RequestGuid);
			if (iIPCRequest != null)
			{
				this.ProcessAnswer(iIPCRequest, message);
			}
			else
			{
				this.ProcessRequest(message);
			}
		}
		public void ReplyRequest(IPCMessage message, IPCMessage request)
		{
			if (request != null)
			{
				message.RequestGuid = request.RequestGuid;
			}
			this.Send(message);
		}
		public void SendRequest<T>(IPCMessage message, RequestCallbackDelegate<T> callback, RequestCallbackErrorDelegate errorCallback, RequestCallbackDefaultDelegate defaultCallback, int timeout) where T : IPCMessage
		{
			Guid guid = Guid.NewGuid();
			message.RequestGuid = guid;
			IPCRequest<T> request = null;
			TimerEntry timeoutTimer = this.RegisterTimer(delegate(int param0)
			{
				this.RequestTimedOut(request);
			}, timeout);
			request = new IPCRequest<T>(message, guid, callback, errorCallback, defaultCallback, timeoutTimer);
			lock (this.m_requests)
			{
				this.m_requests.Add(guid, request);
			}
			this.Send(message);
		}
		public void SendRequest<T>(IPCMessage message, RequestCallbackDelegate<T> callback, RequestCallbackErrorDelegate errorCallback, int timeout) where T : IPCMessage
		{
			this.SendRequest<T>(message, callback, errorCallback, new RequestCallbackDefaultDelegate(this.DefaultRequestUnattemptCallback), timeout);
		}
		public void SendRequest<T>(IPCMessage message, RequestCallbackDelegate<T> callback, RequestCallbackErrorDelegate errorCallback, RequestCallbackDefaultDelegate defaultCallback) where T : IPCMessage
		{
			this.SendRequest<T>(message, callback, errorCallback, defaultCallback, this.RequestTimeout * 1000);
		}
		public void SendRequest<T>(IPCMessage message, RequestCallbackDelegate<T> callback, RequestCallbackErrorDelegate errorCallback) where T : IPCMessage
		{
			this.SendRequest<T>(message, callback, errorCallback, this.RequestTimeout * 1000);
		}
		public void SendRequest<T>(IPCMessage message, RequestCallbackDelegate<T> callback) where T : IPCMessage
		{
			this.SendRequest<T>(message, callback, new RequestCallbackErrorDelegate(this.DefaultRequestErrorCallback), this.RequestTimeout);
		}
		public void SendRequest(IPCMessage message, RequestCallbackDelegate<CommonOKMessage> callback, RequestCallbackErrorDelegate errorCallback, int timeout)
		{
			this.SendRequest<CommonOKMessage>(message, callback, errorCallback, new RequestCallbackDefaultDelegate(this.DefaultRequestUnattemptCallback), timeout);
		}
		public void SendRequest(IPCMessage message, RequestCallbackDelegate<CommonOKMessage> callback, RequestCallbackErrorDelegate errorCallback, RequestCallbackDefaultDelegate defaultCallback)
		{
			this.SendRequest<CommonOKMessage>(message, callback, errorCallback, defaultCallback, this.RequestTimeout * 1000);
		}
		public void SendRequest(IPCMessage message, RequestCallbackDelegate<CommonOKMessage> callback, RequestCallbackErrorDelegate errorCallback)
		{
			this.SendRequest<CommonOKMessage>(message, callback, errorCallback, this.RequestTimeout * 1000);
		}
		public void SendRequest(IPCMessage message, RequestCallbackDelegate<CommonOKMessage> callback)
		{
			this.SendRequest<CommonOKMessage>(message, callback, new RequestCallbackErrorDelegate(this.DefaultRequestErrorCallback), this.RequestTimeout);
		}
		private IIPCRequest TryGetRequest(Guid guid)
		{
			IIPCRequest result;
			if (guid == Guid.Empty)
			{
				result = null;
			}
			else
			{
				IIPCRequest iIPCRequest;
				this.m_requests.TryGetValue(guid, out iIPCRequest);
				result = iIPCRequest;
			}
			return result;
		}
		private void RequestTimedOut(IIPCRequest request)
		{
			request.ProcessMessage(new IPCErrorTimeoutMessage(string.Format("Request {0} timed out", request.RequestMessage.GetType())));
		}
		private void DefaultRequestErrorCallback(IPCErrorMessage errorMessage)
		{
			IIPCRequest iIPCRequest = this.TryGetRequest(errorMessage.RequestGuid);
			IPCEntity.logger.Error("Error received of type {0}. Request {1} Message : {2} StackTrace : {3}", new object[]
			{
				errorMessage.GetType(),
				iIPCRequest.RequestMessage.GetType(),
				errorMessage.Message,
				errorMessage.StackTrace
			});
		}
		private void DefaultRequestUnattemptCallback(IPCMessage message)
		{
			IPCEntity.logger.Error<Type, Type>("Unattempt message {0}. Request {1}", message.GetType(), this.TryGetRequest(message.RequestGuid).RequestMessage.GetType());
		}
	}
}
