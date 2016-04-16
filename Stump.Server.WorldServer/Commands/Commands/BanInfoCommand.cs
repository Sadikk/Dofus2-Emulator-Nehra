using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class BanInfoCommand : CommandBase
	{
		public BanInfoCommand()
		{
			base.Aliases = new string[]
			{
				"baninfo",
				"jailinfo"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Gives info about a ban";
			base.AddParameter<string>("player", "player", "Player name", null, true, null);
			base.AddParameter<string>("account", "acc", "Account login", null, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			bool flag = trigger.IsArgumentDefined("account");
			bool flag2 = trigger.IsArgumentDefined("player");
			if (!(flag ^ flag2))
			{
				trigger.ReplyError("No parameter given");
			}
			else
			{
				if (!IPCAccessor.Instance.IsConnected)
				{
					trigger.ReplyError("IPC service not operational !");
				}
				else
				{
					if (flag)
					{
						AccountRequestMessage message = new AccountRequestMessage
						{
							Login = trigger.Get<string>("account")
						};
						IPCAccessor.Instance.SendRequest<AccountAnswerMessage>(message, delegate(AccountAnswerMessage reply)
						{
							this.OnReply(trigger, reply);
						});
					}
					else
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
								AccountRequestMessage message2 = new AccountRequestMessage
								{
									CharacterId = new int?(character.Id)
								};
								IPCAccessor.Instance.SendRequest<AccountAnswerMessage>(message2, delegate(AccountAnswerMessage reply)
								{
									this.OnReply(trigger, reply);
								}, delegate(IPCErrorMessage error)
								{
									trigger.ReplyError("Cannot get player {0} account : {1}", new object[]
									{
										character.Name,
										error.Message
									});
								});
							}
						});
					}
				}
			}
		}
		private void OnReply(TriggerBase trigger, AccountAnswerMessage reply)
		{
			trigger.Reply("Account : {0} ({1})", new object[]
			{
				trigger.Bold(reply.Account.Login),
				trigger.Bold(reply.Account.Id)
			});
			trigger.Reply("Banned : {0}", new object[]
			{
				trigger.Bold(reply.Account.IsBanned)
			});
			trigger.Reply("Jailed : {0}", new object[]
			{
				trigger.Bold(reply.Account.IsJailed)
			});
			if (reply.Account.IsBanned)
			{
				trigger.Reply("Reason : {0}", new object[]
				{
					trigger.Bold(reply.Account.BanReason)
				});
				trigger.Reply("Until : {0}", new object[]
				{
					trigger.Bold(reply.Account.BanEndDate)
				});
			}
		}
	}
}
