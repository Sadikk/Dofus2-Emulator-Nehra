









// Generated on 07/24/2015 23:19:54
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightPlacementSwapPositionsCancelledMessage : Message
    {
        public const ushort Id = 6546;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public int requestId;
        public uint cancellerId;
        
        public GameFightPlacementSwapPositionsCancelledMessage()
        {
        }
        
        public GameFightPlacementSwapPositionsCancelledMessage(int requestId, uint cancellerId)
        {
            this.requestId = requestId;
            this.cancellerId = cancellerId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(requestId);
            writer.WriteVarUhInt(cancellerId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            requestId = reader.ReadInt();
            if (requestId < 0)
                throw new Exception("Forbidden value on requestId = " + requestId + ", it doesn't respect the following condition : requestId < 0");
            cancellerId = reader.ReadVarUhInt();
            if (cancellerId < 0)
                throw new Exception("Forbidden value on cancellerId = " + cancellerId + ", it doesn't respect the following condition : cancellerId < 0");
        }
        
    }
    
}