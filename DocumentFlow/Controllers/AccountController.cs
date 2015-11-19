using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using DocumentFlow.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Net.Mail;

namespace DocumentFlow.Controllers
{
    public class AccountController : Controller
    {

        public static string FullName;
        public static string UserId;

        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user =
                    new ApplicationUser
                    {
                        UserName = model.Login,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Patronymic = model.Patronymic,
                        Position = model.Position,
                        Email = model.Email
                    };

                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    FullName = user.FirstName + " " + user.LastName;
                    UserId = user.Id;

                    await UserManager.AddToRoleAsync(UserId, "Admin");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindAsync(model.Login, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                    {
                        UserId = user.Id;
                        FullName = user.FirstName + " " + user.LastName;

                        if (UserManager.IsInRole(UserId, "Admin"))
                        {
                            return RedirectToAction("Roles", "Admin");
                        }
                        return RedirectToAction("Index", "Main");
                    }
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }


        [HttpGet]
        public ActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> DeleteConfirmed()
        {
            ApplicationUser user = await UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            ApplicationUser user = await UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                EditModel model = new EditModel
                {
                   // Login = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Patronymic = user.Patronymic,
                  //  Email = user.Email,
                    Position = user.Position
                };
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditModel model)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.Login;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Patronymic = model.Patronymic;
                user.Position = model.Position;
                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    FullName = user.FirstName + " " + user.LastName;
                    return RedirectToAction("Index", "Main");
                }
                else
                {
                    ViewBag.ErrorString = "Не удалось обновить данные. Попробуйте еще раз.";
                    return RedirectToAction("Error", "Account");
                }
            }
            else
            {
                ViewBag.ErrorString = "Не удалось найти пользователя. Попробуйте еще раз.";
                return RedirectToAction("Error", "Account");
            }
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            UserId = null;
            FullName = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View("ForgotPasswordConfirmation");
                }
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Сброс пароля",
                    "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
