









// Generated on 07/24/2015 23:19:58
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class HouseKickRequestMessage : Message
    {
        public const ushort Id = 5698;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public uint id;
        
        public HouseKickRequestMessage()
        {
        }
        
        public HouseKickRequestMessage(uint id)
        {
            this.id = id;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhInt(id);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            id = reader.ReadVarUhInt();
            if (id < 0)
                throw new Exception("Forbidden value on id = " + id + ", it doesn't respect the following condition : id < 0");
        }
        
    }
    
}