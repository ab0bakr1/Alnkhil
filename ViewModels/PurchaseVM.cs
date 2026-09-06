using System.ComponentModel.DataAnnotations;
using alnakhil.Models;

namespace alnakhil.ViewModels
{
    public class PurchaseVM
    {
        public int Id { get; set; }

        public string? InvoiceNumber { get; set; }

        public string SupplierName { get; set; }

        // 🆕 ربط بالمورد الفعلي (بدل الاسم كنص فقط)
        public int? SupplierId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        // ================= الضريبة والخصم =================
        public decimal TaxPercentage { get; set; } = 0;
        public decimal DiscountPercentage { get; set; } = 0;

        // ================= الدفع =================
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public DateTime? DueDate { get; set; }

        // ================= الأصناف =================
        public List<PurchaseItemVM> Items { get; set; } = new();

        // ================= الحسابات =================
        public decimal Subtotal => Items.Sum(i => i.TotalPrice);
        public decimal TaxAmount => Subtotal * (TaxPercentage / 100);
        public decimal DiscountAmount => Subtotal * (DiscountPercentage / 100);
        public decimal TotalAmount => Subtotal + TaxAmount - DiscountAmount;
    }
}