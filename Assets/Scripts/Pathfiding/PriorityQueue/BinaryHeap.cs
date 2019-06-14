using System;
using System.Collections.Generic;

namespace PushingBoxStudios.Pathfinding.PriorityQueues
{
    public enum HeapType
    {
        MinHeap,
        MaxHeap
    }

    public class BinaryHeap<TKey, TValue> : IPriorityQueue<TKey, TValue> where TKey : IComparable, IComparable<TKey>
    {
        private List<IPair<TKey, TValue>> _heap;

        public int Count
        {
            get { return _heap.Count; }
        }

        public BinaryHeap()
        {
            _heap = new List<IPair<TKey, TValue>>(1048576);
        }

        public IPair<TKey, TValue> Push(TKey key, TValue item)
        {
            var node = new PairNode<TKey, TValue>
            {
                Key = key,
                Value = item
            };

            _heap.Add(node);

            var lastIndex = _heap.Count - 1;

            if (_heap[lastIndex].CompareTo(_heap[0]) < 0)
            {
                Swap(0, lastIndex);
            }

            BottomUpHeapfy();

            return node;
        }

        public IPair<TKey, TValue> Pop()
        {
            var aux = _heap[0];
            int lastIndex = Count - 1;

            Swap(0, lastIndex);
            _heap.RemoveAt(lastIndex);

            TopDownHeapfy();

            return aux;
        }

        public void DecreaseKey(IPair<TKey, TValue> node, TKey newKey)
        {
            _heap.Remove(node);

            var pair = node as PairNode<TKey, TValue>;

            pair.Key = newKey;
            _heap.Add(pair);

            var lastIndex = _heap.Count - 1;

            if (_heap[lastIndex].CompareTo(_heap[0]) < 0)
            {
                Swap(0, lastIndex);
            }

            BottomUpHeapfy();
        }

        private void BottomUpHeapfy()
        {
            var nodeIndex = Count - 1;

            while (nodeIndex > 0)
            {
                var current = _heap[nodeIndex];
                var parentIndex = GetParentIndex(nodeIndex);
                var parent = _heap[parentIndex];

                if (current.CompareTo(parent) < 0)
                {
                    Swap(nodeIndex, parentIndex);
                    nodeIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        private void TopDownHeapfy()
        {
            var currentIndex = 0;

            while (true)
            {
                var leftIndex = GetLeftChildIndex(currentIndex);
                var rightIndex = GetRightChildIndex(currentIndex);
                var bestIndex = currentIndex;

                if (leftIndex < Count)
                {
                    var best = _heap[bestIndex];
                    var left = _heap[leftIndex];

                    if (best.CompareTo(left) > 0)
                    {
                        bestIndex = leftIndex;
                    }
                }

                if (rightIndex < Count)
                {
                    var best = _heap[bestIndex];
                    var right = _heap[rightIndex];

                    if (best.CompareTo(right) > 0)
                    {
                        bestIndex = rightIndex;
                    }
                }

                if (bestIndex != currentIndex)
                {
                    Swap(currentIndex, bestIndex);
                    currentIndex = bestIndex;
                }
                else
                {
                    break;
                }
            }
        }

        private int GetParentIndex(int nodeIndex)
        {
            return (nodeIndex - 1) / 2;
        }

        private int GetRightChildIndex(int nodeIndex)
        {
            return 2 * nodeIndex + 2;
        }

        private int GetLeftChildIndex(int nodeIndex)
        {
            return 2 * nodeIndex + 1;
        }

        private void Swap(int index1, int index2)
        {
            var aux = _heap[index1];

            _heap[index1] = _heap[index2];
            _heap[index2] = aux;
        }

        public void Print()
        {
            for (var i = 0; i < _heap.Count; i++)
            {
                Console.Out.Write(_heap[i].ToString() + "\t");
            }
        }
    }

    public class Heap<TKey, TValue> : IPriorityQueue<TKey, TValue> where TKey : IComparable, IComparable<TKey>
    {
        private List<IPair<TKey, TValue>> _heap;

        public HeapType Type { get; private set; }

        public int Count
        {
            get { return _heap.Count; }
        }

        public Heap(HeapType type = HeapType.MinHeap)
        {
            _heap = new List<IPair<TKey, TValue>>(1048576);
            Type = type;
        }

        public IPair<TKey, TValue> Push(TKey key, TValue value)
        {
            var node = new PairNode<TKey, TValue>
            {
                Key = key,
                Value = value
            };

            _heap.Add(node);

            var i = _heap.Count - 1;
            var flag = true;

            if (Type == HeapType.MaxHeap)
            {
                flag = false;
            }

            while (i > 0)
            {
                if ((_heap[i].CompareTo(_heap[(i - 1) / 2]) > 0) ^ flag)
                {
                    var temp = _heap[i];
                    _heap[i] = _heap[(i - 1) / 2];
                    _heap[(i - 1) / 2] = temp;
                    i = (i - 1) / 2;
                }
                else
                {
                    break;
                }
            }

            return node;
        }

        public IPair<TKey, TValue> Pop()
        {
            var result = _heap[0];

            DeleteRoot();

            return result;
        }

        public void DecreaseKey(IPair<TKey, TValue> node, TKey newKey)
        {
            _heap.Remove(node);

            var item = node as PairNode<TKey, TValue>;
            item.Key = newKey;

            Push(node);
        }

        private void Push(IPair<TKey, TValue> node)
        {
            _heap.Add(node);

            var i = _heap.Count - 1;
            var flag = true;

            if (Type == HeapType.MaxHeap)
            {
                flag = false;
            }

            while (i > 0)
            {
                if ((_heap[i].CompareTo(_heap[(i - 1) / 2]) > 0) ^ flag)
                {
                    var temp = _heap[i];
                    _heap[i] = _heap[(i - 1) / 2];
                    _heap[(i - 1) / 2] = temp;
                    i = (i - 1) / 2;
                }
                else
                {
                    break;
                }
            }

        }

        private void DeleteRoot()
        {
            int i = _heap.Count - 1;

            _heap[0] = _heap[i];
            _heap.RemoveAt(i);

            i = 0;

            bool flag = true;

            if (Type == HeapType.MaxHeap)
                flag = false;

            while (true)
            {
                int leftInd = 2 * i + 1;
                int rightInd = 2 * i + 2;
                int largest = i;

                if (leftInd < _heap.Count)
                {
                    if ((_heap[leftInd].CompareTo(_heap[largest]) > 0) ^ flag)
                    {
                        largest = leftInd;
                    }
                }

                if (rightInd < _heap.Count)
                {
                    if ((_heap[rightInd].CompareTo(_heap[largest]) > 0) ^ flag)
                    {
                        largest = rightInd;
                    }
                }

                if (largest != i)
                {
                    var temp = _heap[largest];
                    _heap[largest] = _heap[i];
                    _heap[i] = temp;
                    i = largest;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
