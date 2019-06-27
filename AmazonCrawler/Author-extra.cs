using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonCrawler
{
    partial class Author
    {
        public string DisplayString
        {
            get
            {
                return name + " (" + id + ")";
            }
        }
    }
}
