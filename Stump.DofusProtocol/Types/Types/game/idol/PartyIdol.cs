


















// Generated on 07/24/2015 23:20:25
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class PartyIdol : Idol
{

public const short Id = 490;
public override short TypeId
{
    get { return Id; }
}

public IEnumerable<int> ownersIds;
        

public PartyIdol()
{
}

public PartyIdol(ushort id, ushort xpBonusPercent, ushort dropBonusPercent, IEnumerable<int> ownersIds)
         : base(id, xpBonusPercent, dropBonusPercent)
        {
            this.ownersIds = ownersIds;
        }
        

public override void Serialize(ICustomDataOutput writer)
{

base.Serialize(writer);
            writer.WriteUShort((ushort)ownersIds.Count());
            foreach (var entry in ownersIds)
            {
                 writer.WriteInt(entry);
            }
            

}

public override void Deserialize(ICustomDataInput reader)
{

base.Deserialize(reader);
            var limit = reader.ReadShort();
            ownersIds = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                 (ownersIds as int[])[i] = reader.ReadInt();
            }
            

}


}


}