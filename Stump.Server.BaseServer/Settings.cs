using Stump.Core.Attributes;
using Stump.Core.I18N;
using System;
namespace Stump.Server.BaseServer
{
	public static class Settings
	{
		[Variable]
		public static readonly bool EnableBenchmarking;
		[Variable]
		public static readonly int? InactivityDisconnectionTime = new int?(900);
		[Variable]
		public static readonly Languages Language = Languages.English;
	}
}
