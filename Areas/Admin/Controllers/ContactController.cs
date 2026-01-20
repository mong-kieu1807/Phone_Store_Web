using Microsoft.AspNetCore.Mvc;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        // GET: ContactController
        public ActionResult Index()
        {
            return View();
        }

    }
}
