using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<GatewayController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Services:DepartmentService"]}/api/v1/departments";
            _logger.LogInformation("Forwarding request to: {Url}", url);
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }

        [HttpGet("departments/{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Services:DepartmentService"]}/api/v1/departments/{id}";
            _logger.LogInformation("Forwarding request to: {Url}", url);
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees([FromQuery] string? version = "1.0")
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Services:EmployeeService"]}/api/v{version}/employees";
            _logger.LogInformation("Forwarding request to: {Url}", url);
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }

        [HttpGet("employees/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Services:EmployeeService"]}/api/v1/employees/{id}";
            _logger.LogInformation("Forwarding request to: {Url}", url);
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }

        [HttpGet("employees/department/{departmentId}")]
        public async Task<IActionResult> GetEmployeesByDepartment(int departmentId)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Services:EmployeeService"]}/api/v1/employees/department/{departmentId}";
            _logger.LogInformation("Forwarding request to: {Url}", url);
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects([FromQuery] string? version = "1.0")
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Services:ProjectService"]}/api/v{version}/projects";
            _logger.LogInformation("Forwarding request to: {Url}", url);
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }

        [HttpGet("projects/{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Services:ProjectService"]}/api/v1/projects/{id}";
            _logger.LogInformation("Forwarding request to: {Url}", url);
            
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(content));
        }

        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            var client = _httpClientFactory.CreateClient();
            var services = new Dictionary<string, string>
            {
                { "DepartmentService", _configuration["Services:DepartmentService"] ?? "" },
                { "EmployeeService", _configuration["Services:EmployeeService"] ?? "" },
                { "ProjectService", _configuration["Services:ProjectService"] ?? "" }
            };

            var healthStatus = new Dictionary<string, object>();

            foreach (var service in services)
            {
                try
                {
                    var response = await client.GetAsync($"{service.Value}/api/v1/departments");
                    healthStatus[service.Key] = new { Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy", StatusCode = (int)response.StatusCode };
                }
                catch (Exception ex)
                {
                    healthStatus[service.Key] = new { Status = "Unhealthy", Error = ex.Message };
                }
            }

            return Ok(new { Gateway = "Healthy", Services = healthStatus });
        }
    }
}
