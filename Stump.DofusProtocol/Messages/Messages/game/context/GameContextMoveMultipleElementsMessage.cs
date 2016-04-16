









// Generated on 07/24/2015 23:19:53
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameContextMoveMultipleElementsMessage : Message
    {
        public const ushort Id = 254;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.EntityMovementInformations> movements;
        
        public GameContextMoveMultipleElementsMessage()
        {
        }
        
        public GameContextMoveMultipleElementsMessage(IEnumerable<Types.EntityMovementInformations> movements)
        {
            this.movements = movements;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)movements.Count());
            foreach (var entry in movements)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            movements = new Types.EntityMovementInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (movements as Types.EntityMovementInformations[])[i] = new Types.EntityMovementInformations();
                 (movements as Types.EntityMovementInformations[])[i].Deserialize(reader);
            }
        }
        
    }
    
}