using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Pages.Accounts
{
    public class DeleteModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public DeleteModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        [BindProperty]
        public Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var parameters = new[] { new SqlParameter("@Action", "SELECT_BY_ID"), new SqlParameter("@AccountId", id) };
            var accounts = await _dbAccess.ExecuteQueryAsync("sp_ManageChartofAccounts", parameters, reader => new Account
            {
                AccountId = reader.GetInt32(reader.GetOrdinal("AccountId")),
                AccountName = reader.IsDBNull(reader.GetOrdinal("AccountName")) ? null : reader.GetString(reader.GetOrdinal("AccountName")),
                AccountType = reader.IsDBNull(reader.GetOrdinal("AccountType")) ? null : reader.GetString(reader.GetOrdinal("AccountType")),
                Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ParentAccountId = reader.IsDBNull(reader.GetOrdinal("ParentAccountId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("ParentAccountId"))
            });
            Account = accounts.FirstOrDefault();
            if (Account == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var parameters = new[] { new SqlParameter("@Action", "DELETE"), new SqlParameter("@AccountId", id) };
            await _dbAccess.ExecuteNonQueryAsync("sp_ManageChartofAccounts", parameters);
            return RedirectToPage("/Accounts/AccountsIndex");
        }
    }
}
