namespace PushingBoxStudios.Pathfinding
{
    internal class Agent : IAgent
    {
        private readonly Grid _grid;
        private Location _position;
        private IPath _path;

        public Agent(Grid grid, uint x, uint y)
        {
            _grid = grid;
            _position = new Location((int)x, (int)y);
            _path = null;
        }

        public Agent(Grid grid, Location position)
        {
            _grid = grid;
            _position = position;
            _path = null;
        }

        public IPath FindPath(uint x, uint y)
        {
            return FindPath(new Location((int)x, (int)y));
        }

        public IPath FindPath(Location goal)
        {
            AbstractPathfinder algorithm = new AStarPathfinder();
            _path = algorithm.FindPath(_grid, _position, goal);

            return _path;
        }

        public void MoveOnPath(uint steps)
        {
            if (steps <= _path.OriginalSize)
            {
                for (uint i = 0; i < steps; i++)
                {
                    _path.PopFront();
                }

                _position = _path.Front;
            }
        }
    }
}