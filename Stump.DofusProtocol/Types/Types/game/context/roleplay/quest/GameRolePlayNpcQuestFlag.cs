


















// Generated on 07/24/2015 23:20:23
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class GameRolePlayNpcQuestFlag
{

public const short Id = 384;
public virtual short TypeId
{
    get { return Id; }
}

public IEnumerable<ushort> questsToValidId;
        public IEnumerable<ushort> questsToStartId;
        

public GameRolePlayNpcQuestFlag()
{
}

public GameRolePlayNpcQuestFlag(IEnumerable<ushort> questsToValidId, IEnumerable<ushort> questsToStartId)
        {
            this.questsToValidId = questsToValidId;
            this.questsToStartId = questsToStartId;
        }
        

public virtual void Serialize(ICustomDataOutput writer)
{

writer.WriteUShort((ushort)questsToValidId.Count());
            foreach (var entry in questsToValidId)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteUShort((ushort)questsToStartId.Count());
            foreach (var entry in questsToStartId)
            {
                 writer.WriteVarUhShort(entry);
            }
            

}

public virtual void Deserialize(ICustomDataInput reader)
{

var limit = reader.ReadShort();
            questsToValidId = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (questsToValidId as ushort[])[i] = reader.ReadVarUhShort();
            }
            limit = reader.ReadShort();
            questsToStartId = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (questsToStartId as ushort[])[i] = reader.ReadVarUhShort();
            }
            

}


}


}