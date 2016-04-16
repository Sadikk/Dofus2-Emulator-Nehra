









// Generated on 07/24/2015 23:20:12
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeTypesExchangerDescriptionForUserMessage : Message
    {
        public const ushort Id = 5765;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<uint> typeDescription;
        
        public ExchangeTypesExchangerDescriptionForUserMessage()
        {
        }
        
        public ExchangeTypesExchangerDescriptionForUserMessage(IEnumerable<uint> typeDescription)
        {
            this.typeDescription = typeDescription;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)typeDescription.Count());
            foreach (var entry in typeDescription)
            {
                 writer.WriteVarUhInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            typeDescription = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (typeDescription as uint[])[i] = reader.ReadVarUhInt();
            }
        }
        
    }
    
}