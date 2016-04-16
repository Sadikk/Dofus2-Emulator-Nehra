using System;
namespace Stump.Server.BaseServer.Plugins
{
	public class PluginLoadException : Exception
	{
		public PluginLoadException(string exception) : base(exception)
		{
		}
	}
}
