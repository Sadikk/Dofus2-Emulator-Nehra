









// Generated on 07/24/2015 23:20:11
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ExchangeStartOkJobIndexMessage : Message
    {
        public const ushort Id = 5819;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<uint> jobs;
        
        public ExchangeStartOkJobIndexMessage()
        {
        }
        
        public ExchangeStartOkJobIndexMessage(IEnumerable<uint> jobs)
        {
            this.jobs = jobs;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)jobs.Count());
            foreach (var entry in jobs)
            {
                 writer.WriteVarUhInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            jobs = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (jobs as uint[])[i] = reader.ReadVarUhInt();
            }
        }
        
    }
    
}