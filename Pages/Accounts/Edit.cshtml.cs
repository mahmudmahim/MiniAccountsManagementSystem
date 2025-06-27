using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Pages.Accounts
{
    public class EditModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public EditModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        [BindProperty]
        public Account Account { get; set; }
        public SelectList ParentAccounts { get; set; }

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
            ParentAccounts = new SelectList(await GetParentAccountsAsync(), "AccountId", "AccountName", Account.ParentAccountId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                ParentAccounts = new SelectList(await GetParentAccountsAsync(), "AccountId", "AccountName", Account.ParentAccountId);
                return Page();
            }

            var parameters = new[]
            {
                new SqlParameter("@Action", "UPDATE"),
                new SqlParameter("@AccountId", id),
                new SqlParameter("@AccountName", Account.AccountName),
                new SqlParameter("@AccountType", Account.AccountType),
                new SqlParameter("@Balance", Account.Balance),
                new SqlParameter("@CreatedDate", Account.CreatedDate),
                new SqlParameter("@ParentAccountId", (object)Account.ParentAccountId ?? DBNull.Value)
            };
            await _dbAccess.ExecuteNonQueryAsync("sp_ManageChartofAccounts", parameters);
            return RedirectToPage("/Accounts/AccountsIndex");
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
