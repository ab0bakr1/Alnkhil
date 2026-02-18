using System.Collections.Generic;
using System.Linq;
using alnakhil.Models;

namespace alnakhil.ViewModels
{
    public class SaleVM
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; } // اختياري
        public List<SaleItemVM> Items { get; set; } = new();

        // حقول الفاتورة الجديدة
        public decimal TaxPercentage { get; set; } = 0; // نسبة الضريبة (مثال: 15 لـ 15%)
        public decimal DiscountPercentage { get; set; } = 0; // نسبة الخصم
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public DateTime? DueDate { get; set; }

        // حسابات
        public decimal Subtotal => Items.Sum(i => i.Quantity * i.UnitPrice);
        public decimal TaxAmount => Subtotal * (TaxPercentage / 100);
        public decimal DiscountAmount { get; set; } // Changed to settable property
        public decimal TotalAmount => Subtotal + TaxAmount - DiscountAmount;
    }
}
