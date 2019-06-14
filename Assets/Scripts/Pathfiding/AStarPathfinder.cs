using PushingBoxStudios.Pathfinding.PriorityQueues;
using System;
using System.Collections.Generic;

namespace PushingBoxStudios.Pathfinding
{
    internal class AStarPathfinder : AbstractPathfinder
    {
        private static readonly uint STRAIGHTCOST = 100;
        private static readonly uint DIAGONALCOST = 141;

        public override IPath FindPath(Grid grid, Location start, Location goal)
        {
            OnStarted();

            Statistics.Reset();
            Statistics.TotalGridNodes = grid.Width * grid.Height;
            Statistics.StartTimer();

            if (!ValidateGoal(grid, goal))
            {
                var p = new Path(start, start);
                p.PushBack(start);
                OnPathNotFound();

                return p;
            }

            var openList = new FibonacciHeap<uint, Location>(0);
            var isClosed = new bool[grid.Width, grid.Height];
            var soFarCost = new uint[grid.Width, grid.Height];
            var parents = new Location?[grid.Width, grid.Height];
            var queueNode = new IPair<uint, Location>[grid.Width, grid.Height];
            var hotspot = start;

            queueNode[start.X, start.Y] = openList.Push(0, hotspot);

            var adjacents = new Location[8];

            while (hotspot != goal)
            {
                Statistics.UpdateMaximumOpenNodes((uint)openList.Count);

                if (openList.Count <= 0)
                {
                    OnPathNotFound();
                    return null;
                }

                hotspot = openList.Pop().Value;
                isClosed[hotspot.X, hotspot.Y] = true;
                Statistics.AddClosedNode();

                SetAdjacentArray(adjacents, hotspot);

                for (var i = 0; i < adjacents.Length; i++)
                {
                    var pos = adjacents[i];

                    if (ProbeMode == EProbeMode.FourDirections
                        && pos.X != hotspot.X
                        && pos.Y != hotspot.Y)
                    {
                        continue;
                    }

                    if (grid.InBounds(pos)
                        && hotspot != pos
                        && grid[pos]
                        && !isClosed[pos.X, pos.Y])
                    {
                        if (!HasCornerBetween(grid, hotspot, pos)
                            && !HasDiagonalWall(grid, hotspot, pos))
                        {
                            if (queueNode[pos.X, pos.Y] == null)
                            {
                                parents[pos.X, pos.Y] = hotspot;

                                var scoreH = CalculateHeuristicCost(pos, goal);
                                var scoreG = soFarCost[hotspot.X, hotspot.Y] + CalculateDistance(pos, hotspot);
                                var score = scoreH + scoreG;

                                queueNode[pos.X, pos.Y] = openList.Push(score, pos);
                                soFarCost[pos.X, pos.Y] = scoreG;

                                Statistics.AddOpenedNode();
                            }
                            else
                            {
                                if (!parents[pos.X, pos.Y].HasValue)
                                {
                                    continue;
                                }

                                var currentParent = parents[pos.X, pos.Y].Value;
                                var currentScoreG = soFarCost[currentParent.X, currentParent.Y]
                                    + CalculateDistance(pos, currentParent);

                                var newScoreG = soFarCost[hotspot.X, hotspot.Y]
                                    + CalculateDistance(pos, hotspot);

                                if (newScoreG < currentScoreG)
                                {
                                    parents[pos.X, pos.Y] = hotspot;
                                    soFarCost[pos.X, pos.Y] = newScoreG;

                                    var score = newScoreG + CalculateHeuristicCost(pos, goal);

                                    openList.DecreaseKey(queueNode[pos.X, pos.Y], score);
                                }
                            }
                        }
                    }

                    OnIteration();
                    Statistics.AddIteration();
                }
            }

            Statistics.PathCost = soFarCost[hotspot.X, hotspot.Y];

            var inverter = new Stack<Location>();
            Location? aux = hotspot;

            while (aux.HasValue)
            {
                inverter.Push(aux.Value);
                aux = parents[aux.Value.X, aux.Value.Y];
            }

            var path = new Path(start, goal);

            while (inverter.Count > 0)
            {
                path.PushBack(inverter.Pop());
            }

            OnPathFound();
            Statistics.StopTimer();
            Statistics.PathLength = path.Size;

            return path;
        }

        private bool HasCornerBetween(Grid grid, Location from, Location to)
        {
            return grid[from] && grid[to] && ((grid[(uint)from.X, (uint)to.Y] && !grid[(uint)to.X, (uint)from.Y]) ||
                (!grid[(uint)from.X, (uint)to.Y] && grid[(uint)to.X, (uint)from.Y]));
        }

        private bool HasDiagonalWall(Grid grid, Location from, Location to)
        {
            return grid[from] && grid[to] && (!grid[(uint)from.X, (uint)to.Y] && !grid[(uint)to.X, (uint)from.Y]);
        }

        private bool ValidateGoal(Grid grid, Location goal)
        {
            if (goal.X < grid.Width && goal.Y < grid.Height)
            {
                return grid[(uint)goal.X, (uint)goal.Y];
            }

            return false;
        }

        private void SetAdjacentArray(Location[] adjacents, Location hotspot)
        {
            //adjacents[0].X = hotspot.X;
            //adjacents[0].Y = hotspot.Y - 1;

            //adjacents[1].X = hotspot.X;
            //adjacents[1].Y = hotspot.Y + 1;

            //adjacents[2].X = hotspot.X - 1;
            //adjacents[2].Y = hotspot.Y;

            //adjacents[3].X = hotspot.X + 1;
            //adjacents[3].Y = hotspot.Y;

            //adjacents[4].X = hotspot.X - 1;
            //adjacents[4].Y = hotspot.Y - 1;

            //adjacents[5].X = hotspot.X + 1;
            //adjacents[5].Y = hotspot.Y - 1;

            //adjacents[6].X = hotspot.X - 1;
            //adjacents[6].Y = hotspot.Y + 1;

            //adjacents[7].X = hotspot.X + 1;
            //adjacents[7].Y = hotspot.Y + 1;

            adjacents[0].X = hotspot.X - 1;
            adjacents[0].Y = hotspot.Y - 1;

            adjacents[1].X = hotspot.X + 1;
            adjacents[1].Y = hotspot.Y - 1;

            adjacents[2].X = hotspot.X - 1;
            adjacents[2].Y = hotspot.Y + 1;

            adjacents[3].X = hotspot.X + 1;
            adjacents[3].Y = hotspot.Y + 1;

            adjacents[4].X = hotspot.X;
            adjacents[4].Y = hotspot.Y - 1;

            adjacents[5].X = hotspot.X;
            adjacents[5].Y = hotspot.Y + 1;

            adjacents[6].X = hotspot.X - 1;
            adjacents[6].Y = hotspot.Y;

            adjacents[7].X = hotspot.X + 1;
            adjacents[7].Y = hotspot.Y;
        }

        private uint CalculateHeuristicCost(Location from, Location goal)
        {
            uint dX = (uint)Math.Abs(goal.X - from.X);
            uint dY = (uint)Math.Abs(goal.Y - from.Y);

            return STRAIGHTCOST * (dX + dY) + (DIAGONALCOST - 2 * STRAIGHTCOST) * Math.Min(dX, dY);
        }

        private uint CalculateDistance(Location pos, Location parent)
        {
            if (parent.X != pos.X && parent.Y != pos.Y)
            {
                return DIAGONALCOST;
            }

            return STRAIGHTCOST;
        }
    }
}
