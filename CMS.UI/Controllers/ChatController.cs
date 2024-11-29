using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Employee> _employeeRepository;

        public ChatController(UserManager<AppUser> userManager,
            IGenericRepository<Employee> employeeRepository)
        {
            _userManager = userManager;
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Chat()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            List<UserVM> userModels = new();
            if (isAdmin)
            {
                List<EmployeeVM> employeeModels = new();
                var users = await _userManager.GetUsersInRoleAsync("User");
                foreach (var item in users)
                {
                    userModels.Add(new()
                    {
                        
                        Id=item.Id,
                        UserName=item.UserName,
                        ConnectionId=item.ConnectionId
                    });
                }
                
            }
            else
            {
                var users = await _userManager.GetUsersInRoleAsync("Admin");
              
                foreach (var item in users)
                {
                    userModels.Add(new()
                    {
                        Id=item.Id,
                       UserName = item.UserName,
                       ConnectionId=item.ConnectionId
                    });
                }
            }
            return View(userModels);
        }
    }
}
