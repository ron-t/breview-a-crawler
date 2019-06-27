using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.PAAPI.AWS;

namespace AmazonCrawler
{
    class ItemExtended
    {
        public Item item { get; set; }

        public ItemExtended(Item item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            return item.ASIN + " :: " + item.ItemAttributes.Title;
        }
    }
}
