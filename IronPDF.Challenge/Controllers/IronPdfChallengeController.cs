using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IronPdf;
using IronPDF.Challenge.Services;

namespace IronPDF.Challenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IronPdfChallengeController : ControllerBase
    {
        [HttpGet]
        [Route("weekly-report")]
        public ActionResult GenerateTestPdf()
        {
            var pdfReportInfo = PdfChallengeService.GenerateWeeklyReportdf();

            return File(pdfReportInfo.ByteArray, pdfReportInfo.MimeType, pdfReportInfo.FileName);
        }


        [HttpGet]
        [Route("doughnut-chart-data")]
        public ActionResult GetDoughnutChartData()
        {
            var doughnutChartData = PdfChallengeService.GetDoughnutChartDataset();
            
            return Ok(new { doughnutChartData });
        }
    }
}
