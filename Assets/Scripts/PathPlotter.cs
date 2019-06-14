using PushingBoxStudios.Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PathPlotter : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pathPointPrefab;

        [SerializeField]
        private GameObject _pathLinePrefab;

        [SerializeField]
        private GameObject _playerRobot;

        private void Start()
        {
            if (_playerRobot != null)
            {
                var robot = _playerRobot.GetComponent<PathfinderRobot>();
                robot.PathFound += OnPlayerRobotPathFound;
            }
        }

        private void OnPlayerRobotPathFound(object sender, EventArgs e)
        {
            var robot = (PathfinderRobot)sender;

            PlotPath(robot.CurrentPath);
        }

        private void PlotPath(IPath path)
        {
            var pathTransform = transform.Find("Path");

            if (pathTransform != null)
            {
                DestroyImmediate(pathTransform.gameObject);
            }

            var pathObj = new GameObject("Path");
            var pathToPlot = path.Clone();
            pathToPlot = RemoveEdges(pathToPlot);

            if (pathToPlot.Size > 0)
            {
                var map = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();
                var from = map.GridToSpace(pathToPlot.Front);

                var pointObj = Instantiate(_pathPointPrefab);
                pointObj.transform.position = from;
                pointObj.transform.SetParent(pathObj.transform);

                pathToPlot.PopFront();

                while (pathToPlot.Size > 0)
                {
                    var location = pathToPlot.Front;
                    var pos = map.GridToSpace(location);

                    pointObj = Instantiate(_pathPointPrefab);
                    pointObj.transform.position = pos;
                    pointObj.transform.SetParent(pathObj.transform);

                    var lineObj = Instantiate(_pathLinePrefab);

                    var line = lineObj.GetComponent<LineRenderer>();
                    line.positionCount = 2;
                    line.SetPosition(0, from);
                    line.SetPosition(1, pos);

                    lineObj.transform.SetParent(pathObj.transform);

                    pathToPlot.PopFront();
                    from = pos;
                }
            }

            pathObj.transform.SetParent(transform);
        }

        public IPath RemoveEdges(IPath path)
        {
            var waypoints = path.ToList();
            var waypointsToRemove = new List<Location>();

            for (int i = 1; i < waypoints.Count - 1; i++)
            {
                var hotspot = waypoints[i];
                var prev = waypoints[i - 1];
                var next = waypoints[i + 1];
                var prevDirection = new Location(prev.X - hotspot.X, prev.Y - hotspot.Y);
                var nextDirection = new Location(hotspot.X - next.X, hotspot.Y - next.Y);

                if (prevDirection == nextDirection)
                {
                    waypointsToRemove.Add(hotspot);
                }
            }

            for (int i = 0; i < waypointsToRemove.Count; i++)
            {
                waypoints.Remove(waypointsToRemove[i]);
            }

            var finalPath = new Path(waypoints[0], waypoints[waypoints.Count - 1]);

            foreach (var w in waypoints)
            {
                finalPath.PushBack(w);
            }

            return finalPath;
        }
    }
}
