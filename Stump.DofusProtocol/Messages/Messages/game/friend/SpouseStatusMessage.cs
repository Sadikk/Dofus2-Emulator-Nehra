









// Generated on 07/24/2015 23:20:05
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SpouseStatusMessage : Message
    {
        public const ushort Id = 6265;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool hasSpouse;
        
        public SpouseStatusMessage()
        {
        }
        
        public SpouseStatusMessage(bool hasSpouse)
        {
            this.hasSpouse = hasSpouse;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(hasSpouse);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            hasSpouse = reader.ReadBoolean();
        }
        
    }
    
}