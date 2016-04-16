









// Generated on 07/24/2015 23:19:52
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ChatAbstractClientMessage : Message
    {
        public const ushort Id = 850;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string content;
        
        public ChatAbstractClientMessage()
        {
        }
        
        public ChatAbstractClientMessage(string content)
        {
            this.content = content;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(content);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            content = reader.ReadUTF();
        }
        
    }
    
}