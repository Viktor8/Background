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
		public static DateTime? LastSuccess { get; set; }
		public static DateTime LastAttempt { get; set; }
		public static int TotalCount { get; set; }
		public static int SuccesCount { get; set; }

		public static string GetReport()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"Himawari{(NowUpdating ? " (update in progress)" : "")}");
			sb.AppendLine($"Last update: {LastSuccess?.ToString("HH:mm:ss") ?? "-"}");
			sb.AppendLine($"Next update: {LastAttempt.AddMinutes(10):HH:mm:ss}");
			sb.AppendLine($"Total attempts: {TotalCount}, success: {SuccesCount}");
			return sb.ToString();
		}
	}
}
