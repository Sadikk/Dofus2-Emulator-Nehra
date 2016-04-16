


















// Generated on 07/24/2015 23:20:25
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class UpdateMountIntBoost : UpdateMountBoost
{

public const short Id = 357;
public override short TypeId
{
    get { return Id; }
}

public int value;
        

public UpdateMountIntBoost()
{
}

public UpdateMountIntBoost(sbyte type, int value)
         : base(type)
        {
            this.value = value;
        }
        

public override void Serialize(ICustomDataOutput writer)
{

base.Serialize(writer);
            writer.WriteInt(value);
            

}

public override void Deserialize(ICustomDataInput reader)
{

base.Deserialize(reader);
            value = reader.ReadInt();
            

}


}


}