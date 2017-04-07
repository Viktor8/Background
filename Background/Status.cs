using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Background
{
    class Status
    {
        public static bool NowUpdating { get; set; }
        public static string Message { get; set; } 
        public static DateTime LastSucces { get; set; }
        public static DateTime LastTry { get; set; }
        public static int TotalCount { get; set; }
        public static int SuccesCount { get; set; }

        public static string GetReport()
        {
            StringBuilder sb = new StringBuilder();
            if (NowUpdating)
                sb.AppendLine("Himawari (updating)");
            else
                sb.AppendLine("Himawari"); ;
            sb.Append("Last succes update ");
            sb.AppendLine(LastSucces != DateTime.MinValue ? LastSucces.ToString("HH:mm:ss") : "never");
            sb.Append("Next update ");
            sb.AppendLine(LastTry.AddMinutes(10).ToString("HH:mm:ss"));
            sb.Append("Total count: ");
            sb.Append(TotalCount.ToString());
            sb.Append(", success: ");
            sb.Append(SuccesCount);
            return sb.ToString();
        }
    }
}
