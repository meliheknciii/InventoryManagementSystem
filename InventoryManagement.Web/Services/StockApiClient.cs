using System.Net.Http.Json;
using System.Text.Json;
using InventoryManagement.Web.Models;

namespace InventoryManagement.Web.Services
{
    public class StockApiClient : IStockApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public StockApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<StockViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("api/stocks", cancellationToken);
            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<List<StockViewModel>>(JsonOptions, cancellationToken)
                ?? new List<StockViewModel>();
        }

        public async Task<StockViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/stocks/{id}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<StockViewModel>(JsonOptions, cancellationToken);
        }

        public async Task<StockViewModel> CreateAsync(StockFormViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/stocks", new
            {
                productId = model.ProductId,
                quantity = model.Quantity
            }, JsonOptions, cancellationToken);

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return (await response.Content.ReadFromJsonAsync<StockViewModel>(JsonOptions, cancellationToken))!;
        }

        public async Task<StockViewModel> UpdateAsync(int id, StockFormViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/stocks/{id}", new
            {
                quantity = model.Quantity
            }, JsonOptions, cancellationToken);

            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
            return (await response.Content.ReadFromJsonAsync<StockViewModel>(JsonOptions, cancellationToken))!;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/stocks/{id}", cancellationToken);
            await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        }
    }
}
