using System;

namespace PushingBoxStudios.Pathfinding.PriorityQueues
{
    public interface IPriorityQueue<TKey, TValue> where TKey : IComparable, IComparable<TKey>
    {
        int Count { get; }

        IPair<TKey, TValue> Push(TKey key, TValue val);
        IPair<TKey, TValue> Pop();
        void DecreaseKey(IPair<TKey, TValue> node, TKey newKey);
    }
}
