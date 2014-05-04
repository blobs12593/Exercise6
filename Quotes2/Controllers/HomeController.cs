using System.Linq;
using System.Web.Mvc;
using Quotes2.Models;
using System.Collections.Generic;
using System.Net.Http;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Quotes2.Controllers
{
    public class HomeController : Controller
    {
        private Quotes2Context db = new Quotes2Context();

        private UserManager<ApplicationUser> manager;

        public HomeController()
        {
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        private static bool myrun = true;
        private static string quote;
        private static string author;
        private static string category;

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

            //God Mode
            if (User.IsInRole("Admin"))
            {
                God_Mode();
            }
            return View(myController.Get());
        }

        [Authorize(Roles = "Admin")]
        public void God_Mode()
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
                foreach (Quotation quotes in db.Quotations)
                {
                    if (quotes.Category.Name == myCats.Name)
                    {
                        count += 1;
                        ViewBag.NumberOfQuotes += 1;
                    }
                }
                ViewBag.CatReps.Add(count);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Import(string id)
        {
            ViewBag.Message = null;
            if (id != null)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    string url = Request.Url.GetLeftPart(UriPartial.Authority);
                    HttpResponseMessage response = client.GetAsync(id).Result;
                    Quotation tempQuote = new Quotation();
                    Boolean catMatch;
                    foreach (SimpleQuotes pulledQuote in response.Content.ReadAsAsync<SimpleQuotes[]>().Result)
                    {
                        catMatch = false;
                        tempQuote.Quote = pulledQuote.Quote;
                        tempQuote.Author = pulledQuote.Author;
                        tempQuote.Date = DateTime.Now.Date;
                        tempQuote.Category = new Category();
                        tempQuote.Category.Name = pulledQuote.Category;
                        foreach (Category cat in db.Categories)
                        {
                            if (cat.Name.Equals(tempQuote.Category.Name))
                            {
                                tempQuote.Category = cat;
                                catMatch = true;
                                break;
                            }
                        }
                        if (!catMatch)
                        {
                            tempQuote.Category = db.Categories.Add(tempQuote.Category);
                        }
                        ApplicationUser currentUser = manager.FindById(User.Identity.GetUserId());
                        currentUser.UserQuotes.Add(db.Quotations.Add(tempQuote));
                        db.SaveChanges();
                    }
                    var quoteList = response.Content.ReadAsAsync<SimpleQuotes>().Result;
                    ViewBag.Message = "Import Successful!";
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Import Failed!";
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
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

            return RedirectToAction("Index");
        }
    }
}