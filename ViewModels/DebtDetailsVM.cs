namespace alnakhil.ViewModels
{
    public class DebtDetailsVM
    {
        public string Name { get; set; }
        public DebtType Type { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalRemaining { get; set; }

        public List<DebtVM> Invoices { get; set; } = new();
    }
}
