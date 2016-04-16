









// Generated on 07/24/2015 23:19:54
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightPlacementPossiblePositionsMessage : Message
    {
        public const ushort Id = 703;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<ushort> positionsForChallengers;
        public IEnumerable<ushort> positionsForDefenders;
        public sbyte teamNumber;
        
        public GameFightPlacementPossiblePositionsMessage()
        {
        }
        
        public GameFightPlacementPossiblePositionsMessage(IEnumerable<ushort> positionsForChallengers, IEnumerable<ushort> positionsForDefenders, sbyte teamNumber)
        {
            this.positionsForChallengers = positionsForChallengers;
            this.positionsForDefenders = positionsForDefenders;
            this.teamNumber = teamNumber;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)positionsForChallengers.Count());
            foreach (var entry in positionsForChallengers)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteUShort((ushort)positionsForDefenders.Count());
            foreach (var entry in positionsForDefenders)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteSByte(teamNumber);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            positionsForChallengers = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (positionsForChallengers as ushort[])[i] = reader.ReadVarUhShort();
            }
            limit = reader.ReadShort();
            positionsForDefenders = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (positionsForDefenders as ushort[])[i] = reader.ReadVarUhShort();
            }
            teamNumber = reader.ReadSByte();
            if (teamNumber < 0)
                throw new Exception("Forbidden value on teamNumber = " + teamNumber + ", it doesn't respect the following condition : teamNumber < 0");
        }
        
    }
    
}