using Newtonsoft.Json;

namespace IronPDF.Challenge.Models
{
    public class DoughnutChartData
    {
        [JsonProperty("datasets")]
        public DoughnutChartDataset Datasets { get; set; }
    }
}
