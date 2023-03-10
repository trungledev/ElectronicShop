// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ElectronicShop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

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
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [Display(Name = "H??? v?? t??n")]
            public string HoVaTen { get; set; }

        }
        //Duoc goi thong qua page cua ExternalLogin.cshtml
        public IActionResult OnGet() => RedirectToPage("./Login");
        //submit tai 2 trang dang nhap va dang ky => chuyen den trang xac thuc tu provider(fb, google)
        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Ki???m tra y??u c???u d???ch v??? provider t???n t???i






            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        //Duoc goi khi da ket thuc trang xac thuc tu provider
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            // L???y th??ng tin do d???ch v??? ngo??i chuy???n ?????n
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "L???i th??ng tin t??? d???ch v??? ????ng nh???p.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            //???? c??  user t???n t???i trong database v?? ???? t???o li??n k???t
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // User ????ng nh???p th??nh c??ng v??o h??? th???ng theo th??ng tin info
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                // B??? t???m kh??a
                return RedirectToPage("./Lockout");
            }
            else
            {

                // ???? c?? Acount, ???? li??n k???t v???i t??i kho???n ngo??i - nh??ng kh??ng ????ng nh???p ???????c
                // c?? th??? do ch??a k??ch ho???t email => chuy???n h?????ng sang page RegisterConfirm


                // Ch??a c?? Account li??n k???t v???i t??i kho???n ngo??i
                // Hi???n th??? form ????? th???c hi???n b?????c ti???p theo ??? OnPostConfirmationAsync
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        HoVaTen = info.Principal.FindFirstValue(ClaimTypes.Name)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Nh???n th??ng tin v??? ng?????i d??ng t??? nh?? cung c???p ????ng nh???p b??n ngo??i
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "L???i khi t???i th??ng tin ????ng nh???p b??n ngo??i trong khi x??c nh???n.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                //C??c tr?????ng h???p khi ????ng nh???p/k?? t??? dv ngo??i 
                /*
                    1,???? c?? t??i kho???n trong database nh??ng ch??a t???o li??n k???t
                    2,T???o m???i t??i kho???n t??? d???ch v??? ngo??i 
                */
                //T???o li??n k???t v???i t??i kho???n trong database

                //  T??m user c?? email l?? email t??? provider
                string externalEmail = null;
                //Ki???m tra trong privider c?? claim lo???i email ?
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                }
                //Ki???m tra c?? t???n t???i user c?? externalEmail ?
                var userExternal = (externalEmail != null) ? (await _userManager.FindByEmailAsync(externalEmail)) : null;
                if(userExternal != null)
                {
                    
                     if (!userExternal.EmailConfirmed) {
                        var codeActive = await _userManager.GenerateEmailConfirmationTokenAsync (userExternal);
                        await _userManager.ConfirmEmailAsync (userExternal, codeActive);
                    }
                    //C?? user c?? externalEmail => t???o li??n k???t
                    var resultAdd = await _userManager.AddLoginAsync(userExternal,info);
                    if(resultAdd.Succeeded)
                    {
                        //T???o li??n k???t th??nh c??ng => ????ng nh???p
                        await _signInManager.SignInAsync(userExternal,isPersistent:false,info.LoginProvider);
                        return LocalRedirect(returnUrl);

                    }
                }

                //T???o m???i t??i kho???n t??? d???ch v??? ngo??i 
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FullName = Input.HoVaTen;

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //T???o li??n k???t v???i _userManager.AddLoginAsync(user, info);
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        //Li??n k???t th??nh c??ng
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        //X??c th???c email
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        //N???u c???n x??c nh???n t??i kho???n, ch??ng t??i c???n hi???n th??? li??n k???t n???u ch??ng t??i kh??ng c?? ng?????i g???i email th???c
                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
