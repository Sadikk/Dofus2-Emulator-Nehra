









// Generated on 07/24/2015 23:20:14
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SpellListMessage : Message
    {
        public const ushort Id = 1200;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool spellPrevisualization;
        public IEnumerable<Types.SpellItem> spells;
        
        public SpellListMessage()
        {
        }
        
        public SpellListMessage(bool spellPrevisualization, IEnumerable<Types.SpellItem> spells)
        {
            this.spellPrevisualization = spellPrevisualization;
            this.spells = spells;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(spellPrevisualization);
            writer.WriteUShort((ushort)spells.Count());
            foreach (var entry in spells)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            spellPrevisualization = reader.ReadBoolean();
            var limit = reader.ReadShort();
            spells = new Types.SpellItem[limit];
            for (int i = 0; i < limit; i++)
            {
                 (spells as Types.SpellItem[])[i] = new Types.SpellItem();
                 (spells as Types.SpellItem[])[i].Deserialize(reader);
            }
        }
        
    }
    
}