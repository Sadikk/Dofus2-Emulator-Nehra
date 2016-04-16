using Stump.Core.Reflection;
using Stump.ORM;
using System;
namespace Stump.Server.BaseServer.Database
{
	public abstract class DataManager
	{
        public static Stump.ORM.Database DefaultDatabase;
        private Stump.ORM.Database m_database;
        public Stump.ORM.Database Database
		{
			get
			{
				return this.m_database ?? DataManager.DefaultDatabase;
			}
		}
        public void ChangeDataSource(Stump.ORM.Database datasource)
		{
			if (this.m_database == null)
			{
				this.m_database = datasource;
			}
			else
			{
				this.m_database = datasource;
				this.TearDown();
				this.Initialize();
			}
		}
		public virtual void Initialize()
		{
		}
		public virtual void TearDown()
		{
		}
	}
	public abstract class DataManager<T> : Singleton<T> where T : class
	{
        private Stump.ORM.Database m_database;
		public Stump.ORM.Database Database
		{
			get
			{
				return this.m_database ?? DataManager.DefaultDatabase;
			}
		}
        public void ChangeDataSource(Stump.ORM.Database datasource)
		{
			if (this.m_database == null)
			{
				this.m_database = datasource;
			}
			else
			{
				this.m_database = datasource;
				this.TearDown();
				this.Initialize();
			}
		}
		public virtual void Initialize()
		{
		}
		public virtual void TearDown()
		{
		}
	}
}
