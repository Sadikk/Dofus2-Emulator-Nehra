









// Generated on 07/24/2015 23:20:10
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeObjectTransfertListWithQuantityToInvMessage : Message
    {
        public const ushort Id = 6470;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<uint> ids;
        public IEnumerable<uint> qtys;
        
        public ExchangeObjectTransfertListWithQuantityToInvMessage()
        {
        }
        
        public ExchangeObjectTransfertListWithQuantityToInvMessage(IEnumerable<uint> ids, IEnumerable<uint> qtys)
        {
            this.ids = ids;
            this.qtys = qtys;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)ids.Count());
            foreach (var entry in ids)
            {
                 writer.WriteVarUhInt(entry);
            }
            writer.WriteUShort((ushort)qtys.Count());
            foreach (var entry in qtys)
            {
                 writer.WriteVarUhInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            ids = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (ids as uint[])[i] = reader.ReadVarUhInt();
            }
            limit = reader.ReadShort();
            qtys = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (qtys as uint[])[i] = reader.ReadVarUhInt();
            }
        }
        
    }
    
}