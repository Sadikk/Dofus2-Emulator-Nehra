using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Linq;
using Stump.Server.WorldServer.Game.Effects.Spells;

namespace Stump.Server.WorldServer.Game.Spells.Casts
{
	[DefaultSpellCastHandler]
	public class DefaultSpellCastHandler : SpellCastHandler
	{
		protected bool m_initialized;
		
        public SpellEffectHandler[] Handlers
		{
			get;
			protected set;
		}
		public override bool SilentCast
		{
			get
			{
				bool arg_33_0;
				if (this.m_initialized)
				{
					arg_33_0 = this.Handlers.Any((SpellEffectHandler entry) => entry.RequireSilentCast());
				}
				else
				{
					arg_33_0 = false;
				}
				return arg_33_0;
			}
		}

        public DefaultSpellCastHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical)
            : base(caster, spell, targetedCell, critical)
        { }
		
		public override void Initialize()
		{
			AsyncRandom asyncRandom = new AsyncRandom();
			System.Collections.Generic.List<EffectDice> list = base.Critical ? base.SpellLevel.CriticalEffects : base.SpellLevel.Effects;
			System.Collections.Generic.List<SpellEffectHandler> list2 = new System.Collections.Generic.List<SpellEffectHandler>();
			double num = asyncRandom.NextDouble();
			double num2 = (double)list.Sum((EffectDice entry) => entry.Random);
			bool flag = false;
			foreach (EffectDice current in list)
			{
				if (current.Random > 0)
				{
					if (flag)
					{
						continue;
					}
					if (num > (double)current.Random / num2)
					{
						num -= (double)current.Random / num2;
						continue;
					}
					flag = true;
				}
				SpellEffectHandler spellEffectHandler = Singleton<EffectManager>.Instance.GetSpellEffectHandler(current, base.Caster, base.Spell, base.TargetedCell, base.Critical);
				if (base.MarkTrigger != null)
				{
					spellEffectHandler.MarkTrigger = base.MarkTrigger;
				}
				list2.Add(spellEffectHandler);
			}
			this.Handlers = list2.ToArray();
			this.m_initialized = true;
		}
		public override void Execute()
		{
			if (!this.m_initialized)
			{
				this.Initialize();
			}
			SpellEffectHandler[] handlers = this.Handlers;
			for (int i = 0; i < handlers.Length; i++)
			{
				SpellEffectHandler spellEffectHandler = handlers[i];
                if (spellEffectHandler is DefaultSpellEffect && (int)spellEffectHandler.Effect.EffectId != 1160)
                {
                    string dump = string.Format("{0}[{1}] : EffectId = {2} ; Target = {3} ; AffectedCells = {4} \n", this.Spell.Template.Name, i, spellEffectHandler.Effect.EffectId, spellEffectHandler.Targets.ToString(), string.Join("/", spellEffectHandler.AffectedCells.Select(x => x.Id)));
                    System.IO.File.AppendAllText("spell_effects.txt", dump);
                }
				spellEffectHandler.Apply();
			}
		}
		public override System.Collections.Generic.IEnumerable<SpellEffectHandler> GetEffectHandlers()
		{
			return this.Handlers;
		}
	}
}
