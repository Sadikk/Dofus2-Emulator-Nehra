

















// Generated on 07/24/2015 19:45:54
using System;
using System.Collections.Generic;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Tools.D2o;

namespace Stump.DofusProtocol.Classes
{

    [D2OClass("EffectInstance"), Serializable]
    public class EffectInstance : IDataObject
    {

        public uint effectUid;
        public uint effectId;
        public int targetId;
        public String targetMask;
        public int duration;
        public int delay;
        public int random;
        public int group;
        public int modificator;
        public Boolean trigger;
        public String triggers;
        public Boolean visibleInTooltip = true;
        public Boolean visibleInBuffUi = true;
        public Boolean visibleInFightLog = true;
        public Object zoneSize;
        public uint zoneShape;
        public Object zoneMinSize;
        public Object zoneEfficiencyPercent;
        public Object zoneMaxEfficiency;
        public Boolean rawZoneInit;
        public String rawZone;


    }
}