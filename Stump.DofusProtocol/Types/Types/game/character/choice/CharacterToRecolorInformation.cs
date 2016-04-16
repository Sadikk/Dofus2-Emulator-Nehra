


















// Generated on 07/24/2015 23:20:20
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class CharacterToRecolorInformation : AbstractCharacterToRefurbishInformation
{

public const short Id = 212;
public override short TypeId
{
    get { return Id; }
}



public CharacterToRecolorInformation()
{
}

public CharacterToRecolorInformation(uint id, IEnumerable<int> colors, uint cosmeticId)
         : base(id, colors, cosmeticId)
        {
        }
        

public override void Serialize(ICustomDataOutput writer)
{

base.Serialize(writer);
            

}

public override void Deserialize(ICustomDataInput reader)
{

base.Deserialize(reader);
            

}


}


}