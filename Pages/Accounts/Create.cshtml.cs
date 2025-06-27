using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Pages.Accounts
{
    public class CreateModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public CreateModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        [BindProperty]
        public Account Account { get; set; }
        public SelectList ParentAccounts { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var accounts = await _dbAccess.ExecuteQueryAsync("sp_ManageChartofAccounts",
                new[] { new SqlParameter("@Action", "SELECT") },
                reader => new Account
                {
                    AccountId = reader.GetInt32(reader.GetOrdinal("AccountId")),
                    AccountName = reader.GetString(reader.GetOrdinal("AccountName"))
                });
            ParentAccounts = new SelectList(accounts, "AccountId", "AccountName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ParentAccounts = new SelectList(await GetParentAccountsAsync(), "AccountId", "AccountName");
                return Page();
            }

            var parameters = new[]
            {
                new SqlParameter("@Action", "INSERT"),
                new SqlParameter("@AccountName", Account.AccountName),
                new SqlParameter("@AccountType", Account.AccountType),
                new SqlParameter("@Balance", Account.Balance),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@ParentAccountId", (object)Account.ParentAccountId ?? DBNull.Value)
            };
            await _dbAccess.ExecuteNonQueryAsync("sp_ManageChartofAccounts", parameters);
            return RedirectToPage("/Index");
        }

        private async Task<List<Account>> GetParentAccountsAsync()
        {
            return await _dbAccess.ExecuteQueryAsync("sp_ManageChartofAccounts",
                new[] { new SqlParameter("@Action", "SELECT") },
                reader => new Account
                {
                    AccountId = reader.GetInt32(reader.GetOrdinal("AccountId")),
                    AccountName = reader.GetString(reader.GetOrdinal("AccountName"))
                });
        }
    }
}
