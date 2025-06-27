using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;
using System.Text.Json;

namespace MiniAccountManagementSystem.Pages.Vouchers
{
    public class EditModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public EditModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        [BindProperty]
        public Voucher Voucher { get; set; }
        public IEnumerable<Account> AccountsList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Action", "SELECT_BY_ID"), 
                new SqlParameter("@VoucherId", id)
            };

            var voucherData = await _dbAccess.ExecuteQueryAsync("sp_SaveVoucher", parameters, reader => new
            {
                VoucherId = reader.GetInt32(reader.GetOrdinal("VoucherId")),
                VoucherType = reader.IsDBNull(reader.GetOrdinal("VoucherType")) ? null : reader.GetString(reader.GetOrdinal("VoucherType")),
                ReferenceNo = reader.IsDBNull(reader.GetOrdinal("ReferenceNo")) ? null : reader.GetString(reader.GetOrdinal("ReferenceNo")),
                VoucherDate = reader.GetDateTime(reader.GetOrdinal("VoucherDate")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                EntryId = reader.IsDBNull(reader.GetOrdinal("EntryId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("EntryId")),
                AccountId = reader.IsDBNull(reader.GetOrdinal("AccountId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AccountId")),
                Debit = reader.GetDecimal(reader.GetOrdinal("Debit")),
                Credit = reader.GetDecimal(reader.GetOrdinal("Credit"))
            });

            var voucherGroup = voucherData.Where(v => v.VoucherId == id)
                .GroupBy(v => v.VoucherId)
                .FirstOrDefault();
            if (voucherGroup == null)
            {
                return NotFound();
            }

            Voucher = new Voucher
            {
                VoucherId = voucherGroup.Key,
                VoucherType = voucherGroup.First().VoucherType,
                ReferenceNo = voucherGroup.First().ReferenceNo,
                VoucherDate = voucherGroup.First().VoucherDate,
                CreatedBy = voucherGroup.First().CreatedBy,
                Entries = voucherGroup.Where(e => e.EntryId.HasValue)
                    .Select(e => new VoucherEntry
                    {
                        EntryId = e.EntryId.Value,
                        VoucherId = id,
                        AccountId = e.AccountId.Value,
                        Debit = e.Debit,
                        Credit = e.Credit
                    }).ToList()
            };

            AccountsList = await GetAccountsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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
                new SqlParameter("@Action", "UPDATE"),
                new SqlParameter("@VoucherId", id),
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
