using System.ComponentModel.DataAnnotations.Schema;

namespace alnakhil.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        // التصنيفات
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // الكمية   
        public int Quantity { get; set; }

        // تاريخ انتهاء الصلاحية
        public DateTime? ExpiryDate { get; set; }

        // سعر الشراء 
        public decimal PurchasePrice { get; set; }

        public decimal UnitPurchasePrice => Quantity == 0 ? 0 : PurchasePrice / Quantity;
  
        // إجمالي الصنف
        [NotMapped]
        public decimal TotalPrice => PurchasePrice;
    }
}
