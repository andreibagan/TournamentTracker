using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TournamentTracker.Attributes;

namespace TrackerLibrary.DataAccess
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
            var cols = entry.GetType().GetProperties().Where(p => p.CanWrite && !p.IsDefined(typeof(ListDefinedAttribute), false)).ToList();

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
                            if (!col.IsDefined(typeof(PropertyAttribute), false) && vals[i] != "null")
                            {
                                col.SetValue(entry, Convert.ChangeType(vals[i], col.PropertyType));
                                break;
                            }
                            else
                            if (vals[i] == "null")
                            {
                                col.SetValue(entry, Convert.ChangeType(null, col.PropertyType));
                                break;
                            }
                            else
                            if (col.IsDefined(typeof(PropertyAttribute), false))
                            {
                                var propertyAttribute = (PropertyAttribute)col.GetCustomAttributes(typeof(PropertyAttribute), false).First();
                                var propertyInfo = col.PropertyType.GetProperty(propertyAttribute.PropertyName);

                                if (propertyInfo == null)
                                {
                                    throw new NullReferenceException("Invalid attribute property name.");
                                }

                                var valueInstance = Activator.CreateInstance(col.PropertyType);
                                propertyInfo.SetValue(valueInstance, Convert.ChangeType(vals[i], propertyInfo.PropertyType));

                                col.SetValue(entry, Convert.ChangeType(valueInstance, col.PropertyType));
                                break;
                            }
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

            var cols = data[0].GetType().GetProperties().Where(p => !p.IsDefined(typeof(ListDefinedAttribute), false)).ToList();

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
                    if (col.IsDefined(typeof(PropertyAttribute), false))
                    {
                        var property = (PropertyAttribute)col.GetCustomAttributes(typeof(PropertyAttribute), false).First();

                        var propertyInfo = col.PropertyType.GetProperty(property.PropertyName);

                        if (propertyInfo == null)
                        {
                            throw new NullReferenceException("Invalid attribute property name.");
                        }

                        var propertyValue = col.GetValue(row);

                        if (propertyValue != null)
                        {
                            line.Append(propertyInfo.GetValue(propertyValue));
                        }
                        else
                        {
                            line.Append("null");
                        }
                    }
                    else
                    {
                        line.Append(col.GetValue(row));
                    }

                    line.Append(",");
                }

                lines.Add(line.ToString().Substring(0, line.Length - 1));
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
