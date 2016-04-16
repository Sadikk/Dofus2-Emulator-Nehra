









// Generated on 07/24/2015 23:20:13
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ObjectUseMessage : Message
    {
        public const ushort Id = 3019;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint objectUID;
        
        public ObjectUseMessage()
        {
        }
        
        public ObjectUseMessage(uint objectUID)
        {
            this.objectUID = objectUID;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(objectUID);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            objectUID = reader.ReadVarUhInt();
            if (objectUID < 0)
                throw new Exception("Forbidden value on objectUID = " + objectUID + ", it doesn't respect the following condition : objectUID < 0");
        }
        
    }
    
}