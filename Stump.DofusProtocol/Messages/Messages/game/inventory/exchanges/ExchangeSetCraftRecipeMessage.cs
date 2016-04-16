









// Generated on 07/24/2015 23:20:11
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeSetCraftRecipeMessage : Message
    {
        public const ushort Id = 6389;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort objectGID;
        
        public ExchangeSetCraftRecipeMessage()
        {
        }
        
        public ExchangeSetCraftRecipeMessage(ushort objectGID)
        {
            this.objectGID = objectGID;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(objectGID);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            objectGID = reader.ReadVarUhShort();
            if (objectGID < 0)
                throw new Exception("Forbidden value on objectGID = " + objectGID + ", it doesn't respect the following condition : objectGID < 0");
        }
        
    }
    
}