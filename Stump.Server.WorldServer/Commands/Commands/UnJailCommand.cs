using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Commands.Commands.Patterns;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System.Drawing;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class UnJailCommand : TargetCommand
	{
		public UnJailCommand()
		{
			base.Aliases = new string[]
			{
				"unjail"
			};
			base.RequiredRole = RoleEnum.Moderator;
			base.Description = "Unjail a player";
			base.AddTargetParameter(false, "Defined target");
		}
		public override void Execute(TriggerBase trigger)
		{
			if (!IPCAccessor.Instance.IsConnected)
			{
				trigger.ReplyError("IPC service not operational !");
			}
			else
			{
				Character[] targets = base.GetTargets(trigger);
				for (int i = 0; i < targets.Length; i++)
				{
					Character character = targets[i];
					Character target1 = character;
					IPCAccessor.Instance.SendRequest(new UnBanAccountMessage(character.Account.Login), delegate(CommonOKMessage ok)
					{
						trigger.Reply("Account {0} unjailed", new object[]
						{
							target1.Account.Login
						});
					}, delegate(IPCErrorMessage error)
					{
						trigger.ReplyError("Account {0} not unjailed : {1}", new object[]
						{
							target1.Account.Login,
							error.Message
						});
					});
					if (character.Account.IsJailed)
					{
						character.Account.IsJailed = false;
						character.Account.BanEndDate = null;
						Character target2 = character;
						character.Area.ExecuteInContext(delegate
						{
							target2.Teleport(target2.Breed.GetStartPosition(), true);
						});
						character.SendServerMessage("Vous avez été libéré de prison.", Color.Red);
					}
				}
			}
		}
	}
}
