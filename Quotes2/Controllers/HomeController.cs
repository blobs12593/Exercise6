using System.Linq;
using System.Web.Mvc;
using Quotes2.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Quotes2.Controllers
{
    public class HomeController : Controller
    {
        private Quotes2Context db = new Quotes2Context();
        //private UserManager<ApplicationUser> manager;
        //private UserStore<ApplicationUser> manager2;

        //public HomeController()
        //{
            //manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            //manager2 = new UserStore<ApplicationUser>(db);
        //}
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult God_Mode()
        {
            ViewBag.Message = "God powers activate!";
            ViewBag.NumberOfUsers = 0;
            ViewBag.NumberOfQuotes = 0;
            ViewBag.NumberOfCats = 0;

            var myContext = new ApplicationDbContext();
            var users = myContext.Users.ToList();
            foreach (ApplicationUser currentUser in users)
            {
                ViewBag.NumberOfUsers += 1;
            }
            foreach (Quotation quote in db.Quotations)
            {
                ViewBag.NumberOfQuotes += 1;
            }
            foreach (Category myCats in db.Categories)
            {
                ViewBag.NumberOfCats += 1;
            }

            return View();
        }

        public ActionResult Nuke()
        {
            //ApplicationUser currentUser = manager.FindById(User.Identity.GetUserId());
            var myContext = new ApplicationDbContext();
            var users = myContext.Users.ToList();
            foreach (ApplicationUser currentUser in users)
            {
                foreach (Quotation quote in currentUser.UserQuotes.AsQueryable())
                {
                    currentUser.UserQuotes.Remove(quote);
                }
            }
            db.Quotations.RemoveRange(db.Quotations);
            db.Categories.RemoveRange(db.Categories);
            db.SaveChanges();

            return RedirectToAction("God_Mode");
        }
    }
}