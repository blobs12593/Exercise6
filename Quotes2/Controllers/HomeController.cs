﻿using System.Linq;
using System.Web.Mvc;
using Quotes2.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Quotes2.Controllers
{
    public class HomeController : Controller
    {
        private Quotes2Context db = new Quotes2Context();

        public ActionResult Index()
        {
            ValuesController myController = new ValuesController();
            SimpleQuotes QuoteOfDay = myController.GetQuoteOfDay();
            ViewBag.Quote = QuoteOfDay.Quote;
            ViewBag.Author = QuoteOfDay.Author;
            ViewBag.Category = QuoteOfDay.Category;
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