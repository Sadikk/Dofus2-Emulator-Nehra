









// Generated on 07/24/2015 23:20:05
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuestModeMessage : Message
    {
        public const ushort Id = 6505;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool active;
        
        public GuestModeMessage()
        {
        }
        
        public GuestModeMessage(bool active)
        {
            this.active = active;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(active);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            active = reader.ReadBoolean();
        }
        
    }
    
}