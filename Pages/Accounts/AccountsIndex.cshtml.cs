using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using MiniAccountManagementSystem.Models;
using MiniAccountManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
namespace MiniAccountManagementSystem.Pages.Accounts
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DbAccess _dbAccess;

        public IndexModel(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

        public async Task<IActionResult> OnPostExportAsync()
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

            return await ExportToExcel();
        }

        private async Task<IActionResult> ExportToExcel()
        {
            if (Accounts == null || !Accounts.Any())
            {
                return new NotFoundResult(); 
            }
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Accounts");
            worksheet.Cells[1, 1].Value = "Account ID";
            worksheet.Cells[1, 2].Value = "Account Name";
            worksheet.Cells[1, 3].Value = "Account Type";
            worksheet.Cells[1, 4].Value = "Balance";
            worksheet.Cells[1, 5].Value = "Created Date";
            worksheet.Cells[1, 6].Value = "Parent Account ID";

            for (int i = 0; i < Accounts.Count; i++)
            {
                var account = Accounts[i];
                worksheet.Cells[i + 2, 1].Value = account.AccountId;
                worksheet.Cells[i + 2, 2].Value = account.AccountName;
                worksheet.Cells[i + 2, 3].Value = account.AccountType;
                worksheet.Cells[i + 2, 4].Value = account.Balance;
                worksheet.Cells[i + 2, 5].Value = account.CreatedDate.ToString("yyyy-MM-dd");
                worksheet.Cells[i + 2, 6].Value = account.ParentAccountId ?? (object)"None";
            }

            worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AccountsData.xlsx");
        }

    }
}
