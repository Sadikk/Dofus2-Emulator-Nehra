









// Generated on 07/24/2015 23:19:56
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MountReleasedMessage : Message
    {
        public const ushort Id = 6308;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int mountId;
        
        public MountReleasedMessage()
        {
        }
        
        public MountReleasedMessage(int mountId)
        {
            this.mountId = mountId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarInt(mountId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            mountId = reader.ReadVarInt();
        }
        
    }
    
}