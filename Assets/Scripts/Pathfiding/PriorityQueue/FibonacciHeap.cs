using System;
using System.Collections.Generic;

namespace PushingBoxStudios.Pathfinding.PriorityQueues
{
    public class FibonacciHeap<TKey, TValue> : IPriorityQueue<TKey, TValue>
        where TKey : IComparable, IComparable<TKey>
    {
        private static readonly double OneOverLogPhi = 1.0 / Math.Log((1.0 + Math.Sqrt(5.0)) / 2.0);
        private readonly TKey _minKeyValue;
        private FibonacciHeapNode<TKey, TValue> _minNode;

        public int Count { get; private set; }

        public FibonacciHeap(TKey minKeyValue)
        {
            _minKeyValue = minKeyValue;
        }

        public void Clear()
        {
            _minNode = null;
            Count = 0;
        }

        public void DecreaseKey(IPair<TKey, TValue> node, TKey newKey)
        {
            if (newKey.CompareTo(node.Key) > 0)
            {
                throw new ArgumentException("decreaseKey() got larger key value");
            }

            var nodeH = node as FibonacciHeapNode<TKey, TValue>;
            nodeH.Key = newKey;

            var parent = nodeH.Parent;

            if ((parent != null) && (node.CompareTo(parent) < 0))
            {
                Cut(nodeH, parent);
                CascadingCut(parent);
            }

            if (node.CompareTo(_minNode) < 0)
            {
                _minNode = nodeH;
            }
        }

        public void Delete(IPair<TKey, TValue> x)
        {
            // make newParent as small as possible
            DecreaseKey(x, _minKeyValue);

            // remove the smallest, which decreases n also
            Pop();
        }

        public IPair<TKey, TValue> Push(TKey key, TValue value)
        {
            var node = new FibonacciHeapNode<TKey, TValue>(key, value);

            if (_minNode != null)
            {
                node.Left = _minNode;
                node.Right = _minNode.Right;
                _minNode.Right = node;
                node.Right.Left = node;

                if (node.CompareTo(_minNode) < 0)
                {
                    _minNode = node;
                }
            }
            else
            {
                _minNode = node;
            }

            Count++;

            return node;
        }

        public IPair<TKey, TValue> Pop()
        {
            var minNode = _minNode;

            if (minNode != null)
            {
                var numKids = minNode.Degree;
                var oldMinChild = minNode.Child;

                // for each child of minNode do...
                while (numKids > 0)
                {
                    var tempRight = oldMinChild.Right;

                    // remove oldMinChild from child list
                    oldMinChild.Left.Right = oldMinChild.Right;
                    oldMinChild.Right.Left = oldMinChild.Left;

                    // add oldMinChild to root list of heap
                    oldMinChild.Left = _minNode;
                    oldMinChild.Right = _minNode.Right;
                    _minNode.Right = oldMinChild;
                    oldMinChild.Right.Left = oldMinChild;

                    // set parent[oldMinChild] to null
                    oldMinChild.Parent = null;
                    oldMinChild = tempRight;
                    numKids--;
                }

                // remove minNode from root list of heap
                minNode.Left.Right = minNode.Right;
                minNode.Right.Left = minNode.Left;

                if (minNode == minNode.Right)
                {
                    _minNode = null;
                }
                else
                {
                    _minNode = minNode.Right;
                    Consolidate();
                }

                // decrement size of heap
                Count--;
            }

            return minNode;
        }

        public static FibonacciHeap<TKey, TValue> Union(FibonacciHeap<TKey, TValue> heapA, FibonacciHeap<TKey, TValue> heapB)
        {
            var minValue = heapA._minKeyValue.CompareTo(heapB._minKeyValue) < 0
                    ? heapA._minKeyValue
                    : heapB._minKeyValue;

            var newHeap = new FibonacciHeap<TKey, TValue>(minValue);

            if ((heapA != null) && (heapB != null))
            {
                newHeap._minNode = heapA._minNode;

                if (newHeap._minNode != null)
                {
                    if (heapB._minNode != null)
                    {
                        newHeap._minNode.Right.Left = heapB._minNode.Left;
                        heapB._minNode.Left.Right = newHeap._minNode.Right;
                        newHeap._minNode.Right = heapB._minNode;
                        heapB._minNode.Left = newHeap._minNode;

                        if (heapB._minNode.CompareTo(heapA._minNode) < 0)
                        {
                            newHeap._minNode = heapB._minNode;
                        }
                    }
                }
                else
                {
                    newHeap._minNode = heapB._minNode;
                }

                newHeap.Count = heapA.Count + heapB.Count;
            }

            return newHeap;
        }

        private void CascadingCut(FibonacciHeapNode<TKey, TValue> node)
        {
            while (node.Parent != null)
            {
                var parent = node.Parent;

                if (!node.IsMarked)
                {
                    node.IsMarked = true;
                    break;
                }
                else
                {
                    Cut(node, parent);
                    node = parent;
                }
            }
        }

        protected void Consolidate()
        {
            var arraySize = ((int)Math.Floor(Math.Log(Count) * OneOverLogPhi)) + 1;
            var array = new List<FibonacciHeapNode<TKey, TValue>>(arraySize);

            // Initialize degree array
            for (var i = 0; i < arraySize; i++)
            {
                array.Add(null);
            }

            // Find the number of root nodes.
            var numRoots = 0;
            var x = _minNode;

            if (x != null)
            {
                numRoots++;
                x = x.Right;

                while (x != _minNode)
                {
                    numRoots++;
                    x = x.Right;
                }
            }

            // For each node in root list do...
            while (numRoots > 0)
            {
                // Access this node's degree..
                var d = x.Degree;
                var next = x.Right;

                // ..and see if there's another of the same degree.
                for (; ; )
                {
                    var y = array[d];

                    if (y == null)
                    {
                        // Nope.
                        break;
                    }

                    // There is, make one of the nodes a child of the other.
                    // Do this based on the key value.
                    if (x.CompareTo(y) > 0)
                    {
                        var temp = y;
                        y = x;
                        x = temp;
                    }

                    // FibonacciHeapNode<T> newChild disappears from root list.
                    Link(y, x);

                    // We've handled this degree, go to next one.
                    array[d] = null;
                    d++;
                }

                // Save this node for later when we might encounter another
                // of the same degree.
                array[d] = x;

                // Move forward through list.
                x = next;
                numRoots--;
            }

            // Set min to null (effectively losing the root list) and
            // reconstruct the root list from the array entries in array[].
            _minNode = null;

            for (var i = 0; i < arraySize; i++)
            {
                var y = array[i];
                if (y == null)
                {
                    continue;
                }

                // We've got a live one, add it to root list.
                if (_minNode != null)
                {
                    // First remove node from root list.
                    y.Left.Right = y.Right;
                    y.Right.Left = y.Left;

                    // Now add to root list, again.
                    y.Left = _minNode;
                    y.Right = _minNode.Right;
                    _minNode.Right = y;
                    y.Right.Left = y;

                    // Check if this is a new min.
                    if (y.CompareTo(_minNode) < 0)
                    {
                        _minNode = y;
                    }
                }
                else
                {
                    _minNode = y;
                }
            }
        }

        private void Cut(FibonacciHeapNode<TKey, TValue> nodeA, FibonacciHeapNode<TKey, TValue> nodeB)
        {
            // remove newParent from childlist of newChild and decrement degree[newChild]
            nodeA.Left.Right = nodeA.Right;
            nodeA.Right.Left = nodeA.Left;
            nodeB.Degree--;

            // reset newChild.child if necessary
            if (nodeB.Child == nodeA)
            {
                nodeB.Child = nodeA.Right;
            }

            if (nodeB.Degree == 0)
            {
                nodeB.Child = null;
            }

            // add newParent to root list of heap
            nodeA.Left = _minNode;
            nodeA.Right = _minNode.Right;
            _minNode.Right = nodeA;
            nodeA.Right.Left = nodeA;

            // set parent[newParent] to nil
            nodeA.Parent = null;

            // set mark[newParent] to false
            nodeA.IsMarked = false;
        }

        private void Link(FibonacciHeapNode<TKey, TValue> newChild, FibonacciHeapNode<TKey, TValue> newParent)
        {
            // remove newChild from root list of heap
            newChild.Left.Right = newChild.Right;
            newChild.Right.Left = newChild.Left;

            // make newChild a child of newParent
            newChild.Parent = newParent;

            if (newParent.Child == null)
            {
                newParent.Child = newChild;
                newChild.Right = newChild;
                newChild.Left = newChild;
            }
            else
            {
                newChild.Left = newParent.Child;
                newChild.Right = newParent.Child.Right;
                newParent.Child.Right = newChild;
                newChild.Right.Left = newChild;
            }

            // increase degree[newParent]
            newParent.Degree++;

            // set mark[newChild] false
            newChild.IsMarked = false;
        }
    }
}