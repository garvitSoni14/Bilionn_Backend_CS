using Bilionn_backend.DTOs.Catalog;
using Bilionn_backend.Models;
using Bilionn_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bilionn_backend.Controllers;

[ApiController]
[Route("api")]
public class ItemController: ControllerBase
{
    private readonly IItemService itemService;
    public ItemController( IItemService _itemService) => itemService = _itemService;


    [Authorize]
    [HttpGet("items")]
    public async Task<ActionResult<List<ItemResponse>>> Items()
    {
        return Ok(await itemService.GetItemsAsync());
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("admin/items")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddItem([FromForm] ItemForm form)
    {
        if (string.IsNullOrWhiteSpace(form.Name))
            return ValidationProblem(Single("name", "Item name is required."));
        if (form.Price <= 0)
            return ValidationProblem(Single("price", "Price must be greater than 0."));
        if (string.IsNullOrWhiteSpace(form.CategoryId))
            return ValidationProblem(Single("categoryId", "Please choose a category."));
        return StatusCode(StatusCodes.Status201Created, await itemService.AddItemAsync(form));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("admin/items/{itemId}")]
    public async Task<IActionResult> DeleteItem(string itemId)
    {
        await itemService.DeleteItemAsync(itemId);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("items/{itemId}/image")]
    public async Task<IActionResult> ItemImage(string itemId) =>
        ImageResult(await itemService.GetItemImageAsync(itemId));

    // ----- helpers -----
    private IActionResult ImageResult(byte[]? bytes) =>
        bytes is null || bytes.Length == 0 ? NotFound() : File(bytes, DetectMime(bytes));

    private static string DetectMime(byte[] b) =>
        b.Length > 3 && b[0] == 0xFF && b[1] == 0xD8 ? "image/jpeg" :
        b.Length > 4 && b[0] == 0x89 && b[1] == 0x50 ? "image/png" :
        b.Length > 3 && b[0] == 0x47 && b[1] == 0x49 ? "image/gif" :
        b.Length > 11 && b[8] == 0x57 && b[9] == 0x45 ? "image/webp" :
        "application/octet-stream";

    private static ValidationProblemDetails Single(string field, string message)
    {
        return new(new Dictionary<string, string[]> { [field] = new[] { message } });
    }
}


