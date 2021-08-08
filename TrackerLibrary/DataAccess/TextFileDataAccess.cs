using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TrackerLibrary;

namespace TournamentTracker.DataAccess
{
    public class TextFileDataAccess : ITextFileDataAccess
    {
        public List<T> LoadFromTextFile<T>(string fileName) where T : new()
        {
            var filePath = GlobalConfig.GetFullFilePath(fileName);

            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            var lines = File.ReadAllLines(filePath).ToList();
            List<T> output = new List<T>();
            T entry = new T();
            var cols = entry.GetType().GetProperties();

            if (lines.Count < 2)
            {
                throw new IndexOutOfRangeException("The file was either empty or missing.");
            }

            var headers = lines[0].Split(',');

            lines.RemoveAt(0);

            foreach (var line in lines)
            {
                entry = new T();
                var vals = line.Split(',');

                for (int i = 0; i < headers.Length; i++)
                {
                    foreach (var col in cols)
                    {
                        if (col.Name == headers[i])
                        {
                            col.SetValue(entry, Convert.ChangeType(vals[i], col.PropertyType));
                        }
                    }
                }

                output.Add(entry);
            }

            return output;
        }

        public void SaveToTextFile<T>(List<T> data, string fileName)
        {
            var filePath = GlobalConfig.GetFullFilePath(fileName);
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();

            if (data == null || data.Count == 0)
            {
                throw new ArgumentException(nameof(data), "The data was either empty or null");
            }

            var cols = data[0].GetType().GetProperties();

            foreach (var col in cols)
            {
                line.Append(col.Name);
                line.Append(",");
            }

            lines.Add(line.ToString().Substring(0, line.Length - 1));

            foreach (var row in data)
            {
                line.Clear();

                foreach (var col in cols)
                {
                    line.Append(col.GetValue(row));
                    line.Append(",");
                }

                lines.Add(line.ToString().Substring(0, line.Length - 1));
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
