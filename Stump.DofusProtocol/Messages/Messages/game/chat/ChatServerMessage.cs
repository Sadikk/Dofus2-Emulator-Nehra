









// Generated on 07/24/2015 23:19:53
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ChatServerMessage : ChatAbstractServerMessage
    {
        public const ushort Id = 881;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int senderId;
        public string senderName;
        public int senderAccountId;
        
        public ChatServerMessage()
        {
        }
        
        public ChatServerMessage(sbyte channel, string content, int timestamp, string fingerprint, int senderId, string senderName, int senderAccountId)
         : base(channel, content, timestamp, fingerprint)
        {
            this.senderId = senderId;
            this.senderName = senderName;
            this.senderAccountId = senderAccountId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteInt(senderId);
            writer.WriteUTF(senderName);
            writer.WriteInt(senderAccountId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            senderId = reader.ReadInt();
            senderName = reader.ReadUTF();
            senderAccountId = reader.ReadInt();
            if (senderAccountId < 0)
                throw new Exception("Forbidden value on senderAccountId = " + senderAccountId + ", it doesn't respect the following condition : senderAccountId < 0");
        }
        
    }
    
}