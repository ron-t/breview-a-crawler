using System;
using HtmlAgilityPack;
using NCrawler;
using NCrawler.HtmlProcessor;
using NCrawler.Interfaces;
using NCrawler.IsolatedStorageServices;
using System.Collections.Generic;

namespace AmazonCrawler
{
    class BookHtmlCrawler
    {
        public static MainForm form;
        private static Crawler c = null;

        public static Dictionary<string, bool> UserAgentTracker = CrawlUtil.InitUserAgentTracker();

        private static bool _getDetailsAndAuthor = false;
        public static bool GetDetailsAndAuthor
        {
            get { return _getDetailsAndAuthor; }
            set { _getDetailsAndAuthor = value; }
        }

        private static bool _getRanks = false;
        public static bool GetRanks
        {
            get { return _getRanks; }
            set { _getRanks = value; }
        }

        public static string baseUri = "";
        public static Book currentBook = null;

        public static void Run(MainForm parentForm, Book book)
        {
            Run(parentForm, book, true, false);
        }


        public static void Run(MainForm parentForm, Book book, bool getDetailsAndAuthor, bool getRanks)
        {
            form = parentForm;
            IsolatedStorageModule.Setup(false);

            GetDetailsAndAuthor = getDetailsAndAuthor;
            GetRanks = getRanks;
            currentBook = book;
            baseUri = book.detailPageURL;

            Uri u = new Uri(Uri.EscapeUriString(baseUri));

            c = new Crawler(u,
                new HtmlDocumentProcessor(), // Process html
                new DetailPageDumperStep());

            // Custom step to visualize crawl
            c.MaximumThreadCount = 1;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;

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
    }

    internal class DetailPageDumperStep : IPipelineStep
    {
        #region IPipelineStep Members
        private static Random RandomNumber = new Random();
        #endregion

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            //using c.MaximumCrawlDepth = 1 means only the details page will be processed (no other links are crawled as they're depth 2)

            DateTime currentDateTime = DateTime.Now;

            HtmlDocument htmlDoc = propertyBag["HtmlDoc"].Value as HtmlDocument;
            if (htmlDoc == null)
            {
                return; //this happens with HTTP errors etc. We don't bother with retrying or anything like that :(
            }

            BookHtmlCrawler.form.appendLineToLog("Crawling 'details page' for " + BookHtmlCrawler.currentBook.DisplayString);
            BookHtmlCrawler.form.appendLineToLog(propertyBag.ResponseUri.OriginalString);

