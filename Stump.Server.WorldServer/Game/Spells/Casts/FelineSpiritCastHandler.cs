using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using System.Linq;
using Stump.Server.WorldServer.Game.Effects.Spells.Damage;

namespace Stump.Server.WorldServer.Game.Spells.Casts
{
	[SpellCastHandler(SpellIdEnum.FelineSpirit)]
	public class FelineSpiritCastHandler : DefaultSpellCastHandler
	{
		public FelineSpiritCastHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
		{
		}
		public override void Initialize()
		{
			base.Initialize();
			DirectDamage directDamage = base.Handlers[1] as DirectDamage;
			if (directDamage != null)
			{
				directDamage.BuffTriggerType = BuffTriggerType.BUFF_ENDED_TURNEND;
			}
		}
		public override void Execute()
		{
			Buff[] array = base.Caster.GetBuffs((Buff entry) => entry.Spell == base.Spell).ToArray<Buff>();
			Buff[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Buff buff = array2[i];
				base.Caster.RemoveBuff(buff);
			}
			base.Execute();
		}
	}
}
