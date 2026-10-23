using Bilionn_backend.Data;
using Bilionn_backend.DTOs.Catalog;
using Bilionn_backend.Exceptions;
using Bilionn_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Bilionn_backend.Services;

public class ItemService : IItemService
{
    private readonly AppDbContext appDbContext;
    public ItemService(AppDbContext db)
    {
        appDbContext = db;
    }

    public async Task<ItemResponse> AddItemAsync(ItemForm f)
    {
        var category = await appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == f.CategoryId)
            ?? throw AppException.NotFound($"Category not found: {f.CategoryId}");

        var i = new Items
        {
            ItemId = Guid.NewGuid().ToString(),
            Name = f.Name.Trim(),
            Price = f.Price,
            Description = f.Description?.Trim(),
            Image = await ReadAsync(f.Image),
            Category = category
        };
        appDbContext.Items.Add(i);
        await appDbContext.SaveChangesAsync();

        return new ItemResponse(i.ItemId, i.Name, i.Price, category.CategoryId, category.Name,
            i.Description, $"/api/items/{i.ItemId}/image", i.CreatedAt, i.UpdatedAt);

    }

    public async Task DeleteItemAsync(string itemId)
    {
        var i = await appDbContext.Items.FirstOrDefaultAsync(x => x.ItemId == itemId)
            ?? throw AppException.NotFound("Item not found.");
        appDbContext.Items.Remove(i);
        await appDbContext.SaveChangesAsync();
    }


    public async Task<byte[]?> GetItemImageAsync(string itemId)
    {
        return await appDbContext.Items.AsNoTracking().Where(i => i.ItemId == itemId)
            .Select(i => i.Image).FirstOrDefaultAsync();
    }


    private static async Task<byte[]?> ReadAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return ms.ToArray();
    }

    public async Task<List<ItemResponse>> GetItemsAsync()
    {
        return await appDbContext.Items.AsNoTracking()
            .Select(i => new ItemResponse(
                i.ItemId, i.Name, i.Price, i.Category.CategoryId, i.Category.Name,
                i.Description, "/api/items/" + i.ItemId + "/image", i.CreatedAt, i.UpdatedAt))
            .ToListAsync();
    }
}
