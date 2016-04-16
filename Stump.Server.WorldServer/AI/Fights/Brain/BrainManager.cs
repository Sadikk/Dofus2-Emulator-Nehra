using NLog;
using Stump.Core.Reflection;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Game.Actors.Fight;
using System.Linq;
using System.Reflection;
namespace Stump.Server.WorldServer.AI.Fights.Brain
{
	public class BrainManager : Singleton<BrainManager>
	{
		protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private readonly System.Collections.Generic.Dictionary<int, System.Type> m_brains = new System.Collections.Generic.Dictionary<int, System.Type>();
		[Initialization(InitializationPass.Fourth)]
		public void Initialize()
		{
			this.RegisterAll(System.Reflection.Assembly.GetExecutingAssembly());
		}
		public void RegisterAll(System.Reflection.Assembly assembly)
		{
			if (!(assembly == null))
			{
				foreach (System.Type current in 
					from x in assembly.GetTypes()
					where x.IsSubclassOf(typeof(Brain))
					select x)
				{
					this.RegisterBrain(current);
				}
			}
		}
		public void RegisterBrain(System.Type brain)
		{
			System.Collections.Generic.IEnumerable<BrainIdentifierAttribute> enumerable = brain.GetCustomAttributes(typeof(BrainIdentifierAttribute)) as System.Collections.Generic.IEnumerable<BrainIdentifierAttribute>;
			if (enumerable != null)
			{
				foreach (int current in 
					from brainIdentifierAttribute in enumerable
					select brainIdentifierAttribute.Identifiers into identifiers
					from identifier in identifiers
					where !this.m_brains.ContainsKey(identifier)
					select identifier)
				{
					this.m_brains.Add(current, brain);
				}
			}
		}
		public Brain GetDefaultBrain(AIFighter fighter)
		{
			return new Brain(fighter);
		}
		public Brain GetBrain(int identifier, AIFighter fighter)
		{
			Brain result;
			if (!this.m_brains.ContainsKey(identifier))
			{
				result = this.GetDefaultBrain(fighter);
			}
			else
			{
				System.Type type = this.m_brains[identifier];
				result = (Brain)System.Activator.CreateInstance(type, new object[]
				{
					fighter
				});
			}
			return result;
		}
	}
}
