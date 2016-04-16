









// Generated on 07/24/2015 23:19:45
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class NicknameChoiceRequestMessage : Message
    {
        public const ushort Id = 5639;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string nickname;
        
        public NicknameChoiceRequestMessage()
        {
        }
        
        public NicknameChoiceRequestMessage(string nickname)
        {
            this.nickname = nickname;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(nickname);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            nickname = reader.ReadUTF();
        }
        
    }
    
}