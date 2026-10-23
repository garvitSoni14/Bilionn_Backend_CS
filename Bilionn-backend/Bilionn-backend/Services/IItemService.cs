using Bilionn_backend.DTOs.Catalog;

namespace Bilionn_backend.Services
{
    public interface IItemService
    {
        Task<List<ItemResponse>> GetItemsAsync();
        Task<ItemResponse> AddItemAsync(ItemForm f);
        Task DeleteItemAsync(string itemId);
        Task<byte[]?> GetItemImageAsync(string itemId); 
    }
}
