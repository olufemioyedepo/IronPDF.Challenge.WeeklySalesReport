namespace IronPDF.Challenge.Models
{
    public class WeeklyReportChartModelData
    {
        public List<string> labels { get; set; }
        public List<WeeklyReportChartModelDataset> datasets { get; set; }
    }
}
