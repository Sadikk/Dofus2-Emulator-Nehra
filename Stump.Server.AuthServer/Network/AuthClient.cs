using Stump.Core.Extensions;
using Stump.DofusProtocol.Messages;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Managers;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Network;
using System;
using System.Net.Sockets;

namespace Stump.Server.AuthServer.Network
{
	public sealed class AuthClient : BaseClient
	{
		public Account Account
		{
			get;
			set;
		}

        public IdentificationMessage IdentificationMessage { get; set; }

        public byte[] AesKey { get; set; }

		public bool LookingOfServers
		{
			get;
			set;
		}
		public AuthClient(Socket socket) : base(socket)
		{
			this.Send(new ProtocolRequired(VersionExtension.ProtocolRequired, VersionExtension.ActualProtocol));
            this.Send(new RawDataMessage(CredentialManager.Instance.RawData));
			base.CanReceive = true;
		}
		protected override void OnMessageReceived(Message message)
		{
			AuthPacketHandler.Instance.Dispatch(this, message);

			base.OnMessageReceived(message);
		}
		public void Save()
		{
			AuthServer.Instance.IOTaskPool.AddMessage(delegate
			{
				this.SaveNow();
			});
		}
		public void SaveNow()
		{
			this.Account.Tokens += this.Account.NewTokens;
			this.Account.NewTokens = 0;
            AuthServer.Instance.DBAccessor.Database.Update(this.Account);
        }
		public override string ToString()
		{
			return base.ToString() + ((this.Account != null) ? (" (" + this.Account.Login + ")") : "");
		}
	}
}
