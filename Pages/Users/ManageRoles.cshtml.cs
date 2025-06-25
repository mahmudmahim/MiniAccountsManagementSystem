using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniAccountManagementSystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;

        public ManageRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public string SelectedRole { get; set; }

        public IdentityUser User { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            try
            {
                UserId = userId;
                User = await _userManager.FindByIdAsync(userId);
                if (User == null) return NotFound();

                var allRoles = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
                Roles.AddRange(allRoles);

                var userRoles = await _userManager.GetRolesAsync(User);
                UserRoles.AddRange(userRoles);

                return Page();
            }
            catch (Exception ex) 
            {
                throw new Exception("Not Found");
                return Page();
            }
          
        }


        public async Task<IActionResult> OnPostAssignAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                User = await _userManager.FindByIdAsync(UserId);
                if (User != null && !string.IsNullOrEmpty(SelectedRole))
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("sp_ManageUsersAndRoles", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Action", "ASSIGN_ROLE");
                            command.Parameters.AddWithValue("@UserId", UserId);
                            command.Parameters.AddWithValue("@RoleName", SelectedRole);
                            command.ExecuteNonQuery();
                        }
                    }
                    await _userManager.AddToRoleAsync(User, SelectedRole);
                }
                return RedirectToPage(new { userId = UserId });
            }
            catch (Exception ex)
            {
                throw new Exception("Update Failed. Please try again!!");
            }
        }

        public async Task<IActionResult> OnPostRemoveAsync(string roleName)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            User = await _userManager.FindByIdAsync(UserId);
            if (User != null && !string.IsNullOrEmpty(roleName))
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_ManageUsersAndRoles", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Action", "REMOVE_ROLE");
                        command.Parameters.AddWithValue("@UserId", UserId);
                        command.Parameters.AddWithValue("@RoleName", roleName);
                        command.ExecuteNonQuery();
                    }
                }
                await _userManager.RemoveFromRoleAsync(User, roleName);
            }
            return RedirectToPage(new { userId = UserId });
        }
    }
}
