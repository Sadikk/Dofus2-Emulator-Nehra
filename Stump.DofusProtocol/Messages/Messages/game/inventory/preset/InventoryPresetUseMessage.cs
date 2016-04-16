









// Generated on 07/24/2015 23:20:14
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class InventoryPresetUseMessage : Message
    {
        public const ushort Id = 6167;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte presetId;
        
        public InventoryPresetUseMessage()
        {
        }
        
        public InventoryPresetUseMessage(sbyte presetId)
        {
            this.presetId = presetId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(presetId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            presetId = reader.ReadSByte();
            if (presetId < 0)
                throw new Exception("Forbidden value on presetId = " + presetId + ", it doesn't respect the following condition : presetId < 0");
        }
        
    }
    
}