using System.ComponentModel.DataAnnotations;

namespace alnakhil.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }
        public string Role { get; set; } // Admin - Cashier
    }
}
