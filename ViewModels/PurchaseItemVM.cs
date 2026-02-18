using System.ComponentModel.DataAnnotations;

namespace alnakhil.ViewModels
{
    public class PurchaseItemVM
    {
        public int? ProductId { get; set; }

        public string? ProductName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ExpiryDate { get; set; }


        public string? Barcode { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PurchasePrice { get; set; }

        public decimal TotalPrice => PurchasePrice;
    }
}
