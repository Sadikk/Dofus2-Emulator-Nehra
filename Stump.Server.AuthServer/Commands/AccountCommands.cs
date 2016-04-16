using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using System;
namespace Stump.Server.AuthServer.Commands
{
	public class AccountCommands : SubCommandContainer
	{
		public AccountCommands()
		{
			base.Aliases = new string[]
			{
				"account",
				"acc"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.Description = "Provides many commands to manage accounts";
		}
	}
}
