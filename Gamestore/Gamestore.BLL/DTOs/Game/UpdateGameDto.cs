using System.ComponentModel.DataAnnotations;

namespace Gamestore.BLL.DTOs.Game;

public class UpdateGameDto
{
    [Required(ErrorMessage = "Id is required for updating the game.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Game Name is mandatory.")]
    [MinLength(1, ErrorMessage = "Game Name cannot be empty.")]
    public string Name { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is mandatory.")]
    [Range(0.0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
    public double Price { get; set; }

    [Required(ErrorMessage = "Unit in stock is mandatory.")]
    [Range(0, int.MaxValue, ErrorMessage = "Unit in stock must be greater than or equal to 0.")]
    public int UnitInStock { get; set; }

    [Required(ErrorMessage = "Discount is mandatory.")]
    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public int Discount { get; set; }
}