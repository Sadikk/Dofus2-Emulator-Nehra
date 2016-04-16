using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Handlers.Context;

namespace Stump.Server.WorldServer.Game.Fights
{
	public class FightSpectator
	{
		public event System.Action<FightSpectator> Left;
		public Character Character
		{
			get;
			private set;
		}
		public WorldClient Client
		{
			get
			{
				return this.Character.Client;
			}
		}
		public Fight Fight
		{
			get;
			private set;
		}
		public System.DateTime JoinTime
		{
			get;
			internal set;
		}
		public FightSpectator(Character character, Fight fight)
		{
			this.Character = character;
			this.Fight = fight;
		}
		public void ShowCell(Cell cell)
		{
			ContextHandler.SendShowCellSpectatorMessage(this.Fight.SpectatorClients, this, cell);
		}
		public void Leave()
		{
			this.OnLeft();
		}
		protected virtual void OnLeft()
		{
			System.Action<FightSpectator> left = this.Left;
			if (left != null)
			{
				left(this);
			}
		}
	}
}
