using NCrawler;
using NCrawler.HtmlProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonCrawler
{
    class SavePageCrawler
    {
        private static SavePageForm Form;
        private static Crawler c = null;
        private static string Url = "";

        public static Dictionary<string, bool> UserAgentTracker = CrawlUtil.InitUserAgentTracker();

        public static void Run(SavePageForm parentForm, string url)
        {
            Form = parentForm;
            Url = url;

            c = new Crawler(new Uri(url),
                new HtmlDocumentProcessor(), // Process html
                new SaveFileStep());

            c.MaximumThreadCount = 1;
            c.MaximumCrawlDepth = 1;
            c.ExcludeFilter = CrawlUtil.ExtensionsToSkip;

            c.AdhereToRobotRules = false;
            c.CrawlFinished += new EventHandler<NCrawler.Events.CrawlFinishedEventArgs>(c_CrawlFinished);

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

        static void c_CrawlFinished(object sender, NCrawler.Events.CrawlFinishedEventArgs e)
        {
            Form.appendLineToLog("Finished: " + Url);
        }
    }
}
