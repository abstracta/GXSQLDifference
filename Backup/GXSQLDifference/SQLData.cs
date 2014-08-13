using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXSQLDifference
{
    class SQLData
    {
        public double TotalTime { get; set; }
        public double Count { get; set; }
        public double Average { get; set; }
        public double WorstTime { get; set; }
        public double BestTime { get; set; }


        internal string GetDifData(SQLData sQLData)
        {
            string result = "";
            if (sQLData!=null)
            {
                //totaltime(dif);count(dif);post average;pre averga;best time (post);worstTime(post)";
                result = (TotalTime - sQLData.TotalTime) + ";" + (Count - sQLData.Count) + ";" + Average + ";" + sQLData.Average + ";" + BestTime + ";" + WorstTime;

            }else
	        {
                result = TotalTime+";"+Count+";"+Average+";"+0+";"+BestTime+";"+WorstTime;
	        }
            return result;
        }
    }
}
