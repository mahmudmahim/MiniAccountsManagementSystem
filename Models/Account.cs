namespace MiniAccountManagementSystem.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ParentAccountId { get; set; } 
        public Account? ParentAccount { get; set; }
        public int? Level { get; set; }
    }
}
