









// Generated on 07/24/2015 23:19:50
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ReloginTokenStatusMessage : Message
    {
        public const ushort Id = 6539;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool validToken;
        public string token;
        
        public ReloginTokenStatusMessage()
        {
        }
        
        public ReloginTokenStatusMessage(bool validToken, string token)
        {
            this.validToken = validToken;
            this.token = token;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(validToken);
            writer.WriteUTF(token);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            validToken = reader.ReadBoolean();
            token = reader.ReadUTF();
        }
        
    }
    
}