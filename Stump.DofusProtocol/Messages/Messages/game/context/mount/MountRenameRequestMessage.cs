









// Generated on 07/24/2015 23:19:56
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class MountRenameRequestMessage : Message
    {
        public const ushort Id = 5987;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string name;
        public int mountId;
        
        public MountRenameRequestMessage()
        {
        }
        
        public MountRenameRequestMessage(string name, int mountId)
        {
            this.name = name;
            this.mountId = mountId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(name);
            writer.WriteVarInt(mountId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            name = reader.ReadUTF();
            mountId = reader.ReadVarInt();
        }
        
    }
    
}