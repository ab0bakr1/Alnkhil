using System.ComponentModel.DataAnnotations;

namespace alnakhil.Models
{
    public enum InventoryTransactionType
    {
        Purchase = 1,
        Sale = 2,
        Adjustment = 3
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // قبل وبعد
        public int QuantityBefore { get; set; }
        public int QuantityChanged { get; set; } // + أو -
        public int QuantityAfter { get; set; }

        public InventoryTransactionType Type { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string UserName { get; set; }

        [MaxLength(200)]
        public string? Note { get; set; }
    }
}
