using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quotes2.Models
{
    public class Quotation
    {
        public int QuotationID { get; set; }
        public string Quote { get; set; }
        public string Author { get; set; }
        public virtual Category Category { get; set; }
        public DateTime Date { get; set; }
        public int CategoryID { get; set; }
    }
}