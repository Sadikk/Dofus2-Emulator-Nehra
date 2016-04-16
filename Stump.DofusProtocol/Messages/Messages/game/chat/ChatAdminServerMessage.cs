









// Generated on 07/24/2015 23:19:52
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ChatAdminServerMessage : ChatServerMessage
    {
        public const ushort Id = 6135;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        
        public ChatAdminServerMessage()
        {
        }
        
        public ChatAdminServerMessage(sbyte channel, string content, int timestamp, string fingerprint, int senderId, string senderName, int senderAccountId)
         : base(channel, content, timestamp, fingerprint, senderId, senderName, senderAccountId)
        {
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
        }
        
    }
    
}