









// Generated on 07/24/2015 23:19:55
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ChallengeTargetsListMessage : Message
    {
        public const ushort Id = 5613;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<int> targetIds;
        public IEnumerable<short> targetCells;
        
        public ChallengeTargetsListMessage()
        {
        }
        
        public ChallengeTargetsListMessage(IEnumerable<int> targetIds, IEnumerable<short> targetCells)
        {
            this.targetIds = targetIds;
            this.targetCells = targetCells;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)targetIds.Count());
            foreach (var entry in targetIds)
            {
                 writer.WriteInt(entry);
            }
            writer.WriteUShort((ushort)targetCells.Count());
            foreach (var entry in targetCells)
            {
                 writer.WriteShort(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            targetIds = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                 (targetIds as int[])[i] = reader.ReadInt();
            }
            limit = reader.ReadShort();
            targetCells = new short[limit];
            for (int i = 0; i < limit; i++)
            {
                 (targetCells as short[])[i] = reader.ReadShort();
            }
        }
        
    }
    
}