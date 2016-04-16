









// Generated on 07/24/2015 23:20:05
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class IgnoredListMessage : Message
    {
        public const ushort Id = 5674;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.IgnoredInformations> ignoredList;
        
        public IgnoredListMessage()
        {
        }
        
        public IgnoredListMessage(IEnumerable<Types.IgnoredInformations> ignoredList)
        {
            this.ignoredList = ignoredList;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)ignoredList.Count());
            foreach (var entry in ignoredList)
            {
                 writer.WriteShort(entry.TypeId);
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            ignoredList = new Types.IgnoredInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (ignoredList as Types.IgnoredInformations[])[i] = Types.ProtocolTypeManager.GetInstance<Types.IgnoredInformations>(reader.ReadShort());
                 (ignoredList as Types.IgnoredInformations[])[i].Deserialize(reader);
            }
        }
        
    }
    
}