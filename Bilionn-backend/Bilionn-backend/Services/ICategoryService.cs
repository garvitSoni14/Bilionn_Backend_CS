using Bilionn_backend.DTOs.Catalog;

namespace Bilionn_backend.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetCategoriesAsync();
        Task<CategoryResponse> AddCategoryAsync(CategoryForm f);
        Task DeleteCategoryAsync(string categoryId);
        Task<byte[]?> GetCategoryImageAsync(string categoryId);

        //Task<List<ItemResponse>> GetItemsAsync();
        //Task<ItemResponse> AddItemAsync(ItemForm f);
        //Task DeleteItemAsync(string itemId);
        //Task<byte[]?> GetItemImageAsync(string itemId);
    }

    
}
