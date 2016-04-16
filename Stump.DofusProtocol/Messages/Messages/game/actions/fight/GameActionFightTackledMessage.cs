









// Generated on 07/24/2015 23:19:48
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameActionFightTackledMessage : AbstractGameActionMessage
    {
        public const ushort Id = 1004;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<int> tacklersIds;
        
        public GameActionFightTackledMessage()
        {
        }
        
        public GameActionFightTackledMessage(ushort actionId, int sourceId, IEnumerable<int> tacklersIds)
         : base(actionId, sourceId)
        {
            this.tacklersIds = tacklersIds;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteUShort((ushort)tacklersIds.Count());
            foreach (var entry in tacklersIds)
            {
                 writer.WriteInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            var limit = reader.ReadShort();
            tacklersIds = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                 (tacklersIds as int[])[i] = reader.ReadInt();
            }
        }
        
    }
    
}