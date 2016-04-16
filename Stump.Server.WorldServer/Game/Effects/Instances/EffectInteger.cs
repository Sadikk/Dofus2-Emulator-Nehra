using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;

namespace Stump.Server.WorldServer.Game.Effects.Instances
{
	[System.Serializable]
	public class EffectInteger : EffectBase
	{
		protected short m_value;
		public override int ProtocoleId
		{
			get
			{
				return 70;
			}
		}
		public override byte SerializationIdenfitier
		{
			get
			{
				return 6;
			}
		}
		public short Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
				base.IsDirty = true;
			}
		}
		public EffectInteger()
		{
		}
		public EffectInteger(EffectInteger copy) : this(copy.Id, copy.Value, copy)
		{
		}
		public EffectInteger(short id, short value, EffectBase effect) : base(id, effect)
		{
			this.m_value = value;
		}
		public EffectInteger(EffectsEnum id, short value) : this((short)id, value, new EffectBase())
		{
		}
		public EffectInteger(EffectInstanceInteger effect) : base(effect)
		{
			this.m_value = (short)effect.value;
		}
		public override object[] GetValues()
		{
			return new object[]
			{
				this.Value
			};
		}
		public override ObjectEffect GetObjectEffect()
		{
            return new ObjectEffectInteger((ushort)base.Id, (ushort)this.Value);
		}
		public override EffectBase GenerateEffect(EffectGenerationContext context, EffectGenerationType type = EffectGenerationType.Normal)
		{
			return new EffectInteger(this);
		}
		protected override void InternalSerialize(ref System.IO.BinaryWriter writer)
		{
			base.InternalSerialize(ref writer);
			writer.Write(this.m_value);
		}
		protected override void InternalDeserialize(ref System.IO.BinaryReader reader)
		{
			base.InternalDeserialize(ref reader);
			this.m_value = reader.ReadInt16();
		}
		public bool Equals(EffectInteger other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (base.Equals(other) && other.m_value == this.m_value));
		}
		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || this.Equals(obj as EffectInteger));
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() * 397 ^ this.m_value.GetHashCode();
		}
		public static bool operator ==(EffectInteger left, EffectInteger right)
		{
			return object.Equals(left, right);
		}
		public static bool operator !=(EffectInteger left, EffectInteger right)
		{
			return !object.Equals(left, right);
		}
	}
}
