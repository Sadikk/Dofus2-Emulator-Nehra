


















// Generated on 07/24/2015 23:20:21
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class GameFightResumeSlaveInfo
{

public const short Id = 364;
public virtual short TypeId
{
    get { return Id; }
}

public int slaveId;
        public IEnumerable<Types.GameFightSpellCooldown> spellCooldowns;
        public sbyte summonCount;
        public sbyte bombCount;
        

public GameFightResumeSlaveInfo()
{
}

public GameFightResumeSlaveInfo(int slaveId, IEnumerable<Types.GameFightSpellCooldown> spellCooldowns, sbyte summonCount, sbyte bombCount)
        {
            this.slaveId = slaveId;
            this.spellCooldowns = spellCooldowns;
            this.summonCount = summonCount;
            this.bombCount = bombCount;
        }
        

public virtual void Serialize(ICustomDataOutput writer)
{

writer.WriteInt(slaveId);
            writer.WriteUShort((ushort)spellCooldowns.Count());
            foreach (var entry in spellCooldowns)
            {
                 entry.Serialize(writer);
            }
            writer.WriteSByte(summonCount);
            writer.WriteSByte(bombCount);
            

}

public virtual void Deserialize(ICustomDataInput reader)
{

slaveId = reader.ReadInt();
            var limit = reader.ReadShort();
            spellCooldowns = new Types.GameFightSpellCooldown[limit];
            for (int i = 0; i < limit; i++)
            {
                 (spellCooldowns as Types.GameFightSpellCooldown[])[i] = new Types.GameFightSpellCooldown();
                 (spellCooldowns as Types.GameFightSpellCooldown[])[i].Deserialize(reader);
            }
            summonCount = reader.ReadSByte();
            if (summonCount < 0)
                throw new Exception("Forbidden value on summonCount = " + summonCount + ", it doesn't respect the following condition : summonCount < 0");
            bombCount = reader.ReadSByte();
            if (bombCount < 0)
                throw new Exception("Forbidden value on bombCount = " + bombCount + ", it doesn't respect the following condition : bombCount < 0");
            

}


}


}