using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace GXSQLDifference
{
    class Program
    {
       static Dictionary<string, SQLData> preEjecValues;
       static Dictionary<string, SQLData> postEjecValues;
        static void Main(string[] args)
        {
            if (args.Length!=0)
            {
                string path1 = args[0];
                string path2 = args[1];
                preEjecValues = LoadValuesFromXml(path1);
                postEjecValues = LoadValuesFromXml(path2);
            }
            else
            {
                LoadData();
            }
            
            string result = "objeto;sql;totaltime(dif);count(dif);post average;pre averga;best time (post);worstTime(post)\n";
            foreach (KeyValuePair<string, SQLData> item in postEjecValues)
            {
                //Poner condición para que el value no sea null
                if (preEjecValues.ContainsKey(item.Key))
                {                    
                    
                    string[] objectAndSql = item.Key.Split('@');
                    result += objectAndSql[0] + ";" + objectAndSql[1] + ";" + item.Value.GetDifData(preEjecValues[item.Key]);
                }
                else
                {
                    string[] objectAndSql = item.Key.Split('@');
                    result += objectAndSql[0] + ";" + objectAndSql[1] + ";" + item.Value.GetDifData(null);
                }
                result += "\n";
            }
            System.Console.Write(result);
            string pathBase2 = @"C:\Users\andrei\Abstracta\Unifruti\etapa del medio - Noviembre\Consulta de pallets - filtro especie - Cambiar de pagina\";
            FileStream fs = new FileStream(pathBase2 + @"salida.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(result);
            sw.Close();

            }

        private static void LoadData()
        {
            string pathBase = @"C:\Users\andrei\Abstracta\Unifruti\etapa del medio - Noviembre\Consulta de pallets - filtro especie - Cambiar de pagina\";
            preEjecValues = LoadValuesFromXml(pathBase + @"DataStoreProviders_20131122152414_a.xml");
            postEjecValues = LoadValuesFromXml(pathBase + @"DataStoreProviders_20131122155618_d.xml");
        }

        private static Dictionary<string, SQLData> LoadValuesFromXml(string path)
        {
            Dictionary<string, SQLData> result = new Dictionary<string, SQLData>();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            foreach (XmlNode node in doc["DataStoreProviders_Information"].GetElementsByTagName("DataStoreProvider"))
            {
                string objectNAme = node.Attributes["Name"].Value;
                foreach (XmlNode sqlNode in node.ChildNodes)
                {
                    if (sqlNode.Name == "SQLStatement")
                    {
                        string sql = sqlNode["SQLStatement"].InnerText;
                        if (!string.IsNullOrEmpty(sql))
                        {
                            SQLData datos = new SQLData();
                            datos.TotalTime = double.Parse(sqlNode["TotalTime"].InnerText);
                            datos.WorstTime = double.Parse(sqlNode["WorstTime"].InnerText);
                            datos.BestTime = double.Parse(sqlNode["BestTime"].InnerText);
                            datos.Average = double.Parse(sqlNode["AverageTime"].InnerText);
                            datos.Count = double.Parse(sqlNode["Count"].InnerText);


                            string key = objectNAme + "@" + sql;
                            if (!result.ContainsKey(key))
                            {
                                result.Add(key, datos);
                                
                            }
                        }
                    }
                    
                }
                    

                
            }
            return result;
        }

        


    }
}
