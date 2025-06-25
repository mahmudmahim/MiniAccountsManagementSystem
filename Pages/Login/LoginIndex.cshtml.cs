using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiniAccountManagementSystem.Pages.Login
{
    public class LoginIndexModel : PageModel
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginIndexModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
               var user = await _userManager.FindByNameAsync(Input.Email) ?? await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToPage("/Index");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return Page();
        }
    }
}
