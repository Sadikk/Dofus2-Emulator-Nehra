









// Generated on 07/24/2015 23:20:04
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SpellUpgradeRequestMessage : Message
    {
        public const ushort Id = 5608;
        public override ushort MessageId
        {
            get { return Id; }
        }

        public ushort spellId;
        public sbyte spellLevel;

        public SpellUpgradeRequestMessage()
        {
        }

        public SpellUpgradeRequestMessage(ushort spellId, sbyte spellLevel)
        {
            this.spellId = spellId;
            this.spellLevel = spellLevel;
        }

        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(spellId);
            writer.WriteSByte(spellLevel);
        }

        public override void Deserialize(ICustomDataInput reader)
        {
            spellId = reader.ReadVarUhShort();
            if (spellId < 0)
                throw new Exception("Forbidden value on spellId = " + spellId + ", it doesn't respect the following condition : spellId < 0");
            spellLevel = reader.ReadSByte();
            if (spellLevel < 1 || spellLevel > 6)
                throw new Exception("Forbidden value on spellLevel = " + spellLevel + ", it doesn't respect the following condition : spellLevel < 1 || spellLevel > 6");
        }
    }
}