using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Spells;
using TreeSharp;
namespace Stump.Server.WorldServer.AI.Fights.Actions
{
	public class SpellCastAction : AIAction
	{
		public const int MaxCastLimit = 20;
		public Spell Spell
		{
			get;
			private set;
		}
		public Cell Target
		{
			get;
			private set;
		}
		public bool MultipleCast
		{
			get;
			set;
		}
		public SpellCastAction(AIFighter fighter, Spell spell, Cell target) : base(fighter)
		{
			this.Spell = spell;
			this.Target = target;
		}
		public SpellCastAction(AIFighter fighter, Spell spell, Cell target, bool multipleCast) : base(fighter)
		{
			this.Spell = spell;
			this.Target = target;
			this.MultipleCast = multipleCast;
		}
		protected override RunStatus Run(object context)
		{
			RunStatus result;
			if (this.Spell == null)
			{
				result = RunStatus.Failure;
			}
			else
			{
				if (base.Fighter.CanCastSpell(this.Spell, this.Target) != SpellCastResult.OK)
				{
					result = RunStatus.Failure;
				}
				else
				{
					int num = 0;
					while (base.Fighter.CastSpell(this.Spell, this.Target))
					{
						num++;
						if (!this.MultipleCast || base.Fighter.CanCastSpell(this.Spell, this.Target) != SpellCastResult.OK || num > 20)
						{
							break;
						}
					}
					result = RunStatus.Success;
				}
			}
			return result;
		}
	}
}
