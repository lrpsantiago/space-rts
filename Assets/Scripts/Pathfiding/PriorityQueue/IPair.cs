using System;

namespace PushingBoxStudios.Pathfinding.PriorityQueues
{
    public interface IPair<TKey, TValue> : IComparable, IComparable<IPair<TKey, TValue>> 
        where TKey : IComparable, IComparable<TKey>
    {
        TKey Key { get; }
        TValue Value { get; }
    }
}
