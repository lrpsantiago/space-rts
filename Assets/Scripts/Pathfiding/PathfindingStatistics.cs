using System;

namespace PushingBoxStudios.Pathfinding
{
    public class PathfindingStatistics
    {
        private double m_timeStampStart;
        private double m_timeStampEnd;
        private uint m_totalGridNodes;
        private uint m_openedNodes;
        private uint m_closedNodes;
        private uint m_maximumOpenNodes;
        private uint m_iterations;
        private uint m_pathLength;
        private uint m_pathCost;

        public double TimeLapsed
        {
            get { return m_timeStampEnd - m_timeStampStart; }
        }

        public uint TotalGridNodes
        {
            get { return m_totalGridNodes; }
            set { m_totalGridNodes = value; }
        }

        public uint IterationsCount
        {
            get { return m_iterations; }
        }

        public uint OpenedNodesCount
        {
            get { return m_openedNodes; }
        }

        public uint ClosedNodesCount
        {
            get { return m_closedNodes; }
        }

        public uint MaximumOpenNodesCount
        {
            get { return m_maximumOpenNodes; }
        }

        public uint PathLength
        {
            get { return m_pathLength; }
            set { m_pathLength = value; }
        }

        public uint PathCost
        {
            get { return m_pathCost; }
            set { m_pathCost = value; }
        }

        internal PathfindingStatistics()
        {
            Reset();
        }

        public void AddIteration()
        {
            m_iterations++;
        }

        public void AddOpenedNode()
        {
            m_openedNodes++;
        }

        public void AddClosedNode()
        {
            m_closedNodes++;
        }

        public void UpdateMaximumOpenNodes(uint value)
        {
            if(value <= m_maximumOpenNodes)
            {
                return;
            }

            m_maximumOpenNodes = value;
        }

        public void StartTimer()
        {
            m_timeStampStart = DateTime.Now.TimeOfDay.TotalSeconds;
        }

        public void StopTimer()
        {
            m_timeStampEnd = DateTime.Now.TimeOfDay.TotalSeconds;
        }

        public void Reset()
        {
            m_timeStampStart = 0;
            m_timeStampEnd = 0;
            m_totalGridNodes = 0;
            m_openedNodes = 0;
            m_closedNodes = 0;
            m_maximumOpenNodes = 0;
            m_iterations = 0;
            m_pathLength = 0;
            m_pathCost = 0;
        }

        public override string ToString()
        {
            return "Time Lapsed........:\t" + TimeLapsed + " s" + "\n" +
                "Iterations.........:\t" + IterationsCount + "\n" +
                "Total Nodes........:\t" + TotalGridNodes + "\n" +
                "Opened Nodes.......:\t" + OpenedNodesCount + "\n" +
                "Closed Nodes.......:\t" + ClosedNodesCount + "\n" +
                "Maximum Open Nodes.:\t" + MaximumOpenNodesCount + "\n" +
                "Path Length........:\t" + PathLength + "\n" +
                "Path Cost..........:\t" + PathCost + "\n";
        }

        public PathfindingStatisticsRecord Record()
        {
            return new PathfindingStatisticsRecord
            {
                TimeLapsed = this.TimeLapsed,
                TotalGridNodes = this.TotalGridNodes,
                IterationsCount = this.IterationsCount,
                OpenedNodesCount = this.OpenedNodesCount,
                ClosedNodesCount = this.ClosedNodesCount,
                MaximumOpenNodesCount = this.MaximumOpenNodesCount,
                PathLength = this.PathLength,
                PathCost = this.PathCost
            };
        }
    }

    public class PathfindingStatisticsRecord
    {
        public double TimeLapsed { get; set; }

        public uint TotalGridNodes { get; set; }

        public uint IterationsCount { get; set; }

        public uint OpenedNodesCount { get; set; }

        public uint ClosedNodesCount { get; set; }

        public uint MaximumOpenNodesCount { get; set; }

        public uint PathLength { get; set; }

        public uint PathCost { get; set; }
    }
}
