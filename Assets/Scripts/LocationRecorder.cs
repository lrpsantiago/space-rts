using PushingBoxStudios.Pathfinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class LocationRecorder
    {
        public static LocationRecorder _instance;
        private IList<Location> _records;

        public static LocationRecorder Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocationRecorder();
                }

                return _instance;
            }
        }

        public LocationRecorder()
        {
            _records = new List<Location>();
        }

        public void ClearRecords()
        {
            _records.Clear();
        }

        public void Add(Location statistics)
        {
            _records.Add(statistics);

            if (_records.Count >= 10000)
            {
                Debug.Break();
            }
        }

        public void SaveAsCsvFile(string fileName)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Index;X;Y;");

            for (int i = 0; i < _records.Count; i++)
            {
                var line = CreateCsvLine(i, _records[i]);
                builder.AppendLine(line);
            }

            var strContent = builder.ToString();
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            File.WriteAllText(desktopPath + "\\" + fileName + ".csv", strContent);
        }

        private string CreateCsvLine(int index, Location statstics)
        {
            return index + ";" +
                statstics.X + ";" +
                statstics.Y + ";";
        }
    }
}
