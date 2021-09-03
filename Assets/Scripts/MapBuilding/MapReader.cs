using UnityEngine;

namespace Assets.Scripts.MapBuilding
{
    public class MapReader
    {
        public static MapTileType[,] Read(Texture2D mapImage)
        {
            var tileColors = mapImage.GetPixels();
            var tiles = new MapTileType[mapImage.width, mapImage.height];

            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    int tileColorsIndex = ((mapImage.height - 1) - y) * mapImage.width + x;

                    tiles[x, y] = ConvertToTile(tileColors[tileColorsIndex]);
                }
            }

            return tiles;
        }

        private static MapTileType ConvertToTile(Color tileColor)
        {
            var nebulaColor = Color.green;
            var asteroidColor = Color.magenta;
            var wormholeColor = Color.cyan;
            var glacialPlanetColor = Color.white;
            var terranPlanetColor = Color.blue;
            var aridPlanetColor = Color.red;

            if (tileColor == nebulaColor)
            {
                return MapTileType.Nebula;
            }

            if (tileColor == asteroidColor)
            {
                return MapTileType.Asteroid;
            }

            if (tileColor == wormholeColor)
            {
                return MapTileType.Wormhole;
            }

            if (tileColor == glacialPlanetColor)
            {
                return MapTileType.GlacialPlanet;
            }

            if (tileColor == terranPlanetColor)
            {
                return MapTileType.TerranPlanet;
            }

            if (tileColor == aridPlanetColor)
            {
                return MapTileType.AridPlanet;
            }

            return MapTileType.Empty;
        }
    }
}
