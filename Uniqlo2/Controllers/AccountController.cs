using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using System.Net.Mail;
using System.Text;
using Uniqlo2.Enums;
using Uniqlo2.Helpers;
using Uniqlo2.Models;
using Uniqlo2.Services.Abstracts;
using Uniqlo2.ViewModels.Auths;

namespace Uniqlo2.Controllers
{
    public class AccountController(UserManager<User> _userManager,SignInManager<User> _signInManager ,IEmailService _service) : Controller
    {
       
        bool isAuthenticated => User.Identity?.IsAuthenticated ?? false;
        public    IActionResult Register()
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserCreateVM vm)
        {   if (isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid)
                return View();
            User user = new User
            {
                Email=vm.Email ,
                FullName =vm.Fullname ,
                UserName =vm.Username ,
                ImageUrl="photo.jpg" ,//@*ProfilImageUrl*@,
                
                
                

            };
            var result = await _userManager.CreateAsync(user,vm.Password );
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
                return View();
            }
            var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(); 
                

            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _service.SendEmailConfirmation(user.Email, user.UserName, token);
            return Content("Email sent!");
        }
        public async Task <IActionResult> Login()
        {
            return View();
        }
        [HttpPost ]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl=null)
        {
            if (!ModelState.IsValid) return View();
            User? user = null;
            if (vm.UsernameOrEmail.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);

            }
            else
            {
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
                
            }
            if(user is null)
            {
                ModelState.AddModelError("", "Username or Password is wrong");
                return View();
            }
            var result=await _signInManager.PasswordSignInAsync(user,vm.Password,vm.RememberMe,true);
            if(!result.Succeeded)
            {
                if(result.IsNotAllowed)
                    ModelState.AddModelError("", "You must confirm your account");
                if(result.IsLockedOut)
                    ModelState.AddModelError("", "Wait untill" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                return View();
                

            }
            if(string.IsNullOrEmpty(returnUrl))
            {   if(await _userManager.IsInRoleAsync(user,"Admin"))
                {
                    return RedirectToAction("Index", new { Controller = "DashBoard", Area = "Admin" });
                }
                return RedirectToAction("Index", "Home");
            }

            return LocalRedirect(returnUrl);

        }
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> VerifyEmail(string token, string user)
        {
            var entity = await _userManager.FindByNameAsync(user);
            if (entity is null) return BadRequest();
            var result = await _userManager.ConfirmEmailAsync(entity, token.Replace(' ', '+'));
            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in token)
                {
                    sb.AppendLine(item.ToString());

                }
                return Content(sb.ToString());
            }
            await _signInManager.SignInAsync(entity, true);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
        {
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if(user!=null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = vm.Email, token = token }, Request.Scheme);
               
            }
            return View();
        }
        
            
    }
}



