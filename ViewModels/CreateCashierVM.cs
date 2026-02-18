using System.ComponentModel.DataAnnotations;

namespace alnakhil.ViewModels
{
    public class CreateCashierVM
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string FullName { get; set; }    
    }
}
