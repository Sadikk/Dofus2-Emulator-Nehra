


















// Generated on 07/24/2015 23:20:23
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class Item
{

public const short Id = 7;
public virtual short TypeId
{
    get { return Id; }
}



public Item()
{
}



public virtual void Serialize(ICustomDataOutput writer)
{



}

public virtual void Deserialize(ICustomDataInput reader)
{



}


}


}