using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,Merchant")]
    public class BranchsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
