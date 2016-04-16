using System;
namespace Stump.Server.BaseServer.Handler
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public abstract class HandlerAttribute : Attribute
	{
		public uint MessageId
		{
			get;
			set;
		}
		protected HandlerAttribute(uint messageId)
		{
			this.MessageId = messageId;
		}
		public override string ToString()
		{
			return this.MessageId.ToString();
		}
	}
}
