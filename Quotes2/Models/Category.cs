using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quotes2.Models
{
    public class Category
    {
        public virtual List<Quotation> Quotation { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
    }
}