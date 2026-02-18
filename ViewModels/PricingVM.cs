namespace alnakhil.ViewModels
{
    public class PricingVM
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal LastPurchasePrice { get; set; }

        public int Quantity { get; set; }

        public decimal SalePrice { get; set; }
    }
}
