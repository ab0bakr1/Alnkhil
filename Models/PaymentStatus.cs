namespace alnakhil.Models
{
    public enum PaymentStatus
    {
        Unpaid = 0,   // غير مدفوع
        Paid = 1,     // مدفوع
        Deferred = 2,  // آجل
        Suspended = 3   // 🔥 فاتورة معلّقة
    }
}
