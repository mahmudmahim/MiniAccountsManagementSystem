using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniAccountManagementSystem.Pages.Vouchers
{
    public class CreateModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public CreateModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        [BindProperty]
        public Voucher Voucher { get; set; }
        public IEnumerable<Account> AccountsList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AccountsList = await GetAccountsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AccountsList = await GetAccountsAsync();
                return Page();
            }

            // Debug: Check if Entries is populated
            if (Voucher.Entries == null || !Voucher.Entries.Any())
            {
                ModelState.AddModelError("", "Please add at least one entry.");
                AccountsList = await GetAccountsAsync();
                return Page();
            }


            var entriesJson = JsonSerializer.Serialize(Voucher.Entries);
            var parameters = new[]
            {
                new SqlParameter("@VoucherType", Voucher.VoucherType),
                new SqlParameter("@ReferenceNo", Voucher.ReferenceNo),
                new SqlParameter("@VoucherDate", Voucher.VoucherDate),
                new SqlParameter("@CreatedBy", Voucher.CreatedBy ?? User.Identity?.Name),
                new SqlParameter("@Entries", entriesJson)
            };
            await _dbAccess.ExecuteNonQueryAsync("sp_SaveVoucher", parameters);
            return RedirectToPage("/Vouchers/VouchersIndex");
        }

        private async Task<List<Account>> GetAccountsAsync()
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
