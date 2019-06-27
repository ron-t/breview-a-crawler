using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonCrawler
{
    partial class Reviewer
    {
        private bool getBadges = false;

        public bool GetBadges
        {
            get { return getBadges; }
            set { getBadges = value; }
        }
        
    }
}
