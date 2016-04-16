









// Generated on 07/24/2015 23:20:03
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class PartyCompanionUpdateLightMessage : PartyUpdateLightMessage
    {
        public const ushort Id = 6472;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte indexId;
        
        public PartyCompanionUpdateLightMessage()
        {
        }
        
        public PartyCompanionUpdateLightMessage(uint partyId, uint id, uint lifePoints, uint maxLifePoints, ushort prospecting, byte regenRate, sbyte indexId)
         : base(partyId, id, lifePoints, maxLifePoints, prospecting, regenRate)
        {
            this.indexId = indexId;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteSByte(indexId);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            indexId = reader.ReadSByte();
            if (indexId < 0)
                throw new Exception("Forbidden value on indexId = " + indexId + ", it doesn't respect the following condition : indexId < 0");
        }
        
    }
    
}