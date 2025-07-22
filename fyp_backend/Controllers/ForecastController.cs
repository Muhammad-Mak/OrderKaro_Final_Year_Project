using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FYP_Backend.DTOs.Forecast;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: api/forecast
    public class ForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory; // For making outbound HTTP requests
        private readonly ILogger<ForecastController> _logger;   // For logging errors/info

        // URL of the external forecasting API (e.g., FastAPI + Prophet)
        private const string ForecastApiUrl = "https://smartcafeforecast.onrender.com/forecast";

        public ForecastController(IHttpClientFactory httpClientFactory, ILogger<ForecastController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // ---------------- GET FORECAST ----------------
        // GET: api/forecast/sales?itemId=1&days=7
        // Calls external service to get forecast for a menu item over N days (default: 7)
        [HttpGet("sales")]
        public async Task<IActionResult> GetForecast(int itemId, int days = 7)
        {
            var client = _httpClientFactory.CreateClient(); // Create a new HttpClient instance

            var requestUrl = $"{ForecastApiUrl}?item_id={itemId}&days={days}";

            try
            {
                // Make the request to the external forecasting API
                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    // Read the error message and return appropriate status code
                    var msg = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Forecast service failed: {msg}");
                }

                // Read and deserialize the JSON response into a list of ForecastDTOs
                var result = await response.Content.ReadFromJsonAsync<List<ForecastDTO>>();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling forecast service."); // Log internal error
                return StatusCode(500, "Internal server error while getting forecast.");
            }
        }
    }
}
