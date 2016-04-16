using MongoDB.Bson;
using MongoDB.Driver;
using Stump.Core.Attributes;
using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.ORM;
using Stump.Server.BaseServer.Initialization;
using System;
namespace Stump.Server.BaseServer.Logging
{
	public class MongoLogger : Singleton<MongoLogger>
	{
		[Variable(Priority = 10)]
		public static bool IsMongoLoggerEnabled = true;
		[Variable(Priority = 10)]
		public static DatabaseConfiguration MongoDBConfiguration = new DatabaseConfiguration
		{
			Host = "localhost",
			DbName = "stump_logs",
			User = "root",
			Password = ""
		};
		private SelfRunningTaskPool m_taskPool;
		private MongoDatabase m_database;
		[Initialization(InitializationPass.Fourth)]
		public void Initialize()
		{
			if (MongoLogger.IsMongoLoggerEnabled)
			{
				MongoClient mongoClient = new MongoClient(string.Format("mongodb://{0}:{1}@{2}:{3}", new object[]
				{
					MongoLogger.MongoDBConfiguration.User,
					MongoLogger.MongoDBConfiguration.Password,
					MongoLogger.MongoDBConfiguration.Host,
					3307
				}));
				MongoServer server = mongoClient.GetServer();
				this.m_database = server.GetDatabase(MongoLogger.MongoDBConfiguration.DbName);
				this.m_taskPool = new SelfRunningTaskPool(100, "Mongo logger");
			}
		}
		public bool Insert(string collection, BsonDocument document)
		{
			bool result;
			if (!MongoLogger.IsMongoLoggerEnabled)
			{
				result = false;
			}
			else
			{
				if (this.m_database != null)
				{
					this.m_taskPool.AddMessage(delegate
					{
						this.m_database.GetCollection<BsonDocument>(collection).Insert(document);
					});
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
	}
}
