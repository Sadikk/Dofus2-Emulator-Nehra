using Stump.Core.I18N;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using System.Linq;
namespace Stump.Server.WorldServer.Database.I18n
{
	public class TextManager : DataManager<TextManager>
	{
		private Languages? m_defaultLanguages;
		private System.Collections.Generic.Dictionary<uint, LangText> m_texts = new System.Collections.Generic.Dictionary<uint, LangText>();
		private System.Collections.Generic.Dictionary<string, LangTextUi> m_textsUi = new System.Collections.Generic.Dictionary<string, LangTextUi>();
		[Initialization(InitializationPass.First)]
		public override void Initialize()
		{
			this.m_texts = base.Database.Fetch<LangText>(LangTextRelator.FetchQuery, new object[0]).ToDictionary((LangText entry) => entry.Id);
			this.m_textsUi = base.Database.Fetch<LangTextUi>(LangTextUiRelator.FetchQuery, new object[0]).ToDictionary((LangTextUi entry) => entry.Name);
		}
		public void SetDefaultLanguage(Languages languages)
		{
			this.m_defaultLanguages = new Languages?(languages);
		}
		public Languages GetDefaultLanguage()
		{
			return this.m_defaultLanguages.HasValue ? this.m_defaultLanguages.Value : Stump.Server.BaseServer.Settings.Language;
		}
		public string GetText(int id)
		{
			return this.GetText(id, this.GetDefaultLanguage());
		}
		public string GetText(int id, Languages lang)
		{
			return this.GetText((uint)id, lang);
		}
		public string GetText(uint id)
		{
			return this.GetText(id, this.GetDefaultLanguage());
		}
		public string GetText(uint id, Languages lang)
		{
			LangText record;
			string result;
			if (!this.m_texts.TryGetValue(id, out record))
			{
				result = "(not found)";
			}
			else
			{
				result = this.GetText(record, lang);
			}
			return result;
		}
		public string GetText(LangText record)
		{
			return this.GetText(record, this.GetDefaultLanguage());
		}
		public string GetText(LangText record, Languages lang)
		{
			string result;
			switch (lang)
			{
			case Languages.English:
				result = (record.English ?? "(not found)");
				break;
			case Languages.French:
				result = (record.French ?? "(not found)");
				break;
			case Languages.German:
				result = (record.German ?? "(not found)");
				break;
			case Languages.Spanish:
				result = (record.Spanish ?? "(not found)");
				break;
			case Languages.Italian:
				result = (record.Italian ?? "(not found)");
				break;
			case Languages.Japanish:
				result = (record.Japanish ?? "(not found)");
				break;
			case Languages.Dutsh:
				result = (record.Dutsh ?? "(not found)");
				break;
			case Languages.Portugese:
				result = (record.Portugese ?? "(not found)");
				break;
			case Languages.Russish:
				result = (record.Russish ?? "(not found)");
				break;
			default:
				result = "(not found)";
				break;
			}
			return result;
		}
		public string GetUiText(string id)
		{
			return this.GetUiText(id, this.GetDefaultLanguage());
		}
		public string GetUiText(string id, Languages lang)
		{
			LangTextUi langTextUi;
			string result;
			if (!this.m_textsUi.TryGetValue(id, out langTextUi))
			{
				result = "(not found)";
			}
			else
			{
				switch (lang)
				{
				case Languages.English:
					result = (langTextUi.English ?? "(not found)");
					break;
				case Languages.French:
					result = (langTextUi.French ?? "(not found)");
					break;
				case Languages.German:
					result = (langTextUi.German ?? "(not found)");
					break;
				case Languages.Spanish:
					result = (langTextUi.Spanish ?? "(not found)");
					break;
				case Languages.Italian:
					result = (langTextUi.Italian ?? "(not found)");
					break;
				case Languages.Japanish:
					result = (langTextUi.Japanish ?? "(not found)");
					break;
				case Languages.Dutsh:
					result = (langTextUi.Dutsh ?? "(not found)");
					break;
				case Languages.Portugese:
					result = (langTextUi.Portugese ?? "(not found)");
					break;
				case Languages.Russish:
					result = (langTextUi.Russish ?? "(not found)");
					break;
				default:
					result = "(not found)";
					break;
				}
			}
			return result;
		}
	}
}
