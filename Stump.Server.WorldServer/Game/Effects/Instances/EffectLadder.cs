using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Types;

namespace Stump.Server.WorldServer.Game.Effects.Instances
{
	[System.Serializable]
	public class EffectLadder : EffectCreature
	{
		protected short m_monsterCount;
		public short MonsterCount
		{
			get
			{
				return this.m_monsterCount;
			}
			set
			{
				this.m_monsterCount = value;
				base.IsDirty = true;
			}
		}
		public override int ProtocoleId
		{
			get
			{
				return 81;
			}
		}
		public override byte SerializationIdenfitier
		{
			get
			{
				return 7;
			}
		}
		public EffectLadder()
		{
		}
		public EffectLadder(EffectLadder copy) : this(copy.Id, copy.MonsterFamily, copy.MonsterCount, copy)
		{
		}
		public EffectLadder(short id, short monsterfamily, short monstercount, EffectBase effect) : base(id, monsterfamily, effect)
		{
			this.m_monsterCount = monstercount;
		}
		public EffectLadder(EffectInstanceLadder effect) : base(effect)
		{
			this.m_monsterCount = (short)effect.monsterCount;
		}
		public override object[] GetValues()
		{
			return new object[]
			{
				this.m_monsterCount,
				this.m_monsterfamily
			};
		}
		public override ObjectEffect GetObjectEffect()
		{
            return new ObjectEffectLadder((ushort)base.Id, (ushort)base.MonsterFamily, (uint)this.MonsterCount);
		}
		public override EffectInstance GetEffectInstance()
		{
			return new EffectInstanceLadder
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
				monsterCount = (uint)this.m_monsterCount,
				monsterFamilyId = (uint)this.m_monsterfamily
			};
		}
		public override EffectBase GenerateEffect(EffectGenerationContext context, EffectGenerationType type = EffectGenerationType.Normal)
		{
			return new EffectLadder(this);
		}
		protected override void InternalSerialize(ref System.IO.BinaryWriter writer)
		{
			base.InternalSerialize(ref writer);
			writer.Write(this.m_monsterCount);
		}
		protected override void InternalDeserialize(ref System.IO.BinaryReader reader)
		{
			base.InternalDeserialize(ref reader);
			this.m_monsterCount = reader.ReadInt16();
		}
		public override bool Equals(object obj)
		{
			return obj is EffectLadder && base.Equals(obj) && this.m_monsterCount == (obj as EffectLadder).m_monsterCount;
		}
		public static bool operator ==(EffectLadder a, EffectLadder b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
		}
		public static bool operator !=(EffectLadder a, EffectLadder b)
		{
			return !(a == b);
		}
		public bool Equals(EffectLadder other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (base.Equals(other) && other.m_monsterCount == this.m_monsterCount));
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() * 397 ^ this.m_monsterCount.GetHashCode();
		}
	}
}
