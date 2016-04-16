using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects.Instances;

namespace Stump.Server.WorldServer.Game.Items
{
	public interface IItem
	{
		IItemRecord Record
		{
			get;
		}
		int Guid
		{
			get;
		}
		uint Stack
		{
			get;
			set;
		}
		ItemTemplate Template
		{
			get;
		}
		System.Collections.Generic.List<EffectBase> Effects
		{
			get;
		}
	}
}
