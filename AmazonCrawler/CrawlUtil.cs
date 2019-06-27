using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCrawler.Interfaces;
using NCrawler.Services;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace AmazonCrawler
{
    class CrawlUtil
    {
        public const string FIVE = "Five";
        public const string FOUR = "Four";
        public const string THREE = "Three";
        public const string TWO = "Two";
        public const string ONE = "One";

        public static IFilter[] ExtensionsToSkip = new[]
			{
				(RegexFilter)new Regex(@"(\.jpg|\.css|\.js|\.gif|\.jpeg|\.png|\.ico)",
					RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
			};

        public static Regex regexFmtKindle = new Regex(@"kindle", RegexOptions.IgnoreCase);
        public static Regex regexFmtHardcover = new Regex(@"hard.*cover", RegexOptions.IgnoreCase);
        public static Regex regexFmtPaperback = new Regex(@"paper.*back", RegexOptions.IgnoreCase);
        public static Regex regexFmtAudio = new Regex(@"audio", RegexOptions.IgnoreCase);
        public static Regex regexFmtUnknownBinding = new Regex(@"unknown", RegexOptions.IgnoreCase);

        private static Random random = new Random();

        public static Dictionary<string, bool> InitUserAgentTracker()
        {
            Dictionary<string, bool> d = new Dictionary<string, bool>(50);

            string[] UserAgents = new string[]{
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
             #region UserAgents list
            ,"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/601.2.7 (KHTML, like Gecko) Version/9.0.1 Safari/601.2.7"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko"
            ,"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.3; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/601.2.7 (KHTML, like Gecko) Version/9.0.1 Safari/601.2.7"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10.11; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10.10; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (iPad; CPU OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1"
            ,"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240"
            ,"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.1; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11) AppleWebKit/601.1.56 (KHTML, like Gecko) Version/9.0 Safari/601.1.56"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko"
            ,"Mozilla/5.0 (X11; Linux x86_64; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0"
            ,"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"
            ,"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Ubuntu Chromium/45.0.2454.101 Chrome/45.0.2454.101 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_5) AppleWebKit/601.2.7 (KHTML, like Gecko) Version/9.0.1 Safari/537.86.2"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/600.8.9 (KHTML, like Gecko) Version/8.0.8 Safari/600.8.9"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10.9; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0"
            ,"Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 5.1; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:41.0) Gecko/20100101 Firefox/41.0"
            ,"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)"
            ,"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586"
            ,"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.0; Trident/5.0;  Trident/5.0)"
            ,"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.1; rv:38.0) Gecko/20100101 Firefox/38.0"
            ,"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"
            ,"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.71 Safari/537.36"
            ,"Mozilla/5.0 (X11; Linux x86_64; rv:38.0) Gecko/20100101 Firefox/38.0 Iceweasel/38.4.0"
            ,"Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0"
            ,"Mozilla/5.0 (X11; Linux x86_64; rv:38.0) Gecko/20100101 Firefox/38.0"
            ,"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36"
            ,"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0;  Trident/5.0)"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:42.0) Gecko/20100101 Firefox/42.0"
            ,"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/601.1.56 (KHTML, like Gecko) Version/9.0 Safari/601.1.56"
            ,"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"
        };
             #endregion

            foreach (var ua in UserAgents)
            {
                d.Add(ua, false);
            }

            return d;
        }

        public static string GetRandomUnblockedUserAgent(Dictionary<string, bool> d)
        {
            return d.FirstOrDefault(ua => ua.Value == false).Key;
        }


        internal static string TidyHtmlText(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }
            else
            {
                return HttpUtility.HtmlDecode(
                    Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(
                            html, @"(\n)+", @" ").Trim()
                        , @"\s+", " ")
                    , @" ", " ")
                    );
            }
        }

        private static Regex ratingFromSummaryString = new Regex(@"(.+) out of", RegexOptions.IgnoreCase);
        internal static decimal ExtractRatingFromSummaryString(string s)
        {
            decimal result = -1;
            try
            {
                Match m = ratingFromSummaryString.Match(s);

                if (m.Groups.Count > 1)
                {
                    result = decimal.Parse(m.Groups[1].Value);
                }
            }
            catch (Exception) { }

            return result;
        }

        private static Regex ratingFromString = new Regex(@"\d.*star\:.*\(([\d\,]+)\)", RegexOptions.IgnoreCase);
        internal static int ExtractRatingFromStarString(string s)
        {
            int result = -1;
            try
            {
                Match m = ratingFromString.Match(s);

                if (m.Groups.Count > 1)
                {
                    result = int.Parse(Regex.Replace(m.Groups[1].Value, ",", ""));
                }
            }
            catch (Exception) { }

            return result;
        }

        private static Regex ratingFromStringFromDetailPage = new Regex(@"\d\ star\ ([\d\,]+)", RegexOptions.IgnoreCase);
        internal static int ExtractRatingFromStarStringFromDetailPage(string s)
        {
            int result = -1;
            try
            {
                Match m = ratingFromStringFromDetailPage.Match(s);

                if (m.Groups.Count > 1)
                {
                    result = int.Parse(Regex.Replace(m.Groups[1].Value, ",", ""));
                }
            }
            catch (Exception) { }

            return result;
        }

        #region get star rating method
        /*
        [numFiveStarRatings][numFourStarRatings][numThreeStarRatings][numTwoStarRatings][numOneStarRatings]
        <tr>
            <td align="left" style="padding-right:0.5em;padding-bottom:1px;white-space:nowrap;font-size:10px;">
                <a href="http://www.amazon.com/Already-Dead-Joe-Pitt-Novel/product-reviews/034547824X/ref=cm_cr_pr_hist_5?ie=UTF8&filterBy=addFiveStar&showViewpoints=0" style="font-family:Verdana,Arial,Helvetica,Sans-serif;">
                    5 star
                </a>:
            </td>
            <td style="min-width:60; background-color: #eeeecc" width="60" align="left" class="tiny" title="49%">
                <...>
            </td>
            <td align="right" style="font-family:Verdana,Arial,Helvetica,Sans-serif;;font-size:10px;">
                &nbsp;(82)
            </td>
        </tr>
        */
        internal static int GetStarRating(HtmlDocument htmlDoc, Book b, string starNum)
        {
            int rating = 0;

            var nStarNode = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@href,'filterBy=add" + starNum + "Star')]/../..");
            if (nStarNode != null)
            {
                try
                {
                    //this method works for a review page
                    rating = CrawlUtil.ExtractRatingFromStarString(CrawlUtil.TidyHtmlText(nStarNode.InnerText));

                    //try another method (for detail page)
                    if (rating == -1)
                    {
                        var n = nStarNode.SelectSingleNode("//a[contains(@href,'filterBy=add" + starNum + "Star')]");
                        rating = CrawlUtil.ExtractRatingFromStarStringFromDetailPage(CrawlUtil.TidyHtmlText(n.InnerText));
                    }

                    //if it failed, try another method (for detail page)
                    if (rating == -1)
                    {
                        rating = CrawlUtil.ExtractRatingFromStarStringFromDetailPage(CrawlUtil.TidyHtmlText(nStarNode.InnerText));
                    }


                }
                catch (Exception ex)
                {
                    //old code
                    //ReviewHtmlCrawler.form.appendLineToLog("***ERROR*** " + "\t** [" + starNum + " star rating] not found (" + ex.Message + ")"); 
                }
            }

            return rating;
        }
        #endregion


        private static Regex pageNumberFromUrl = new Regex(@".+&pageNumber=(\d+).+", RegexOptions.IgnoreCase);
        internal static int GetReviewLastPageNumber(HtmlAgilityPack.HtmlNode html)
        {
            /*
            <ul class="a-pagination">
                <li class="a-disabled">
                    <span class="a-declarative" data-action="reviews:page-action" data-reviews:page-action="{&quot;allowLinkDefault&quot;:&quot;1&quot;}">
                        &larr;<span class="a-letter-space"></span><span class="a-letter-space"></span>Previous
                    </span>
                </li>
                <li class="a-selected page-button" data-reftag="cm_cr_pr_btm_link">
                    <a href="/We-Yevgeny-Zamyatin/product-reviews/0140185852/ref=cm_cr_pr_btm_link_1?ie=UTF8&pageSize=50&sortBy=recent">
                        1
                    </a>
                </li>
               ...
                <li class="page-button" data-reftag="cm_cr_pr_btm_link">
                    <a href="/We-Yevgeny-Zamyatin/product-reviews/0140185852/ref=cm_cr_pr_btm_link_5?ie=UTF8&pageNumber=5&pageSize=50&sortBy=recent">
                        5
                    </a>
                </li>
                <li class="a-last">
                    <a href="/We-Yevgeny-Zamyatin/product-reviews/0140185852/ref=cm_cr_pr_paging_btm_2?ie=UTF8&pageNumber=2&pageSize=50&sortBy=recent">
                        Next<span class="a-letter-space"></span><span class="a-letter-space"></span>&rarr;
                    </a>
                </li>
            </ul>            
            */
            int pageNum = 0;
            var lastPageNode = html.SelectSingleNode("//ul/li[@class='page-button' and position() = (last()-1)]/a");

            if (lastPageNode != null)
            {
                Match m = pageNumberFromUrl.Match(lastPageNode.GetAttributeValue("href", ""));

                if (m.Groups.Count > 1)
                {
                    int.TryParse(m.Groups[1].Value, out pageNum);
                }
            }

            return pageNum;
        }

        internal static void AddReviewPagesToCrawl_Old(NCrawler.Crawler crawler, string lastPageUrl, int maxPage)
        {
            int pageNum = 0;
            Match m = pageNumberFromUrl.Match(lastPageUrl);

            if (m.Groups.Count > 1)
            {
                int.TryParse(m.Groups[1].Value, out pageNum);

                if (pageNum < maxPage)
                {
                    maxPage = pageNum; //if there are less than maxPage pages then only add up to last page
                }
            }
            else
            {
                maxPage = 0;
            }

            for (int i = 2; i <= maxPage; i++) //only crawl up to maxPage pages
            {
                //http://www.amazon.com/The-Screwtape-Letters-Proposes-Toast/product-reviews/0060652896/ref=cm_cr_pr_top_link_18?ie=UTF8&pageNumber=18&showViewpoints=0&sortBy=bySubmissionDateAscending
                string s = lastPageUrl.Replace("&pageNumber=" + pageNum + "&", "&pageNumber=" + i + "&");
                crawler.AddStep(new Uri(lastPageUrl.Replace("&pageNumber=" + pageNum + "&", "&pageNumber=" + i + "&")), 0);
            }
        }

        internal static void AddReviewPagesToCrawl(NCrawler.Crawler crawler, string baseUrl, int lastPageNum)
        {
            for (int i = 2; i <= lastPageNum; i++)
            {
                //http://www.amazon.com/We-Yevgeny-Zamyatin/product-reviews/0140185852/ref=cm_cr_pr_btm_link_4?pageSize=50&pageNumber=4&sortBy=recent

                string url = baseUrl.Replace("&pageNumber=1", "&pageNumber=" + i);
                url = url.Replace("ref=cm_cr_pr_btm_link_1", "ref=cm_cr_pr_btm_link_" + i);
                crawler.AddStep(new Uri(url), 0);
            }
        }

        private static Regex numberFromCommentText = new Regex(@".*\(([\d\,]+)\).*", RegexOptions.IgnoreCase);
        internal static int GetNumberFromCommentText(string s)
        {
            int num = 0;
            Match m = numberFromCommentText.Match(s);

            if (m.Groups.Count > 1)
            {
                int.TryParse(m.Groups[1].Value, out num);
            }

            return num;
        }

        private static Regex numbersFromHelpfulText = new Regex(@"([\d\,]+).?of.?([\d\,]+)", RegexOptions.IgnoreCase);
        internal static int[] GetNumbersFromHelpfulText(string s)
        {
            //18,331 of 18,972 people found the following review helpful
            int[] nums = new int[2];
            Match m = numbersFromHelpfulText.Match(s);

            if (m.Groups.Count > 2)
            {
                try
                {
                    nums[0] = int.Parse(Regex.Replace(m.Groups[1].Value, ",", "")); //numHelpful
                    nums[1] = int.Parse(Regex.Replace(m.Groups[2].Value, ",", "")); //numTotal
                }
                catch (Exception) { }
            }

            return nums;
        }

        private static Regex reviewerIdFromURL = new Regex(@"profile/([^/]+)/", RegexOptions.IgnoreCase);
        internal static string ExtractReviewerIdFromUrlString(string s)
        {
            string id = "";

            if (s == null)
            {
                return id;
            }
            //<a href="http://www.amazon.com/gp/pdp/profile/A265NE6H6LYX87/ref=cm_cr_pr_pdp" >

            Match m = reviewerIdFromURL.Match(s);
            if (m.Groups.Count > 1)
            {
                id = m.Groups[1].Value;
            }

            return id;
        }

        internal static string ExtractUrlFromUrlString(string s) //url without the .../ref=cm_cr_pr_pdp/177-2514665-6497836 part
        {
            return s.Substring(0, s.IndexOf("ref="));
        }


        private static Regex dateFromHelpfulString = new Regex(@"on (.+) by", RegexOptions.IgnoreCase);
        internal static DateTime? ExtractPublishedDateFromHelpfulString(string s)
        {
            //Published on May 28, 2005 by Tamela Mccann
            //Published 13 months ago by Andrea Thompson
            DateTime? d = null;

            Match m = dateFromHelpfulString.Match(s);
            if (m.Groups.Count > 1)
            {
                try
                {
                    d = DateTime.Parse(m.Groups[1].Value);
                }
                catch (Exception) { d = null; } //keep date as null
            }

            return d;
        }

        internal static short GetStarRatingFromString(string s)
        {
            //<i class="a-icon a-icon-star a-star-3 review-rating">
            //        <span class="a-icon-alt">
            //            3.0 out of 5
            //            stars
            //        </span>
            //    </i>

            short rating = -1;
            Regex star = new Regex(@"([0-5])\.0");
            Match m = star.Match(s);

            if (m != null && m.Groups.Count > 1)
            {
                short.TryParse(m.Groups[1].Value, out rating);
            }

            return rating;
        }

        internal static string RemoveExcessWhitespaceFromString(string s)
        {
            return Regex.Replace(s, @"\s+", " ");

        }
    }
}
