using System;
using System.Collections.Generic;

namespace PushingBoxStudios.Pathfinding.PriorityQueues
{
    internal class SortedList<TKey, TValue> : IPriorityQueue<TKey, TValue> where TKey : IComparable, IComparable<TKey>
    {
        private List<IPair<TKey, TValue>> _list;

        public int Count
        {
            get { return _list.Count; }
        }

        public SortedList()
        {
            _list = new List<IPair<TKey, TValue>>(1048576);
        }

        public IPair<TKey, TValue> Push(TKey key, TValue value)
        {
            var node = new PairNode<TKey, TValue>
            {
                Key = key,
                Value = value
            };

            return Push(node);
        }

        public IPair<TKey, TValue> Pop()
        {
            var node = _list[0];
            _list.RemoveAt(0);

            return node;
        }

        public void DecreaseKey(IPair<TKey, TValue> item, TKey newKey)
        {
            _list.Remove(item);

            var node = item as PairNode<TKey, TValue>;
            node.Key = newKey;

            Push(node);
        }

        private IPair<TKey, TValue> Push(IPair<TKey, TValue> node)
        {
            LinearAddition(node);
            //BinarySearchAddition(node);

            return node;
        }

        private void LinearAddition(IPair<TKey, TValue> item)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var otherNode = _list[i];

                if (item.CompareTo(otherNode) < 0)
                {
                    _list.Insert(i, item);
                    return;
                }
            }

            _list.Add(item);
        }

        private void BinarySearchAddition(IPair<TKey, TValue> item)
        {
            if (Count <= 0)
            {
                _list.Add(item);
                return;
            }

            var index = _list.BinarySearch(item);

            index = index < 0 
                ? ~index 
                : index;

            _list.Insert(index, item);
        }
    }
}
