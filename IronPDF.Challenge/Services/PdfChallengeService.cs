using Grpc.Core;
using IronPdf.Engines.Chrome;
using IronPDF.Challenge.Constants;
using IronPDF.Challenge.Models;
using Newtonsoft.Json;
using System.Text;

namespace IronPDF.Challenge.Services
{
    public static class PdfChallengeService
    {
        public static PdfReportFileInfo GenerateWeeklyReportdf()
        {
            var renderer = new ChromePdfRenderer();
            
            string imagePath = $"<img src='{IronPdfChallengeConstants.COMPANY_LOGO_URL}' alt='{IronPdfChallengeConstants.COMPANY_LOGO_TEXT}' width='200' />";
            string htmlContent = ReadHtmlTemplate(@"Assets\html\WeeklyReport.html");

            var barChartData = GetReportBarChartModelData();
            string keyMetricsDataStringContent = GetWeeklyKeyMetricsData();
            string dateCovered = GenerateDateCovered();
            string challengesBulletPoint = GetChallengesBulletPointListText();

            htmlContent = htmlContent.Replace("@ChartData", JsonConvert.SerializeObject(barChartData, Formatting.Indented));
            htmlContent = htmlContent.Replace("@WeeklySummaryTableData", keyMetricsDataStringContent);
            htmlContent = htmlContent.Replace("@companyLogo", imagePath);
            htmlContent = htmlContent.Replace("@dateCovered", dateCovered);
            htmlContent = htmlContent.Replace("@ChallengesSectionText", challengesBulletPoint);


            renderer.RenderingOptions.MarginTop = 2.5;
            renderer.RenderingOptions.MarginLeft = 8;
            HtmlHeaderFooter htmlFooter = new()
            {
                HtmlFragment = IronPdfChallengeConstants.WEEKLY_REPORT_FOOTER_HTML,
                LoadStylesAndCSSFromMainHtmlDocument = true,
            };
            
            renderer.RenderingOptions.EnableJavaScript = true;
            renderer.RenderingOptions.WaitFor.RenderDelay(5000); // Wait for 5 seconds to allow JavaScript to execute

            var pdf = renderer.RenderHtmlAsPdf(htmlContent);
            pdf.AddHtmlFooters(htmlFooter);
            return new PdfReportFileInfo
            {
                ByteArray = pdf.BinaryData,
                MimeType = "application/pdf",
                FileName = "WeeklyReport.pdf"
            };
        }

        // 
        /// <summary>
        /// Returns a harcdoed report data.
        /// In a real life scenario, this data might come from some database query result or json response from an API call
        /// </summary>
        /// <returns>A hardcoded list of sales metrics with sales numbers for this week & last week</returns>
        private static List<WeeklyReportInfo> GetWeeklyReportInfo()
        {
            var metricsData = new List<WeeklyReportInfo>()
            {
                new() {LastWeekSales = 50400, Metric = IronPdfChallengeConstants.COST_OF_GOODS_SOLD, ThisWeekSales = 45900, PercentageIncrement = 10.2},
                new () {LastWeekSales = 54000, Metric = IronPdfChallengeConstants.REVENUE, ThisWeekSales = 68000, PercentageIncrement = -4.8},
                new () {LastWeekSales = 95400, Metric = IronPdfChallengeConstants.GROSS_PROFIT, ThisWeekSales = 89000, PercentageIncrement = 12.5},
                new () {LastWeekSales = 30000, Metric = IronPdfChallengeConstants.OPERATING_EXPENSES, ThisWeekSales =49000, PercentageIncrement = -15.7},
                new () {LastWeekSales = 22000, Metric = IronPdfChallengeConstants.NET_PROFIT, ThisWeekSales = 30000, PercentageIncrement = 36.4},
                new () {LastWeekSales = 22000, Metric = IronPdfChallengeConstants.CASH_ON_HAND, ThisWeekSales = 25000, PercentageIncrement = 13.6},
            };

            return metricsData;
        }

