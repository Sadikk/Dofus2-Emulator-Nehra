


















// Generated on 07/24/2015 23:20:23
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Types
{

public class QuestActiveDetailedInformations : QuestActiveInformations
{

public const short Id = 382;
public override short TypeId
{
    get { return Id; }
}

public ushort stepId;
        public IEnumerable<Types.QuestObjectiveInformations> objectives;
        

public QuestActiveDetailedInformations()
{
}

public QuestActiveDetailedInformations(ushort questId, ushort stepId, IEnumerable<Types.QuestObjectiveInformations> objectives)
         : base(questId)
        {
            this.stepId = stepId;
            this.objectives = objectives;
        }
        

public override void Serialize(ICustomDataOutput writer)
{

base.Serialize(writer);
            writer.WriteVarUhShort(stepId);
            writer.WriteUShort((ushort)objectives.Count());
            foreach (var entry in objectives)
            {
                 writer.WriteShort(entry.TypeId);
                 entry.Serialize(writer);
            }
            

}

public override void Deserialize(ICustomDataInput reader)
{

base.Deserialize(reader);
            stepId = reader.ReadVarUhShort();
            if (stepId < 0)
                throw new Exception("Forbidden value on stepId = " + stepId + ", it doesn't respect the following condition : stepId < 0");
            var limit = reader.ReadShort();
            objectives = new Types.QuestObjectiveInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (objectives as Types.QuestObjectiveInformations[])[i] = Types.ProtocolTypeManager.GetInstance<Types.QuestObjectiveInformations>(reader.ReadShort());
                 (objectives as Types.QuestObjectiveInformations[])[i].Deserialize(reader);
            }
            

}


}


}