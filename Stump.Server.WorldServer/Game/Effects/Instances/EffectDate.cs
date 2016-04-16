using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;

namespace Stump.Server.WorldServer.Game.Effects.Instances
{
	[System.Serializable]
	public class EffectDate : EffectBase
	{
		protected short m_day;
		protected short m_hour;
		protected short m_minute;
		protected short m_month;
		protected short m_year;
		public override int ProtocoleId
		{
			get
			{
				return 72;
			}
		}
		public override byte SerializationIdenfitier
		{
			get
			{
				return 3;
			}
		}
		public EffectDate()
		{
		}
		public EffectDate(EffectDate copy) : this(copy.Id, copy.m_year, copy.m_month, copy.m_day, copy.m_hour, copy.m_minute, copy)
		{
		}
		public EffectDate(short id, short year, short month, short day, short hour, short minute, EffectBase effect) : base(id, effect)
		{
			this.m_year = year;
			this.m_month = month;
			this.m_day = day;
			this.m_hour = hour;
			this.m_minute = minute;
		}
		public EffectDate(EffectInstanceDate effect) : base(effect)
		{
			this.m_year = (short)effect.year;
			this.m_month = (short)effect.month;
			this.m_day = (short)effect.day;
			this.m_hour = (short)effect.hour;
			this.m_minute = (short)effect.minute;
		}
		public EffectDate(EffectsEnum id, System.DateTime date) : this((short)id, (short)date.Year, (short)date.Month, (short)date.Day, (short)date.Hour, (short)date.Minute, new EffectBase())
		{
		}
		public override object[] GetValues()
		{
			return new object[]
			{
				this.m_year.ToString(),
				this.m_month.ToString("00") + this.m_day.ToString("00"),
				this.m_hour.ToString("00") + this.m_minute.ToString("00")
			};
		}
		public override EffectBase GenerateEffect(EffectGenerationContext context, EffectGenerationType type = EffectGenerationType.Normal)
		{
			return new EffectDate(this);
		}
		public System.DateTime GetDate()
		{
			return new System.DateTime((int)this.m_year, (int)this.m_month, (int)this.m_day, (int)this.m_hour, (int)this.m_minute, 0);
		}
		public void SetDate(System.DateTime date)
		{
			this.m_year = (short)date.Year;
			this.m_month = (short)date.Month;
			this.m_day = (short)date.Day;
			this.m_hour = (short)date.Hour;
			this.m_minute = (short)date.Minute;
		}
		public override ObjectEffect GetObjectEffect()
		{
            return new ObjectEffectDate((ushort)base.Id, (ushort)this.m_year, (sbyte)this.m_month, (sbyte)this.m_day, (sbyte)this.m_hour, (sbyte)this.m_minute);
		}
		public override EffectInstance GetEffectInstance()
		{
			return new EffectInstanceDate
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
				year = (uint)this.m_year,
				month = (uint)this.m_month,
				day = (uint)this.m_day,
				hour = (uint)this.m_hour,
				minute = (uint)this.m_minute
			};
		}
		protected override void InternalSerialize(ref System.IO.BinaryWriter writer)
		{
			base.InternalSerialize(ref writer);
			writer.Write(this.m_year);
			writer.Write(this.m_month);
			writer.Write(this.m_day);
			writer.Write(this.m_hour);
			writer.Write(this.m_minute);
		}
		protected override void InternalDeserialize(ref System.IO.BinaryReader reader)
		{
			base.InternalDeserialize(ref reader);
			this.m_year = reader.ReadInt16();
			this.m_month = reader.ReadInt16();
			this.m_day = reader.ReadInt16();
			this.m_hour = reader.ReadInt16();
			this.m_minute = reader.ReadInt16();
		}
		public override bool Equals(object obj)
		{
			return obj is EffectDate && base.Equals(obj) && this.GetDate().Equals((obj as EffectDate).GetDate());
		}
		public static bool operator ==(EffectDate a, EffectDate b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
		}
		public static bool operator !=(EffectDate a, EffectDate b)
		{
            return !(a != null && b != null && a.Equals(b));
		}
		public bool Equals(EffectDate other)
		{
			return (base.Equals(other) && other.m_day == this.m_day && other.m_hour == this.m_hour && other.m_minute == this.m_minute && other.m_month == this.m_month && other.m_year == this.m_year);
		}
		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num = (num * 397 ^ this.m_day.GetHashCode());
			num = (num * 397 ^ this.m_hour.GetHashCode());
			num = (num * 397 ^ this.m_minute.GetHashCode());
			num = (num * 397 ^ this.m_month.GetHashCode());
			return num * 397 ^ this.m_year.GetHashCode();
		}
	}
}
