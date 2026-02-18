using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace alnakhil.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Barcode { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalePrice { get; set; }

        public bool IsManualPrice { get; set; }
        public decimal? ManualSalePrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        public int? CategoryId { get; set; }

        [ValidateNever] 
        public string CategoryName { get; set; }
    }
}
