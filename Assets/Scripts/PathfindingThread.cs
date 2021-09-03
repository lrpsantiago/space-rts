using PushingBoxStudios.Pathfinding;
using System;

namespace Assets.Scripts
{
    public class PathfindingThread
    {
        private AbstractPathfinder _pathfinder;
        private Grid _grid;
        private Location _start;
        private Location _goal;
        private Action<IPath> _callback;

        public PathfindingThread(AbstractPathfinder pathfinder, Grid grid, Location start, Location goal,
            Action<IPath> callback)
        {
            _pathfinder = pathfinder;
            _grid = grid;
            _start = start;
            _goal = goal;
            _callback = callback;
        }

        public void Run()
        {
            var path = _pathfinder.FindPath(_grid, _start, _goal);
            _callback(path);
        }
    }
}
