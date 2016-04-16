using Stump.Core.Cache;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using System;

namespace Stump.Server.WorldServer.Game.Actors.Look
{
	public class SubActorLook
	{
		private SubEntityBindingPointCategoryEnum m_bindingCategory;
		private sbyte m_bindingIndex;
		private ActorLook m_look;
		private readonly ObjectValidator<SubEntity> m_subEntity;
		public sbyte BindingIndex
		{
			get
			{
				return this.m_bindingIndex;
			}
			set
			{
				this.m_bindingIndex = value;
				this.m_subEntity.Invalidate();
			}
		}
		public SubEntityBindingPointCategoryEnum BindingCategory
		{
			get
			{
				return this.m_bindingCategory;
			}
			set
			{
				this.m_bindingCategory = value;
				this.m_subEntity.Invalidate();
			}
		}
		public ActorLook Look
		{
			get
			{
				return this.m_look;
			}
			private set
			{
				if (this.m_look != null)
				{
					this.m_look.EntityLookValidator.ObjectInvalidated -= new System.Action<ObjectValidator<EntityLook>>(this.OnLookInvalidated);
				}
				this.m_look = value;
				this.m_look.EntityLookValidator.ObjectInvalidated += new System.Action<ObjectValidator<EntityLook>>(this.OnLookInvalidated);
			}
		}
		public ObjectValidator<SubEntity> SubEntityValidator
		{
			get
			{
				return this.m_subEntity;
			}
		}
		public SubActorLook(sbyte index, SubEntityBindingPointCategoryEnum category, ActorLook look)
		{
			this.m_bindingIndex = index;
			this.m_bindingCategory = category;
			this.m_look = look;
			this.m_subEntity = new ObjectValidator<SubEntity>(new Func<SubEntity>(this.BuildSubEntity));
		}
		private void OnLookInvalidated(ObjectValidator<EntityLook> obj)
		{
			this.m_subEntity.Invalidate();
		}
		public override string ToString()
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append((sbyte)this.BindingCategory);
			stringBuilder.Append("@");
			stringBuilder.Append(this.BindingIndex);
			stringBuilder.Append("=");
			stringBuilder.Append(this.Look);
			return stringBuilder.ToString();
		}
		private SubEntity BuildSubEntity()
		{
			return new SubEntity((sbyte)this.BindingCategory, this.BindingIndex, this.Look.GetEntityLook());
		}
		public SubEntity GetSubEntity()
		{
			return this.m_subEntity;
		}
	}
}
