using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects.Spells.Damage;
using Stump.Server.WorldServer.Game.Effects.Spells.Summon;

namespace Stump.Server.WorldServer.Game.Spells.Casts
{
	[SpellCastHandler(SpellIdEnum.LivingChest)]
	public class LivingChestCastHandler : DefaultSpellCastHandler
	{
		public LivingChestCastHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
		{
		}
		public override void Execute()
		{
			if (!this.m_initialized)
			{
				this.Initialize();
			}
			if (base.Handlers.Length == 2)
			{
				Kill kill = base.Handlers[0] as Kill;
				Summon summonEffect = base.Handlers[1] as Summon;
				if (kill != null && summonEffect != null)
				{
					System.Collections.Generic.IEnumerable<SummonedMonster> allFighters = base.Fight.GetAllFighters<SummonedMonster>((SummonedMonster entry) => entry.Team == this.Caster.Team && entry.Monster.MonsterId == (int)summonEffect.Dice.DiceNum);
					kill.SetAffectedActors(allFighters);
					kill.Apply();
					summonEffect.Apply();
				}
			}
		}
	}
}
