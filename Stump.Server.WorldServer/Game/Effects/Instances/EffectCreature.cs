using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Types;

namespace Stump.Server.WorldServer.Game.Effects.Instances
{
	[System.Serializable]
	public class EffectCreature : EffectBase
	{
		protected short m_monsterfamily;
		public override int ProtocoleId
		{
			get
			{
				return 71;
			}
		}
		public override byte SerializationIdenfitier
		{
			get
			{
				return 2;
			}
		}
		public short MonsterFamily
		{
			get
			{
				return this.m_monsterfamily;
			}
		}
		public EffectCreature()
		{
		}
		public EffectCreature(EffectCreature copy) : this(copy.Id, copy.MonsterFamily, copy)
		{
		}
		public EffectCreature(short id, short monsterfamily, EffectBase effectBase) : base(id, effectBase)
		{
			this.m_monsterfamily = monsterfamily;
		}
		public EffectCreature(EffectInstanceCreature effect) : base(effect)
		{
			this.m_monsterfamily = (short)effect.monsterFamilyId;
		}
		public override object[] GetValues()
		{
			return new object[]
			{
				this.m_monsterfamily
			};
		}
		public override ObjectEffect GetObjectEffect()
		{
            return new ObjectEffectCreature((ushort)base.Id, (ushort)this.MonsterFamily);
		}
		public override EffectBase GenerateEffect(EffectGenerationContext context, EffectGenerationType type = EffectGenerationType.Normal)
		{
			return new EffectCreature(this);
		}
		public override EffectInstance GetEffectInstance()
		{
			return new EffectInstanceCreature
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
				monsterFamilyId = (uint)this.MonsterFamily
			};
		}
		protected override void InternalSerialize(ref System.IO.BinaryWriter writer)
		{
			base.InternalSerialize(ref writer);
			writer.Write(this.MonsterFamily);
		}
		protected override void InternalDeserialize(ref System.IO.BinaryReader reader)
		{
			base.InternalDeserialize(ref reader);
			this.m_monsterfamily = reader.ReadInt16();
		}
		public override bool Equals(object obj)
		{
			return obj is EffectCreature && base.Equals(obj) && this.m_monsterfamily == (obj as EffectCreature).m_monsterfamily;
		}
		public static bool operator ==(EffectCreature a, EffectCreature b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
		}
		public static bool operator !=(EffectCreature a, EffectCreature b)
		{
			return !(a == b);
		}
		public bool Equals(EffectCreature other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (base.Equals(other) && other.m_monsterfamily == this.m_monsterfamily));
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() * 397 ^ this.m_monsterfamily.GetHashCode();
		}
	}
}
