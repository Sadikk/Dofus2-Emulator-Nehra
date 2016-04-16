using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Effects.Instances;

namespace Stump.Server.WorldServer.Database.Items
{
	public interface IItemRecord
	{
		int Id
		{
			get;
			set;
		}
		ItemTemplate Template
		{
			get;
			set;
		}
		uint Stack
		{
			get;
			set;
		}
		System.Collections.Generic.List<EffectBase> Effects
		{
			get;
			set;
		}
		bool IsNew
		{
			get;
			set;
		}
		bool IsDirty
		{
			get;
			set;
		}
		void AssignIdentifier();
	}
}
