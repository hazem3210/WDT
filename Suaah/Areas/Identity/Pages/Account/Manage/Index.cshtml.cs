// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Suaah.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string CPassword { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            //public string Role { get; set; }

        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var password = await _userManager.GeneratePasswordResetTokenAsync(user);
            var confPassword = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var role = await _userManager.roles

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Email = email,
                Password = password,
                ConfirmPassword = confPassword 
            };
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            IdentityUser user = new();

            if (id == null)
            {
                user = await _userManager.GetUserAsync(User);
                
                ViewData["flag"] = 1;

                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
            }
            else
            {
                user = _userManager.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{id}'.");
                }
            }
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            IdentityUser user = new();

            if (id == null)
            {
                user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
            }
            else
            {
                user = _userManager.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{id}'.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var password = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set Email.";
                    return RedirectToPage();
                }
                if (!setUserNameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set UserName.";
                    return RedirectToPage();
                }
            }

            if (Input.Password != password && Input.Password != null)
            {
                var setPasswordResult = await _userManager.ChangePasswordAsync(user, Input.CPassword, Input.Password);
                if (!setPasswordResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set Password.";
                    return RedirectToPage();
                }
            }

            if (id == null)
            {
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();

            }

            return Redirect("/Customer/Customers/Users");

        }
    }
}
