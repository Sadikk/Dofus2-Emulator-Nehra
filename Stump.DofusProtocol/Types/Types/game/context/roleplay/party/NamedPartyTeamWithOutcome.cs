


















// Generated on 07/24/2015 23:20:23
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class NamedPartyTeamWithOutcome
{

public const short Id = 470;
public virtual short TypeId
{
    get { return Id; }
}

public Types.NamedPartyTeam team;
        public ushort outcome;
        

public NamedPartyTeamWithOutcome()
{
}

public NamedPartyTeamWithOutcome(Types.NamedPartyTeam team, ushort outcome)
        {
            this.team = team;
            this.outcome = outcome;
        }
        

public virtual void Serialize(ICustomDataOutput writer)
{

team.Serialize(writer);
            writer.WriteVarUhShort(outcome);
            

}

public virtual void Deserialize(ICustomDataInput reader)
{

team = new Types.NamedPartyTeam();
            team.Deserialize(reader);
            outcome = reader.ReadVarUhShort();
            if (outcome < 0)
                throw new Exception("Forbidden value on outcome = " + outcome + ", it doesn't respect the following condition : outcome < 0");
            

}


}


}