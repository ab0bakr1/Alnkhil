using System.Collections.Generic;


namespace alnakhil.Models
{
    public class PricingSetting
    {
        public int Id { get; set; }
        public decimal ProfitPercentage { get; set; } // مثال: 20
        public decimal TaxPercentage { get; set; } = 0; // نسبة الضريبة الثابتة
    }
}