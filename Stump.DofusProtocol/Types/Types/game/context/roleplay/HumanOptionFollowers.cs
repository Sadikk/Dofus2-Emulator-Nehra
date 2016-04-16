


















// Generated on 07/24/2015 23:20:22
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class HumanOptionFollowers : HumanOption
{

public const short Id = 410;
public override short TypeId
{
    get { return Id; }
}

public IEnumerable<Types.IndexedEntityLook> followingCharactersLook;
        

public HumanOptionFollowers()
{
}

public HumanOptionFollowers(IEnumerable<Types.IndexedEntityLook> followingCharactersLook)
        {
            this.followingCharactersLook = followingCharactersLook;
        }
        

public override void Serialize(ICustomDataOutput writer)
{

base.Serialize(writer);
            writer.WriteUShort((ushort)followingCharactersLook.Count());
            foreach (var entry in followingCharactersLook)
            {
                 entry.Serialize(writer);
            }
            

}

public override void Deserialize(ICustomDataInput reader)
{

base.Deserialize(reader);
            var limit = reader.ReadShort();
            followingCharactersLook = new Types.IndexedEntityLook[limit];
            for (int i = 0; i < limit; i++)
            {
                 (followingCharactersLook as Types.IndexedEntityLook[])[i] = new Types.IndexedEntityLook();
                 (followingCharactersLook as Types.IndexedEntityLook[])[i].Deserialize(reader);
            }
            

}


}


}