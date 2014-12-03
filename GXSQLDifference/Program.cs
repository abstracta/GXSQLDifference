using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace GXSQLDifference
{
    internal class Program
    {
        private static Dictionary<string, SqlData> _preEjecValues = new Dictionary<string, SqlData>();
        private static Dictionary<string, SqlData> _postEjecValues = new Dictionary<string, SqlData>();

        private static void Main(string[] args)
        {
            string path1, path2;
            if (args.Length == 2)
            {
                path1 = args[0];
                path2 = args[1];
            }
            else
            {
                const string f1 = "before.xml";
                const string f2 = "after.xml";

                System.Console.WriteLine("File names missed. Using default file names in current folder: " + f1 + ", " + f2);

                const string path = @"";
                path1 = path + f1;
                path2 = path + f2;
            }

            var succes = LoadData(path1, path2);

            if (!succes)
            {
                System.Console.WriteLine("Couldn't load the data");
                return;
            }

            if (_preEjecValues.Count == 0)
            {
                System.Console.WriteLine("File is empty: " + path1);
                return;
            }

            if (_postEjecValues.Count == 0)
            {
                System.Console.WriteLine("File is empty: " + path2);
                return;
            }

            var result = "objeto;sql;totaltime(dif);count(dif);post average;pre averga;best time (post);worstTime(post)\n";
            foreach (var item in _postEjecValues)
            {
                // Poner condición para que el value no sea null
                if (_preEjecValues.ContainsKey(item.Key))
                {
                    var objectAndSql = item.Key.Split('@');
                    result += objectAndSql[0] + ";" + objectAndSql[1] + ";" +
                              item.Value.GetDifData(_preEjecValues[item.Key]);
                }
                else
                {
                    var objectAndSql = item.Key.Split('@');
                    result += objectAndSql[0] + ";" + objectAndSql[1] + ";" + item.Value.GetDifData(null);
                }

                result += "\n";
            }

            System.Console.Write(result);
            var fs = new FileStream(@"salida.csv", FileMode.Create);
            var sw = new StreamWriter(fs);
            sw.Write(result);
            sw.Close();
        }

        private static bool LoadData(string f1, string f2)
        {
            if (File.Exists(f1))
            {
                _preEjecValues = LoadValuesFromXml(f1);
            }
            else
            {
                System.Console.WriteLine("File doesn't exists: " + f1);
                return false;
            }

            if (File.Exists(f2))
            {
                _postEjecValues = LoadValuesFromXml(f2);
            }
            else
            {
                System.Console.WriteLine("File doesn't exists: " + f2);
                return false;
            }

            return true;
        }

        private static Dictionary<string, SqlData> LoadValuesFromXml(string path)
        {
            var result = new Dictionary<string, SqlData>();
            var doc = new XmlDocument();
            doc.Load(path);

            var xmlElement = doc["DataStoreProviders_Information"];
            if (xmlElement != null)
            {
                foreach (XmlNode node in xmlElement.GetElementsByTagName("DataStoreProvider"))
                {
                    if (node.Attributes == null) continue;

                    var objectNAme = node.Attributes["Name"].Value;
                    foreach (XmlNode sqlNode in node.ChildNodes)
                    {
                        if (sqlNode.Name != "SQLStatement") continue;

                        var element = sqlNode["SQLStatement"];
                        if (element == null) continue;

                        var sql = element.InnerText;
                        if (string.IsNullOrEmpty(sql)) continue;

                        var datos = new SqlData
                            {
                                TotalTime = GetDoubleOrDefault(sqlNode, "TotalTime", -1),
                                WorstTime = GetDoubleOrDefault(sqlNode, "WorstTime", -1),
                                BestTime = GetDoubleOrDefault(sqlNode, "BestTime", -1),
                                Average = GetDoubleOrDefault(sqlNode, "AverageTime", -1),
                                Count = GetDoubleOrDefault(sqlNode, "Count", -1),
                            };

                        var key = objectNAme + "@" + sql;
                        if (!result.ContainsKey(key))
                        {
                            result.Add(key, datos);
                        }
                    }
                }
            }

            return result;
        }

        private static double GetDoubleOrDefault(XmlNode sqlNode, string attrName, int defaultValue)
        {
            var xmlElement = sqlNode[attrName];
            return xmlElement != null ? double.Parse(xmlElement.InnerText) : defaultValue;
        }
    }
}