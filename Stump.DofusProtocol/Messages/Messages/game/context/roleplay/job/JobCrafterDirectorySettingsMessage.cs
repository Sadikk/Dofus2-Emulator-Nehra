









// Generated on 07/24/2015 23:19:59
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class JobCrafterDirectorySettingsMessage : Message
    {
        public const ushort Id = 5652;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.JobCrafterDirectorySettings> craftersSettings;
        
        public JobCrafterDirectorySettingsMessage()
        {
        }
        
        public JobCrafterDirectorySettingsMessage(IEnumerable<Types.JobCrafterDirectorySettings> craftersSettings)
        {
            this.craftersSettings = craftersSettings;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)craftersSettings.Count());
            foreach (var entry in craftersSettings)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            craftersSettings = new Types.JobCrafterDirectorySettings[limit];
            for (int i = 0; i < limit; i++)
            {
                 (craftersSettings as Types.JobCrafterDirectorySettings[])[i] = new Types.JobCrafterDirectorySettings();
                 (craftersSettings as Types.JobCrafterDirectorySettings[])[i].Deserialize(reader);
            }
        }
        
    }
    
}