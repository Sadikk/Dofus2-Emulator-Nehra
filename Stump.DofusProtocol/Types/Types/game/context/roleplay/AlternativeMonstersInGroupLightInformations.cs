


















// Generated on 07/24/2015 23:20:21
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class AlternativeMonstersInGroupLightInformations
{

public const short Id = 394;
public virtual short TypeId
{
    get { return Id; }
}

public int playerCount;
        public IEnumerable<Types.MonsterInGroupLightInformations> monsters;
        

public AlternativeMonstersInGroupLightInformations()
{
}

public AlternativeMonstersInGroupLightInformations(int playerCount, IEnumerable<Types.MonsterInGroupLightInformations> monsters)
        {
            this.playerCount = playerCount;
            this.monsters = monsters;
        }
        

public virtual void Serialize(ICustomDataOutput writer)
{

writer.WriteInt(playerCount);
            writer.WriteUShort((ushort)monsters.Count());
            foreach (var entry in monsters)
            {
                 entry.Serialize(writer);
            }
            

}

public virtual void Deserialize(ICustomDataInput reader)
{

playerCount = reader.ReadInt();
            var limit = reader.ReadShort();
            monsters = new Types.MonsterInGroupLightInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (monsters as Types.MonsterInGroupLightInformations[])[i] = new Types.MonsterInGroupLightInformations();
                 (monsters as Types.MonsterInGroupLightInformations[])[i].Deserialize(reader);
            }
            

}


}


}