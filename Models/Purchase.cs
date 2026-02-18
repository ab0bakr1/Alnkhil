using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using alnakhil.Models;

namespace alnakhil.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? InvoiceNumber { get; set; }

        [MaxLength(150)]
        public string SupplierName { get; set; }

        public int? SupplierId { get; set; }
        
        public Supplier? Supplier { get; set; }

        // ================= الحسابات =================
        public decimal Subtotal { get; set; }        // قبل الضريبة والخصم
        public decimal TaxPercentage { get; set; }   // نسبة الضريبة %
        public decimal TaxAmount { get; set; }       // قيمة الضريبة
        public decimal DiscountPercentage { get; set; } // نسبة الخصم %
        public decimal DiscountAmount { get; set; }  // قيمة الخصم
        public decimal TotalAmount { get; set; }     // الإجمالي النهائي

        // ================= الدفع =================

        public decimal AmountPaid { get; set; } = 0;

        [NotMapped]
        public decimal RemainingAmount => TotalAmount - AmountPaid;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public DateTime? DueDate { get; set; } // تاريخ الاستحقاق إذا آجل

        // ================= العلاقات =================
        public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }
}
