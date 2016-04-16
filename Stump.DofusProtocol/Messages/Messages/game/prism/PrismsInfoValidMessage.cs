









// Generated on 07/24/2015 23:20:15
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class PrismsInfoValidMessage : Message
    {
        public const ushort Id = 6451;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.PrismFightersInformation> fights;
        
        public PrismsInfoValidMessage()
        {
        }
        
        public PrismsInfoValidMessage(IEnumerable<Types.PrismFightersInformation> fights)
        {
            this.fights = fights;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)fights.Count());
            foreach (var entry in fights)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            fights = new Types.PrismFightersInformation[limit];
            for (int i = 0; i < limit; i++)
            {
                 (fights as Types.PrismFightersInformation[])[i] = new Types.PrismFightersInformation();
                 (fights as Types.PrismFightersInformation[])[i].Deserialize(reader);
            }
        }
        
    }
    
}