            lock (this)
            {
                //check if the correct page is loaded (sometimes Amazon redirects to a robot check
                if (propertyBag.Title.ToLower() == "robot check")
                {
                    BookHtmlCrawler.form.appendLineToLog(propertyBag.Title + Environment.NewLine + propertyBag.Text.Substring(0, 100));

                    //block the user agent that's currently in use
                    BookHtmlCrawler.UserAgentTracker[crawler.UserAgent] = true;

                    //sleep between 30 secs and 3 minutes
                    TimeSpan duration = new TimeSpan(0, 0, RandomNumber.Next(30, 3*60));

                    BookHtmlCrawler.form.appendLineToLog(string.Format("Sleeping for {0}:{1} mins and secs before trying next book.", duration.Minutes, duration.Seconds));
                    System.Threading.Thread.Sleep(duration);
                    return;
                }

                string updated = "";
                using (AmazonCrawlerEntities context = DbUtil.getNewContext())
                {

                    Book b = DbUtil.getBook(context, BookHtmlCrawler.currentBook.ISBN);
                    Author a = null;
                    

                    if (b == null) //this should never happen
                    {
                        BookHtmlCrawler.form.appendLineToLog("[error] ISBN " + BookHtmlCrawler.currentBook.ISBN + " not found in database");
                        return;
                    }

                    if (BookHtmlCrawler.GetDetailsAndAuthor)
                    {
                        //author, language, reading level, format, sales ranks not stored.
                        #region average rating

                        var averageRatingNode = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@id='avgRating']/span/a");
                        if (averageRatingNode != null)
                        {
                            string ratingText = averageRatingNode.InnerText;
                            decimal rating = CrawlUtil.ExtractRatingFromSummaryString(CrawlUtil.TidyHtmlText(ratingText));
                            b.avgCustomerReview = rating;
                            BookHtmlCrawler.form.appendLineToLog("\tavg: " + rating);
                        }
                        #endregion

                        #region review page URL
                        string url = null;
                        var reviewUrlNode = htmlDoc.DocumentNode.SelectSingleNode(@"//a[@class='a-link-emphasis a-nowrap']/@href");
                        if (reviewUrlNode != null)
                        {
                            var working_url = reviewUrlNode.GetAttributeValue("href", null);

                            if (!string.IsNullOrEmpty(working_url) && working_url.IndexOf("/ref=") > 0)
                            {
                                url = working_url.Substring(0, working_url.IndexOf("/ref="));
                            }
                        }

                        try //save for rating and reviewURL
                        {
                            BookHtmlCrawler.form.appendLineToLog("\treview URL added: " + url);
                            b.reviewPageURL = url ?? "-";

                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            //ignore :(
                            BookHtmlCrawler.form.appendLineToLog(ex.Message);
                        }
                        #endregion

                        //2015-12-01: disabled this feature. It looks like the ratings are now in %s rather than numbers.
                        #region rating stats
                        //TEMP 2014-06-12: get star rating from details page
                        //b.statsCollectedAt = currentDateTime;

                        //b.numFiveStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.FIVE);
                        //b.numFourStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.FOUR);
                        //b.numThreeStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.THREE);
                        //b.numTwoStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.TWO);
                        //b.numOneStarRatings = CrawlUtil.getStarRating(htmlDoc, b, CrawlUtil.ONE);

                        //BookHtmlCrawler.form.appendLineToLog("\t" + CrawlUtil.FIVE + " star: " + b.numFiveStarRatings);
                        //BookHtmlCrawler.form.appendLineToLog("\t" + CrawlUtil.FOUR + " star: " + b.numFourStarRatings);
                        //BookHtmlCrawler.form.appendLineToLog("\t" + CrawlUtil.THREE + " star: " + b.numThreeStarRatings);
                        //BookHtmlCrawler.form.appendLineToLog("\t" + CrawlUtil.TWO + " star: " + b.numTwoStarRatings);
                        //BookHtmlCrawler.form.appendLineToLog("\t" + CrawlUtil.ONE + " star: " + b.numOneStarRatings);

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

                        updated += "DETAILS (avg rating, reviewURL)";
                    }
                }

                BookHtmlCrawler.form.appendLineToLog("Updated " + updated + " for " + BookHtmlCrawler.currentBook.DisplayString);
            }


        }

        
    }
}


#region REFERENCE code from previous crawls

#region sales rank
//else if (BookHtmlCrawler.GetRanks)
//{
//    #region sales rank(s)
//    var rankNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@id='SalesRank']");
//    if (rankNode != null)
//    {
//        string text = null;

//        var rankTextNodes = rankNode.SelectNodes("./text()");
//        if (rankTextNodes != null && rankTextNodes.Count > 0)
//        {
//            foreach (var node in rankTextNodes)
//            {
//                text = CrawlUtil.TidyHtmlText(node.InnerText);

//                if (text.StartsWith("#"))
//                {
//                    Ranking r = new Ranking();
//                    r.Book = b;
//                    r.rankString = text;
//                    r.statsCollectedAt = currentDateTime;

//                    try //save for rank
//                    {
//                        context.SaveChanges();
//                        BookHtmlCrawler.form.appendLineToLog("\t" + text);
//                    }
//                    catch (Exception ex)
//                    {
//                        //ignore :(
//                        BookHtmlCrawler.form.appendLineToLog(ex.Message);
//                    }

