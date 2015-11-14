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

namespace DocumentFlow.Controllers
{
    public class AdminController : Controller
    {
        // GET

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



        public ActionResult DocumentTemplates()
        {
            return View("Index/DocumentTemplates");
        }
        public ActionResult EditTemplate()
        {
            return View("Edit/EditTemplate");
        }
        public ActionResult CreateTemplate()
        {
            return View("Create/CreateTemplate",new DocumentTemplate());
        }
       public ActionResult Users()
        {
            return View("Index/Users", UserManager.Users.ToList());
        }

        public ActionResult Roles()
        {
            return View("Index/Roles",RoleManager.Roles.ToList());
        }

        public ActionResult CreateRole()
        {
            return View("Create/CreateRole");
        }
        [HttpPost]
        public async Task<ActionResult> CreateRole(CreateRoleModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new ApplicationRole
                {
                    Name = model.Name,
                    Description = model.Description
                });
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    ModelState.AddModelError("", "Что-то пошло не так");
                }
            }
            return View("Create/CreateRole",model);
        }

        public async Task<ActionResult> EditUser(string id)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(id);
                if (user != null)
                {
                    return View("Edit/EditUser", new  EditUserModel  { FirstName = user.FirstName, LastName = user.LastName, Patronymic = user.Patronymic, Position = user.Position });
                }
            }
            return RedirectToAction("Users");
        }
        [HttpPost]
        public async Task<ActionResult> EditUser(EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Patronymic = model.Patronymic;
                    user.Position = model.Position;
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Что-то пошло не так");
                    }
                }
            }
            return View("Edit/EditUser", model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }

        public async Task<ActionResult> EditRole(string id)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole role = await RoleManager.FindByIdAsync(id);
                if (role != null)
                {
                    return View("Edit/EditRole",new EditRoleModel { Name = role.Name, Description = role.Description });
                }
            }
            return RedirectToAction("Roles");
        }
        [HttpPost]
        public async Task<ActionResult> EditRole(EditRoleModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole role = await RoleManager.FindByIdAsync(model.Id);
                if (role != null)
                {
                    role.Description = model.Description;
                    role.Name = model.Name;
                    IdentityResult result = await RoleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Что-то пошло не так");
                    }
                }
            }
            return View("Edit/EditRole",model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteRole(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
            }
            return RedirectToAction("Roles");
        }

        [HttpGet]
        public ActionResult Positions()
        {
            var positions = new List<Position>();
            using (ApplicationContext context = new ApplicationContext())
            {
                positions = context.Positions.ToList();
            }
            return View("Index/Positions",positions);
        }

        [HttpGet]
        public ActionResult CreatePosition()
        {
            return View("Create/CreatePosition");
        }

        [HttpPost]
        public ActionResult CreatePosition(Position model)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    context.Positions.Add(new Position() { Id = "", Name = model.Name });
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Positions");
        }

        [HttpGet]
        public async Task<ActionResult> EditPosition(string id)
        {
            Position editPosition = new Position();
            using(ApplicationContext context = new ApplicationContext())
            {
                var position = await context.Positions.FindAsync(id);
                editPosition.Id = position.Id;
                editPosition.Name = position.Name;
            }
            return View("Edit/EditPosition",editPosition);
        }

        [HttpPost]
        public async Task<ActionResult> EditPosition(Position model)
        {
            using(ApplicationContext context = new ApplicationContext())
            {
                var position = await context.Positions.FindAsync(model.Id);
                context.Entry(position).CurrentValues.SetValues(model);
                context.SaveChanges();
            }
            return RedirectToAction("Positions");
        }

        [HttpGet]
        public async Task<ActionResult> DeletePosition(string id)
        {
            //make async
            using(ApplicationContext context = new ApplicationContext())
            {
                var position = await context.Positions.FindAsync(id);
                context.Positions.Remove(position);
                context.SaveChanges();
            }
            return RedirectToAction("Positions");
        }
    }
}