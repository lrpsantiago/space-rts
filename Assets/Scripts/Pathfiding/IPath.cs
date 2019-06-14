
using System.Collections.Generic;

namespace PushingBoxStudios.Pathfinding
{
    public interface IPath
    {
        Location Front { get; }
        Location Start { get; }
        Location Goal { get; }
        uint OriginalSize { get; }
        uint Size { get; }

        void PopFront();
        IPath Clone();
        IList<Location> ToList();
        void DiscardUpTo(Location location);
    }
}
