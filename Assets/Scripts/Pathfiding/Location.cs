using System;

namespace PushingBoxStudios.Pathfinding
{
    public struct Location
    {
        public int X;
        public int Y;

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Location left, Location right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Location left, Location right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Location)
            {
                var other = (Location)obj;
                return this == other;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return ShiftAndWrap(X.GetHashCode(), 2) ^ Y.GetHashCode();
        }

        private int ShiftAndWrap(int value, int positions)
        {
            positions = positions & 0x1F;

            // Save the existing bit pattern, but interpret it as an unsigned integer.
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);

            // Preserve the bits to be discarded.
            uint wrapped = number >> (32 - positions);

            // Shift and wrap the discarded bits.
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
    }
}
