using Stump.Core.Reflection;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Matching.Characters;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Database.Spells;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Spells;

namespace Stump.Server.WorldServer.Commands
{
	public static class ParametersConverter
	{
		public static ConverterHandler<Character> CharacterConverter = delegate(string entry, TriggerBase trigger)
		{
			Character characterByPattern;
			if (trigger is GameTrigger && (trigger as GameTrigger).Character != null)
			{
				characterByPattern = Singleton<World>.Instance.GetCharacterByPattern((trigger as GameTrigger).Character, entry);
			}
			else
			{
				characterByPattern = Singleton<World>.Instance.GetCharacterByPattern(entry);
			}
			if (characterByPattern == null)
			{
				throw new ConverterException(string.Format("'{0}' is not found or not connected", entry));
			}
			return characterByPattern;
		};
		public static ConverterHandler<Character[]> CharactersConverter = delegate(string entry, TriggerBase trigger)
		{
			CharacterMatching characterMatching = new CharacterMatching(entry, (trigger is GameTrigger) ? (trigger as GameTrigger).Character : null);
			return characterMatching.FindMatchs();
		};
		public static ConverterHandler<ItemTemplate> ItemTemplateConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			ItemTemplate result;
			if (int.TryParse(entry, out id))
			{
				ItemTemplate itemTemplate = Singleton<ItemManager>.Instance.TryGetTemplate(id);
				if (itemTemplate == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid item", entry));
				}
				result = itemTemplate;
			}
			else
			{
				ItemTemplate itemTemplate2 = Singleton<ItemManager>.Instance.TryGetTemplate(entry, CommandBase.IgnoreCommandCase);
				if (itemTemplate2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid item", entry));
				}
				result = itemTemplate2;
			}
			return result;
		};
		public static ConverterHandler<ItemSetTemplate> ItemSetTemplateConverter = delegate(string entry, TriggerBase trigger)
		{
			uint id;
			ItemSetTemplate result;
			if (uint.TryParse(entry, out id))
			{
				ItemSetTemplate itemSetTemplate = Singleton<ItemManager>.Instance.TryGetItemSetTemplate(id);
				if (itemSetTemplate == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid item set", entry));
				}
				result = itemSetTemplate;
			}
			else
			{
				ItemSetTemplate itemSetTemplate2 = Singleton<ItemManager>.Instance.TryGetItemSetTemplate(entry, CommandBase.IgnoreCommandCase);
				if (itemSetTemplate2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid item set", entry));
				}
				result = itemSetTemplate2;
			}
			return result;
		};
		public static ConverterHandler<SpellTemplate> SpellTemplateConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			SpellTemplate result;
			if (int.TryParse(entry, out id))
			{
				SpellTemplate spellTemplate = Singleton<SpellManager>.Instance.GetSpellTemplate(id);
				if (spellTemplate == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid spell", entry));
				}
				result = spellTemplate;
			}
			else
			{
				SpellTemplate spellTemplate2 = Singleton<SpellManager>.Instance.GetSpellTemplate(entry, CommandBase.IgnoreCommandCase);
				if (spellTemplate2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid spell", entry));
				}
				result = spellTemplate2;
			}
			return result;
		};
		public static ConverterHandler<Fight> FightConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			if (!int.TryParse(entry, out id))
			{
				throw new ConverterException(string.Format("'{0}' invalid fight id. Must be a number.", entry));
			}
			Fight fight = Singleton<FightManager>.Instance.GetFight(id);
			if (fight == null)
			{
				throw new ConverterException(string.Format("Fight not found'{0}'", entry));
			}
			return fight;
		};
		public static ConverterHandler<NpcTemplate> NpcTemplateConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			NpcTemplate result;
			if (int.TryParse(entry, out id))
			{
				NpcTemplate npcTemplate = Singleton<NpcManager>.Instance.GetNpcTemplate(id);
				if (npcTemplate == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid npc template id", entry));
				}
				result = npcTemplate;
			}
			else
			{
				NpcTemplate npcTemplate2 = Singleton<NpcManager>.Instance.GetNpcTemplate(entry, CommandBase.IgnoreCommandCase);
				if (npcTemplate2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a npc template name", entry));
				}
				result = npcTemplate2;
			}
			return result;
		};
		public static ConverterHandler<MonsterTemplate> MonsterTemplateConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			MonsterTemplate result;
			if (int.TryParse(entry, out id))
			{
				MonsterTemplate template = Singleton<MonsterManager>.Instance.GetTemplate(id);
				if (template == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid monster template id", entry));
				}
				result = template;
			}
			else
			{
				MonsterTemplate template2 = Singleton<MonsterManager>.Instance.GetTemplate(entry, CommandBase.IgnoreCommandCase);
				if (template2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a monster template name", entry));
				}
				result = template2;
			}
			return result;
		};
		public static ConverterHandler<SuperArea> SuperAreaConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			SuperArea result;
			if (int.TryParse(entry, out id))
			{
				SuperArea superArea = Singleton<World>.Instance.GetSuperArea(id);
				if (superArea == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid super area id", entry));
				}
				result = superArea;
			}
			else
			{
				SuperArea superArea2 = Singleton<World>.Instance.GetSuperArea(entry);
				if (superArea2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a super area name", entry));
				}
				result = superArea2;
			}
			return result;
		};
		public static ConverterHandler<Area> AreaConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			Area result;
			if (int.TryParse(entry, out id))
			{
				Area area = Singleton<World>.Instance.GetArea(id);
				if (area == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid area id", entry));
				}
				result = area;
			}
			else
			{
				Area area2 = Singleton<World>.Instance.GetArea(entry);
				if (area2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not an area name", entry));
				}
				result = area2;
			}
			return result;
		};
		public static ConverterHandler<SubArea> SubAreaConverter = delegate(string entry, TriggerBase trigger)
		{
			int id;
			SubArea result;
			if (int.TryParse(entry, out id))
			{
				SubArea subArea = Singleton<World>.Instance.GetSubArea(id);
				if (subArea == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a valid sub area id", entry));
				}
				result = subArea;
			}
			else
			{
				SubArea subArea2 = Singleton<World>.Instance.GetSubArea(entry);
				if (subArea2 == null)
				{
					throw new ConverterException(string.Format("'{0}' is not a sub area name", entry));
				}
				result = subArea2;
			}
			return result;
		};
		public static ConverterHandler<Map> MapConverter = delegate(string entry, TriggerBase trigger)
		{
			Map result;
			if (entry.Contains(","))
			{
				string[] array = entry.Split(new char[]
				{
					','
				});
				if (array.Length != 2)
				{
					throw new ConverterException(string.Format("'{0}' is not of 'mapid' or'x,y'", entry));
				}
				int num = int.Parse(array[0].Trim());
				int num2 = int.Parse(array[1].Trim());
				Map map = Singleton<World>.Instance.GetMap(num, num2, true);
				if (map == null)
				{
					throw new ConverterException(string.Format("'x:{0} y:{1}' map not found", num, num2));
				}
				result = map;
			}
			else
			{
				int id;
				if (!int.TryParse(entry, out id))
				{
					throw new ConverterException(string.Format("'{0}' is not of format 'mapid' or 'x,y'", entry));
				}
				Map map = Singleton<World>.Instance.GetMap(id);
				if (map == null)
				{
					throw new ConverterException(string.Format("'{0}' map not found", entry));
				}
				result = map;
			}
			return result;
		};
		public static ConverterHandler<T> GetEnumConverter<T>() where T : struct
		{
			System.Type type = typeof(T);
			if (!type.IsEnum)
			{
				throw new ConverterException("Cannot convert non-enum type");
			}
			return delegate(string entry, TriggerBase trigger)
			{
				T result;
				if (!System.Enum.TryParse<T>(entry, CommandBase.IgnoreCommandCase, out result))
				{
					throw new ConverterException(string.Format("Cannot convert '{0}' to a {1}", entry, type.Name));
				}
				return result;
			};
		}
	}
}
