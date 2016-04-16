


















// Generated on 07/24/2015 23:20:21
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class FightTeamMemberWithAllianceCharacterInformations : FightTeamMemberCharacterInformations
{

public const short Id = 426;
public override short TypeId
{
    get { return Id; }
}

public Types.BasicAllianceInformations allianceInfos;
        

public FightTeamMemberWithAllianceCharacterInformations()
{
}

public FightTeamMemberWithAllianceCharacterInformations(int id, string name, byte level, Types.BasicAllianceInformations allianceInfos)
         : base(id, name, level)
        {
            this.allianceInfos = allianceInfos;
        }
        

public override void Serialize(ICustomDataOutput writer)
{

base.Serialize(writer);
            allianceInfos.Serialize(writer);
            

}

public override void Deserialize(ICustomDataInput reader)
{

base.Deserialize(reader);
            allianceInfos = new Types.BasicAllianceInformations();
            allianceInfos.Deserialize(reader);
            

}


}


}