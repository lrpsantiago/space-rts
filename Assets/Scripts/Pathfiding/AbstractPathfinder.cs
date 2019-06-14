using System;

namespace PushingBoxStudios.Pathfinding
{
    public abstract class AbstractPathfinder
    {
        public PathfindingStatistics Statistics { get; private set; }

        public EProbeMode ProbeMode { get; set; }

        public event EventHandler Started;
        public event EventHandler Iteration;
        public event EventHandler PathFound;
        public event EventHandler PathNotFound;

        public AbstractPathfinder()
        {
            Statistics = new PathfindingStatistics();
            ProbeMode = EProbeMode.EightDirections;
        }

        public abstract IPath FindPath(Grid grid, Location start, Location goal);
        //{
        //    return FindPath(grid, start.X, start.Y, goal.X, goal.Y);
        //}

        //public abstract Path FindPath(Grid grid, uint startX, uint startY, uint goalX, uint goalY);

        protected virtual void OnStarted()
        {
            if (Started != null)
            {
                Started(this, EventArgs.Empty);
            }
        }

        protected virtual void OnIteration()
        {
            if (Iteration != null)
            {
                Iteration(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPathFound()
        {
            if (PathFound != null)
            {
                PathFound(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPathNotFound()
        {
            if (PathNotFound != null)
            {
                PathNotFound(this, EventArgs.Empty);
            }
        }
    }
}
