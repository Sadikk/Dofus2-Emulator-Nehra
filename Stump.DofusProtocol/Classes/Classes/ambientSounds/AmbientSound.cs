

















// Generated on 07/24/2015 19:45:54
using System;
using System.Collections.Generic;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Tools.D2o;

namespace Stump.DofusProtocol.Classes
{

[D2OClass("AmbientSounds")]
    
public class AmbientSound : PlaylistSound
{

public const int AMBIENT_TYPE_ROLEPLAY = 1;
        public const int AMBIENT_TYPE_AMBIENT = 2;
        public const int AMBIENT_TYPE_FIGHT = 3;
        public const int AMBIENT_TYPE_BOSS = 4;
        public const String MODULE = "AmbientSounds";
        public int criterionId;
        public uint silenceMin;
        public uint silenceMax;
        public int type_id;
        

}

}