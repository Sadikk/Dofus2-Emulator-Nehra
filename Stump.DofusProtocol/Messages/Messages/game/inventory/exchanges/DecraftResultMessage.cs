









// Generated on 07/24/2015 23:20:08
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class DecraftResultMessage : Message
    {
        public const ushort Id = 6569;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.DecraftedItemStackInfo> results;
        
        public DecraftResultMessage()
        {
        }
        
        public DecraftResultMessage(IEnumerable<Types.DecraftedItemStackInfo> results)
        {
            this.results = results;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)results.Count());
            foreach (var entry in results)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            results = new Types.DecraftedItemStackInfo[limit];
            for (int i = 0; i < limit; i++)
            {
                 (results as Types.DecraftedItemStackInfo[])[i] = new Types.DecraftedItemStackInfo();
                 (results as Types.DecraftedItemStackInfo[])[i].Deserialize(reader);
            }
        }
        
    }
    
}