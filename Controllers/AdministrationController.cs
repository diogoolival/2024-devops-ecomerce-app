using ECommerceApp.Areas.Identity.Data;
using ECommerceApp.Data;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ECommerceApp.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly ECommerceIdentityContext _context = new ECommerceIdentityContext();
        private readonly UserManager<ECommerceAppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdministrationController(UserManager<ECommerceAppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Users()
        {
            
            return View(_userManager.Users.ToList());
        }

        public IActionResult AddRemoveRole(string userId, string roleId)
        {
            var dbUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            var role = _roleManager.Roles.First(r => r.Id == roleId);
            var userRoleEntry = _context.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId && ur.UserId == userId);

            if (userRoleEntry != null)
            {
                _context.UserRoles.Remove(userRoleEntry);
            }
            else
            {
                _context.UserRoles.Add(new IdentityUserRole<string>() { UserId = userId, RoleId = roleId });
            }

            _context.SaveChanges();

            return RedirectToAction("UserRoles", new { id = userId });
        }

        public IActionResult UserRoles(string id)
        {
            var dbUser = _userManager.Users.FirstOrDefault(u => u.Id == id);
            
            var model = new UserRoleModel();
            model.Id = dbUser.Id;
            model.UserName = dbUser.UserName;
            model.Email = dbUser.Email;
            model.PhoneNumber = dbUser.PhoneNumber;

            model.roles = new List<RoleModel>();

            foreach (var role in _roleManager.Roles.ToList())
            {
                var hasRole = _context.UserRoles.Count(ur => ur.RoleId == role.Id && ur.UserId == dbUser.Id) > 0;
                var roleModel = new RoleModel() { RoleId = role.Id, Name = role.Name, HasRole = hasRole };


                model.roles.Add(roleModel);
            }

            return View(model);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(ECommerceAppUser user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            return View(user);
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            return View(role);
        }
    }
}
