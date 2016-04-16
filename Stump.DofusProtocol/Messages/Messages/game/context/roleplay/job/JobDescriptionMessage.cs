









// Generated on 07/24/2015 23:19:59
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class JobDescriptionMessage : Message
    {
        public const ushort Id = 5655;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.JobDescription> jobsDescription;
        
        public JobDescriptionMessage()
        {
        }
        
        public JobDescriptionMessage(IEnumerable<Types.JobDescription> jobsDescription)
        {
            this.jobsDescription = jobsDescription;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)jobsDescription.Count());
            foreach (var entry in jobsDescription)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            jobsDescription = new Types.JobDescription[limit];
            for (int i = 0; i < limit; i++)
            {
                 (jobsDescription as Types.JobDescription[])[i] = new Types.JobDescription();
                 (jobsDescription as Types.JobDescription[])[i].Deserialize(reader);
            }
        }
        
    }
    
}