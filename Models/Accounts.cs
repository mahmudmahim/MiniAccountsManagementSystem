namespace MiniAccountManagementSystem.Models
{
    public class Accounts
    {
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public int? ParentAccountId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
