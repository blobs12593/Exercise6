using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quotes2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Quotes2.Controllers
{
    public class Quotes2Controller : Controller
    {
        private Quotes2Context db = new Quotes2Context();

        private UserManager<ApplicationUser> manager;

        public Quotes2Controller()
        {
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        // GET: /Quotes2/
        public ActionResult Index(string Search, int? id, string delete, string doNotAddID, string viewMine, string findThatUser, string FindUserQuotes)
        {
            ViewBag.Cookie = false;
            
            if (viewMine == "View My Quotes")
            {
                ViewBag.ClearSearch = false;
                ViewBag.MyQuotes = true;

                ApplicationUser currentUser = manager.FindById(User.Identity.GetUserId());
                var quotations = currentUser.UserQuotes.AsQueryable();
                return View(quotations.ToList());
            }
            else if (User.IsInRole("Admin") && findThatUser == "Find User Quotes")
            {
                ViewBag.ClearSearch = false;
                ViewBag.MyQuotes = false;

                try
                {
                    var quotations = manager.FindByName(FindUserQuotes).UserQuotes.AsQueryable();
                    return View(quotations.ToList());
                }
                catch(Exception)
                {
                    //If error return an empty list
                    var quotations = db.Quotations.Include(q => q.Category).Where(q => q.Category.Name.Equals(q.Category.Name + "nomatch"));
                    return View(quotations.ToList());
                }
            }
            else
            {
                HttpCookie myCookie = Request.Cookies.Get("Quote");
                ViewBag.MyQuotes = false;
                // Prevent accidental hiding
                if (doNotAddID == "Search" || doNotAddID == "Clear Search")
                {
                    id = null;
                }
                // If Unhide All was pressed
                if (delete == "Unhide All")
                {
                    myCookie.Expires = DateTime.Now.AddHours(-1);
                    Response.Cookies.Add(myCookie);
                    if (String.IsNullOrEmpty(Search))
                    {
                        ViewBag.ClearSearch = false;
                        var quotations = db.Quotations.Include(q => q.Category);
                        return View(quotations.ToList());
                    }
                    else
                    {
                        ViewBag.ClearSearch = true;
                        var quotations = db.Quotations.Include(q => q.Category).Where(q => q.Category.Name.Contains(Search) || q.Quote.Contains(Search) || q.Author.Contains(Search));
                        return View(quotations.ToList());
                    }
                }
                // If unhide all was not pressed
                else
                {
                    // If there is no cookie and no cookie going to be created
                    if (Request.Cookies.Get("Quote") == null && id == null)
                    {
                        // Uncookied results
                        if (String.IsNullOrEmpty(Search))
                        {
                            ViewBag.ClearSearch = false;
                            var quotations = db.Quotations.Include(q => q.Category);
                            return View(quotations.ToList());
                        }
                        else
                        {
                            ViewBag.ClearSearch = true;
                            var quotations = db.Quotations.Include(q => q.Category).Where(q => q.Category.Name.Contains(Search) || q.Quote.Contains(Search) || q.Author.Contains(Search));
                            return View(quotations.ToList());
                        }
                    }
                    // If the cookie does exist or will exist
                    else
                    {
                        ViewBag.Cookie = true;
                        // Check if add value was requested to existing cookie
                        if (Request.Cookies.Get("Quote") != null && id != null)
                        {
                            myCookie.Value = myCookie.Value + " " + id.ToString();
                        }
                        // Check if a cookie needs to be created
                        else if (id != null)
                        {
                            myCookie = new HttpCookie("Quote", id.ToString());
                        }
                        myCookie.Expires = DateTime.Now.AddDays(2);
                        Response.Cookies.Add(myCookie);

                        // Grab the cookies values and convert them to integers
                        String[] allHidden = myCookie.Value.Split(' ');
                        int[] allHiddenNums = new int[allHidden.Length];
                        int counter = 0;
                        foreach (string value in allHidden)
                        {
                            allHiddenNums[counter] = int.Parse(value);
                            counter += 1;
                        }

                        if (String.IsNullOrEmpty(Search))
                        {
                            ViewBag.ClearSearch = false;
                            var quotations = db.Quotations.Include(q => q.Category).Where(q => !allHiddenNums.Contains(q.QuotationID));
                            return View(quotations.ToList());
                        }
                        else
                        {
                            ViewBag.ClearSearch = true;
                            var quotations = db.Quotations.Include(q => q.Category).Where(q => q.Category.Name.Contains(Search) || q.Quote.Contains(Search) || q.Author.Contains(Search));
                            return View(quotations.ToList());
                        }
                    }
                }
            }
        }

        // GET: /Quotes2/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            return View(quotation);
        }

        // GET: /Quotes2/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            return View();
        }

        // POST: /Quotes2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="QuotationID,Quote,Author,Date,CategoryID")] Quotation quotation)
        {
            if (quotation.Author == null || quotation.Quote == null)
            {
                return RedirectToAction("Create");
            }
            if (quotation.Author.Trim() == null || quotation.Quote.Trim() == null || quotation.CategoryID == 0)
            {
                return RedirectToAction("Create");
            }
            if (ModelState.IsValid)
            {
                quotation.Date = DateTime.Now.Date;
                ApplicationUser currentUser = manager.FindById(User.Identity.GetUserId());
                currentUser.UserQuotes.Add(quotation);
                db.Quotations.Add(quotation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", quotation.CategoryID);
            return View(quotation);
        }

        // GET: /Quotes2/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", quotation.CategoryID);
            return View(quotation);
        }

        // POST: /Quotes2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="QuotationID,Quote,Author,Date,CategoryID")] Quotation quotation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quotation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", quotation.CategoryID);
            return View(quotation);
        }

        // GET: /Quotes2/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            return View(quotation);
        }

        // POST: /Quotes2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quotation quotation = db.Quotations.Find(id);
            db.Quotations.Remove(quotation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("AddCategory")]
        public ActionResult AddCategory([Bind(Include = "CategoryID,Name")]String value)
        {
            if (value.Trim() != "")
            {
                Category Ct = new Category();
                Ct.Name = value;
                foreach (Category myCats in db.Categories)
                {
                    if (myCats.Name == Ct.Name)
                    {
                        return RedirectToAction("Create");
                    }
                }
                db.Categories.Add(Ct);
                db.SaveChanges();
            }
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
