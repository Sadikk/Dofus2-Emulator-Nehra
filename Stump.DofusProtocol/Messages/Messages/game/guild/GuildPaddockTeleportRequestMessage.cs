









// Generated on 07/24/2015 23:20:07
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GuildPaddockTeleportRequestMessage : Message
    {
        public const ushort Id = 5957;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int paddockId;
        
        public GuildPaddockTeleportRequestMessage()
        {
        }
        
        public GuildPaddockTeleportRequestMessage(int paddockId)
        {
            this.paddockId = paddockId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(paddockId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            paddockId = reader.ReadInt();
        }
        
    }
    
}