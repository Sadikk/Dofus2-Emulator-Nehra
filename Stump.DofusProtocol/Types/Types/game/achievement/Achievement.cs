// Generated on 07/24/2015 23:20:19
using Stump.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.DofusProtocol.Types
{
    public class Achievement
    {
        public const short Id = 363;
        public virtual short TypeId
        {
            get { return Id; }
        }

        public ushort id;
        public IEnumerable<Types.AchievementObjective> finishedObjective;
        public IEnumerable<Types.AchievementStartedObjective> startedObjectives;

        public Achievement()
        {
        }

        public Achievement(ushort id, IEnumerable<Types.AchievementObjective> finishedObjective, IEnumerable<Types.AchievementStartedObjective> startedObjectives)
        {
            this.id = id;
            this.finishedObjective = finishedObjective;
            this.startedObjectives = startedObjectives;
        }

        public virtual void Serialize(ICustomDataOutput writer)
        {
            writer.WriteVarUhShort(id);
            writer.WriteUShort((ushort)finishedObjective.Count());
            foreach (var entry in finishedObjective)
            {
                entry.Serialize(writer);
            }
            writer.WriteUShort((ushort)startedObjectives.Count());
            foreach (var entry in startedObjectives)
            {
                entry.Serialize(writer);
            }
        }

        public virtual void Deserialize(ICustomDataInput reader)
        {
            id = reader.ReadVarUhShort();
            if (id < 0)
                throw new Exception("Forbidden value on id = " + id + ", it doesn't respect the following condition : id < 0");
            var limit = reader.ReadShort();
            finishedObjective = new Types.AchievementObjective[limit];
            for (int i = 0; i < limit; i++)
            {
                (finishedObjective as Types.AchievementObjective[])[i] = new Types.AchievementObjective();
                (finishedObjective as Types.AchievementObjective[])[i].Deserialize(reader);
            }
            limit = reader.ReadShort();
            startedObjectives = new Types.AchievementStartedObjective[limit];
            for (int i = 0; i < limit; i++)
            {
                (startedObjectives as Types.AchievementStartedObjective[])[i] = new Types.AchievementStartedObjective();
                (startedObjectives as Types.AchievementStartedObjective[])[i].Deserialize(reader);
            }
        }
    }
}