using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player.Custom.LivingObjects
{
	[ItemHasEffect(EffectsEnum.Effect_LivingObjectId)]
	public sealed class BoundLivingObjectItem : CommonLivingObject
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private readonly ItemTemplate m_livingObjectTemplate;
		public override bool AllowFeeding
		{
			get
			{
				return false;
			}
		}
		public BoundLivingObjectItem(Character owner, PlayerItemRecord record) : base(owner, record)
		{
			EffectInteger effectInteger = (EffectInteger)this.Effects.First((EffectBase x) => x.EffectId == EffectsEnum.Effect_LivingObjectId);
			this.m_livingObjectTemplate = Singleton<ItemManager>.Instance.TryGetTemplate((int)effectInteger.Value);
			base.LivingObjectRecord = Singleton<ItemManager>.Instance.TryGetLivingObjectRecord((int)effectInteger.Value);
			if (base.LivingObjectRecord == null || this.m_livingObjectTemplate == null)
			{
				BoundLivingObjectItem.logger.Error("Living Object {0} has no template", this.Template.Id);
			}
			this.Initialize();
		}
		public override bool Feed(BasePlayerItem food)
		{
			bool result;
			if ((ulong)food.Template.TypeId != (ulong)((long)base.LivingObjectRecord.ItemType))
			{
				result = false;
			}
			else
			{
				short num = (short)System.Math.Ceiling(food.Template.Level / 2.0);
				base.Experience += num;
				base.Mood = 1;
				base.LastMeal = new System.DateTime?(System.DateTime.Now);
				base.Owner.Inventory.RefreshItem(this);
				result = true;
			}
			return result;
		}
		public void Dissociate()
		{
			System.Collections.Generic.List<EffectBase> list = new System.Collections.Generic.List<EffectBase>
			{
				base.MoodEffect,
				base.ExperienceEffect,
				base.CategoryEffect,
				base.SelectedLevelEffect,
				new EffectInteger(EffectsEnum.Effect_NonExchangeable_982, 0)
			};
			if (base.LastMealEffect != null)
			{
				list.Add(base.LastMealEffect);
			}
			BasePlayerItem item = Singleton<ItemManager>.Instance.CreatePlayerItem(base.Owner, this.m_livingObjectTemplate, 1u, list);
			this.Effects.RemoveAll(new System.Predicate<EffectBase>(list.Contains));
			this.Effects.RemoveAll((EffectBase x) => x.EffectId == EffectsEnum.Effect_LivingObjectId);
			base.Owner.Inventory.RefreshItemInstance(this);
			base.Owner.Inventory.AddItem(item);
			base.Owner.UpdateLook(true);
			base.OnObjectModified();
		}
	}
}
