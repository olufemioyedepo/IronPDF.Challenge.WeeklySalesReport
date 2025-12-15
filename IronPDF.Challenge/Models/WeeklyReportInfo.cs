namespace IronPDF.Challenge.Models
{
    public class WeeklyReportInfo
    {
        public string Metric { get; set; }
        public double ThisWeekSales { get; set; }
        public double LastWeekSales { get; set; }
        public double PercentageIncrement { get; set; }
    }
}
