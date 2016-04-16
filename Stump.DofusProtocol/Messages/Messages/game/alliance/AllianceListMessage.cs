// Generated on 07/24/2015 23:19:50
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class AllianceListMessage : Message
    {
        public const ushort Id = 6408;
        public override ushort MessageId
        {
            get { return Id; }
        }

        public IEnumerable<Types.AllianceFactSheetInformations> alliances;

        public AllianceListMessage()
        {
        }

        public AllianceListMessage(IEnumerable<Types.AllianceFactSheetInformations> alliances)
        {
            this.alliances = alliances;
        }

        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)alliances.Count());
            foreach (var entry in alliances)
            {
                entry.Serialize(writer);
            }
        }

        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            alliances = new Types.AllianceFactSheetInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                (alliances as Types.AllianceFactSheetInformations[])[i] = new Types.AllianceFactSheetInformations();
                (alliances as Types.AllianceFactSheetInformations[])[i].Deserialize(reader);
            }
        }
    }
}