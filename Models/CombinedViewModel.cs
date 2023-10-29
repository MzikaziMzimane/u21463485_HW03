using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u21463485_HW03.Models
{
    public class CombinedViewModel
    {
        public IEnumerable<student> Students { get; set; }
        public IEnumerable<borrow> Borrows { get; set; }
        public IEnumerable<type> Types { get; set; }
        public IEnumerable<book> Books { get; set; }
        public IEnumerable<author> Authors { get; set; }
    }
}