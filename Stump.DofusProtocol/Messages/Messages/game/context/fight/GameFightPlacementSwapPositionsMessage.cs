









// Generated on 07/24/2015 23:19:55
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightPlacementSwapPositionsMessage : Message
    {
        public const ushort Id = 6544;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.IdentifiedEntityDispositionInformations> dispositions;
        
        public GameFightPlacementSwapPositionsMessage()
        {
        }
        
        public GameFightPlacementSwapPositionsMessage(IEnumerable<Types.IdentifiedEntityDispositionInformations> dispositions)
        {
            this.dispositions = dispositions;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            foreach (var entry in dispositions)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            dispositions = new Types.IdentifiedEntityDispositionInformations[2];
            for (int i = 0; i < 2; i++)
            {
                 (dispositions as Types.IdentifiedEntityDispositionInformations[])[i] = new Types.IdentifiedEntityDispositionInformations();
                 (dispositions as Types.IdentifiedEntityDispositionInformations[])[i].Deserialize(reader);
            }
        }
        
    }
    
}