using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AmazonCrawler
{
    public partial class SavePageForm : Form
    {
        private static MainForm Main;

        delegate void SetTextCallback(string text);

        BackgroundWorker bgwHtmlDump;

        Random random = new Random();

        public SavePageForm(MainForm form)
        {
            Main = form;

            InitializeComponent();

            textBoxUrls.MaxLength = Int32.MaxValue;
        }

        private void HtmlDump_Click(object sender, EventArgs e)
        {
            bgwHtmlDump = new BackgroundWorker();
            bgwHtmlDump.WorkerSupportsCancellation = true;

            string[] urls = textBoxUrls.SelectedText.Split();

            bgwHtmlDump.DoWork += delegate
                {
                    appendLineToLog("*** STARTING ***");

                    foreach (string url in urls)
                    {
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            try
                            {
                                SavePageCrawler.Run(this, url);
                                System.Threading.Thread.Sleep(random.Next(1000 * 1, 1000 * 5));
                            }
                            catch (Exception ex)
                            {
                                appendLineToLog(ex.Message);
                            }
                        }

                    }
                };

            bgwHtmlDump.RunWorkerCompleted += delegate
            {
                appendLineToLog("*** FINISHED ***");
            };

            bgwHtmlDump.RunWorkerAsync();
        }

        internal void appendLineToLog(string text)
        {
            if (this.textLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(appendLineToLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textLog.AppendText("[" + System.DateTime.Now.ToString() + "] " + text + Environment.NewLine);

                Console.Out.WriteLine("[" + System.DateTime.Now.ToString() + "] " + text);
            }
        }

        private void HtmlRead_Click(object sender, EventArgs e)
        {
            string[] urls = textBoxUrls.SelectedText.Split();

            BackgroundWorker bgwHtmlRead = new BackgroundWorker();
            bgwHtmlRead.DoWork += delegate
            {
                appendLineToLog("*** STARTING ***");

                var files = Directory.EnumerateFiles("HtmlRead", "*.html", SearchOption.TopDirectoryOnly);


                foreach (string htmlFile in files)
                {
                    if (!string.IsNullOrWhiteSpace(htmlFile))
                    {
                        try
                        {
                            ProcessPage(htmlFile);
                        }
                        catch (Exception ex)
                        {
                            appendLineToLog(ex.Message);
                            appendLineToLog(ex.StackTrace);
                        }
                    }

                }
            };

            bgwHtmlRead.RunWorkerCompleted += delegate
            {
                appendLineToLog("*** FINISHED ***");
            };

            bgwHtmlRead.RunWorkerAsync();
        }

        private void ProcessPage(string filePath)
        {
            DateTime currentDateTime = DateTime.Today;
            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.Load(filePath);

            HtmlNode doc = html.DocumentNode;

            appendLineToLog("Crawling a 'review page' for " + filePath);

            AmazonCrawlerEntities context = DbUtil.getNewContext();

            HtmlNode isbnNode = doc.SelectSingleNode(".//link[@rel='canonical']/@href");
            //<link rel="canonical" href="http://www.amazon.com/Breaking-Dawn-Twilight-Saga-Book/product-reviews/031606792X?pageNumber=113">
            Match isbnMatch = Regex.Match(isbnNode.OuterHtml, @".+/product-reviews/(.+)\?");
            string isbn = "";

            if (isbnMatch == Match.Empty)
            {
                isbnMatch = Regex.Match(isbnNode.OuterHtml, @".+/product-reviews/(.+)(/|"")");
            }
            isbn = isbnMatch.Groups[1].Value;

            Book b = DbUtil.getBook(context, isbn);

            if (b == null) //this should never happen
            {
                appendLineToLog("[error] ISBN " + isbn + " not found in database");
                return;
            }

            HtmlNodeCollection reviews = doc.SelectNodes(".//div[@class='a-section review']");

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

                ////debugging output
                //appendLineToLog("Processing review " + reviewId);
                //appendLineToLog(string.Format("\trating: {0}", rating));
                //appendLineToLog(string.Format("\ttitle: {0}", title));
                //appendLineToLog(string.Format("\tReviewer: {0} | {1}", reviewToAdd.reviewerId, reviewToAdd.reviewerName));
                ////badges not output
                //appendLineToLog(string.Format("\tPublished: {0}", publishedDate));
                //appendLineToLog(string.Format("\tContent: {0}", reviewContent.Substring(0, chars)));
                //appendLineToLog(string.Format("\tHelpful: {0} of {1}", reviewToAdd.numHelpful, reviewToAdd.numTotal));
                //appendLineToLog(string.Format("\tCommments: {0}", numComments));
                //appendLineToLog(string.Format("\tisVerifiedPurchase: {0}", isVerifiedPurchase));
                //appendLineToLog(string.Format("\tFormat: {0}", reviewToAdd.format));

                try
                {
                    context.SaveChanges();
                    numProcessed += 1;
                }
                catch (Exception ex)
                {
                    appendLineToLog(ex.Message);
                    appendLineToLog(ex.StackTrace);
                    numNotProcessed += 1;
                }
            }

            appendLineToLog(string.Format("{0} reviews saved/updated on page; {1} not saved; {2} badges.", numProcessed, numNotProcessed, numBadges));

        }

        private void SavePageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwHtmlDump != null)
            {
                try
                {
                    bgwHtmlDump.CancelAsync();
                } catch (Exception ex)
                {
                    Main.appendLineToLog(ex.Message);
                    Main.appendLineToLog(ex.StackTrace);
                }
            }
        }
    }
}
