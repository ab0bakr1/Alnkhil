using System.ComponentModel.DataAnnotations;

namespace alnakhil.ViewModels
{
    public class SaleItemVM
    {
        public int Id { get; set; } // ⭐ مهم جدًا للتعديل

        [Required]
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductBarcode { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; } // سعر البيع للوحدة
    }
}
