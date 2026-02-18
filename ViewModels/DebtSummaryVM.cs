namespace alnakhil.ViewModels
{
    public class DebtSummaryVM
    {
        public string Name { get; set; }

        public DebtType Type { get; set; }

        public decimal TotalRemaining { get; set; }

        public int InvoicesCount { get; set; }

        public DateTime? NearestDueDate { get; set; }
    }
}
