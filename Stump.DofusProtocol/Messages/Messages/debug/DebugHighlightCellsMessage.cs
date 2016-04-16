









// Generated on 07/24/2015 23:19:45
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class DebugHighlightCellsMessage : Message
    {
        public const ushort Id = 2001;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int color;
        public IEnumerable<ushort> cells;
        
        public DebugHighlightCellsMessage()
        {
        }
        
        public DebugHighlightCellsMessage(int color, IEnumerable<ushort> cells)
        {
            this.color = color;
            this.cells = cells;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(color);
            writer.WriteUShort((ushort)cells.Count());
            foreach (var entry in cells)
            {
                 writer.WriteVarUhShort(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            color = reader.ReadInt();
            var limit = reader.ReadShort();
            cells = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (cells as ushort[])[i] = reader.ReadVarUhShort();
            }
        }
        
    }
    
}