using alnakhil.Models;


namespace alnakhil.ViewModels
{
    public class DebtVM
    {
        // معلومات عامة
        public int Id { get; set; }
        public string Name { get; set; }          // مورد أو عميل
        public string InvoiceNumber { get; set; }

        // مبالغ
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal RemainingAmount { get; set; }

        // تواريخ
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }

        // حالة
        public PaymentStatus PaymentStatus { get; set; }

        // نوع الدين
        public DebtType Type { get; set; }
    }

    public enum DebtType
    {
        Payable,   // علينا (Purchases)
        Receivable // لنا (Sales)
    }
}
