using System.Net.Http.Json;
using System.Text.Json;
using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Services
{
    public class CategoryApiClient : ICategoryApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public CategoryApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("api/categories", cancellationToken);
            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<List<CategoryViewModel>>(JsonOptions, cancellationToken)
                ?? new List<CategoryViewModel>();
        }

        public async Task<CategoryViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/categories/{id}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<CategoryViewModel>(JsonOptions, cancellationToken);
        }

        public async Task<CategoryViewModel> CreateAsync(CategoryFormViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/categories", new
            {
                name = model.Name,
                description = model.Description
            }, JsonOptions, cancellationToken);

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return (await response.Content.ReadFromJsonAsync<CategoryViewModel>(JsonOptions, cancellationToken))!;
        }

        public async Task<CategoryViewModel> UpdateAsync(int id, CategoryFormViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", new
            {
                name = model.Name,
                description = model.Description
            }, JsonOptions, cancellationToken);

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return (await response.Content.ReadFromJsonAsync<CategoryViewModel>(JsonOptions, cancellationToken))!;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/categories/{id}", cancellationToken);
            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        }
    }
}
