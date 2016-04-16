using Stump.Core.IO;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Trigger
{
	public abstract class GameTrigger : TriggerBase
	{
		public override RoleEnum UserRole
		{
			get
			{
				return this.Character.UserGroup.Role;
			}
		}
		public override bool CanFormat
		{
			get
			{
				return true;
			}
		}
		public Character Character
		{
			get;
			protected set;
		}
		protected GameTrigger(StringStream args, Character character) : base(args, character.UserGroup.Role)
		{
			this.Character = character;
		}
		protected GameTrigger(string args, Character character) : base(args, character.UserGroup.Role)
		{
			this.Character = character;
		}
		public override bool CanAccessCommand(CommandBase command)
		{
			return this.Character.UserGroup.IsCommandAvailable(command);
		}
	}
}
