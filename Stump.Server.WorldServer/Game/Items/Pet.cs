using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;

namespace Stump.Server.WorldServer.Game.Items
{
	public class Pet
	{
		private System.Collections.Generic.List<EffectBase> m_effects = new System.Collections.Generic.List<EffectBase>();
		public Character Owner
		{
			get;
			private set;
		}
		public BasePlayerItem Item
		{
			get;
			private set;
		}
		public ItemTemplate LastFood
		{
			get;
			private set;
		}
		public PetTemplate PetTemplate
		{
			get;
			private set;
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<EffectBase> Effects
		{
			get
			{
				return this.m_effects.AsReadOnly();
			}
		}
		public bool TryToFeed(BasePlayerItem item)
		{
			return false;
		}
		private void OnFightFinished(Character character, CharacterFighter fighter)
		{
		}
	}
}
