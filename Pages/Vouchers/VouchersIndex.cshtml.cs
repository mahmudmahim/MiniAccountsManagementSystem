using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Data;
using MiniAccountManagementSystem.Models;

namespace MiniAccountManagementSystem.Pages.Vouchers
{
    public class IndexModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public IndexModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }
        public List<Voucher> Vouchers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var vouchersData = await _dbAccess.ExecuteQueryAsync("sp_GetVouchers",
                null, 
                reader =>
                {
                    // Handle all potential null values
                    int voucherId = reader.IsDBNull(reader.GetOrdinal("VoucherId")) ? 0 : reader.GetInt32(reader.GetOrdinal("VoucherId"));
                    string? voucherType = reader.IsDBNull(reader.GetOrdinal("VoucherType")) ? null : reader.GetString(reader.GetOrdinal("VoucherType"));
                    string? referenceNo = reader.IsDBNull(reader.GetOrdinal("ReferenceNo")) ? null : reader.GetString(reader.GetOrdinal("ReferenceNo"));
                    DateTime voucherDate = reader.IsDBNull(reader.GetOrdinal("VoucherDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("VoucherDate"));
                    string? createdBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy"));
                    int? entryId = reader.IsDBNull(reader.GetOrdinal("EntryId")) ? null : reader.GetInt32(reader.GetOrdinal("EntryId"));
                    int? accountId = reader.IsDBNull(reader.GetOrdinal("AccountId")) ? null : reader.GetInt32(reader.GetOrdinal("AccountId"));
                    decimal debit = reader.IsDBNull(reader.GetOrdinal("Debit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Debit"));
                    decimal credit = reader.IsDBNull(reader.GetOrdinal("Credit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Credit"));

                    return new
                    {
                        VoucherId = voucherId,
                        VoucherType = voucherType,
                        ReferenceNo = referenceNo,
                        VoucherDate = voucherDate,
                        CreatedBy = createdBy,
                        EntryId = entryId,
                        AccountId = accountId,
                        Debit = debit,
                        Credit = credit
                    };
                });

            // Check if vouchersData is empty and set Vouchers accordingly
            if (!vouchersData.Any())
            {
                Vouchers = new List<Voucher>();
            }
            else
            {
                Vouchers = vouchersData
                    .Where(v => v.VoucherId > 0) // Filter out invalid or default rows
                    .GroupBy(v => v.VoucherId)
                    .Select(g => new Voucher
                    {
                        VoucherId = g.Key,
                        VoucherType = g.First().VoucherType,
                        ReferenceNo = g.First().ReferenceNo,
                        VoucherDate = g.First().VoucherDate,
                        CreatedBy = g.First().CreatedBy,
                        Entries = g.Where(e => e.EntryId.HasValue)
                                  .Select(e => new VoucherEntry
                                  {
                                      EntryId = e.EntryId.Value,
                                      VoucherId = g.Key,
                                      AccountId = e.AccountId.Value,
                                      Debit = e.Debit,
                                      Credit = e.Credit
                                  }).ToList()
                    }).ToList();
            }

            return Page();
        }
    }
}
