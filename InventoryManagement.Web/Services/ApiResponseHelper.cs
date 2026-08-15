using System.Net.Http.Json;
using System.Text.Json;

namespace InventoryManagement.Web.Services
{
    internal static class ApiResponseHelper
    {
        private record ProblemDetails(string? title, int status, string? detail, string? instance);

        public static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            string message = $"API isteği başarısız oldu (HTTP {(int)response.StatusCode}).";

            try
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(
                    new JsonSerializerOptions(JsonSerializerDefaults.Web), cancellationToken);

                if (!string.IsNullOrWhiteSpace(problem?.detail))
                {
                    message = problem!.detail!;
                }
            }
            catch (JsonException)
            {
                // Response body wasn't a problem+json payload; fall back to the generic message.
            }

            throw new ApiException((int)response.StatusCode, message);
        }
    }
}
