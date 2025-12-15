using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IronPDF.Challenge.Models
{
    public class DoughnutChartDataset
    {
        [JsonProperty("data")]
        public List<int> Data { get; set; }

        [JsonProperty("backgroundColor")]
        public List<string> BackgroundColor { get; set; }

        [JsonProperty("borderWidth")]
        public int BorderWidth { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
