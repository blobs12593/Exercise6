using System.Linq;
using System.Web.Mvc;
using Quotes2.Models;
using System.Collections.Generic;
using System.Net.Http;
using System;

namespace Quotes2.Controllers
{
    public class HomeController : Controller
    {
        private static bool myrun = true;
        private static string quote;
        private static string author;
        private static string category;

        private Quotes2Context db = new Quotes2Context();

        public ActionResult Index()
        {
            ValuesController myController = new ValuesController();
            if (myrun)
            {
                SimpleQuotes QuoteOfDay = myController.GetQuoteOfDay();
                quote = QuoteOfDay.Quote;
                author = QuoteOfDay.Author;
                category = QuoteOfDay.Category;
                myrun = false;
            }
            ViewBag.Quote = quote;
            ViewBag.Author = author;
            ViewBag.Category = category;
            return View(myController.Get());
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

            ViewBag.CatNames = new List<string>();
            ViewBag.CatReps = new List<int>();
            var myContext = new ApplicationDbContext();
            var users = myContext.Users.ToList();
            foreach (ApplicationUser currentUser in users)
            {
                ViewBag.NumberOfUsers += 1;
            }
            foreach (Category myCats in db.Categories)
            {
                ViewBag.CatNames.Add(myCats.Name);
                ViewBag.NumberOfCats += 1;
                int count = 0;
                foreach (Quotation quote in db.Quotations)
                {
                    if (quote.Category.Name == myCats.Name)
                    {
                        count += 1;
                        ViewBag.NumberOfQuotes += 1;
                    }
                }
                ViewBag.CatReps.Add(count);
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Import(string id)
        {
            if (id != null)
            {
                HttpClient client = new HttpClient();
                string url = Request.Url.GetLeftPart(UriPartial.Authority);
                HttpResponseMessage response = client.GetAsync("api/HomeController").Result;
                var quote = response.Content.ReadAsAsync<SimpleQuotes>().Result;
            }
            return View();
        }
        public ActionResult Nuke()
        {
            var myContext = new ApplicationDbContext();
            var users = myContext.Users.ToList();
            foreach (ApplicationUser currentUser in users)
            {
                foreach (Quotation quote in currentUser.UserQuotes.ToList())
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