        /// <summary>
        /// You can read more about bar charts using chart.js here => https://www.chartjs.org/docs/latest/charts/bar.html
        /// </summary>
        /// <returns>Returns an object needed to draw/render a bar chart of weekly sales.</returns>
        private static WeeklyReportChartModelData GetReportBarChartModelData()
        {
            var chartData = new WeeklyReportChartModelData
            {
                labels = [
                    IronPdfChallengeConstants.COST_OF_GOODS_SOLD,
                    IronPdfChallengeConstants.REVENUE,
                    IronPdfChallengeConstants.GROSS_PROFIT,
                    IronPdfChallengeConstants.OPERATING_EXPENSES,
                    IronPdfChallengeConstants.NET_PROFIT,
                    IronPdfChallengeConstants.CASH_ON_HAND
                ],
                datasets =
                [
                    new() {
                        label = IronPdfChallengeConstants.THIS_WEEK,
                        data = GetThisWeekSales(),
                        backgroundColor = IronPdfChallengeConstants.COLOR_TEAL
                    },
                    new WeeklyReportChartModelDataset() {
                        label = IronPdfChallengeConstants.LAST_WEEK,
                        data = GetLastWeekSales(),
                        backgroundColor = IronPdfChallengeConstants.COLOR_RED
                    }
                ]
            };

            return chartData;
        }

        /// <summary>
        /// Filters out last week sales from the report data and returns just this week's sales numbers.
        /// </summary>
        /// <returns>A list of sales metrics numbers</returns>
        private static List<long> GetThisWeekSales()
        {
            var thisWeekSales = new List<long>();
            var reportInfo = GetWeeklyReportInfo();

            thisWeekSales = [.. reportInfo.Select(s => Convert.ToInt64(s.ThisWeekSales))];
            return thisWeekSales;
        }

