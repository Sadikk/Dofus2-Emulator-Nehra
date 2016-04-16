using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DofusProtocol.Classes
{
    [Serializable]
    public class AnimFunData : IDataObject
    {
        public String animName;
        public int animWeight;
    }
}
