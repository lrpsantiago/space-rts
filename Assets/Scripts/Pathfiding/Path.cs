using System.Collections.Generic;

namespace PushingBoxStudios.Pathfinding
{
    internal class Path : IPath
    {
        private List<Location> _waypoints;

        public Location Front
        {
            get
            {
                return _waypoints[0];
            }
        }

        public Location Start { get; private set; }

        public Location Goal { get; private set; }

        public uint OriginalSize { get; private set; }

        public uint Size
        {
            get { return (uint)_waypoints.Count; }
        }

        internal Path(Location start, Location goal)
        {
            _waypoints = new List<Location>();
            Start = start;
            Goal = goal;
        }

        public void PushBack(Location pos)
        {
            _waypoints.Add(pos);
            OriginalSize = (uint)_waypoints.Count;
        }

        public void PopFront()
        {
            if (Size > 0)
            {
                _waypoints.RemoveAt(0);
            }
        }

        public IPath Clone()
        {
            var clonePath = new Path(Start, Goal);
            var waypoints = _waypoints.ToArray();

            for (int i = 0; i < waypoints.Length; i++)
            {
                clonePath.PushBack(waypoints[i]);
            }

            return clonePath;
        }

        public IList<Location> ToList()
        {
            return new List<Location>(_waypoints.ToArray());
        }

        public void DiscardUpTo(Location location)
        {
            if(!_waypoints.Contains(location))
            {
                return;
            }

            while (Front != location)
            {
                PopFront();
            }

            PopFront();
        }

        //public void RemoveEdges()
        //{
        //    var waypointsToRemove = new List<Location>();

        //    for (var i = 1; i < _waypoints.Count - 1; i++)
        //    {
        //        var hotspot = _waypoints[i];
        //        var prev = _waypoints[i - 1];
        //        var next = _waypoints[i + 1];
                
        //        var prevDirection = new Location(prev.X - hotspot.X, prev.Y - hotspot.Y);
        //        var nextDirection = new Location(hotspot.X - next.X, hotspot.Y - next.Y);

        //        if (prevDirection == nextDirection)
        //        {
        //            waypointsToRemove.Add(hotspot);
        //        }
        //    }

        //    for (var i = 0; i < waypointsToRemove.Count; i++)
        //    {
        //        _waypoints.Remove(waypointsToRemove[i]);
        //    }
        //}
    }
}