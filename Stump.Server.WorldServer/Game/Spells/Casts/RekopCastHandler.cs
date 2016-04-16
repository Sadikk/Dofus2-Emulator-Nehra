using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using System.Linq;
using Stump.Server.WorldServer.Game.Effects.Spells;
using Stump.Server.WorldServer.Game.Effects.Spells.Damage;

namespace Stump.Server.WorldServer.Game.Spells.Casts
{
	[SpellCastHandler(SpellIdEnum.Rekop)]
	public class RekopCastHandler : DefaultSpellCastHandler
	{
		public int CastRound
		{
			get;
			set;
		}
		public RekopCastHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical) : base(caster, spell, targetedCell, critical)
		{
		}
		public override void Initialize()
		{
			base.Initialize();
			this.CastRound = new System.Random().Next(0, 4);
			base.Handlers = (
				from entry in base.Handlers
				where entry.Effect.Duration == this.CastRound
				select entry).ToArray<SpellEffectHandler>();
			SpellEffectHandler[] handlers = base.Handlers;
			for (int i = 0; i < handlers.Length; i++)
			{
				SpellEffectHandler spellEffectHandler = handlers[i];
				DirectDamage directDamage = spellEffectHandler as DirectDamage;
				if (directDamage != null)
				{
					directDamage.BuffTriggerType = BuffTriggerType.BUFF_ENDED;
				}
			}
		}
	}
}
