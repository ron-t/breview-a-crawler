using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonCrawler
{
    class DbUtil
    {
        public static AmazonCrawlerEntities getNewContext()
        {
            return new AmazonCrawlerEntities("metadata=res://*/AmazonModel.csdl|res://*/AmazonModel.ssdl|res://*/AmazonModel.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source=xxx;Initial Catalog=AmazonCrawler;;MultipleActiveResultSets=True\"");
        }

        public static Book getBook(AmazonCrawlerEntities context, string isbn)
        {
            return context.Books.FirstOrDefault(b => b.ISBN == isbn);
        }

        public static Author getAuthor(AmazonCrawlerEntities context, string authorId)
        {
            return context.Authors.FirstOrDefault(a => a.id == authorId);
        }

        public static Review getOrCreateReview(AmazonCrawlerEntities context, string reviewId)
        {
            Review r = context.Reviews.FirstOrDefault(rev => rev.id == reviewId);

            if (r == null)
            {
                r = new Review();
                r.id = reviewId;
                context.Reviews.AddObject(r);
            }

            return r;
        }

        public static Reviewer getOrCreateReviewer(AmazonCrawlerEntities context, string reviewerId)
        {
            return getOrCreateReviewer(context, reviewerId, null, null);
        }

        internal static Reviewer getOrCreateReviewer(AmazonCrawlerEntities context, string reviewerId, string name, string url)
        {
            Reviewer r = context.Reviewers.FirstOrDefault(rev => rev.id == reviewerId);

            if (r == null)
            {
                r = new Reviewer();
                r.id = reviewerId;
                r.reviewerName = name;
                r.profileURL = url;

                context.Reviewers.AddObject(r);

                r.GetBadges = true;
            }

            return r;
        }
    }
}
