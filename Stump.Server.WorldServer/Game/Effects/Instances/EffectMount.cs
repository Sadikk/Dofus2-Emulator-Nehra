using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Types;

namespace Stump.Server.WorldServer.Game.Effects.Instances
{
	[System.Serializable]
	public class EffectMount : EffectBase
	{
		protected double m_date;
		protected short m_modelId;
		protected int m_mountId;
		public override int ProtocoleId
		{
			get
			{
				return 179;
			}
		}
		public override byte SerializationIdenfitier
		{
			get
			{
				return 9;
			}
		}
		public EffectMount()
		{
		}
		public EffectMount(EffectMount copy) : this(copy.Id, copy.m_mountId, copy.m_date, (int)copy.m_modelId, copy)
		{
		}
		public EffectMount(short id, int mountid, double date, int modelid, EffectBase effect) : base(id, effect)
		{
			this.m_mountId = mountid;
			this.m_date = date;
			this.m_modelId = (short)modelid;
		}
		public EffectMount(EffectInstanceMount effect) : base(effect)
		{
			this.m_mountId = (int)effect.mountId;
			this.m_date = effect.date;
			this.m_modelId = (short)effect.modelId;
		}
		public override object[] GetValues()
		{
			return new object[]
			{
				this.m_mountId,
				this.m_date,
				this.m_modelId
			};
		}
		public override ObjectEffect GetObjectEffect()
		{
            return new ObjectEffectMount((ushort)base.Id, this.m_mountId, this.m_date, (ushort)this.m_modelId);
		}
		public override EffectInstance GetEffectInstance()
		{
			return new EffectInstanceMount
			{
				effectId = (uint)base.Id,
				targetId = (int)base.Targets,
				delay = base.Delay,
				duration = base.Duration,
				group = base.Group,
				random = base.Random,
				modificator = base.Modificator,
				trigger = base.Trigger,
                visibleInTooltip = base.Hidden,
				zoneMinSize = base.ZoneMinSize,
				zoneSize = base.ZoneSize,
				zoneShape = (uint)base.ZoneShape,
				modelId = (uint)this.m_modelId,
                date = (float)(this.m_date),
				mountId = (uint)this.m_mountId
			};
		}
		public override EffectBase GenerateEffect(EffectGenerationContext context, EffectGenerationType type = EffectGenerationType.Normal)
		{
			return new EffectMount(this);
		}
		protected override void InternalSerialize(ref System.IO.BinaryWriter writer)
		{
			base.InternalSerialize(ref writer);
			writer.Write(this.m_mountId);
			writer.Write(this.m_date);
			writer.Write(this.m_modelId);
		}
		protected override void InternalDeserialize(ref System.IO.BinaryReader reader)
		{
			base.InternalDeserialize(ref reader);
			this.m_mountId = (int)reader.ReadInt16();
			this.m_date = (double)reader.ReadInt32();
			this.m_modelId = reader.ReadInt16();
		}
		public override bool Equals(object obj)
		{
			bool result;
			if (!(obj is EffectMount))
			{
				result = false;
			}
			else
			{
				EffectMount effectMount = obj as EffectMount;
				result = (base.Equals(obj) && this.m_mountId == effectMount.m_mountId && this.m_date == effectMount.m_date && this.m_modelId == effectMount.m_modelId);
			}
			return result;
		}
		public static bool operator ==(EffectMount a, EffectMount b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
		}
		public static bool operator !=(EffectMount a, EffectMount b)
		{
			return !(a == b);
		}
		public bool Equals(EffectMount other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (base.Equals(other) && other.m_date.Equals(this.m_date) && other.m_modelId == this.m_modelId && other.m_mountId == this.m_mountId));
		}
		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num = (num * 397 ^ this.m_date.GetHashCode());
			num = (num * 397 ^ (int)this.m_modelId);
			return num * 397 ^ this.m_mountId;
		}
	}
}
