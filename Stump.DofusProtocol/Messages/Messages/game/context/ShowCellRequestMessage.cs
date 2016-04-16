









// Generated on 07/24/2015 23:19:54
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ShowCellRequestMessage : Message
    {
        public const ushort Id = 5611;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort cellId;
        
        public ShowCellRequestMessage()
        {
        }
        
        public ShowCellRequestMessage(ushort cellId)
        {
            this.cellId = cellId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(cellId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            cellId = reader.ReadVarUhShort();
            if (cellId < 0 || cellId > 559)
                throw new Exception("Forbidden value on cellId = " + cellId + ", it doesn't respect the following condition : cellId < 0 || cellId > 559");
        }
        
    }
    
}