using PushingBoxStudios.Pathfinding;
using PushingBoxStudios.Pathfinding.PriorityQueues;
using PushingBoxStudios.SteampunkTd.Cameras;
using PushingBoxStudios.SteampunkTd.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapBuilder : MonoBehaviour
    {
        [SerializeField]
        private GameObject _ground;

        [SerializeField]
        private GameObject _obstaclePrefab;

        [SerializeField]
        private string _mapImageFile;

        public Grid Grid { get; private set; }

        public Vector3 Spawnpoint { get; private set; }

        public event EventHandler MapBuilt;

        private void Awake()
        {
            var tex = Resources.Load<Texture2D>("Textures/Maps/" + _mapImageFile);
            var nodes = MapReader.Read(tex);
            Grid = new Grid(nodes);
        }

        private void Start()
        {
            var rtsCam = GameObject.Find("RtsCamera")
                .GetComponent<RtsCameraHandler>();

            rtsCam.MinBounds = new Vector3(-Grid.Width / 2, 0, -Grid.Width / 2);
            rtsCam.MaxBounds = new Vector3(Grid.Width / 2, 0, Grid.Width / 2);

            SetupGround();
            CreateObstacles();
            SetSpawnpoint();
            OnMapBuilt();
        }

        private void SetSpawnpoint()
        {
            var startX = (int)Grid.Width / 2;
            var startY = (int)Grid.Height / 2;
            var hotspot = new Location(startX, startY);
            var frontier = new PushingBoxStudios.Pathfinding.PriorityQueues.SortedList<int, Location>();
            var closedList = new List<Location>();
            var location = new Location();

            while (!Grid[hotspot])
            {
                for (int y = hotspot.Y - 1; y <= hotspot.Y + 1; y++)
                {
                    for (int x = hotspot.X - 1; x <= hotspot.X + 1; x++)
                    {
                        location.X = x;
                        location.Y = y;

                        if (hotspot.Equals(location) || closedList.Contains(location))
                        {
                            continue;
                        }

                        var key = Math.Abs(x - startX) + Math.Abs(y - startY);
                        frontier.Push(key, new Location(x, y));
                    }
                }

                hotspot = frontier.Pop().Value;
                closedList.Add(hotspot);
            }

            Spawnpoint = GridToSpace(hotspot);
        }

        private void SetupGround()
        {
            var scale = new Vector3
            {
                x = (float)Grid.Width / 10,
                y = 1,
                z = (float)Grid.Height / 10
            };

            _ground.transform.localScale = scale;
            var pos = _ground.transform.position;

            if (Grid.Width % 2 == 0)
            {
                pos.x -= 0.5f;
            }

            if (Grid.Height % 2 == 0)
            {
                pos.z += 0.5f;
            }

            _ground.transform.position = pos;

            var renderer = _ground.GetComponent<Renderer>();
            renderer.material.mainTextureScale = new Vector2(Grid.Width, Grid.Height);

            var collider = _ground.AddComponent<MeshCollider>();
            collider.enabled = true;
        }

        private void CreateObstacles()
        {
            var location = new Location();

            for (location.Y = 0; location.Y < Grid.Height; location.Y++)
            {
                for (location.X = 0; location.X < Grid.Width; location.X++)
                {
                    if (Grid[location])
                    {
                        continue;
                    }

                    var pos = GridToSpace(location);

                    Instantiate(_obstaclePrefab, pos, Quaternion.identity);
                }
            }
        }

        public Vector3 GridToSpace(Location location)
        {
            return new Vector3
            {
                x = location.X - (Grid.Width / 2),
                y = 0,
                z = -location.Y + (Grid.Height / 2)
            };
        }

        public Location SpaceToGrid(Vector3 pos)
        {
            return new Location
            {
                X = Mathf.RoundToInt(pos.x) + (int)Grid.Width / 2,
                Y = Mathf.RoundToInt(-pos.z) + (int)Grid.Height / 2
            };
        }

        protected virtual void OnMapBuilt()
        {
            if (MapBuilt != null)
            {
                MapBuilt(this, EventArgs.Empty);
            }
        }
    }
}
