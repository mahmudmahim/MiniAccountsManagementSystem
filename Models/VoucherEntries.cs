namespace MiniAccountManagementSystem.Models
{
    public class VoucherEntries
    {
        public int EntryId { get; set; }
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
