using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Triggers;
using System.Linq;
using Stump.Server.WorldServer.Game.Effects.Spells;

namespace Stump.Server.WorldServer.Game.Spells.Casts
{
	[SpellCastHandler(SpellIdEnum.ChakraConcentration)]
	public class ChakraConcentrationCastHandler : DefaultSpellCastHandler
	{
		private FightActor[] m_affectedActors;
		public ChakraConcentrationCastHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
		{
		}
		public override void Execute()
		{
			if (!this.m_initialized)
			{
				this.Initialize();
			}
			if (base.Handlers.Length == 1)
			{
				this.m_affectedActors = base.Handlers[0].GetAffectedActors().ToArray<FightActor>();
				FightActor[] affectedActors = this.m_affectedActors;
				for (int i = 0; i < affectedActors.Length; i++)
				{
					FightActor fightActor = affectedActors[i];
					int id = fightActor.PopNextBuffId();
					fightActor.AddAndApplyBuff(new TriggerBuff(id, fightActor, base.Caster, base.Handlers[0].Dice, base.Spell, false, false, BuffTriggerType.BEFORE_ATTACKED, new TriggerBuffApplyHandler(this.ChakraConcentrationBuffTrigger))
					{
						Duration = 1
					}, true);
				}
			}
		}
		private void ChakraConcentrationBuffTrigger(TriggerBuff buff, BuffTriggerType trigger, object token)
		{
			Damage damage = token as Damage;
			if (damage != null && damage.MarkTrigger is Trap)
			{
				Trap trap = damage.MarkTrigger as Trap;
				SpellEffectHandler[] handlers = base.Handlers;
				for (int i = 0; i < handlers.Length; i++)
				{
					SpellEffectHandler spellEffectHandler = handlers[i];
					spellEffectHandler.SetAffectedActors(
						from x in this.m_affectedActors
						where trap.ContainsCell(x.Cell)
						select x);
					spellEffectHandler.Apply();
				}
			}
		}
	}
}
