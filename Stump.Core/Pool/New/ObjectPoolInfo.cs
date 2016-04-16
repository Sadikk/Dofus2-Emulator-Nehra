namespace Stump.Core.Pool.New
{
    public struct ObjectPoolInfo
    {
        public int HardReferences;
        public int WeakReferences;

        public ObjectPoolInfo(int weak, int hard)
        {
            this.HardReferences = hard;
            this.WeakReferences = weak;
        }
    }
}