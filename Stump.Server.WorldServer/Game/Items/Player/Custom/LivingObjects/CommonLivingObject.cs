using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Instances;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Items.Player.Custom.LivingObjects
{
	public abstract class CommonLivingObject : BasePlayerItem
	{
		protected static short[] LevelsSteps = new short[]
		{
			0,
			10,
			21,
			33,
			46,
			60,
			75,
			91,
			108,
			126,
			145,
			165,
			186,
			208,
			231,
			255,
			280,
			306,
			333,
			361
		};
		private LivingObjectRecord m_record;
		private EffectInteger m_categoryEffect;
		private EffectInteger m_experienceEffect;
		private EffectInteger m_moodEffect;
		private EffectDate m_lastMealEffect;
		private EffectInteger m_selectedLevelEffect;
		protected LivingObjectRecord LivingObjectRecord
		{
			get
			{
				return this.m_record;
			}
			set
			{
				this.m_record = value;
			}
		}
		protected EffectInteger SelectedLevelEffect
		{
			get
			{
				return this.m_selectedLevelEffect;
			}
			set
			{
				this.m_selectedLevelEffect = value;
				base.OnObjectModified();
			}
		}
		protected EffectInteger CategoryEffect
		{
			get
			{
				return this.m_categoryEffect;
			}
			set
			{
				this.m_categoryEffect = value;
				base.OnObjectModified();
			}
		}
		protected EffectInteger ExperienceEffect
		{
			get
			{
				return this.m_experienceEffect;
			}
			set
			{
				this.m_experienceEffect = value;
				base.OnObjectModified();
			}
		}
		protected EffectInteger MoodEffect
		{
			get
			{
				return this.m_moodEffect;
			}
			set
			{
				this.m_moodEffect = value;
				base.OnObjectModified();
			}
		}
		protected EffectDate LastMealEffect
		{
			get
			{
				return this.m_lastMealEffect;
			}
			set
			{
				this.m_lastMealEffect = value;
				base.OnObjectModified();
			}
		}
		public short Mood
		{
			get
			{
				return this.m_moodEffect.Value;
			}
			set
			{
				this.m_moodEffect.Value = value;
				base.OnObjectModified();
				this.Invalidate();
			}
		}
		public short Experience
		{
			get
			{
				return this.m_experienceEffect.Value;
			}
			set
			{
				this.m_experienceEffect.Value = value;
				base.OnObjectModified();
				this.Invalidate();
			}
		}
		public short Level
		{
			get
			{
				short num = 1;
				while (CommonLivingObject.LevelsSteps.Length > (int)num && CommonLivingObject.LevelsSteps[(int)num] <= this.Experience)
				{
					num += 1;
				}
				return num;
			}
		}
		public short SelectedLevel
		{
			get
			{
				return this.m_selectedLevelEffect.Value;
			}
			set
			{
				if (value > 0 && value <= this.Level)
				{
					this.m_selectedLevelEffect.Value = value;
					this.Invalidate();
					base.Owner.Inventory.RefreshItem(this);
					base.Owner.UpdateLook(true);
					base.OnObjectModified();
				}
			}
		}
		public int IconId
		{
			get
			{
				int result = this.Template.IconId;
				if (this.m_record.Moods.Count > (int)this.Mood && this.m_record.Moods[(int)this.Mood].Count > (int)(this.SelectedLevel - 1))
				{
					result = this.m_record.Moods[(int)this.Mood][(int)(this.SelectedLevel - 1)];
				}
				return result;
			}
		}
		public override uint AppearanceId
		{
			get
			{
				uint result = this.Template.AppearanceId;
				if (this.SelectedLevel > 0 && this.m_record.Skins.Count > (int)(this.SelectedLevel - 1))
				{
					result = (uint)this.m_record.Skins[(int)(this.SelectedLevel - 1)];
				}
				return result;
			}
		}
		public System.DateTime? LastMeal
		{
			get
			{
				return (this.m_lastMealEffect != null) ? new System.DateTime?(this.m_lastMealEffect.GetDate()) : null;
			}
			set
			{
				if (!value.HasValue)
				{
					this.m_lastMealEffect = null;
					this.Effects.RemoveAll((EffectBase x) => x.EffectId == EffectsEnum.Effect_LastMeal);
				}
				else
				{
					if (this.m_lastMealEffect == null)
					{
						this.m_lastMealEffect = new EffectDate(EffectsEnum.Effect_LastMeal, value.Value);
						this.Effects.Add(this.m_lastMealEffect);
					}
					else
					{
						this.m_lastMealEffect.SetDate(value.Value);
					}
				}
				base.OnObjectModified();
				this.Invalidate();
			}
		}
		protected CommonLivingObject(Character owner, PlayerItemRecord record) : base(owner, record)
		{
		}
		protected virtual void Initialize()
		{
			if ((this.m_moodEffect = (EffectInteger)this.Effects.FirstOrDefault((EffectBase x) => x.EffectId == EffectsEnum.Effect_LivingObjectMood)) == null)
			{
				this.m_moodEffect = new EffectInteger(EffectsEnum.Effect_LivingObjectMood, 0);
				this.Effects.Add(this.m_moodEffect);
			}
			if ((this.m_selectedLevelEffect = (EffectInteger)this.Effects.FirstOrDefault((EffectBase x) => x.EffectId == EffectsEnum.Effect_LivingObjectSkin)) == null)
			{
				this.m_selectedLevelEffect = new EffectInteger(EffectsEnum.Effect_LivingObjectSkin, 1);
				this.Effects.Add(this.m_selectedLevelEffect);
			}
			if ((this.m_experienceEffect = (EffectInteger)this.Effects.FirstOrDefault((EffectBase x) => x.EffectId == EffectsEnum.Effect_LivingObjectLevel)) == null)
			{
				this.m_experienceEffect = new EffectInteger(EffectsEnum.Effect_LivingObjectLevel, 0);
				this.Effects.Add(this.m_experienceEffect);
			}
			if (!((this.m_categoryEffect = (EffectInteger)this.Effects.FirstOrDefault((EffectBase x) => x.EffectId == EffectsEnum.Effect_LivingObjectCategory)) != null))
			{
				this.m_categoryEffect = new EffectInteger(EffectsEnum.Effect_LivingObjectCategory, (short)this.m_record.ItemType);
				this.Effects.Add(this.m_categoryEffect);
				base.OnObjectModified();
			}
		}
	}
}
