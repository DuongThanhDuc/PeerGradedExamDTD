using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Utilities
{
    public class ClientGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ClientGenerator(string baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = new HttpClient();
        }

        public async Task<string> FetchOpenApiSpecAsync()
        {
            try
            {
                var swaggerUrl = $"{_baseUrl}/swagger/v1/swagger.json";
                var response = await _httpClient.GetAsync(swaggerUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                throw new HttpRequestException($"Failed to fetch OpenAPI spec. Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error fetching OpenAPI specification", ex);
            }
        }

        public HttpClient CreateApiClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "ClientGenerator/1.0");

            return client;
        }

        public HttpClient CreateAuthenticatedClient(string authToken)
        {
            var client = CreateApiClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
            return client;
        }

        public async Task<bool> ValidateApiAccessAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
