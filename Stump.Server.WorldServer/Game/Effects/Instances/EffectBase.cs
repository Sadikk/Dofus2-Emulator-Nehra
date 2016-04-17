using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Effects;

namespace Stump.Server.WorldServer.Game.Effects.Instances
{
	[System.Serializable]
	public class EffectBase : System.ICloneable
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private int m_delay;
		private int m_duration;
		private int m_group;
		private bool m_hidden;
		private short m_id;
		private int m_modificator;
		private int m_random;
		private SpellTargetType m_targets;
        private string m_targetMask;

		[System.NonSerialized]
		protected EffectTemplate m_template;
		private bool m_trigger;
		private uint m_zoneMinSize;
		private SpellShapeEnum m_zoneShape;
		private uint m_zoneSize;
		public virtual int ProtocoleId
		{
			get
			{
				return 76;
			}
		}
		public virtual byte SerializationIdenfitier
		{
			get
			{
				return 1;
			}
		}
		public short Id
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
				this.IsDirty = true;
			}
		}
		public EffectsEnum EffectId
		{
			get
			{
				return (EffectsEnum)this.Id;
			}
			set
			{
				this.Id = (short)value;
				this.IsDirty = true;
			}
		}
		public EffectTemplate Template
		{
			get
			{
				EffectTemplate arg_23_0;
				if ((arg_23_0 = this.m_template) == null)
				{
					arg_23_0 = (this.m_template = Singleton<EffectManager>.Instance.GetTemplate(this.Id));
				}
				return arg_23_0;
			}
			protected set
			{
				this.m_template = value;
				this.IsDirty = true;
			}
		}
		public SpellTargetType Targets
		{
			get
			{
				return this.m_targets;
			}
			set
			{
				this.m_targets = value;
				this.IsDirty = true;
			}
		}
		public int Duration
		{
			get
			{
				return this.m_duration;
			}
			set
			{
				this.m_duration = value;
				this.IsDirty = true;
			}
		}
		public int Delay
		{
			get
			{
				return this.m_delay;
			}
			set
			{
				this.m_delay = value;
				this.IsDirty = true;
			}
		}
		public int Random
		{
			get
			{
				return this.m_random;
			}
			set
			{
				this.m_random = value;
				this.IsDirty = true;
			}
		}
		public int Group
		{
			get
			{
				return this.m_group;
			}
			set
			{
				this.m_group = value;
				this.IsDirty = true;
			}
		}
		public int Modificator
		{
			get
			{
				return this.m_modificator;
			}
			set
			{
				this.m_modificator = value;
				this.IsDirty = true;
			}
		}
		public bool Trigger
		{
			get
			{
				return this.m_trigger;
			}
			set
			{
				this.m_trigger = value;
				this.IsDirty = true;
			}
		}
		public bool Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
				this.IsDirty = true;
			}
		}
		public uint ZoneSize
		{
			get
			{
				return (uint)((this.m_zoneSize >= 63u) ? 63 : ((byte)this.m_zoneSize));
			}
			set
			{
				this.m_zoneSize = value;
				this.IsDirty = true;
			}
		}
		public SpellShapeEnum ZoneShape
		{
			get
			{
				return this.m_zoneShape;
			}
			set
			{
				this.m_zoneShape = value;
				this.IsDirty = true;
			}
		}
		public uint ZoneMinSize
		{
			get
			{
				return (uint)((this.m_zoneMinSize >= 63u) ? 63 : ((byte)this.m_zoneMinSize));
			}
			set
			{
				this.m_zoneMinSize = value;
				this.IsDirty = true;
			}
		}
		public bool IsDirty
		{
			get;
			set;
		}
        public string TargetMask
        {
            get
            {
                return this.m_targetMask;
            }
            set
            {
                this.m_targetMask = value;
                this.IsDirty = true;
            }
        }

		public EffectBase()
		{
		}
		public EffectBase(EffectBase effect)
		{
			this.m_id = effect.Id;
			this.m_template = Singleton<EffectManager>.Instance.GetTemplate(effect.Id);
			this.m_targets = effect.Targets;
            this.m_targetMask = effect.TargetMask;
			this.m_delay = effect.Delay;
			this.m_duration = effect.Duration;
			this.m_group = effect.Group;
			this.m_random = effect.Random;
			this.m_modificator = effect.Modificator;
			this.m_trigger = effect.Trigger;
			this.m_hidden = effect.Hidden;
			this.m_zoneSize = effect.m_zoneSize;
			this.m_zoneMinSize = effect.m_zoneMinSize;
			this.m_zoneShape = effect.ZoneShape;
		}
		public EffectBase(short id, EffectBase effect)
		{
			this.m_id = id;
			this.m_template = Singleton<EffectManager>.Instance.GetTemplate(id);
			this.m_targets = effect.Targets;
            this.m_targetMask = effect.TargetMask;
            this.m_delay = effect.Delay;
			this.m_duration = effect.Duration;
			this.m_group = effect.Group;
			this.m_random = effect.Random;
			this.m_modificator = effect.Modificator;
			this.m_trigger = effect.Trigger;
			this.m_hidden = effect.Hidden;
			this.m_zoneSize = effect.m_zoneSize;
			this.m_zoneMinSize = effect.m_zoneMinSize;
			this.m_zoneShape = effect.ZoneShape;
		}
		public EffectBase(EffectInstance effect)
		{
			this.m_id = (short)effect.effectId;
			this.m_template = Singleton<EffectManager>.Instance.GetTemplate(this.Id);
			this.m_targets = (SpellTargetType)effect.targetId;
            this.m_targetMask = effect.targetMask;
            this.m_delay = effect.delay;
			this.m_duration = effect.duration;
			this.m_group = effect.group;
			this.m_random = effect.random;
			this.m_modificator = effect.modificator;
			this.m_trigger = effect.trigger;
            this.m_hidden = effect.visibleInTooltip;
			this.ParseRawZone(effect.rawZone);
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
		protected void ParseRawZone(string rawZone)
		{
			if (string.IsNullOrEmpty(rawZone))
			{
				this.m_zoneShape = (SpellShapeEnum)0;
				this.m_zoneSize = 0u;
				this.m_zoneMinSize = 0u;
			}
			else
			{
				SpellShapeEnum zoneShape = (SpellShapeEnum)rawZone[0];
				byte zoneSize = 0;
				byte zoneMinSize = 0;
				int num = rawZone.IndexOf(',');
				try
				{
					if (num == -1 && rawZone.Length > 1)
					{
						zoneSize = byte.Parse(rawZone.Remove(0, 1));
					}
					else
					{
						if (rawZone.Length > 1)
						{
							zoneSize = byte.Parse(rawZone.Substring(1, num - 1));
							zoneMinSize = byte.Parse(rawZone.Remove(0, num + 1));
						}
					}
				}
				catch (System.Exception)
				{
					this.m_zoneShape = (SpellShapeEnum)0;
					this.m_zoneSize = 0u;
					this.m_zoneMinSize = 0u;
					EffectBase.logger.Error("ParseRawZone() => Cannot parse {0}", rawZone);
				}
				this.m_zoneShape = zoneShape;
				this.m_zoneSize = (uint)zoneSize;
				this.m_zoneMinSize = (uint)zoneMinSize;
			}
		}
		protected string BuildRawZone()
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append((char)this.ZoneShape);
			stringBuilder.Append(this.ZoneSize);
			if (this.ZoneMinSize > 0u)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(this.ZoneMinSize);
			}
			return stringBuilder.ToString();
		}
		public virtual object[] GetValues()
		{
			return new object[0];
		}
		public virtual EffectBase GenerateEffect(EffectGenerationContext context, EffectGenerationType type = EffectGenerationType.Normal)
		{
			return new EffectBase(this);
		}
		public virtual ObjectEffect GetObjectEffect()
		{
            return new ObjectEffect((ushort)this.Id);
		}
		public virtual EffectInstance GetEffectInstance()
		{
			return new EffectInstance
			{
				effectId = (uint)this.Id,
				targetId = (int)this.Targets,
				delay = this.Delay,
				duration = this.Duration,
				group = this.Group,
				random = this.Random,
				modificator = this.Modificator,
				trigger = this.Trigger,
                visibleInTooltip = this.Hidden,
				zoneMinSize = this.ZoneMinSize,
				zoneSize = this.ZoneSize,
				zoneShape = (uint)this.ZoneShape
			};
		}

		public byte[] Serialize()
		{
			System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(new System.IO.MemoryStream());
			binaryWriter.Write(this.SerializationIdenfitier);
			this.InternalSerialize(ref binaryWriter);
			return ((System.IO.MemoryStream)binaryWriter.BaseStream).ToArray();
		}
		protected virtual void InternalSerialize(ref System.IO.BinaryWriter writer)
		{
			if (this.Targets == SpellTargetType.NONE && this.Duration == 0 && this.Delay == 0 && this.Random == 0 && this.Group == 0 && this.Modificator == 0 && !this.Trigger && !this.Hidden && this.ZoneSize == 0u && this.ZoneShape == (SpellShapeEnum)0)
			{
				writer.Write('C');
				writer.Write(this.Id);
			}
			else
			{
				writer.Write((int)this.Targets);
				writer.Write(this.Id);
				writer.Write(this.Duration);
				writer.Write(this.Delay);
				writer.Write(this.Random);
				writer.Write(this.Group);
				writer.Write(this.Modificator);
				writer.Write(this.Trigger);
				writer.Write(this.Hidden);
				string text = this.BuildRawZone();
				if (text == null)
				{
					writer.Write(string.Empty);
				}
				else
				{
					writer.Write(text);
				}
			}
		}

		internal void DeSerialize(byte[] buffer, ref int index)
		{
			System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(new System.IO.MemoryStream(buffer, index, buffer.Length - index));
			this.InternalDeserialize(ref binaryReader);
			index += (int)binaryReader.BaseStream.Position;
		}
		protected virtual void InternalDeserialize(ref System.IO.BinaryReader reader)
		{
			if (reader.PeekChar() == 67)
			{
				reader.ReadChar();
				this.m_id = reader.ReadInt16();
			}
			else
			{
				this.m_targets = (SpellTargetType)reader.ReadInt32();
                this.m_targetMask = reader.ReadString();
				this.m_id = reader.ReadInt16();
				this.m_duration = reader.ReadInt32();
				this.m_delay = reader.ReadInt32();
				this.m_random = reader.ReadInt32();
				this.m_group = reader.ReadInt32();
				this.m_modificator = reader.ReadInt32();
				this.m_trigger = reader.ReadBoolean();
				this.m_hidden = reader.ReadBoolean();
				this.ParseRawZone(reader.ReadString());
			}
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != typeof(EffectBase)) && this.Equals((EffectBase)obj)));
		}
		public static bool operator ==(EffectBase left, EffectBase right)
		{
			return object.Equals(left, right);
		}
		public static bool operator !=(EffectBase left, EffectBase right)
		{
			return !object.Equals(left, right);
		}
		public bool Equals(EffectBase other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || other.Id == this.Id);
		}
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}
	}
}
