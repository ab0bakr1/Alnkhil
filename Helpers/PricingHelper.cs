namespace alnakhil.Helpers
{
    public static class PricingHelper
    {
        public static decimal CalculateSalePrice(
            decimal unitPurchasePrice,
            decimal profitPercentage,
            decimal? manualSalePrice = null
        )
        {
            if (manualSalePrice.HasValue)
                return manualSalePrice.Value;

            var priceWithProfit =
                unitPurchasePrice * (1 + profitPercentage / 100m);

            // 🔴 رفع دائم إلى الـ 100 التالية
            return (Math.Floor(priceWithProfit / 100) + 1) * 100;
        }
    }
}
