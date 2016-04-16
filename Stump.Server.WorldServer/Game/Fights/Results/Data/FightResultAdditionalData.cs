using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Fights.Results.Data
{
	public abstract class FightResultAdditionalData
	{
		public Character Character
		{
			get;
			private set;
		}
		protected FightResultAdditionalData(Character character)
		{
			this.Character = character;
		}
		public abstract Stump.DofusProtocol.Types.FightResultAdditionalData GetFightResultAdditionalData();
		public abstract void Apply();
	}
}
