using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace alnakhil.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Required]
        public string InvoiceNumber { get; set; } // رقم الفاتورة

        public decimal Subtotal { get; set; } // المجموع الفرعي قبل الضريبة والخصم
        public decimal TaxAmount { get; set; } // مبلغ الضريبة
        public decimal DiscountAmount { get; set; } // مبلغ الخصم
        public decimal TotalAmount { get; set; } // المجموع النهائي

        public string? CustomerName { get; set; } // اختياري

        public decimal AmountPaid { get; set; } = 0;

        public bool IsSuspended { get; set; }


        [NotMapped]
        public decimal RemainingAmount => TotalAmount - AmountPaid;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid; // حالة الدفع
        public DateTime? DueDate { get; set; } // تاريخ الاستحقاق

        public ICollection<SaleItem> Items { get; set; }

    }
}
