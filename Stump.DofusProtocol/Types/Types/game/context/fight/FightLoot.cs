


















// Generated on 07/24/2015 23:20:20
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class FightLoot
{

public const short Id = 41;
public virtual short TypeId
{
    get { return Id; }
}

public IEnumerable<ushort> objects;
        public uint kamas;
        

public FightLoot()
{
}

public FightLoot(IEnumerable<ushort> objects, uint kamas)
        {
            this.objects = objects;
            this.kamas = kamas;
        }
        

public virtual void Serialize(ICustomDataOutput writer)
{

writer.WriteUShort((ushort)objects.Count());
            foreach (var entry in objects)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteVarUhInt(kamas);
            

}

public virtual void Deserialize(ICustomDataInput reader)
{

var limit = reader.ReadShort();
            objects = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (objects as ushort[])[i] = reader.ReadVarUhShort();
            }
            kamas = reader.ReadVarUhInt();
            if (kamas < 0)
                throw new Exception("Forbidden value on kamas = " + kamas + ", it doesn't respect the following condition : kamas < 0");
            

}


}


}