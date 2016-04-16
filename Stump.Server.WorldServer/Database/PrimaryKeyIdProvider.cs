using Stump.Core.Pool;
using Stump.Server.BaseServer;
using Stump.Server.BaseServer.Initialization;
using System.Collections.Concurrent;

namespace Stump.Server.WorldServer.Database
{
	public class PrimaryKeyIdProvider : UniqueIdProvider
	{
		private static readonly ConcurrentBag<PrimaryKeyIdProvider> m_pool = new ConcurrentBag<PrimaryKeyIdProvider>();
		private static bool m_synchronised;
		public Stump.ORM.Database Database
		{
			get;
			set;
		}
		public string ColumnName
		{
			get;
			private set;
		}
		public string TableName
		{
			get;
			private set;
		}
		public PrimaryKeyIdProvider(string columnName, string tableName)
		{
			this.ColumnName = columnName;
			this.TableName = tableName;
			if (PrimaryKeyIdProvider.m_synchronised)
			{
				this.Synchronize();
			}
			else
			{
				PrimaryKeyIdProvider.m_pool.Add(this);
			}
			this.Database = ServerBase<WorldServer>.Instance.DBAccessor.Database;
		}
		[Initialization(InitializationPass.Eighth, "Synchronize id providers")]
		public static void SynchronizeAll()
		{
			foreach (PrimaryKeyIdProvider current in PrimaryKeyIdProvider.m_pool)
			{
				current.Synchronize();
			}
			PrimaryKeyIdProvider.m_synchronised = true;
		}
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Synchronize()
		{
			int highestId;
			try
			{
				object obj = this.Database.ExecuteScalar<object>(string.Format("SELECT max({0}) FROM {1}", this.ColumnName, this.TableName), new object[0]);
				if (obj is System.DBNull)
				{
					highestId = 0;
				}
				else
				{
					highestId = (int)obj;
				}
			}
			catch (System.Exception arg)
			{
				throw new System.Exception(string.Format("Cannot retrieve max({0}) from table {1} : {2}", this.ColumnName, this.TableName, arg));
			}
			this.m_highestId = highestId;
		}
		public override int Pop()
		{
			return base.Pop();
		}
		public override void Push(int freeId)
		{
		}
	}
}
