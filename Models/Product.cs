using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alnakhil.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Barcode { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }  // سعر الشراء لكل المنتج

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }  // سعر البيع

        public bool IsManualPrice { get; set; } = false; // هل السعر يدوي؟

        public decimal? ManualSalePrice { get; set; } // سعر البيع اليدوي

        public DateTime? ExpirationDate { get; set; } // تاريخ الانتهاء

        // العلاقة مع التصنيف (اختياري)
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
