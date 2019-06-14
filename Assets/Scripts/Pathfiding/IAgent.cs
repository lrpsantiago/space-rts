
namespace PushingBoxStudios.Pathfinding
{
    public interface IAgent
    {
        IPath FindPath(uint x, uint y);
        void MoveOnPath(uint steps);
    }
}
