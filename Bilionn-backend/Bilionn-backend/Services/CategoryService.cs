using Bilionn_backend.Data;
using Bilionn_backend.DTOs.Catalog;
using Bilionn_backend.Exceptions;
using Bilionn_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Bilionn_backend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext dbContext;
        public CategoryService(AppDbContext db)
        {
            dbContext = db;
        }

        public async Task<CategoryResponse> AddCategoryAsync(CategoryForm f)
        {
            var name = f.Name.Trim();
            if (await dbContext.Categories.AnyAsync(c => c.Name == name))
                throw AppException.Conflict("A category with this name already exists.");
            var c = new Category
            {
                CategoryId = Guid.NewGuid().ToString(),
                Name = name,
                Description = f.Description?.Trim(),
                BgColor = f.BgColor,
                Image = await ReadAsync(f.Img)
            };
            dbContext.Categories.Add(c);
            await dbContext.SaveChangesAsync();

            return new CategoryResponse(c.CategoryId, c.Name, c.Description, c.BgColor,
                $"/api/categories/{c.CategoryId}/image", c.CreatedAt, c.UpdatedAt, 0);
        }

        public async Task DeleteCategoryAsync(string categoryId)
        {
            var c = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId)
           ?? throw AppException.NotFound($"Category not found: {categoryId}");
            try
            {
                dbContext.Categories.Remove(c);
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)   // FK is Restrict, so a category with items can't be deleted
            {
                throw AppException.Conflict("This category still has items. Delete or move them first.");
            }
        }

        public async Task<List<CategoryResponse>> GetCategoriesAsync() =>
        await dbContext.Categories.AsNoTracking()          // projection: the image blob is never loaded
            .Select(c => new CategoryResponse(
                c.CategoryId, c.Name, c.Description, c.BgColor,
                "/api/categories/" + c.CategoryId + "/image",
                c.CreatedAt, c.UpdatedAt,
                dbContext.Items.Count(i => EF.Property<long>(i, "CategoryId") == c.Id)))
            .ToListAsync();

        public async Task<byte[]?> GetCategoryImageAsync(string categoryId)
        {
            return await dbContext.Categories.AsNoTracking().Where(c => c.CategoryId == categoryId)
             .Select(c => c.Image).FirstOrDefaultAsync();
        }
        private static async Task<byte[]?> ReadAsync(IFormFile? file)
        {
            if (file is null || file.Length == 0) return null;
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
