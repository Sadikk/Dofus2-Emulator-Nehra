









// Generated on 07/24/2015 23:20:14
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class InventoryPresetItemUpdateMessage : Message
    {
        public const ushort Id = 6168;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte presetId;
        public Types.PresetItem presetItem;
        
        public InventoryPresetItemUpdateMessage()
        {
        }
        
        public InventoryPresetItemUpdateMessage(sbyte presetId, Types.PresetItem presetItem)
        {
            this.presetId = presetId;
            this.presetItem = presetItem;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(presetId);
            presetItem.Serialize(writer);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            presetId = reader.ReadSByte();
            if (presetId < 0)
                throw new Exception("Forbidden value on presetId = " + presetId + ", it doesn't respect the following condition : presetId < 0");
            presetItem = new Types.PresetItem();
            presetItem.Deserialize(reader);
        }
        
    }
    
}