using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Types;

namespace Stump.Server.WorldServer.Game.Effects.Instances
{
	[System.Serializable]
	public class EffectString : EffectBase
	{
		protected string m_value;
		public override int ProtocoleId
		{
			get
			{
				return 74;
			}
		}
		public override byte SerializationIdenfitier
		{
			get
			{
				return 10;
			}
		}
		public EffectString()
		{
		}
		public EffectString(EffectString copy) : this(copy.Id, copy.m_value, copy)
		{
		}
		public EffectString(short id, string value, EffectBase effect) : base(id, effect)
		{
			this.m_value = value;
		}
		public EffectString(EffectInstanceString effect) : base(effect)
		{
			this.m_value = effect.text;
		}
		public override object[] GetValues()
		{
			return new object[]
			{
				this.m_value
			};
		}
		public override ObjectEffect GetObjectEffect()
		{
            return new ObjectEffectString((ushort)base.Id, this.m_value);
		}
		public override EffectInstance GetEffectInstance()
		{
			return new EffectInstanceString
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
				text = this.m_value
			};
		}
		public override EffectBase GenerateEffect(EffectGenerationContext context, EffectGenerationType type = EffectGenerationType.Normal)
		{
			return new EffectString(this);
		}
		protected override void InternalSerialize(ref System.IO.BinaryWriter writer)
		{
			base.InternalSerialize(ref writer);
			writer.Write(this.m_value);
		}
		protected override void InternalDeserialize(ref System.IO.BinaryReader reader)
		{
			base.InternalDeserialize(ref reader);
			this.m_value = reader.ReadString();
		}
		public override bool Equals(object obj)
		{
			return obj is EffectString && base.Equals(obj) && this.m_value == (obj as EffectString).m_value;
		}
		public static bool operator ==(EffectString a, EffectString b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
		}
		public static bool operator !=(EffectString a, EffectString b)
		{
			return !(a == b);
		}
		public bool Equals(EffectString other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (base.Equals(other) && object.Equals(other.m_value, this.m_value)));
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() * 397 ^ ((this.m_value != null) ? this.m_value.GetHashCode() : 0);
		}
	}
}
