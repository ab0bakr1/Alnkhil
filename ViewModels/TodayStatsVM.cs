namespace alnakhil.ViewModels
{
    public class TodayStatsVM
    {
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal Profit { get; set; }

        public int SalesCount { get; set; }
        public int PurchasesCount { get; set; }

        public decimal NewDebts { get; set; }
        public decimal CollectedToday { get; set; }

        // مقارنة مع أمس
        public decimal YesterdaySales { get; set; }
        public decimal Difference => TotalSales - YesterdaySales;

        // رسم بياني (ساعات اليوم)
        public List<string> Hours { get; set; } = new();
        public List<decimal> HourlySales { get; set; } = new();

        // أكثر المنتجات مبيعاً
        public List<TopProductVM> TopProducts { get; set; } = new();
    }

    public class TopProductVM
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
