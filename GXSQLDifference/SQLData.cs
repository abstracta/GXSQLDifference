namespace GXSQLDifference
{
    internal class SqlData
    {
        public double TotalTime { get; set; }
        public double Count { get; set; }
        public double Average { get; set; }
        public double WorstTime { get; set; }
        public double BestTime { get; set; }
        
        internal string GetDifData(SqlData sqlData)
        {
            string result;
            if (sqlData != null)
            {
                //totaltime(dif);count(dif);post average;pre averga;best time (post);worstTime(post)";
                result = (TotalTime - sqlData.TotalTime) + ";" +
                         (Count - sqlData.Count) + ";" +
                         Average + ";" +
                         sqlData.Average + ";" +
                         ((BestTime < sqlData.BestTime) ? BestTime : sqlData.BestTime) + ";" +
                         ((WorstTime > sqlData.WorstTime) ? WorstTime : sqlData.WorstTime) + ";";

            }
            else
	        {
                result = TotalTime+";"+Count+";"+Average+";"+0+";"+BestTime+";"+WorstTime;
	        }
            
            return result;
        }
    }
}
