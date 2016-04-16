using NLog;
using Stump.Core.Reflection;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Spells.Casts;
using System.Linq;
using System.Reflection;
namespace Stump.Server.WorldServer.Game.Spells
{
	public class SpellManager : DataManager<SpellManager>
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private delegate SpellCastHandler SpellCastConstructor(FightActor caster, Spell spell, Cell targetedCell, bool critical);

        private readonly System.Collections.Generic.Dictionary<int, SpellManager.SpellCastConstructor> m_spellsCastHandler = new System.Collections.Generic.Dictionary<int, SpellManager.SpellCastConstructor>();
		private System.Collections.Generic.Dictionary<uint, SpellLevelTemplate> m_spellsLevels;
		private System.Collections.Generic.Dictionary<int, SpellTemplate> m_spells;
		private System.Collections.Generic.Dictionary<int, SpellType> m_spellsTypes;
		private System.Collections.Generic.Dictionary<int, SpellState> m_spellsState;
        private System.Collections.Generic.Dictionary<int, SpellBombTemplate> m_spellsBombs;

		[Initialization(typeof(EffectManager))]
		public override void Initialize()
		{
			this.m_spellsLevels = base.Database.Fetch<SpellLevelTemplate>(SpellLevelTemplateRelator.FetchQuery, new object[0]).ToDictionary((SpellLevelTemplate entry) => entry.Id);
			this.m_spells = base.Database.Fetch<SpellTemplate>(SpellTemplateRelator.FetchQuery, new object[0]).ToDictionary((SpellTemplate entry) => entry.Id);
			this.m_spellsTypes = base.Database.Fetch<SpellType>(SpellTypeRelator.FetchQuery, new object[0]).ToDictionary((SpellType entry) => entry.Id);
			this.m_spellsState = base.Database.Fetch<SpellState>(SpellStateRelator.FetchQuery, new object[0]).ToDictionary((SpellState entry) => entry.Id);
            this.m_spellsBombs = base.Database.Fetch<SpellBombTemplate>(SpellBombRelator.FetchQuery, new object[0]).ToDictionary((SpellBombTemplate entry) => entry.Id);
			this.InitializeHandlers();
		}

		private void InitializeHandlers()
		{
			foreach (System.Type current in 
				from entry in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
				where entry.IsSubclassOf(typeof(SpellCastHandler)) && !entry.IsAbstract
				select entry)
			{
                if (current.GetCustomAttribute < DefaultSpellCastHandlerAttribute>(false) == null)
				{
					SpellCastHandlerAttribute spellCastHandlerAttribute = current.GetCustomAttributes<SpellCastHandlerAttribute>().SingleOrDefault<SpellCastHandlerAttribute>();
					if (spellCastHandlerAttribute == null)
					{
						SpellManager.logger.Error("SpellCastHandler '{0}' has no SpellCastHandlerAttribute, or more than 1", current.Name);
					}
					else
					{
						SpellTemplate spellTemplate = this.GetSpellTemplate(spellCastHandlerAttribute.Spell);
						if (spellTemplate == null)
						{
							SpellManager.logger.Error<string, int>("SpellCastHandler '{0}' -> Spell {1} not found", current.Name, spellCastHandlerAttribute.Spell);
						}
						else
						{
							this.AddSpellCastHandler(current, spellTemplate);
						}
					}
				}
			}
		}

		public CharacterSpellRecord CreateSpellRecord(CharacterRecord owner, SpellTemplate template)
		{
			return new CharacterSpellRecord
			{
				OwnerId = owner.Id,
				Level = 1,
				Position = 63,
				SpellId = template.Id
			};
		}

		public SpellTemplate GetSpellTemplate(int id)
		{
			SpellTemplate spellTemplate;
			return this.m_spells.TryGetValue(id, out spellTemplate) ? spellTemplate : null;
		}
		public SpellTemplate GetSpellTemplate(string name, bool ignorecase)
		{
			return this.m_spells.Values.FirstOrDefault((SpellTemplate entry) => entry.Name.Equals(name, ignorecase ? System.StringComparison.InvariantCultureIgnoreCase : System.StringComparison.InvariantCulture));
		}

        public SpellBombTemplate GetSpellBombTemplate(int id)
        {
            SpellBombTemplate spellBombTemplate;
            return this.m_spellsBombs.TryGetValue(id, out spellBombTemplate) ? spellBombTemplate : null;
        }

		public SpellTemplate GetFirstSpellTemplate(System.Predicate<SpellTemplate> predicate)
		{
			return this.m_spells.Values.FirstOrDefault((SpellTemplate entry) => predicate(entry));
		}

		public System.Collections.Generic.IEnumerable<SpellTemplate> GetSpellTemplates()
		{
			return this.m_spells.Values;
		}

		public SpellLevelTemplate GetSpellLevel(int id)
		{
			SpellLevelTemplate spellLevelTemplate;
			return this.m_spellsLevels.TryGetValue((uint)id, out spellLevelTemplate) ? spellLevelTemplate : null;
		}
		public SpellLevelTemplate GetSpellLevel(int templateid, int level)
		{
			SpellTemplate spellTemplate = this.GetSpellTemplate(templateid);
			SpellLevelTemplate result;
			if (spellTemplate == null)
			{
				result = null;
			}
			else
			{
				result = ((spellTemplate.SpellLevelsIds.Length <= level - 1) ? null : this.GetSpellLevel((int)spellTemplate.SpellLevelsIds[level - 1]));
			}
			return result;
		}

		public System.Collections.Generic.IEnumerable<SpellLevelTemplate> GetSpellLevels(SpellTemplate template)
		{
			return 
				from x in template.SpellLevelsIds
				select this.m_spellsLevels[x];
		}
		public System.Collections.Generic.IEnumerable<SpellLevelTemplate> GetSpellLevels(int id)
		{
			return 
				from entry in this.m_spellsLevels.Values
				where entry.Spell.Id == id
				orderby entry.Id
				select entry;
		}
		public System.Collections.Generic.IEnumerable<SpellLevelTemplate> GetSpellLevels()
		{
			return this.m_spellsLevels.Values;
		}

		public SpellType GetSpellType(uint id)
		{
			SpellType spellType;
			return this.m_spellsTypes.TryGetValue((int)id, out spellType) ? spellType : null;
		}

		public SpellState GetSpellState(uint id)
		{
			SpellState spellState;
			return this.m_spellsState.TryGetValue((int)id, out spellState) ? spellState : null;
		}

		public System.Collections.Generic.IEnumerable<SpellState> GetSpellStates()
		{
			return this.m_spellsState.Values;
		}

		public void AddSpellCastHandler(System.Type handler, SpellTemplate spell)
		{
			System.Reflection.ConstructorInfo constructor = handler.GetConstructor(new System.Type[]
			{
				typeof(FightActor),
				typeof(Spell),
				typeof(Cell),
				typeof(bool)
			});
			if (constructor == null)
			{
				throw new System.Exception(string.Format("Handler {0} : No valid constructor found !", handler.Name));
			}
			this.m_spellsCastHandler.Add(spell.Id, constructor.CreateDelegate<SpellManager.SpellCastConstructor>());
		}

		public SpellCastHandler GetSpellCastHandler(FightActor caster, Spell spell, Cell targetedCell, bool critical)
		{
			SpellManager.SpellCastConstructor spellCastConstructor;
			return this.m_spellsCastHandler.TryGetValue(spell.Template.Id, out spellCastConstructor) ? spellCastConstructor(caster, spell, targetedCell, critical) : new DefaultSpellCastHandler(caster, spell, targetedCell, critical);
		}
	}
}
