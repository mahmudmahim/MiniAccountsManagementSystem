using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using MiniAccountManagementSystem.Models;
namespace MiniAccountManagementSystem.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        public List<Account> Accounts { get; set; } = new List<Account>();
        private IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ManageChartofAccounts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "SELECT");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Accounts.Add(new Account
                            {
                                AccountId = reader.GetInt32(0),
                                AccountName = reader.GetString(1),
                                AccountType = reader.GetString(2),
                                ParentAccountId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
        }
    }
}