        /// <summary>
        /// Filters out this week sales from the report data and returns just last week's sales numbers.
        /// </summary>
        /// <returns>A list of sales metrics numbers</returns>
        private static List<long> GetLastWeekSales()
        {
            var lastWeekSales = new List<long>();
            var reportInfo = GetWeeklyReportInfo();

            lastWeekSales = [.. reportInfo.Select(s => Convert.ToInt64(s.LastWeekSales))];
            return lastWeekSales;
        }

        
        public static string ReadHtmlTemplate(string templatePath)
        {
            try
            {
                using var sr = new StreamReader(templatePath);
                string data = sr.ReadToEnd();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// `Generates a table row html that would later be injected into the weely report pdf.
        /// </summary>
        /// <returns>Returns a table row (<tr>) string of weekly report data. </returns>
        private static string GetWeeklyKeyMetricsData()
        {
            var weeklyMetricsDataStringBuilder = new StringBuilder();
            var metricsData = GetWeeklyReportInfo();

            foreach (var metricData in metricsData)
            {
                string backgroundColor = metricData.PercentageIncrement < 0 ? "background-color : #faa49d" : "";
                weeklyMetricsDataStringBuilder.Append("<tr>");

                weeklyMetricsDataStringBuilder.Append($"<td>{metricData.Metric}</td>");
                weeklyMetricsDataStringBuilder.Append($"<td style='text-align: right'>{metricData.ThisWeekSales:N2}</td>");
                weeklyMetricsDataStringBuilder.Append($"<td style='text-align: right'>{metricData.LastWeekSales:N2}</td>");
                weeklyMetricsDataStringBuilder.Append($"<td style='{backgroundColor}'>{metricData.PercentageIncrement:N2}</td>");

                weeklyMetricsDataStringBuilder.Append("</tr>");
            }

            return weeklyMetricsDataStringBuilder.ToString();
        }

        /// <summary>
        /// Generates a list of data needed to populate the 3 doughnut charts on the weekly report.
        /// The number (in percentage) rendered by the charts is randomized (for dynamism) everytime the report is generated.
        /// The colour (red, amber, green) of the percentage covered indicator is determined by the number (in percentage).
        /// You can read more about doughnut charts from the chart.js official website via the link below
        /// https://www.chartjs.org/docs/latest/charts/doughnut.html
        /// </summary>
        /// <returns>Returns a list of data required to assign to the dataset property of the doughnut chart widget</returns>
        public static List<DoughnutChartData> GetDoughnutChartDataset()
        {
            Random rnd = new();
            var allDoughnutCharts = new List<DoughnutChartData>();
            string completedPercentageColor = string.Empty;

            // doughnut chart 1
            int completedPercentageChart1 = rnd.Next(1, 20);
            int incompletedPercentageChart1 = IronPdfChallengeConstants.ONE_HUNDRED - completedPercentageChart1;
            completedPercentageColor = DetermineCompletedPercentageChartColor(completedPercentageChart1);

            allDoughnutCharts.Add(
                new DoughnutChartData()
                {
                    Datasets = new DoughnutChartDataset()
                    {
                        BackgroundColor = [completedPercentageColor, IronPdfChallengeConstants.OUTSTANDING_DUOGHNUT_CHART_PERCENTAGE],
                        BorderWidth = 0,
                        Data = [completedPercentageChart1, incompletedPercentageChart1],
                        Label = $"{completedPercentageChart1}%" // shows the completed with the % symbol appended
                    }
                }
            );
            

            // doughnut chart 2
            int completedPercentageChart2 = rnd.Next(30, 70);
            int incompletedPercentageChart2 = IronPdfChallengeConstants.ONE_HUNDRED - completedPercentageChart2;
            completedPercentageColor = DetermineCompletedPercentageChartColor(completedPercentageChart2);

            allDoughnutCharts.Add(
               new DoughnutChartData()
               {
                   Datasets = new DoughnutChartDataset()
                   {
                       BackgroundColor = [completedPercentageColor, IronPdfChallengeConstants.OUTSTANDING_DUOGHNUT_CHART_PERCENTAGE],
                       BorderWidth = 0,
                       Data = [completedPercentageChart2, incompletedPercentageChart2],
                       Label = $"{completedPercentageChart2}%" // shows the completed with the % symbol appended
                   }
               }
            );
            
            // doughnut chart 3
            int completedPercentageChart3 = rnd.Next(40, 90);
            int incompletedPercentageChart3 = IronPdfChallengeConstants.ONE_HUNDRED - completedPercentageChart3;
            completedPercentageColor = DetermineCompletedPercentageChartColor(completedPercentageChart3);

            allDoughnutCharts.Add(
               new DoughnutChartData()
               {
                   Datasets = new DoughnutChartDataset()
                   {
                       BackgroundColor = [completedPercentageColor, IronPdfChallengeConstants.OUTSTANDING_DUOGHNUT_CHART_PERCENTAGE],
                       BorderWidth = 0,
                       Data = [completedPercentageChart3, incompletedPercentageChart3],
                       Label = $"{completedPercentageChart3}%" // shows the completed with the % symbol appended
                   }
               }
            );
           
            return allDoughnutCharts;
        }

        /// <summary>
        /// Takws an integer representing percentage completed and returns a string hex-color value.
        /// If the integer parameter is less than or equal to 30, it returns a danger-looking hex-color value.
        /// If the integer parameter is greater than 30 and less than 50, it returns an amber hex-color value.
        /// Otherwise, it returns a green hex-color value.
        /// </summary>
        /// <param name="completed">Integer value representing percentage completed</param>
        /// <returns>Returns a hex-color value </returns>
        private static string DetermineCompletedPercentageChartColor(int completed)
        {
            // returns a "danger-looking" colour when percentage completed is equal to or below 30
            if (completed <= 30)
            {
                return IronPdfChallengeConstants.DANGER_HEX_VALUE;
            }

            // returns amber when the percentage completed is greater than 30 but less than or equal to 50
            if (completed > 30 && completed < 50)
            {
                return IronPdfChallengeConstants.WARNING_HEX_VALUE;
            }

            return IronPdfChallengeConstants.SUCCESS_HEX_VALUE;
        }


        private static string GenerateDateCovered()
        {
            DateTime start = DateTime.Now.Date.AddDays(-7);
            DateTime end = DateTime.Now.Date;

            string dateCovered = $"{start:MMMM d}-{end:MMMM d}, {start:yyyy}";
            return dateCovered;
        }

        private static string GetChallengesBulletPointListText()
        {
            var challengesBulletPointStringBuilder = new StringBuilder();

            challengesBulletPointStringBuilder.Append("<ul>");
            challengesBulletPointStringBuilder.Append("<li><div class='small-muted mb-2'>Cash outflows related to debt payments increased by 50%, which could impact liquidity in the following weeks.</div></li>");
            challengesBulletPointStringBuilder.Append("<li><div class='small-muted'>While marketing costs decreased, there is concern over whether this will impact future sales. This will be monitored next week.</div></li>");
            challengesBulletPointStringBuilder.Append("</ul>");

            return challengesBulletPointStringBuilder.ToString();
        }
    }
}
