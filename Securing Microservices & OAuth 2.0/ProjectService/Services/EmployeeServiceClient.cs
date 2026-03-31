using ProjectService.DTOs;
using System.Text.Json;

namespace ProjectService.Services
{
    public class EmployeeServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmployeeServiceClient> _logger;

        public EmployeeServiceClient(HttpClient httpClient, ILogger<EmployeeServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<EmployeeDto?> GetEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Calling Employee Service for ID: {EmployeeId}", employeeId);
                var response = await _httpClient.GetAsync($"/api/v1/employees/{employeeId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<EmployeeDto>(content, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                }
                
                _logger.LogWarning("Employee Service returned {StatusCode} for ID: {EmployeeId}", 
                    response.StatusCode, employeeId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Employee Service for ID: {EmployeeId}", employeeId);
                return null;
            }
        }
    }
}
