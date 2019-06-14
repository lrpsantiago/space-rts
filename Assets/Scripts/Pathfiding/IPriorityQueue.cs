using System;

namespace PushingBoxStudios.Pathfinding
{
    internal interface IPriorityQueue<T> where T : IComparable
    {
        int Count { get; }

        void Push(T item);
        T Pop();
    }
}
