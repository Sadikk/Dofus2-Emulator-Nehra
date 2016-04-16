using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Move
{
	[EffectHandler(EffectsEnum.Effect_Teleport)]
	public class Teleportation : SpellEffectHandler
	{
		public Teleportation(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			base.Caster.Position.Cell = base.TargetedCell;
			base.Fight.ForEach(delegate(Character entry)
			{
				ActionsHandler.SendGameActionFightTeleportOnSameMapMessage(entry.Client, base.Caster, base.Caster, base.TargetedCell);
			});
			return true;
		}
	}
}
