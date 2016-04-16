using System;
namespace Stump.Server.BaseServer.Database
{
	public class DiscriminatorAttribute : Attribute
	{
		public string Discriminator
		{
			get;
			set;
		}
		public Type BaseType
		{
			get;
			set;
		}
		public Type[] CtorParameters
		{
			get;
			set;
		}
		public DiscriminatorAttribute(string discriminator, Type baseType, params Type[] ctorParameters)
		{
			this.Discriminator = discriminator;
			this.BaseType = baseType;
			this.CtorParameters = ctorParameters;
		}
	}
}
