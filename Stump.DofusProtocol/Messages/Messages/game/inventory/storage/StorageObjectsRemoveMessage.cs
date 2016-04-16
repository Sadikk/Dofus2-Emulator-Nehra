









// Generated on 07/24/2015 23:20:14
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class StorageObjectsRemoveMessage : Message
    {
        public const ushort Id = 6035;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<uint> objectUIDList;
        
        public StorageObjectsRemoveMessage()
        {
        }
        
        public StorageObjectsRemoveMessage(IEnumerable<uint> objectUIDList)
        {
            this.objectUIDList = objectUIDList;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)objectUIDList.Count());
            foreach (var entry in objectUIDList)
            {
                 writer.WriteVarUhInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            objectUIDList = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (objectUIDList as uint[])[i] = reader.ReadVarUhInt();
            }
        }
        
    }
    
}