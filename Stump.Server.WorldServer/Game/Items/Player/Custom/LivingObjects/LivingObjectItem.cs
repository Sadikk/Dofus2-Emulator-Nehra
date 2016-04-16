using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player.Custom.LivingObjects
{
	[ItemType(ItemTypeEnum.LIVING_OBJECTS)]
	public sealed class LivingObjectItem : CommonLivingObject
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		public override bool AllowDropping
		{
			get
			{
				return true;
			}
		}
		public LivingObjectItem(Character owner, PlayerItemRecord record) : base(owner, record)
		{
			base.LivingObjectRecord = Singleton<ItemManager>.Instance.TryGetLivingObjectRecord(this.Template.Id);
			if (base.LivingObjectRecord == null)
			{
				LivingObjectItem.logger.Error("Living Object {0} has no template", this.Template.Id);
			}
			else
			{
				this.Initialize();
			}
		}
		public override bool Drop(BasePlayerItem dropOnItem)
		{
			bool result;
			if ((ulong)dropOnItem.Template.TypeId != (ulong)((long)base.LivingObjectRecord.ItemType))
			{
				result = false;
			}
			else
			{
				if (dropOnItem.Effects.Any((EffectBase x) => x.EffectId == EffectsEnum.Effect_LivingObjectId))
				{
					result = false;
				}
				else
				{
					dropOnItem.Effects.Add(new EffectInteger(EffectsEnum.Effect_LivingObjectId, (short)this.Template.Id));
					using (System.Collections.Generic.List<EffectBase>.Enumerator enumerator = this.Effects.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							EffectBase effect = enumerator.Current;
							System.Collections.Generic.List<EffectBase> arg_B1_0 = dropOnItem.Effects;
							System.Predicate<EffectBase> match = (EffectBase x) => x.EffectId == effect.EffectId;
							arg_B1_0.RemoveAll(match);
							dropOnItem.Effects.Add(effect);
						}
					}
					base.Owner.Inventory.RefreshItemInstance(dropOnItem);
					base.Owner.UpdateLook(true);
					base.OnObjectModified();
					result = true;
				}
			}
			return result;
		}
	}
}
