using alnakhil.Models; // هذا السطر هو الحل
namespace alnakhil.ViewModels
{
    public class DashboardVM
    {
        // ===== أرقام عامة =====
        public int ProductsCount { get; set; }
        public int LowStockCount { get; set; }
        
        // أضف هذا السطر هنا لحل المشكلة
        public int ExpiringSoonCount { get; set; } 

        public decimal TodaySales { get; set; }
        public decimal TodayPurchases { get; set; }

        public decimal ReceivableDebts { get; set; } 
        public decimal PayableDebts { get; set; }    

        public List<Sale> RecentSales { get; set; } = new List<Sale>(); 

        public decimal MonthlyProfit => TodaySales - TodayPurchases; 

        public List<Product> ExpiringSoonProducts { get; set; } = new List<Product>();

        // ===== رسوم بيانية =====
        public List<string> Months { get; set; } = new();
        public List<decimal> SalesByMonth { get; set; } = new();
        public List<decimal> PurchasesByMonth { get; set; } = new();
    }
}