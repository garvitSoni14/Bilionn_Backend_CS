namespace Bilionn_backend.DTOs.Catalog;

public class CategoryForm
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? BgColor { get; set; }
    public IFormFile? Img { get; set; }          // Angular sends the field as "img"
}

public class ItemForm
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = "";
    public string? Description { get; set; }
    public IFormFile? Image { get; set; }        // Angular sends the field as "image"
}

public record CategoryResponse(
    string CategoryId, string Name, string? Description, string? BgColor,
    string ImgUrl, DateTime CreatedAt, DateTime UpdatedAt, int Items);

public record ItemResponse(
    string ItemId, string Name, decimal Price, string CategoryId, string CategoryName,
    string? Description, string ImgUrl, DateTime CreatedAt, DateTime UpdatedAt);