namespace MiniAccountManagementSystem.Models
{
    public class Vouchers
    {
        public int VoucherId { get; set; }
        public string? VoucherType { get; set; }
        public string? ReferenceNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string? CreatedBy { get; set; }
        public List<VoucherEntries> Entries { get; set; } = new List<VoucherEntries>();
    }
}
