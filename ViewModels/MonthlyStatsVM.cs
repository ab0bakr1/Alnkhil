namespace alnakhil.ViewModels
{
    public class MonthlyStatsVM
    {
        // أرقام الشهر
        public decimal TotalSales { get; set; }
        public int SalesCount { get; set; }

        // مقارنة مع الشهر السابق
        public decimal LastMonthSales { get; set; }
        public decimal Difference => TotalSales - LastMonthSales;

        // الرسم البياني (أيام الشهر)
        public List<string> Days { get; set; } = new();
        public List<decimal> DailySales { get; set; } = new();

        // أكثر المنتجات مبيعاً
        public List<TopProductVM> TopProducts { get; set; } = new();

        // ===== المشتريات =====
        public decimal TotalPurchases { get; set; }

        // ===== الربح =====
        public decimal MonthlyProfit => TotalSales - TotalPurchases;

        // ===== الديون =====
        public decimal SalesDebts { get; set; }      // ديون لنا (عملاء)
        public decimal PurchaseDebts { get; set; }   // ديون علينا (موردين)
        public decimal NetDebts => SalesDebts - PurchaseDebts;

    }
}
