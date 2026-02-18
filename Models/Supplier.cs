using System.ComponentModel.DataAnnotations;

namespace alnakhil.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Phone { get; set; }

        // 🆕 ربط بالمشتريات
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
