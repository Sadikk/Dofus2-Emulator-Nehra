using NLog;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Actions;

namespace Stump.Server.WorldServer.Game.Effects.Spells.Double
{
	[EffectHandler(EffectsEnum.Effect_Double)]
	public class Double : SpellEffectHandler
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public Double(EffectDice effect, FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(effect, caster, spell, targetedCell, critical)
		{
		}
		public override bool Apply()
		{
			SummonedClone summonedClone = new SummonedClone((int)base.Fight.GetNextContextualId(), base.Caster, base.TargetedCell);
			ActionsHandler.SendGameActionFightSummonMessage(base.Fight.Clients, summonedClone);
			base.Caster.AddSummon(summonedClone);
			base.Caster.Team.AddFighter(summonedClone);
			return true;
		}
	}
}
