using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Managers;
using Stump.Server.BaseServer.Commands;
using System;

namespace Stump.Server.AuthServer.Commands
{
    public class AccountCreateCommand : SubCommand
    {
        public AccountCreateCommand()
        {
            base.Aliases = new string[]
			{
				"create",
				"cr",
				"new"
			};
            base.ParentCommand = typeof(AccountCommands);
            base.RequiredRole = RoleEnum.Administrator;
            base.Description = "Create a new account.";
            base.AddParameter<string>("accountname", "name", "Name of the created account", null, false, null);
            base.AddParameter<string>("password", "pass", "Password of the created accont", null, false, null);
            ConverterHandler<RoleEnum> roleConverter = ParametersConverter.RoleConverter;
            base.AddParameter<RoleEnum>("role", "role", "Role of the created account. See RoleEnum", RoleEnum.Player, false, roleConverter);
            base.AddParameter<string>("question", "quest", "Secret question", "dummy?", false, null);
            base.AddParameter<string>("answer", "answer", "Answer to the secret question", "dummy!", false, null);
        }

        public override void Execute(TriggerBase trigger)
        {
            string text = trigger.Get<string>("accountname");
            Account account = new Account
            {
                Login = text,
                PasswordHash = trigger.Get<string>("password").GetMD5(),
                Nickname = trigger.Get<string>("accountname"),
                SecretQuestion = trigger.Get<string>("question"),
                SecretAnswer = trigger.Get<string>("answer"),
                Role = trigger.Get<RoleEnum>("role"),
                Email = "",
                AvailableBreeds = AccountManager.AvailableBreeds,
                Lang = "fr"
            };
            if (Singleton<AccountManager>.Instance.CreateAccount(account))
            {
                trigger.Reply("Created Account \"{0}\". Role : {1}.", new object[]
				{
					text,
					Enum.GetName(typeof(RoleEnum), account.Role)
				});
            }
            else
            {
                trigger.Reply("Couldn't create account. Account may already exists.");
            }
        }
    }
}
