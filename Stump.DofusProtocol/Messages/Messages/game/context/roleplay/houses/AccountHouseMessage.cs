









// Generated on 07/24/2015 23:19:58
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class AccountHouseMessage : Message
    {
        public const ushort Id = 6315;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.AccountHouseInformations> houses;
        
        public AccountHouseMessage()
        {
        }
        
        public AccountHouseMessage(IEnumerable<Types.AccountHouseInformations> houses)
        {
            this.houses = houses;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)houses.Count());
            foreach (var entry in houses)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            houses = new Types.AccountHouseInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (houses as Types.AccountHouseInformations[])[i] = new Types.AccountHouseInformations();
                 (houses as Types.AccountHouseInformations[])[i].Deserialize(reader);
            }
        }
        
    }
    
}