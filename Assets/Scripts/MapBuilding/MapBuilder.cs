using PushingBoxStudios.Pathfinding;
using PushingBoxStudios.SteampunkTd.Cameras;
using System;
using UnityEngine;
using Grid = PushingBoxStudios.Pathfinding.Grid;

namespace Assets.Scripts.MapBuilding
{
    public class MapBuilder : MonoBehaviour
    {
        [SerializeField]
        private GameObject _terranPlanetPrefab;

        [SerializeField]
        private GameObject _aridPlanetPrefab;

        [SerializeField]
        private GameObject _glacialPlanetPrefab;

        [SerializeField]
        private GameObject _jovianPlanetPrefab;

        [SerializeField]
        private GameObject _wormholePrefab;
        
        [SerializeField]
        private GameObject _asteroidPrefab;

        [SerializeField]
        private GameObject _nebulaPrefab;

        [SerializeField]
        private Texture2D _mapTexture;

        private MapTileType[,] _tiles;

        public Grid Grid { get; private set; }

        public Vector3 Spawnpoint { get; private set; }

        public event EventHandler MapBuilt;

        private void Awake()
        {
            _tiles = MapReader.Read(_mapTexture);
            Grid = GenerateGrid(_tiles);
        }

        private void Start()
        {
            var rtsCam = GameObject.Find("RtsCamera")
                .GetComponent<RtsCameraHandler>();

            rtsCam.MinBounds = new Vector3(-Grid.Width / 2, 0, -Grid.Width / 2);
            rtsCam.MaxBounds = new Vector3(Grid.Width / 2, 0, Grid.Width / 2);

            CreatePlanets();
            CreateWormholes();
            CreateAsteroids();
            CreateNebula();
            OnMapBuilt();
        }

        private void CreatePlanets()
        {
            //var location = new Location();

            //for (location.Y = 0; location.Y < Grid.Height; location.Y++)
            //{
            //    for (location.X = 0; location.X < Grid.Width; location.X++)
            //    {
            //        if (Grid[location])
            //        {
            //            continue;
            //        }

            //        var pos = GridToSpace(location);

            //        Instantiate(_obstaclePrefab, pos, Quaternion.identity);
            //    }
            //}
        }
        
        private void CreateWormholes()
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                for (int x = 0; x < Grid.Width; x++)
                {
                    if (_tiles[x, y] != MapTileType.Wormhole)
                    {
                        continue;
                    }

                    var pos = GridToSpace(x, y);
                    Instantiate(_wormholePrefab, pos, Quaternion.identity);
                }
            }
        }

        private void CreateNebula()
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                for (int x = 0; x < Grid.Width; x++)
                {
                    if (_tiles[x, y] != MapTileType.Nebula)
                    {
                        continue;
                    }

                    var pos = GridToSpace(x, y);
                    Instantiate(_nebulaPrefab, pos, Quaternion.identity);
                }
            }
        }

        private void CreateAsteroids()
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                for (int x = 0; x < Grid.Width; x++)
                {
                    if (_tiles[x, y] != MapTileType.Asteroid)
                    {
                        continue;
                    }

                    var pos = GridToSpace(x, y);
                    Instantiate(_asteroidPrefab, pos, Quaternion.identity);
                }
            }
        }

        public Vector3 GridToSpace(Location location)
        {
            return GridToSpace(location.X, location.Y);
        }

        public Vector3 GridToSpace(int x, int y)
        {
            return new Vector3
            {
                x = x - (Grid.Width / 2),
                y = 0,
                z = -y + (Grid.Height / 2)
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

        private Grid GenerateGrid(MapTileType[,] tiles)
        {
            var nodes = new bool[tiles.GetLength(0), tiles.GetLength(1)];
            var width = tiles.GetLength(0);
            var height = tiles.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = tiles[x, y];

                    nodes[x, y] = (tile == MapTileType.AridPlanet
                        || tile == MapTileType.GlacialPlanet
                        || tile == MapTileType.TerranPlanet);
                }
            }

            return new Grid(nodes);
        }
    }
}
