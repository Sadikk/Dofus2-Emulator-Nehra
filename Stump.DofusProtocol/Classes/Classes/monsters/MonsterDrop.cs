

















// Generated on 07/24/2015 19:45:56
using System;
using System.Collections.Generic;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Tools.D2o;

namespace Stump.DofusProtocol.Classes
{

[D2OClass("MonsterDrop")]
    
public class MonsterDrop : IDataObject
{

public uint dropId;
        public int monsterId;
        public int objectId;
        public float percentDropForGrade1;
        public float percentDropForGrade2;
        public float percentDropForGrade3;
        public float percentDropForGrade4;
        public float percentDropForGrade5;
        public int count;
        public Boolean hasCriteria;
        

}

}