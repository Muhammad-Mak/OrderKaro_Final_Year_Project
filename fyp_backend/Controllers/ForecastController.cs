using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FYP_Backend.DTOs.Forecast;

namespace FYP_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ForecastController> _logger;

        private const string ForecastApiUrl = "http://localhost:8000/forecast"; // Change to your hosted FastAPI URL if deployed

        public ForecastController(IHttpClientFactory httpClientFactory, ILogger<ForecastController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sales")]
        public async Task<IActionResult> GetForecast(int itemId, int days = 7)
        {
            var client = _httpClientFactory.CreateClient();

            var requestUrl = $"{ForecastApiUrl}?item_id={itemId}&days={days}";

            try
            {
                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Forecast service returned error: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Failed to get forecast.");
                }

                var result = await response.Content.ReadFromJsonAsync<List<ForecastDTO>>();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling forecast service.");
                return StatusCode(500, "Internal server error while getting forecast.");
            }
        }
    }
}
