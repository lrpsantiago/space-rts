
namespace PushingBoxStudios.Pathfinding
{
    internal interface IPathFinder
    {
        Path FindPath(Grid grid, Location start, Location goal);
        Path FindPath(Grid grid, ushort startX, ushort startY, ushort goalX, ushort goalY);
    }
}
