using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Drawing;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class UnBanCommand : CommandBase
	{
		public UnBanCommand()
		{
			base.Aliases = new string[]
			{
				"unban"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Unban an character";
			base.AddParameter<string>("player", "character", "Player name", null, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			if (!IPCAccessor.Instance.IsConnected)
			{
				trigger.ReplyError("IPC service not operational !");
			}
			else
			{
				Character player = Singleton<World>.Instance.GetCharacter(trigger.Get<string>("player"));
				if (player == null)
				{
					ServerBase<WorldServer>.Instance.IOTaskPool.ExecuteInContext(delegate
					{
						CharacterRecord character = Singleton<CharacterManager>.Instance.GetCharacterByName(trigger.Get<string>("player"));
						if (character == null)
						{
							trigger.ReplyError("Player {0} not found", new object[]
							{
								trigger.Get<string>("player")
							});
						}
						else
						{
							AccountRequestMessage message = new AccountRequestMessage
							{
								CharacterId = new int?(character.Id)
							};
							IPCAccessor.Instance.SendRequest<AccountAnswerMessage>(message, delegate(AccountAnswerMessage reply)
							{
								IPCAccessor.Instance.SendRequest(new UnBanAccountMessage(reply.Account.Login), delegate(CommonOKMessage ok)
								{
									trigger.Reply("Account {0} unbanned", new object[]
									{
										reply.Account.Login
									});
								}, delegate(IPCErrorMessage error)
								{
									trigger.ReplyError("Account {0} not unbanned : {1}", new object[]
									{
										reply.Account.Login,
										error.Message
									});
								});
							}, delegate(IPCErrorMessage error)
							{
								trigger.ReplyError("Player {0} not unbanned : {1}", new object[]
								{
									character.Name,
									error.Message
								});
							});
						}
					});
				}
				else
				{
					IPCAccessor.Instance.SendRequest(new UnBanAccountMessage(player.Account.Login), delegate(CommonOKMessage ok)
					{
						trigger.Reply("Account {0} unbanned", new object[]
						{
							player.Account.Login
						});
					}, delegate(IPCErrorMessage error)
					{
						trigger.ReplyError("Account {0} not unbanned : {1}", new object[]
						{
							player.Account.Login,
							error.Message
						});
					});
					if (player.Account.IsJailed)
					{
						player.Account.IsJailed = false;
						player.Account.BanEndDate = null;
						player.Teleport(player.Breed.GetStartPosition(), true);
						player.SendServerMessage("Vous avez été libéré de prison.", Color.Red);
					}
				}
			}
		}
	}
}
