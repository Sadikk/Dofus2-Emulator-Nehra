

















// Generated on 07/24/2015 19:45:56
using System;
using System.Collections.Generic;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Tools.D2o;

namespace Stump.DofusProtocol.Classes
{

[D2OClass("AchievementObjectives")]
    
public class AchievementObjective : IDataObject
{

public const String MODULE = "AchievementObjectives";
        public uint id;
        public uint achievementId;
        public uint order;
        public uint nameId;
        public String criterion;
        

}

}