









// Generated on 07/24/2015 23:20:14
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class InventoryPresetDeleteMessage : Message
    {
        public const ushort Id = 6169;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte presetId;
        
        public InventoryPresetDeleteMessage()
        {
        }
        
        public InventoryPresetDeleteMessage(sbyte presetId)
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