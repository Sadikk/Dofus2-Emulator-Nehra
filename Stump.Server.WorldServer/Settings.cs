using Stump.Core.Attributes;
using System;
using System.Drawing;
namespace Stump.Server.WorldServer
{
	public class Settings
	{
		[Variable(true)]
        public static string MOTD = "Bienvenue sur le serveur test de &lt;b&gt;Stump v. pre-alpha by bouh2&lt;/b&gt;";
		private static string m_htmlMOTDColor = ColorTranslator.ToHtml(Color.OrangeRed);
		private static Color m_MOTDColor = Color.OrangeRed;
		[Variable(true)]
		public static string HtmlMOTDColor
		{
			get
			{
				return Settings.m_htmlMOTDColor;
			}
			set
			{
				Settings.m_htmlMOTDColor = value;
				Settings.m_MOTDColor = ColorTranslator.FromHtml(value);
			}
		}
		public static Color MOTDColor
		{
			get
			{
				return Settings.m_MOTDColor;
			}
			set
			{
				Settings.m_htmlMOTDColor = ColorTranslator.ToHtml(value);
				Settings.m_MOTDColor = value;
			}
		}

        public static short TimeZoneOffset
        {
            get;
            private set;
        }

        static Settings()
        {
            Settings.TimeZoneOffset = (short)System.TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime()).TotalMinutes;
        }
	}
}
