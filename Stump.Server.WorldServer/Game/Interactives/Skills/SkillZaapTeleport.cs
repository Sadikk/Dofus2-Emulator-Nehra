using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Database.Interactives;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Dialogs.Interactives;

namespace Stump.Server.WorldServer.Game.Interactives.Skills
{
	[Discriminator("ZaapTeleport", typeof(Skill), new System.Type[]
	{
		typeof(int),
		typeof(InteractiveSkillRecord),
		typeof(InteractiveObject)
	})]
	public class SkillZaapTeleport : Skill
	{
		public SkillZaapTeleport(int id, InteractiveSkillRecord record, InteractiveObject interactiveObject) : base(id, record, interactiveObject)
		{
		}
		public override bool IsEnabled(Character character)
		{
			return true;
		}
		public override void Execute(Character character)
		{
			ZaapDialog zaapDialog = new ZaapDialog(character, base.InteractiveObject, character.KnownZaaps);
			zaapDialog.Open();
		}
	}
}
