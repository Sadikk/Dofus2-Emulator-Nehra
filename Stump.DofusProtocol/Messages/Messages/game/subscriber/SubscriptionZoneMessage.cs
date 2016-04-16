









// Generated on 07/24/2015 23:20:17
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SubscriptionZoneMessage : Message
    {
        public const ushort Id = 5573;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool active;
        
        public SubscriptionZoneMessage()
        {
        }
        
        public SubscriptionZoneMessage(bool active)
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