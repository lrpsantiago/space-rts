using UnityEngine;

namespace PushingBoxStudios.SteampunkTd.Maps
{
    public class MapReader
    {
        public static bool[,] Read(Texture2D mapImage)
        {
            var tileColors = mapImage.GetPixels();
            var tiles = new bool[mapImage.width, mapImage.height];

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

        private static bool ConvertToTile(Color tileColor)
        {
            return !(tileColor == Color.black);
        }
    }
}
