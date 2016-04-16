using NLog;
using Stump.ORM;
using Stump.ORM.SubSonic.SQLGeneration.Schema;
using System.Reflection;
namespace Stump.Server.WorldServer.Database
{
	public abstract class AutoAssignedRecord<T> : ISaveIntercepter
	{
		private static readonly Logger logger;
		private static readonly PrimaryKeyIdProvider IdProvider;
		[PrimaryKey("Id", false)]
		public int Id
		{
			get;
			set;
		}
		[Ignore]
		public bool IsNew
		{
			get;
			set;
		}
		static AutoAssignedRecord()
		{
			AutoAssignedRecord<T>.logger = LogManager.GetCurrentClassLogger();
			if (AutoAssignedRecord<T>.IdProvider == null)
			{
				System.Type typeFromHandle = typeof(T);
				TableNameAttribute customAttribute = typeFromHandle.GetCustomAttribute<TableNameAttribute>();
				if (customAttribute == null)
				{
					AutoAssignedRecord<T>.logger.Error("TableNameAttribute not found in {0}", typeFromHandle.Name);
				}
				else
				{
					AutoAssignedRecord<T>.IdProvider = new PrimaryKeyIdProvider("Id", customAttribute.TableName);
				}
			}
		}
		public static int PopNextId()
		{
			return AutoAssignedRecord<T>.IdProvider.Pop();
		}
		public void AssignIdentifier()
		{
			this.Id = AutoAssignedRecord<T>.PopNextId();
		}
		public virtual void BeforeSave(bool insert)
		{
			if (insert && this.Id == 0)
			{
				this.AssignIdentifier();
			}
		}
	}
}
