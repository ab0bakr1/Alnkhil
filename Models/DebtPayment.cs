using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace alnakhil.Models
{
    public class DebtPayment
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        // ربط إما بمشتريات أو مبيعات
        public int? PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }

        public int? SaleId { get; set; }
        public Sale? Sale { get; set; }
    }
}