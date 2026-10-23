using Bilionn_backend.DTOs.Catalog;
using Bilionn_backend.Models;
using Bilionn_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bilionn_backend.Controllers;

[ApiController]
[Route("api")]
public class CategoryController: ControllerBase
{
    private readonly ICategoryService categoryService;
    public CategoryController(ICategoryService category)
    {
        categoryService = category;
    }

    [Authorize]
    [HttpGet("categories")]
    public async Task<ActionResult<List<CategoryResponse>>> Categoryies()
    {
        return Ok(await categoryService.GetCategoriesAsync());
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("admin/categories")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddCategory([FromForm] CategoryForm form)
    {
        if (string.IsNullOrWhiteSpace(form.Name))
            return ValidationProblem(Single("name", "Category name is required."));
        return StatusCode(StatusCodes.Status201Created, await categoryService.AddCategoryAsync(form));
    }


    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("admin/categories/{categoryId}")]
    public async Task<IActionResult> DeleteCategory(string categoryId)
    {
        await categoryService.DeleteCategoryAsync(categoryId);
        return NoContent();
    }

    [AllowAnonymous]   // <img src> tags can't be blocked by auth in simple setups; remove if you prefer
    [HttpGet("categories/{categoryId}/image")]
    public async Task<IActionResult> CategoryImage(string categoryId)
    {
        return ImageResult(await categoryService.GetCategoryImageAsync(categoryId));
    }

    // ----- helpers -----
    private IActionResult ImageResult(byte[]? bytes) =>
        bytes is null || bytes.Length == 0 ? NotFound() : File(bytes, DetectMime(bytes));

    private static string DetectMime(byte[] b) =>
        b.Length > 3 && b[0] == 0xFF && b[1] == 0xD8 ? "image/jpeg" :
        b.Length > 4 && b[0] == 0x89 && b[1] == 0x50 ? "image/png" :
        b.Length > 3 && b[0] == 0x47 && b[1] == 0x49 ? "image/gif" :
        b.Length > 11 && b[8] == 0x57 && b[9] == 0x45 ? "image/webp" :
        "application/octet-stream";
    private static ValidationProblemDetails Single(string field, string message) =>
    new(new Dictionary<string, string[]> { [field] = new[] { message } });
}
