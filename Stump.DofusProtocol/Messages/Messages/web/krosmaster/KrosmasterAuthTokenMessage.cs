









// Generated on 07/24/2015 23:20:18
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class KrosmasterAuthTokenMessage : Message
    {
        public const ushort Id = 6351;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string token;
        
        public KrosmasterAuthTokenMessage()
        {
        }
        
        public KrosmasterAuthTokenMessage(string token)
        {
            this.token = token;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(token);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            token = reader.ReadUTF();
        }
        
    }
    
}