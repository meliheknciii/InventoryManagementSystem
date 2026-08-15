using System.Net.Http.Json;
using System.Text.Json;
using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Services
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public ProductApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("api/products", cancellationToken);
            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<List<ProductViewModel>>(JsonOptions, cancellationToken)
                ?? new List<ProductViewModel>();
        }

        public async Task<ProductViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/products/{id}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ProductViewModel>(JsonOptions, cancellationToken);
        }

        public async Task<ProductViewModel> CreateAsync(ProductFormViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/products", new
            {
                name = model.Name,
                description = model.Description,
                sku = model.Sku,
                price = model.Price,
                categoryId = model.CategoryId
            }, JsonOptions, cancellationToken);

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return (await response.Content.ReadFromJsonAsync<ProductViewModel>(JsonOptions, cancellationToken))!;
        }

        public async Task<ProductViewModel> UpdateAsync(int id, ProductFormViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", new
            {
                name = model.Name,
                description = model.Description,
                sku = model.Sku,
                price = model.Price,
                categoryId = model.CategoryId
            }, JsonOptions, cancellationToken);

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return (await response.Content.ReadFromJsonAsync<ProductViewModel>(JsonOptions, cancellationToken))!;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/products/{id}", cancellationToken);
            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        }
    }
}