//                    break;
//                }

//            }
//        }

//        var otherRankNodes = rankNode.SelectNodes(".//li");
//        if (otherRankNodes != null && otherRankNodes.Count > 0)
//        {
//            foreach (var node in otherRankNodes)
//            {
//                text = CrawlUtil.TidyHtmlText(node.InnerText);

//                Ranking r = new Ranking();
//                r.Book = b;
//                r.rankString = text;
//                r.statsCollectedAt = currentDateTime;

//                BookHtmlCrawler.form.appendLineToLog("\t" + text);
//            }

//            try //save for ranks
//            {
//                context.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                //ignore :(
//                BookHtmlCrawler.form.appendLineToLog(ex.Message);
//            }
//        }
//    }
//    #endregion

//    updated += "RANKS;";
//}
#endregion

#region author
/**********
        [authorId]
        <div class="a-row">
				        <span class="a-size-medium">Jonathan Safran Foer
									 
				        <span class="a-color-secondary">(Author)</span>
									
				        </span>
			        </div>
        **********/
//var authorNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='author notFaded']//a[@class='a-link-normal contributorNameID']");
//string authorName = null;
//string authorId = null;

//if (authorNode != null)
//{
//    authorId = "";
//    authorName = "";
//    //string authorDetailUrl = "";

//    authorId = authorNode.GetAttributeValue("data-asin", null);
//    authorName = authorNode.InnerText.Trim();

//    //authorDetailUrl = authorNode.GetAttributeValue("href", null);

//    //if (!string.IsNullOrEmpty(authorDetailUrl))
//    //{
//    //    authorDetailUrl = "http://www.amazon.com" + authorDetailUrl.Substring(0, authorDetailUrl.IndexOf("/ref="));
//    //}

//}
//else //where there are multiple authors, the author node is in a format
//{
//    //get the first author (selectSINGLEnode)
//    var authorNameNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='contributorNameTrigger']");
//    if (authorNameNode != null)
//    {
//        authorName = authorNameNode.InnerText.Trim();
//    }

//    var authorIdNode = htmlDoc.DocumentNode.SelectSingleNode("//input[contains(@id, 'contributorASIN')]");
//    if (authorIdNode != null)
//    {
//        authorId = authorIdNode.GetAttributeValue("value", null);
//    }

//}

//try //save for author
//{
//    //debugging
//    BookHtmlCrawler.form.appendLineToLog("\tAuthor id: " + authorId);
//    BookHtmlCrawler.form.appendLineToLog("\tAuthor name: " + authorName);

//    if (!string.IsNullOrEmpty(authorId))
//    {
//        a = DbUtil.getAuthor(context, authorId);

//        if (a == null) //the author doesn't yet exist in the db; create first then add as book's author
//        {
//            a = new Author();
//            a.id = authorId;
//            a.name = authorName;
//            //a.detailPageURL = authorDetailUrl;
//        }

//        b.Author = a;
//    }

//    context.SaveChanges();
//}
//catch (Exception ex) //effectively ignore :(
//{

//    b.authorId = null;
//    b.Author = null;
//    b.AuthorReference = null;

//    var objectStateEntries = context
//                .ObjectStateManager
//                .GetObjectStateEntries(EntityState.Added);

//    foreach (var objectStateEntry in objectStateEntries)
//    {
//        context.Detach(objectStateEntry.Entity); //this removes added objects (which cleans up the context if duplicate authors were added
//    }

//    BookHtmlCrawler.form.appendLineToLog(ex.Message);
//    if (ex.InnerException != null)
//        BookHtmlCrawler.form.appendLineToLog(ex.InnerException.Message);
//}
#endregion

#region language
///**********
//[language]
//<li>
//<b>Language:</b> English
//</li>
//**********/
//var langNode = htmlDoc.DocumentNode.SelectSingleNode("//li//*[text()='Language:']/following-sibling::text()");

