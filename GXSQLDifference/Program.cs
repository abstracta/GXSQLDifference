using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace GXSQLDifference
{
    internal class Program
    {
        private static Dictionary<string, SqlData> _preEjecValues;
        private static Dictionary<string, SqlData> _postEjecValues;

        private static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                var path1 = args[0];
                var path2 = args[1];

                if (File.Exists(path1))
                {
                    _preEjecValues = LoadValuesFromXml(path1);
                }
                else
                {
                    System.Console.WriteLine("File doesn't exists: " + path1);
                }

                if (File.Exists(path2))
                {
                    _postEjecValues = LoadValuesFromXml(path2);
                }
                else
                {
                    System.Console.WriteLine("File doesn't exists: " + path2);
                }
            }
            else
            {
                LoadData();
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

        private static void LoadData()
        {
            const string f1 = @"DataStoreProviders_20131122152414_a.xml";
            const string f2 = @"DataStoreProviders_20131122152414_a.xml";

            if (File.Exists(f1))
            {
                _preEjecValues = LoadValuesFromXml(f1);
            }
            else
            {
                System.Console.WriteLine("File doesn't exists: " + f1);
            }

            if (File.Exists(f2))
            {
                _postEjecValues = LoadValuesFromXml(f2);
            }
            else
            {
                System.Console.WriteLine("File doesn't exists: " + f2);
            }
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