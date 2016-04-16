









// Generated on 07/24/2015 23:20:18
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ClientKeyMessage : Message
    {
        public const ushort Id = 5607;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string key;
        
        public ClientKeyMessage()
        {
        }
        
        public ClientKeyMessage(string key)
        {
            this.key = key;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(key);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            key = reader.ReadUTF();
        }
        
    }
    
}