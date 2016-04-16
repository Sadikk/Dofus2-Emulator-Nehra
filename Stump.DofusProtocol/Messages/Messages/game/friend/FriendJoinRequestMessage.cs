









// Generated on 07/24/2015 23:20:05
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class FriendJoinRequestMessage : Message
    {
        public const ushort Id = 5605;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string name;
        
        public FriendJoinRequestMessage()
        {
        }
        
        public FriendJoinRequestMessage(string name)
        {
            this.name = name;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(name);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            name = reader.ReadUTF();
        }
        
    }
    
}