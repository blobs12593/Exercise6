using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Quotes2.Models;

namespace Quotes2.Controllers
{
    public class ValuesController : ApiController
    {
        private Quotes2Context db = new Quotes2Context();
        // GET api/<controller>
        public IEnumerable<SimpleQuotes> Get()
        {
            List<SimpleQuotes> allQuotes = new List<SimpleQuotes>();
            foreach (Quotation myQuote in db.Quotations)
            {
                SimpleQuotes mySimple = new SimpleQuotes();
                mySimple.Quote = myQuote.Quote;
                mySimple.Quote = myQuote.Author;
                mySimple.Quote = myQuote.Category.Name;
                allQuotes.Add(mySimple);
            }

            return allQuotes;
        }

        public SimpleQuotes GetQuoteOfDay()
        {
            List<SimpleQuotes> allQuotes = new List<SimpleQuotes>();
            foreach (Quotation myQuote in db.Quotations)
            {
                SimpleQuotes mySimple = new SimpleQuotes();
                mySimple.Quote = myQuote.Quote;
                mySimple.Quote = myQuote.Author;
                mySimple.Quote = myQuote.Category.Name;
                allQuotes.Add(mySimple);
            }
            Random r = new Random();
            int randomQuoteIndex = r.Next(0, allQuotes.Count);
            return allQuotes[randomQuoteIndex];
        }
        // GET api/<controller>/5
        public SimpleQuotes Get(int id)
        {
            SimpleQuotes foundQuote = new SimpleQuotes();
            foreach (Quotation myQuote in db.Quotations)
            {
                if (id == myQuote.QuotationID)
                {
                    foundQuote.Quote = myQuote.Quote;
                    foundQuote.Quote = myQuote.Author;
                    foundQuote.Quote = myQuote.Category.Name;
                    break;
                }
            }
            return foundQuote;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}