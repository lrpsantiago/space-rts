
namespace PushingBoxStudios.Pathfinding
{
    public class Grid
    {
        private bool[,] m_nodes;
        private readonly uint m_width;
        private readonly uint m_height;

        public bool this[uint x, uint y]
        {
            get { return m_nodes[x, y]; }
            set { m_nodes[x, y] = value; }
        }

        public bool this[Location location]
        {
            get { return m_nodes[location.X, location.Y]; }
            set { m_nodes[location.X, location.Y] = value; }
        }

        public uint Width
        {
            get { return m_width; }
        }

        public uint Height
        {
            get { return m_height; }
        }

        public Grid(bool[,] nodes)
        {
            m_width = (uint)nodes.GetLength(0);
            m_height = (uint)nodes.GetLength(1);
            m_nodes = nodes;
        }

        public Grid(uint width, uint height)
        {
            m_width = width;
            m_height = height;
            m_nodes = new bool[m_width, m_height];

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    m_nodes[x, y] = true;
                }
            }
        }

        //public IAgent CreateAgentAt(uint x, uint y)
        //{
        //    return new Agent(this, x, y);
        //}

        public bool IsCornering(Location location)
        {
            return this.IsCornering(location.X, location.Y);
        }

        public bool IsCornering(int x, int y)
        {
            if (this.InBounds(x, y))
            {
                if (m_nodes[x, y])
                {
                    return (!m_nodes[x - 1, y - 1] || !m_nodes[x + 1, y - 1] || !m_nodes[x - 1, y + 1] || 
                        !m_nodes[x + 1, y + 1]) && 
                        (m_nodes[x + 1, y] && m_nodes[x - 1, y] && m_nodes[x, y + 1] && m_nodes[x, y - 1]);
                }
            }

            return false;
        }

        public bool InBounds(Location location)
        {
            return this.InBounds(location.X, location.Y);
        }

        public bool InBounds(int x, int y)
        {
            return (x >= 0 && x < this.Width && y >= 0 && y < this.Height);
        }
    }
}
