using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using alnakhil.Models;
using alnakhil.Data;

namespace alnakhil.Data
{
    public class alnakhilContext : IdentityDbContext<ApplicationUser>
    {
        public alnakhilContext(DbContextOptions<alnakhilContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        public DbSet<PricingSetting> PricingSetting { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        public DbSet<DebtPayment> DebtPayments { get; set; }

    }
}