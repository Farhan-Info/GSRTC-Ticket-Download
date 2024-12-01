using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GSRTCTicketDownload.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            return View();
        }

        //// POST: Home/DownloadPdf
        //[HttpPost]
        //public async Task<IActionResult> DownloadPdf(string pnrNumber)
        //{
        //    if (string.IsNullOrEmpty(pnrNumber))
        //    {
        //        return BadRequest("PNR number is required.");
        //    }

        //    // Construct the GSRTC API URL with the entered PNR number
        //    var apiUrl = $"https://www.gsrtc.in/OPRSOnline/manageNewViewBookingHistory.do?pnrNo={pnrNumber}&hiddenAction=PrintTicket";

        //    var client = _clientFactory.CreateClient();
        //    var response = await client.GetAsync(apiUrl);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return StatusCode((int)response.StatusCode, "Error fetching HTML content.");
        //    }

        //    // Retrieve the HTML content
        //    var htmlContent = await response.Content.ReadAsStringAsync();

        //    // Now you can call a third-party API to convert HTML to PDF, here using an example placeholder
        //    var pdfBytes = await ConvertHtmlToPdf(htmlContent);

        //    // Return the PDF as a downloadable file
        //    return File(pdfBytes, "application/pdf", $"GSRTC_PNR_{pnrNumber}.pdf");
        //}

        //private async Task<byte[]> ConvertHtmlToPdf(string htmlContent)
        //{
        //    // Here, you can call a third-party API like html2pdf.com or others
        //    // For example, sending the HTML content to a free HTML-to-PDF conversion API

        //    // Placeholder code (you would use the actual API call here)
        //    var apiUrl = "https://api.html2pdf.com/convert"; // This is just a placeholder
        //    var client = new HttpClient();
        //    var content = new StringContent(htmlContent);
        //    var response = await client.PostAsync(apiUrl, content);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Failed to convert HTML to PDF");
        //    }

        //    return await response.Content.ReadAsByteArrayAsync();
        //}


        // POST: Home/DownloadPdf
        //[HttpPost]
        //public async Task<IActionResult> DownloadPdf(string pnrNumber)
        //{
        //    if (string.IsNullOrEmpty(pnrNumber))
        //    {
        //        return BadRequest("PNR number is required.");
        //    }

        //    // Construct the GSRTC API URL with the entered PNR number
        //    var apiUrl = $"https://www.gsrtc.in/OPRSOnline/manageNewViewBookingHistory.do?pnrNo={pnrNumber}&hiddenAction=PrintTicket";

        //    var client = _clientFactory.CreateClient();
        //    var response = await client.GetAsync(apiUrl);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return StatusCode((int)response.StatusCode, "Error fetching HTML content.");
        //    }

        //    // Retrieve the HTML content
        //    var htmlContent = await response.Content.ReadAsStringAsync();

        //    // Now you can call a third-party API to convert HTML to PDF, here using an example placeholder
        //    var pdfBytes = await ConvertHtmlToPdf(htmlContent);

        //    // Return the PDF as a downloadable file
        //    return File(pdfBytes, "application/pdf", $"GSRTC_PNR_{pnrNumber}.pdf");
        //}

        //private async Task<byte[]> ConvertHtmlToPdf(string htmlContent)
        //{
        //    // Here, you can call a third-party API like html2pdf.com or others
        //    // For example, sending the HTML content to a free HTML-to-PDF conversion API

        //    // Placeholder code (you would use the actual API call here)
        //    var apiUrl = "https://api.html2pdf.com/convert"; // This is just a placeholder
        //    var client = new HttpClient();
        //    var content = new StringContent(htmlContent);
        //    var response = await client.PostAsync(apiUrl, content);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Failed to convert HTML to PDF");
        //    }

        //    return await response.Content.ReadAsByteArrayAsync();
        //}

        [HttpPost]
        public async Task<IActionResult> DownloadPdf(string pnrNumber)
        {
            if (string.IsNullOrEmpty(pnrNumber))
            {
                return BadRequest("PNR number is required.");
            }

            // Construct the GSRTC API URL with the entered PNR number
            var apiUrl = $"https://www.gsrtc.in/OPRSOnline/manageNewViewBookingHistory.do?pnrNo={pnrNumber}&hiddenAction=PrintTicket";

            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error fetching content.");
            }

            // Retrieve the HTML content
            var htmlContent = await response.Content.ReadAsStringAsync();

            // Replace the relative image path with the specified absolute URL
            if (htmlContent.Contains("./images/gsrtc_Ticket_Logo.png"))
            {
                htmlContent = htmlContent.Replace(
                    "./images/gsrtc_Ticket_Logo.png",
                    "https://play-lh.googleusercontent.com/VYqu_zWr_HscXe7x77yddFtS3oW4ss4p-fsHzdxJtL0SE3swQ8X-FQNzFrSFjgelL_E=w240-h480-rw");
            }

            // Remove specific JavaScript lines
            if (htmlContent.Contains("window.print();") || htmlContent.Contains("window.close();"))
            {
                htmlContent = htmlContent.Replace("window.print();", "");
                htmlContent = htmlContent.Replace("window.close();", "");
            }

            // Optional: Remove entire <script> tags containing window.print() or window.close()
            htmlContent = Regex.Replace(htmlContent, @"<script[^>]*>.*?(window\.print\(\);|window\.close\(\);).*?</script>", "", RegexOptions.Singleline);

            // Retrieve the response content as bytes
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(htmlContent);

            // Suggest a file name
            var fileName = $"GSRTC_TICKET_PNR_{pnrNumber}.html";

            // Return the file as a downloadable response
            return File(contentBytes, "text/html", fileName);
        }



    }
}
