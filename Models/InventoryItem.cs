using System.ComponentModel.DataAnnotations;

namespace StallFruitsManagement.Models
{
    public enum CategoryType
    {
        Fruit,
        Vegetable
    }

    public enum UnitType
    {
        [Display(Name = "Kilogram (kg)")]
        Kilogram,
        [Display(Name = "Gram (g)")]
        Gram,
        [Display(Name = "Pound (lbs)")]
        Pound,
        [Display(Name = "Piece (pcs)")]
        Piece
    }

    public class InventoryItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Item name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public CategoryType Category { get; set; }

        [Range(0, 10000)]
        public decimal Quantity { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public UnitType Unit { get; set; }

        [Range(0.0, 9999.99)]
        [DataType(DataType.Currency)]
        [Display(Name = "Price (PHP)")]
        public decimal Price { get; set; }

        public string? ImagePath { get; set; }
    }
}
