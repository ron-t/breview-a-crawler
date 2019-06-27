using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonCrawler
{
    partial class Book
    {
        public string DisplayString
        {
            get
            {
                return ISBN + "::" + (string.IsNullOrEmpty(title) ? "<titled not yet updated>" : title);
            }
        }
    }

    class BookEqualityComparer : IEqualityComparer<Book>
    {
        const int MUST_BE_LESS_THAN = 100000000; // 8 decimal digits

        private BookEqualityComparer() { }
        public static BookEqualityComparer Instance = new BookEqualityComparer();

        bool IEqualityComparer<Book>.Equals(Book x, Book y)
        {
            return x.ISBN == y.ISBN;
        }

        int IEqualityComparer<Book>.GetHashCode(Book obj)
        {
            uint hash = 0;
            // if you care this can be done much faster with unsafe 
            // using fixed char* reinterpreted as a byte*
            foreach (byte b in System.Text.Encoding.Unicode.GetBytes(obj.ISBN))
            {
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            // final avalanche
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            // helpfully we only want positive integer < MUST_BE_LESS_THAN
            // so simple truncate cast is ok if not perfect
            return (int)(hash % MUST_BE_LESS_THAN);
        }


    }
}
