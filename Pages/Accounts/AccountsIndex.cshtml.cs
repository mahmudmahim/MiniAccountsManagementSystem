using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using MiniAccountManagementSystem.Models;
using MiniAccountManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
namespace MiniAccountManagementSystem.Pages.Accounts
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public IndexModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public List<Account> Accounts { get; set; }
        public List<Account> AccountTree { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Accounts = await _dbAccess.ExecuteQueryAsync("sp_ManageChartofAccounts",
                new[] { new SqlParameter("@Action", "SELECT") },
                reader => new Account
                {
                    AccountId = reader.GetInt32(reader.GetOrdinal("AccountId")),
                    AccountName = reader.IsDBNull(reader.GetOrdinal("AccountName")) ? null : reader.GetString(reader.GetOrdinal("AccountName")),
                    AccountType = reader.IsDBNull(reader.GetOrdinal("AccountType")) ? null : reader.GetString(reader.GetOrdinal("AccountType")),
                    Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    ParentAccountId = reader.IsDBNull(reader.GetOrdinal("ParentAccountId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("ParentAccountId"))
                });


            // Fetch hierarchical data for the tree
            AccountTree = await _dbAccess.ExecuteQueryAsync("sp_ManageChartofAccounts",
                new[] { new SqlParameter("@Action", "AccountHierarchy") },
                reader => new Account
                {
                    AccountId = reader.GetInt32(reader.GetOrdinal("AccountId")),
                    AccountName = reader.IsDBNull(reader.GetOrdinal("AccountName")) ? null : reader.GetString(reader.GetOrdinal("AccountName")),
                    AccountType = reader.IsDBNull(reader.GetOrdinal("AccountType")) ? null : reader.GetString(reader.GetOrdinal("AccountType")),
                    Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    ParentAccountId = reader.IsDBNull(reader.GetOrdinal("ParentAccountId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("ParentAccountId")),
                    Level = reader.GetInt32(reader.GetOrdinal("Level")) // Include Level for hierarchy
                });
            return Page();
        }

    }
}
