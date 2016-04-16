









// Generated on 07/24/2015 23:20:01
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class DungeonPartyFinderRoomContentUpdateMessage : Message
    {
        public const ushort Id = 6250;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public ushort dungeonId;
        public IEnumerable<Types.DungeonPartyFinderPlayer> addedPlayers;
        public IEnumerable<uint> removedPlayersIds;
        
        public DungeonPartyFinderRoomContentUpdateMessage()
        {
        }
        
        public DungeonPartyFinderRoomContentUpdateMessage(ushort dungeonId, IEnumerable<Types.DungeonPartyFinderPlayer> addedPlayers, IEnumerable<uint> removedPlayersIds)
        {
            this.dungeonId = dungeonId;
            this.addedPlayers = addedPlayers;
            this.removedPlayersIds = removedPlayersIds;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(dungeonId);
            writer.WriteUShort((ushort)addedPlayers.Count());
            foreach (var entry in addedPlayers)
            {
                 entry.Serialize(writer);
            }
            writer.WriteUShort((ushort)removedPlayersIds.Count());
            foreach (var entry in removedPlayersIds)
            {
                 writer.WriteVarUhInt(entry);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            dungeonId = reader.ReadVarUhShort();
            if (dungeonId < 0)
                throw new Exception("Forbidden value on dungeonId = " + dungeonId + ", it doesn't respect the following condition : dungeonId < 0");
            var limit = reader.ReadShort();
            addedPlayers = new Types.DungeonPartyFinderPlayer[limit];
            for (int i = 0; i < limit; i++)
            {
                 (addedPlayers as Types.DungeonPartyFinderPlayer[])[i] = new Types.DungeonPartyFinderPlayer();
                 (addedPlayers as Types.DungeonPartyFinderPlayer[])[i].Deserialize(reader);
            }
            limit = reader.ReadShort();
            removedPlayersIds = new uint[limit];
            for (int i = 0; i < limit; i++)
            {
                 (removedPlayersIds as uint[])[i] = reader.ReadVarUhInt();
            }
        }
        
    }
    
}