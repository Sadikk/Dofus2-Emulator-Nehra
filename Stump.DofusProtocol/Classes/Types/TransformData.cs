using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DofusProtocol.Classes
{
    [Serializable]
    public class TransformData : IDataObject
    {
        public String overrideClip;
        public String originalClip;
        public double x = 0.0;
        public double y = 0.0;
        public double scaleX = 1.0;
        public double scaleY = 1.0;
        public double rotation = 0.0;
    }
}