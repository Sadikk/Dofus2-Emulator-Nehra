









// Generated on 07/24/2015 23:20:10
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeMountsStableAddMessage : Message
    {
        public const ushort Id = 6555;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.MountClientData> mountDescription;
        
        public ExchangeMountsStableAddMessage()
        {
        }
        
        public ExchangeMountsStableAddMessage(IEnumerable<Types.MountClientData> mountDescription)
        {
            this.mountDescription = mountDescription;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)mountDescription.Count());
            foreach (var entry in mountDescription)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            mountDescription = new Types.MountClientData[limit];
            for (int i = 0; i < limit; i++)
            {
                 (mountDescription as Types.MountClientData[])[i] = new Types.MountClientData();
                 (mountDescription as Types.MountClientData[])[i].Deserialize(reader);
            }
        }
        
    }
    
}