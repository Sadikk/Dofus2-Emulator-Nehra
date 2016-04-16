using System;
namespace Stump.DofusProtocol.Classes
{
    [Serializable]
    public class Point : IDataObject
    {
        public int x;
        public int y;
        public double length;
    }
}