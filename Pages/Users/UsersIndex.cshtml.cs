using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniAccountManagementSystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;

        public IndexModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [BindProperty]
        public string SelectedUserId { get; set; }

        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public class UserViewModel
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Roles { get; set; }
        }


        public void OnGet()
        {
            LoadUsers();
        }

        //By this method we can delete the specific User
        public IActionResult OnPostDelete(string userId)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ManageUsersAndRoles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "DELETE_USER");
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToPage();
        }



        //All user loading here 
        private void LoadUsers()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ManageUsersAndRoles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "GET_USERS");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Users.Add(new UserViewModel
                            {
                                Id = reader.GetString(0),
                                UserName = reader.GetString(1),
                                Email = reader.GetString(2),
                                Roles = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }
        }

    }
}
