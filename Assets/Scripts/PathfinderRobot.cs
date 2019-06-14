using PushingBoxStudios.Pathfinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts
{
    public class PathfinderRobot : MonoBehaviour
    {
        private AbstractPathfinder _pathfinder;
        private Animator _animator;
        private bool _doWanderInvoked;
        private Grid _grid;
        private List<Location> _customLocations;

        [SerializeField]
        private MapBuilder _mapBuilder;

        [SerializeField]
        private float _speed = 1.5f;

        [SerializeField]
        private int _rotationSpeed = 360;

        [SerializeField]
        private bool _isWandering = false;

        [SerializeField]
        private string _customLocationsFile;

        public IPath CurrentPath { get; private set; }

        public event EventHandler PathFound;

        public void Awake()
        {
            if (_mapBuilder != null)
            {
                _mapBuilder.MapBuilt += OnMapBuilt;
            }

            LoadCustomLocations();
        }

        public void Start()
        {
            _animator = GetComponent<Animator>();
            _grid = _mapBuilder.Grid;
            _pathfinder = new AStarPathfinder();
        }

        public void Update()
        {
            if (CurrentPath == null || CurrentPath.Size == 0)
            {
                if (_isWandering && !_doWanderInvoked)
                {
                    _doWanderInvoked = true;
                    Invoke("DoWander", 1);
                }

                _animator.SetBool("IsWalking", false);
                return;
            }

            _animator.SetBool("IsWalking", true);

            var pos = transform.position;
            var targetPos = _mapBuilder.GridToSpace(CurrentPath.Front);
            var velocity = _speed * Time.deltaTime;

            if (pos == targetPos || Vector3.Distance(pos, targetPos) <= velocity)
            {
                CurrentPath.PopFront();

                if (CurrentPath.Size > 0)
                {
                    targetPos = _mapBuilder.GridToSpace(CurrentPath.Front);
                }
                else
                {
                    pos = targetPos;
                }
            }

            //while (_currentPath.Size > 1)
            //{
            //    _currentPath.PopFront();
            //}

            //pos = _mapBuilder.GridToSpace(_currentPath.Front);
            //_currentPath.PopFront();

            pos = Vector3.MoveTowards(pos, targetPos, velocity);
            transform.position = pos;

            UpdateRotation(targetPos);
        }

        public void OnDestroy()
        {
            StatisticsRecorder.Instance.SaveAsCsvFile("PathfindingStatistics");
            LocationRecorder.Instance.SaveAsCsvFile("PathfindingLocations");
        }

        public void FindPath(Vector3 pos)
        {
            var start = _mapBuilder.SpaceToGrid(transform.position);
            var goal = _mapBuilder.SpaceToGrid(pos);

            if (start.Equals(goal)
                || CurrentPath != null && CurrentPath.Goal == goal)
            {
                return;
            }

            var pathfinderThread = new PathfindingThread(_pathfinder, _grid, start, goal, ThreadCallback);
            var thread = new Thread(pathfinderThread.Run);

            thread.Start();
            ////_currentPath = _pathfinder.FindPath(_grid, start, goal);

            //if (_currentPath == null)
            //{
            //    return;
            //}

            //var statistics = _pathfinder.Statistics.Record();
            //StatisticsRecorder.Instance.Add(statistics);
            //LocationRecorder.Instance.Add(goal);

            //OnPathFound();
            //_currentPath.PopFront();
        }

        private void ThreadCallback(IPath path)
        {
            CurrentPath = path;

            if (CurrentPath == null)
            {
                return;
            }

            var statistics = _pathfinder.Statistics.Record();
            StatisticsRecorder.Instance.Add(statistics);
            //LocationRecorder.Instance.Add(goal);

            //OnPathFound();

            var myLocation = _mapBuilder.SpaceToGrid(transform.position);
            CurrentPath.DiscardUpTo(myLocation);
        }

        private void UpdateRotation(Vector3 targetPos)
        {
            var dir = targetPos - transform.position;
            var toRot = Quaternion.LookRotation(dir, Vector3.up);
            var fromRot = transform.rotation;
            var rot = Quaternion.RotateTowards(fromRot, toRot, _rotationSpeed * Time.deltaTime);

            transform.rotation = rot;
        }

        private void DoWander()
        {
            _doWanderInvoked = false;

            if (_customLocations != null && _customLocations.Count <= 0)
            {
                return;
            }

            Location location;

            if (_customLocations == null)
            {
                location = GetRandomLocation();
            }
            else
            {
                location = _customLocations[0];
                _customLocations.RemoveAt(0);
            }

            var pos = _mapBuilder.GridToSpace(location);

            FindPath(pos);
        }

        private Location GetRandomLocation()
        {
            var randLocation = new Location();

            do
            {
                randLocation.X = UnityEngine.Random.Range(0, (int)_grid.Width);
                randLocation.Y = UnityEngine.Random.Range(0, (int)_grid.Width);
            }
            while (!_grid[randLocation]);

            return randLocation;
        }

        private void LoadCustomLocations()
        {
            if (string.IsNullOrEmpty(_customLocationsFile))
            {
                return;
            }

            if (_customLocations == null)
            {
                _customLocations = new List<Location>();
            }

            _customLocations.Clear();

            var lines = File.ReadAllLines(_customLocationsFile);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var values = line.Split(';');
                var location = new Location
                {
                    X = Convert.ToInt32(values[1]),
                    Y = Convert.ToInt32(values[2])
                };

                _customLocations.Add(location);
            }
        }

        protected virtual void OnPathFound()
        {
            if (PathFound != null)
            {
                PathFound(this, EventArgs.Empty);
            }
        }

        private void OnMapBuilt(object sender, EventArgs e)
        {
            transform.position = _mapBuilder.Spawnpoint;
        }
    }
}
