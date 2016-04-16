

















// Generated on 07/24/2015 19:45:56
using System;
using System.Collections.Generic;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Tools.D2o;

namespace Stump.DofusProtocol.Classes
{

[D2OClass("QuestSteps")]
    
public class QuestStep : IDataObject
{

public const String MODULE = "QuestSteps";
        public uint id;
        public uint questId;
        public uint nameId;
        public uint descriptionId;
        public int dialogId;
        public uint optimalLevel;
        public float duration;
        public Boolean kamasScaleWithPlayerLevel;
        public float kamasRatio;
        public float xpRatio;
        public List<uint> objectiveIds;
        public List<uint> rewardsIds;
        

}

}