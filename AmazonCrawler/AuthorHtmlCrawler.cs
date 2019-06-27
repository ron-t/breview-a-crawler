using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NCrawler;
using NCrawler.HtmlProcessor;
using NCrawler.Interfaces;
using NCrawler.IsolatedStorageServices;
using System.Data;

namespace AmazonCrawler
{
    class AuthorHtmlCrawler
    {
        public static MainForm form;
        private static Crawler c = null;

        public static string baseUri = "";
        public static Author currentAuthor = null;

        public static void Run(MainForm parentForm, Author author)
        {
            form = parentForm;
            IsolatedStorageModule.Setup(false);

            currentAuthor = author;
            baseUri = author.detailPageURL;

            c = new Crawler(new Uri(baseUri),
                new HtmlDocumentProcessor(), // Process html
                new AuthorDetailPageDumperStep());

            // Custom step to visualize crawl
            c.MaximumThreadCount = 1;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;

            c.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.94 Safari/537.4";

            // Begin crawl
            c.Crawl();
        }

        //public static void Run(MainForm parentForm, string authorId)
        //{

        //}
    }

    internal class AuthorDetailPageDumperStep : IPipelineStep
    {
        #region IPipelineStep Members

        public void Process(Crawler crawler, PropertyBag propertyBag)
        {
            DateTime currentDateTime = DateTime.Now;

            HtmlDocument htmlDoc = propertyBag["HtmlDoc"].Value as HtmlDocument;
            if (htmlDoc == null)
            {
                return; //this happens with HTTP errors etc. We don't bother with retrying or anything like that :(
            }

            AuthorHtmlCrawler.form.appendLineToLog("Crawling 'details (rankings) page' for " + AuthorHtmlCrawler.currentAuthor.DisplayString + " [ " + propertyBag.ResponseUri.OriginalString + " ]");

            lock (this)
            {
                AmazonCrawlerEntities context = DbUtil.getNewContext();

                Author a = DbUtil.getAuthor(context, AuthorHtmlCrawler.currentAuthor.id);
                string updated = "rankings";

                if (a == null) //this should never happen
                {
                    AuthorHtmlCrawler.form.appendLineToLog("[error] author id " + AuthorHtmlCrawler.currentAuthor.id + " not found in database");
                    return;
                }

                //get rankings
                var rankingNodes = htmlDoc.DocumentNode.SelectNodes(".//div[@class='nodeRank']");
                if (rankingNodes != null)
                {
                    foreach (var rankNode in rankingNodes)
                    {
                        try
                        {
                            Ranking r = new Ranking();
                            r.authorId = a.id;
                            r.statsCollectedAt = currentDateTime;
                            r.rankString = CrawlUtil.TidyHtmlText(rankNode.InnerText);

                            AuthorHtmlCrawler.form.appendLineToLog(r.rankString);
                            context.AddToRankings(r);
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            AuthorHtmlCrawler.form.appendLineToLog("**ERROR** " + ex.Message);
                            if (ex.InnerException != null)
                            {
                                AuthorHtmlCrawler.form.appendLineToLog("\t** " + ex.InnerException.Message);
                            }
                        }
                    }
                }
                else
                {
                    try //add a ranking with null rankstring to mark this author has been processed
                    {
                        Ranking r = new Ranking();
                        r.authorId = a.id;

                        context.AddToRankings(r);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

                AuthorHtmlCrawler.form.appendLineToLog("Updated " + updated + " for " + AuthorHtmlCrawler.currentAuthor.DisplayString);
            }


        }

        #endregion
    }
}


