using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NCrawler;
using NCrawler.HtmlProcessor;
using NCrawler.Interfaces;
using NCrawler.IsolatedStorageServices;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace AmazonCrawler
{
    class ReviewHtmlCrawler
    {
        public static MainForm form;
        private static Crawler c = null;

        public static Dictionary<string, bool> UserAgentTracker = CrawlUtil.InitUserAgentTracker();

        private static bool _getRatingStats = false;
        public static bool GetRatingStats
        {
            get { return _getRatingStats; }
            set { _getRatingStats = value; }
        }

        private static bool _getReviews = false;
        public static bool GetReviews
        {
            get { return _getReviews; }
            set { _getReviews = value; }
        }

        public static string baseUri = "";
        public static Book currentBook = null;

        public static void Run(MainForm parentForm, Book book)
        {
            Run(parentForm, book, false, true);
        }


        public static void Run(MainForm parentForm, Book book, bool getRatingStats, bool getReviews)
        {
            form = parentForm;
            IsolatedStorageModule.Setup(false);

            GetRatingStats = getRatingStats;
            GetReviews = getReviews;
            currentBook = book;
            baseUri = book.reviewPageURL;

            /*
            140185852	We	
            http://www.amazon.com/We-Yevgeny-Zamyatin/product-reviews/0140185852
            http://www.amazon.com/We-Yevgeny-Zamyatin/product-reviews/0140185852/ref=cm_cr_pr_btm_link_4?pageSize=50&pageNumber=4&sortBy=recent
            
            618260307	The Hobbit	http://www.amazon.com/The-Hobbit-J-R-R-Tolkien/product-reviews/0618260307
            */
            baseUri += "/ref=cm_cr_pr_btm_link_1?pageSize=50&pageNumber=1";

            if (!currentBook.reviewPageURL.Contains("/ref=cm_cr_pr_btm_link"))
            {
                currentBook.reviewPageURL = baseUri; //hack to make isFirstPage() work [2016-02-04]
            }

            c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new ReviewPageProcessStep(),
                new SaveFileStep());

            // Custom step to visualize crawl
            c.MaximumThreadCount = 1;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;
            c.BeforeDownload += new EventHandler<NCrawler.Events.BeforeDownloadEventArgs>(c_BeforeDownload);

            string ua = CrawlUtil.GetRandomUnblockedUserAgent(UserAgentTracker);

            //if there are no unblocked user agents left then reset the tracker and retry
            if (ua == null)
            {
                UserAgentTracker = CrawlUtil.InitUserAgentTracker();
                ua = CrawlUtil.GetRandomUnblockedUserAgent(UserAgentTracker);
            }
            c.UserAgent = ua;

            // Begin crawl
            c.Crawl();
        }

        static void c_BeforeDownload(object sender, NCrawler.Events.BeforeDownloadEventArgs e)
        {
            System.Threading.Thread.Sleep(1000 + (new Random()).Next(2000));
        }
    }

    internal class ReviewPageProcessStep : IPipelineStep
    {
        private static Random RandomNumber = new Random();

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            DateTime currentDateTime = DateTime.Now;

            HtmlDocument htmlDoc = propertyBag["HtmlDoc"].Value as HtmlDocument;
            if (htmlDoc == null)
            {
                return; //this happens with HTTP errors etc. We don't bother with retrying or anything like that :(
            }

            //check if the correct page is loaded (sometimes Amazon redirects to a robot check
            if (propertyBag.Title.ToLower() == "robot check")
            {
                ReviewHtmlCrawler.form.appendLineToLog(propertyBag.Title + Environment.NewLine + propertyBag.Text.Substring(0, 100));

                //block the user agent that's currently in use
                ReviewHtmlCrawler.UserAgentTracker[crawler.UserAgent] = true;

                //log the page for retry
                File.AppendAllText("REVIEW-to-retry.txt", propertyBag.ResponseUri.OriginalString + System.Environment.NewLine);

                //sleep between 30 secs and 1 minute
                TimeSpan duration = new TimeSpan(0, 0, RandomNumber.Next(30, 60));

                ReviewHtmlCrawler.form.appendLineToLog(string.Format("Sleeping for {0}:{1} mins and secs before trying next book.", duration.Minutes, duration.Seconds));
                System.Threading.Thread.Sleep(duration);
                return;
            }

            ReviewHtmlCrawler.form.appendLineToLog("Crawling a 'review page' for " + ReviewHtmlCrawler.currentBook.DisplayString);
            ReviewHtmlCrawler.form.appendLineToLog(propertyBag.ResponseUri.ToString());

            AmazonCrawlerEntities context = DbUtil.getNewContext();

            Book b = DbUtil.getBook(context, ReviewHtmlCrawler.currentBook.ISBN);

            if (b == null) //this should never happen
            {
                ReviewHtmlCrawler.form.appendLineToLog("[error] ISBN " + ReviewHtmlCrawler.currentBook.ISBN + " not found in database");
                return;
            }

            //add all other pages of reviews if crawling reviews and on page 1
            //TODO: also get star rating distribution
            if (ReviewHtmlCrawler.GetReviews && isFirstPage(propertyBag))
            {
                ReviewHtmlCrawler.form.appendLineToLog("crawling first page");

                int numPages = CrawlUtil.GetReviewLastPageNumber(htmlDoc.DocumentNode);
                CrawlUtil.AddReviewPagesToCrawl(crawler, propertyBag.ResponseUri.OriginalString, numPages);

                ReviewHtmlCrawler.form.appendLineToLog(string.Format("***************************", numPages));
                ReviewHtmlCrawler.form.appendLineToLog(string.Format("*** {0} pages to crawl ***", numPages));
                ReviewHtmlCrawler.form.appendLineToLog(string.Format("***************************", numPages));

                if (ReviewHtmlCrawler.GetRatingStats)
                {
                    #region rating stats
                    //b.statsCollectedAt = currentDateTime;

                    ////as of 2015-12-11 the rating break-down is expressed in percentages rather than numbers.

                    //b.numFiveStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.FIVE);
                    //b.numFourStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.FOUR);
                    //b.numThreeStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.THREE);
                    //b.numTwoStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.TWO);
                    //b.numOneStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.ONE);

                    //try //save for rating stats
                    //{
                    //    context.SaveChanges();
                    //    updated += "RATING STATS;";
                    //}
                    //catch (Exception ex)
                    //{
                    //    //ignore :(
                    //    ReviewHtmlCrawler.form.appendLineToLog(ex.Message);
                    //}
                    #endregion
                }
            }

            if (ReviewHtmlCrawler.GetReviews)
            {
                HtmlNodeCollection reviews = htmlDoc.DocumentNode.SelectNodes(".//div[@class='a-section review']");

                int numProcessed = 0;
                int numNotProcessed = 0;
                int numBadges = 0;

                foreach (HtmlNode r in reviews)
                {
                    //id
                    string reviewId = r.Id;

                    Review reviewToAdd = DbUtil.getOrCreateReview(context, reviewId);
                    reviewToAdd.Book = b;


                    //starRating
                    HtmlNode ratingStringNode = r.SelectSingleNode(".//i[starts-with(@class, 'a-icon a-icon-star')]");
                    short rating = CrawlUtil.GetStarRatingFromString(ratingStringNode.InnerText);
                    reviewToAdd.starRating = rating;


                    //reviewTitle
                    HtmlNode titleNode = r.SelectSingleNode(".//a[contains(@class, 'review-title')]");
                    string title = CrawlUtil.RemoveExcessWhitespaceFromString(titleNode.InnerText.Trim());
                    reviewToAdd.reviewTitle = title;


                    //reviewerId and reviewerName and badges
                    HtmlNode reviewerNode = r.SelectSingleNode(".//a[contains(@class, 'author')]");
                    if (reviewerNode != null)
                    {
                        string profileUrl = reviewerNode.GetAttributeValue("href", "");
                        string reviewerId = "";

                        Match m = Regex.Match(profileUrl, @"profile/(.+)/ref");
                        if (m.Groups.Count == 2)
                        {
                            reviewerId = m.Groups[1].Value;
                        }

                        string reviewerName = CrawlUtil.RemoveExcessWhitespaceFromString(reviewerNode.InnerText);

                        Reviewer reviewer = DbUtil.getOrCreateReviewer(context, reviewerId);
                        reviewer.reviewerName = reviewerName;
                        reviewer.profileURL = profileUrl;

                        reviewToAdd.reviewerId = reviewerId;
                        reviewToAdd.reviewerName = reviewerName;

                        //badges
                        HtmlNodeCollection badgeNodes = r.SelectNodes(".//a[contains(@class, 'c7y-badge-link')]");
                        if (badgeNodes != null)
                        {
                            AmazonCrawlerEntities tmp = DbUtil.getNewContext();

                            foreach (HtmlNode badgeNode in badgeNodes)
                            {
                                try
                                {
                                    string badgeText = CrawlUtil.RemoveExcessWhitespaceFromString(badgeNode.InnerText);

                                    ReviewerBadge newBadge = new ReviewerBadge();
                                    newBadge.reviewerId = reviewerId;
                                    newBadge.badge = badgeText;
                                    newBadge.statsCollectedAt = currentDateTime;

                                    tmp.ReviewerBadges.AddObject(newBadge);
                                    tmp.SaveChanges();

                                    numBadges += 1;
                                }
                                catch (Exception ex)
                                {
                                    //ignore :(
                                }
                            }
                        }
                    }

                    //publishedDate
                    HtmlNode reviewDateNode = r.SelectSingleNode(".//span[contains(@class, 'review-date')]");
                    string dateText = CrawlUtil.RemoveExcessWhitespaceFromString(reviewDateNode.InnerText);
                    dateText = dateText.Replace("on", "");
                    DateTime publishedDate = DateTime.Parse(dateText);
                    reviewToAdd.publishedDate = publishedDate;


                    //reviewContent
                    HtmlNode reviewTextNode = r.SelectSingleNode(".//span[contains(@class, 'review-text')]");
                    string reviewContent = CrawlUtil.RemoveExcessWhitespaceFromString(reviewTextNode.InnerText);
                    reviewToAdd.reviewContent = reviewContent;

                    int chars = reviewContent.Length > 200 ? 200 : reviewContent.Length;

                    //numHelpful and numTotal
                    //as at 2016-03-02 the 'helpful' text forrmat is "3 people found this helpful." (numTotal doesn't exist any more)
                    HtmlNode helpfulNode = r.SelectSingleNode(".//span[contains(@class, 'review-votes')]");
                    if (helpfulNode != null)
                    {
                        if (helpfulNode.InnerText.Contains("of"))
                        {
                            int[] helpful = CrawlUtil.GetNumbersFromHelpfulText(helpfulNode.InnerText);

                            reviewToAdd.numHelpful = helpful[0];
                            reviewToAdd.numTotal = helpful[1];
                        }
                        else
                        {
                            Match m = Regex.Match(helpfulNode.InnerText, @"(\d+) p.+", RegexOptions.IgnoreCase);
                            if (m.Groups.Count == 2)
                            {
                                reviewToAdd.numHelpful = Int32.Parse(m.Groups[1].Value);
                            }
                            reviewToAdd.numTotal = null;
                        }

                    }

                    //numComments
                    HtmlNode numCommentsNode = r.SelectSingleNode(".//span[contains(@class, 'review-comment-total')]");
                    int numComments = 0;
                    Int32.TryParse(numCommentsNode.InnerText, out numComments);
                    reviewToAdd.numComments = numComments;


                    //isAmazonVerifiedPurchase
                    HtmlNode verifiedPurchaseNode = r.SelectSingleNode(".//span[@class = 'a-size-mini a-color-state a-text-bold']");
                    bool isVerifiedPurchase = false;
                    if (verifiedPurchaseNode != null)
                    {
                        isVerifiedPurchase = true;
                    }
                    reviewToAdd.isAmazonVerifiedPurchase = isVerifiedPurchase;

                    //format
                    HtmlNode formatNode = r.SelectSingleNode(".//div[contains(@class, 'review-data')]/a");
                    if (formatNode != null)
                    {
                        string formatText = CrawlUtil.RemoveExcessWhitespaceFromString(formatNode.InnerText).Replace("Format:", "").Trim();
                        reviewToAdd.format = formatText;
                    }

                    reviewToAdd.statsCollectedAt = currentDateTime;
                    reviewToAdd.reviewType = "R";

                    //debugging output

                    //ReviewHtmlCrawler.form.appendLineToLog("Processing review " + reviewId);
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\trating: {0}", rating));
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\ttitle: {0}", title));
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\tReviewer: {0} | {1}", reviewToAdd.reviewerId, reviewToAdd.reviewerName));
                    ////badges not output
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\tPublished: {0}", publishedDate));
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\tContent: {0}", reviewContent.Substring(0, chars)));
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\tHelpful: {0} of {1}", reviewToAdd.numHelpful, reviewToAdd.numTotal));
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\tCommments: {0}", numComments));
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\tisVerifiedPurchase: {0}", isVerifiedPurchase));
                    //ReviewHtmlCrawler.form.appendLineToLog(string.Format("\tFormat: {0}", reviewToAdd.format));

                    try
                    {
                        context.SaveChanges();
                        numProcessed += 1;
                    }
                    catch (Exception ex)
                    {
                        ReviewHtmlCrawler.form.appendLineToLog(ex.Message);
                        ReviewHtmlCrawler.form.appendLineToLog(ex.StackTrace);
                        numNotProcessed += 1;
                    }
                }

                ReviewHtmlCrawler.form.appendLineToLog(string.Format("{0} reviews saved/updated on page; {1} not saved; {2} badges.", numProcessed, numNotProcessed, numBadges));
            }
        }

        #region check if is first page
        private bool isFirstPage(PropertyBag propertyBag)
        {
            return propertyBag.ResponseUri.OriginalString == ReviewHtmlCrawler.currentBook.reviewPageURL;
        }
        #endregion

    }

    internal class SaveFileStep : IPipelineStep
    {
        public static string SanitiseFileName(string input)
        {
            var invalids = System.IO.Path.GetInvalidFileNameChars();
            var newName = String.Join("_", input.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

            return newName;
        }
        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            if (propertyBag["HtmlDoc"].Value == null)
            {
                return;
            }

            HtmlAgilityPack.HtmlDocument doc = propertyBag["HtmlDoc"].Value as HtmlAgilityPack.HtmlDocument;

            doc.Save("HtmlDump/"
                + SanitiseFileName((propertyBag.ResponseUri.OriginalString))
            + ".html");
        }
    }
}


