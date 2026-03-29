using EmployeeService.DTOs;
using System.Text.Json;

namespace EmployeeService.Services
{
    public class DepartmentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DepartmentServiceClient> _logger;

        public DepartmentServiceClient(HttpClient httpClient, ILogger<DepartmentServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<DepartmentDto?> GetDepartmentAsync(int departmentId)
        {
            try
            {
                _logger.LogInformation("Calling Department Service for ID: {DepartmentId}", departmentId);
                var response = await _httpClient.GetAsync($"/api/v1/departments/{departmentId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<DepartmentDto>(content, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                }
                
                _logger.LogWarning("Department Service returned {StatusCode} for ID: {DepartmentId}", 
                    response.StatusCode, departmentId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Department Service for ID: {DepartmentId}", departmentId);
                return null;
            }
        }
    }
}
