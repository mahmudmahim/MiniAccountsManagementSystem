using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Pages.Vouchers
{
    public class DeleteModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public DeleteModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        [BindProperty]
        public Voucher Voucher { get; set; }

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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Action", "DELETE"),
                new SqlParameter("@VoucherId", id),
                new SqlParameter("@VoucherType", DBNull.Value),
                new SqlParameter("@ReferenceNo", DBNull.Value),
                new SqlParameter("@VoucherDate", DBNull.Value),
                new SqlParameter("@CreatedBy", DBNull.Value),
                new SqlParameter("@Entries", DBNull.Value)
            };
            await _dbAccess.ExecuteNonQueryAsync("sp_SaveVoucher", parameters);
            return RedirectToPage("/Vouchers/VouchersIndex");
        }
    }
}
