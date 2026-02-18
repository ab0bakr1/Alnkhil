using System.ComponentModel.DataAnnotations;

namespace alnakhil.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "اسم التصنيف")]
        public string Name { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}