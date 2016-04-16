using System;
namespace Stump.Server.BaseServer.Commands
{
	public delegate T ConverterHandler<out T>(string str, TriggerBase trigger);
}
