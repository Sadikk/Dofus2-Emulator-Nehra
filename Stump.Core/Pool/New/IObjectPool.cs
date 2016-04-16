namespace Stump.Core.Pool.New
{
    public interface IObjectPool
    {
        int AvailableCount
        {
            get;
        }
        int ObtainedCount
        {
            get;
        }

        void Recycle(object obj);

        object ObtainObj();
    }
}