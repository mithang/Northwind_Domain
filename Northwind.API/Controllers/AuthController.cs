using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Auth;
using Northwind.API.ViewModels;
using Google.Apis.Auth;
using Northwind.API.Helpers;
using Northwind.API.Models;
using Northwind.API.Services;

namespace Northwind.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private IAuthService _authService;

        public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _authService = authService;
        }
        

       
        //[AllowAnonymous]
        //public IActionResult Login(string returnUrl)
        //{
        //    return View(new LoginViewModel
        //    {
        //        ReturnUrl = returnUrl
        //    });
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        //{
        //    if (!ModelState.IsValid)
        //        return View(loginViewModel);

        //    var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

        //    if (user != null)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
        //        if (result.Succeeded)
        //        {
        //            if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
        //                return RedirectToAction("Index", "Home");

        //            return Redirect(loginViewModel.ReturnUrl);
        //        }
        //    }

        //    ModelState.AddModelError("", "Username/password not found");
        //    return View(loginViewModel);
        //}

        [HttpPost("logintoken")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            

            if (!ModelState.IsValid)
                return BadRequest(ReponseResult.ReponseValid(IsSuccess:true,Data: loginViewModel));

            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (user != null)
            {
                //isPersistent: lưu password trong cookie
                //lockoutOnFailure: false là không khóa tài khoản
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                //IsLockedOut kết họp với LockedOutEnd và LockedOutEnabled thời gian khóa tài khoản khi số lần login bị sai password AccessFaildCount
                if (result.IsLockedOut)
                {
                    return BadRequest(ReponseResult.ReponseValid(IsSuccess: true, Data: "",Message:"Tài khoản của bạn đã bị khóa !"));
                }
                if (result.IsNotAllowed)
                {

                }
                if (result.RequiresTwoFactor)
                {

                }
                
                if (result.Succeeded)
                {
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    return CreateToken(userClaims, user);
                }
            }

            ModelState.AddModelError("", "Username/password not found");
            return BadRequest(loginViewModel);
        }

        //public IActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(LoginViewModel loginViewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser() { UserName = loginViewModel.UserName };
        //        var result = await _userManager.CreateAsync(user, loginViewModel.Password);

        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    return View(loginViewModel);
        //}

        [HttpPost("registertoken")]
        public async Task<IActionResult> Register(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = loginViewModel.UserName };
                var result = await _userManager.CreateAsync(user, loginViewModel.Password);

                if (result.Succeeded)
                {
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    return CreateToken(userClaims, user);
                }
            }
            return BadRequest(loginViewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index", "Home");
        //}

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return Ok();
        }

        //public IActionResult AccessDenied()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //public IActionResult GoogleLogin(string returnUrl = null)
        //{
        //    var redirectUrl = Url.Action("GoogleLoginCallback", "Account", new { ReturnUrl = returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(ExternalLoginServiceConstants.GoogleProvider, redirectUrl);
        //    return Challenge(properties, ExternalLoginServiceConstants.GoogleProvider);
        //}

        //[AllowAnonymous]
        //public async Task<IActionResult> GoogleLoginCallback(string returnUrl = null, string serviceError = null)
        //{
        //    if (serviceError != null)
        //    {
        //        ModelState.AddModelError(string.Empty, $"Error from external provider: {serviceError}");
        //        return View(nameof(Login));
        //    }

        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return RedirectToAction(nameof(Login));
        //    }

        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        //    if (result.Succeeded)
        //    {
        //        if (returnUrl == null)
        //            return RedirectToAction("index", "home");

        //        return Redirect(returnUrl);
        //    }

        //    var user = new ApplicationUser
        //    {
        //        Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
        //        UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
        //    };

        //    var identityResult = await _userManager.CreateAsync(user);

        //    if (!identityResult.Succeeded) return AccessDenied();

        //    identityResult = await _userManager.AddLoginAsync(user, info);

        //    if (!identityResult.Succeeded) return AccessDenied();

        //    await _signInManager.SignInAsync(user, false);

        //    if (returnUrl == null)
        //        return RedirectToAction("index", "home");

        //    return Redirect(returnUrl);
        //}
        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody]UserView userView)
        {
            try
            {
                //SimpleLogger.Log("userView = " + userView.tokenId);
                var payload = GoogleJsonWebSignature.ValidateAsync(userView.tokenId, new GoogleJsonWebSignature.ValidationSettings()).Result;
                var user = await _authService.Authenticate(payload);
                SimpleLogger.Log(payload.ExpirationTimeSeconds.ToString());

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, Security.Encrypt("minhtv",user.email)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(issuer: _configuration["Tokens:Issuer"],
                    audience: _configuration["Tokens:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                Helpers.SimpleLogger.Log(ex);
                BadRequest(ex.Message);
            }
            return BadRequest();
        }
    
    private dynamic CreateToken(IEnumerable<Claim> userClaims, ApplicationUser user)
        {
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                //new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email)
                //new Claim("NhanVien","All") dùng khi c.AddPolicy("NhanVien", p => p.RequireClaim("NhanVien", "All")); và [Authorize(Policy = "NhanVien")]public class EmployeesController

            }.Union(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _configuration["Tokens:Issuer"],
                audience: _configuration["Tokens:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}