//if (langNode != null && langNode.InnerText != null)
//{
//    BookHtmlCrawler.form.appendLineToLog("\tlanguage: " + langNode.InnerText.Trim());

//    b.language = langNode.InnerText.Trim();

//    try //save for language
//    {
//        context.SaveChanges();
//    }
//    catch (Exception ex)
//    {
//        //ignore :(
//        BookHtmlCrawler.form.appendLineToLog(ex.Message);
//    }
//}
#endregion

#region reading level
//    /**********
//[readingLevel]
//<li>
//    <b>Reading level:</b> Ages 8 and up<br />
//</li>
//**********/
//    var readingLevelNode = htmlDoc.DocumentNode.SelectSingleNode("//li//*[text()='Reading level:']/following-sibling::text()");

//    if (readingLevelNode != null && readingLevelNode.InnerText != null)
//    {
//        BookHtmlCrawler.form.appendLineToLog("\treading level: " + readingLevelNode.InnerText.Trim());

//        b.readingLevel = readingLevelNode.InnerText.Trim();

//        try //save for reading level
//        {
//            context.SaveChanges();
//        }
//        catch (Exception ex)
//        {
//            //ignore :(
//            BookHtmlCrawler.form.appendLineToLog(ex.Message);
//        }
//    }
#endregion

#region format
//    /**********
//[fmtKindle]
//<td  class="tmm_bookTitle  noLinkDecoration ">
//    <a href="http://www.amazon.com/Harry-Potter-Sorcerers-Stone-ebook/dp/B00728DYRO/ref=tmm_kin_title_0">Kindle Edition</a>
//</td>

//[fmtHardcover]
//<td  class="tmm_bookTitle  noLinkDecoration ">
//    Hardcover
//</td>

//[fmtPaperback]
//<td  class="tmm_bookTitle  noLinkDecoration ">
//    <a href="http://www.amazon.com/Harry-Potter-Sorcerers-Stone-Book/dp/059035342X/ref=tmm_pap_title_0">Paperback</a>
//</td>

//[fmtAudio]
//<td  class="tmm_bookTitle  noLinkDecoration ">
//    <a href="http://www.amazon.com/Harry-Potter-Sorcerers-Stone-Book/dp/0807281956/ref=tmm_abk_title_0">Audio, CD, Audiobook, Unabridged</a>
//</td>

//[fmtUnknownBinding]
//<td  class="tmm_bookTitle  noLinkDecoration ">
//    <a href="http://www.amazon.com/A-Bear-Called-Paddington/dp/B003IYO9XY/ref=tmm_other_meta_binding_title_0">Unknown Binding</a>
//</td>
//**********/
//    var formatNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='tmm_bookTitle  noLinkDecoration ']");
//    if (formatNodes != null && formatNodes.Count > 0)
//    {
//        string format;
//        foreach (var node in formatNodes)
//        {
//            format = node.InnerText.Trim();

//            if (CrawlUtil.regexFmtKindle.IsMatch(format))
//            {
//                b.fmtKindle = true;
//            }
//            else if (CrawlUtil.regexFmtHardcover.IsMatch(format))
//            {
//                b.fmtHardcover = true;
//            }
//            else if (CrawlUtil.regexFmtPaperback.IsMatch(format))
//            {
//                b.fmtPaperback = true;
//            }
//            else if (CrawlUtil.regexFmtAudio.IsMatch(format))
//            {
//                b.fmtAudio = true;
//            }
//            else if (CrawlUtil.regexFmtUnknownBinding.IsMatch(format))
//            {
//                b.fmtUnknownBinding = true;
//            }
//        }

//        BookHtmlCrawler.form.appendLineToLog("\tformats added");

//        try //save for formats
//        {
//            context.SaveChanges();
//        }
//        catch (Exception ex)
//        {
//            //ignore :(
//            BookHtmlCrawler.form.appendLineToLog(ex.Message);
//        }
//    }
#endregion

#endregion
