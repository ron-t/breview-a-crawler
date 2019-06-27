using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Amazon.PAAPI.AWS;
using Amazon.PAAPI.WCF;
using System.Threading.Tasks;

namespace AmazonCrawler
{
    public partial class MainForm : Form
    {
        delegate void SetTextCallback(string text);
        delegate void SetObjectCallback(object o);
        delegate void SetArrayCallback(object[] o);
        delegate void SetVoidCallback();


        BasicHttpBinding binding;
        AWSECommerceServicePortTypeClient amazonClient;

        private Random RANDOM = new Random();

        public MainForm()
        {
            InitializeComponent();

            btnUpdateBookDetailFromApi.VerticalText = "1. API - Get details for selected books";
            btnUpdateBookDetailFromHtml.VerticalText = "2. HTML - Get details for selected books";
            btnUpdateBookStatsFromReviewPage.VerticalText = "[redun]3a. REVIEW - Get rating stats for selected books";
            btnUpdateBookRankFromHTML.VerticalText = "3b. HTML - Get rankings for selected books";
            btnUpdateBookReviewFromHTML.VerticalText = "4. REVIEW - Get reviews for selected books";
            btnUpdateAuthorRank.VerticalText = "5. AUTHOR - Get rankings for authors";

            lstBooks.DisplayMember = "DisplayString";
            txtOutputLog.Text = "---OUTPUT LOG---" + Environment.NewLine;

            nudToLoad.Value = 10;
            toolTipAllAuthors.SetToolTip(this.chkAllAuthors, "Default is to only load authors still without rankings (i.e. unprocessed)");

            binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;

            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;
            readerQuotas.MaxBytesPerRead = int.MaxValue;
            readerQuotas.MaxNameTableCharCount = int.MaxValue;

            binding.ReaderQuotas = readerQuotas;

            amazonClient = new AWSECommerceServicePortTypeClient(
                binding,
                new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

            //Adding this uses the code in Amazon.PAAPI.WCF to sign requests automagically
            amazonClient.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void btnLoadNexToProcess_Click(object sender, EventArgs e)
        {
            btnLoadNexToProcess.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                //TEMP for debugging/testing
                if (!string.IsNullOrEmpty(txtISBN.Text.Trim()))
                {
                    using (var context = DbUtil.getNewContext())
                    {
                        var v = from Book b in context.Books
                                where b.ISBN == txtISBN.Text.Trim()
                                select b;

                        Book bk = v.FirstOrDefault();

                        if (bk != null)
                            addToBookList(new Book[] { bk });
                    }
                }
                else
                    loadNextToProcess();

                //selectAllBooks();
            };

            bgw.RunWorkerCompleted += delegate
            {
                btnLoadNexToProcess.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }

        private void loadNextToProcess()
        {
            //List<Book> toLoad;
            var alreadyLoaded = (from Book b in lstBooks.Items
                                 select b).ToList();

            using (var context = DbUtil.getNewContext())
            {
                bool compareMinIsbn = !string.IsNullOrWhiteSpace(txtMinIsbn.Text);
                bool compareMaxIsbn = !string.IsNullOrWhiteSpace(txtMaxIsbn.Text);

                var toLoad = (from Book b in context.Books
                              where (chkTitle.Checked ? b.title == null : true)
                                  && (chkDetailUrl.Checked ? b.detailPageURL == null : true)
                                  && (chkReviewUrl.Checked ? b.reviewPageURL == null : true)
                                  && (chkAuthor.Checked ? b.authorId == null : true)
                                  && (chkStatsCollected.Checked ? b.statsCollectedAt == null : true)
                                  && (compareMinIsbn ? b.ISBN.CompareTo(txtMinIsbn.Text) >= 0 : true)
                                  && (compareMaxIsbn ? b.ISBN.CompareTo(txtMaxIsbn.Text) <= 0 : true)
                              orderby (compareMinIsbn || compareMaxIsbn ? b.ISBN : "")
                              select b).ToList<Book>();



                if (chkReviewsFC.Checked)
                {
                    var reviewsCrawled = (from Review r in DbUtil.getNewContext().Reviews
                                          where r.reviewType != "R"
                                          select r.Book)
                                         .GroupBy(b => b.ISBN)
                                         .ToList()
                                         .Select(g => g.First())
                                         ;

                    toLoad = toLoad.Except(reviewsCrawled, BookEqualityComparer.Instance).ToList();
                }

                toLoad = toLoad.Except(alreadyLoaded, BookEqualityComparer.Instance).Take(Convert.ToInt32(nudToLoad.Value)).ToList();

                addToBookList(toLoad.ToArray());
                //foreach (var book in toLoad)
                //{
                //    addToBookList(book);
                //}
            }
        }

        #region 1. API - Get details for selected books
        private void btnUpdateBookDetailFromApi_Click(object sender, EventArgs e)
        {

            btnUpdateBookDetailFromApi.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                appendLineToLog("***STARTING " + btnUpdateBookDetailFromApi.VerticalText);

                var selected = (from Book b in GetSelectedBooks()
                                select b.ISBN).ToArray();

                updateBooksDetail(selected);
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("***FINISHED " + btnUpdateBookDetailFromApi.VerticalText);
                btnUpdateBookDetailFromApi.Enabled = true;
            };

            bgw.RunWorkerAsync();

        }

        private void updateBooksDetail(string[] ISBNs)
        {
            ItemLookupRequest request = new ItemLookupRequest();

            if (ISBNs.Count() < 1)
            {
                //appendLineToLog("**No books selected");
                return;
            }

            string[] toProcess;
            /***** batch in 10s *****/
            for (int i = 0; i < ISBNs.Length; i = i + 10)
            {
                toProcess = ISBNs.Skip(i).Take(10).ToArray();

                //mark unprocessed books as'processed' by setting dummy values (but retain existing non null values)
                AmazonCrawlerEntities c = DbUtil.getNewContext();
                foreach (var isbn in toProcess)
                {
                    Book b = DbUtil.getBook(c, isbn);

                    if (chkTitle.Checked && b.title == null)
                    {
                        b.title = "-";
                    }

                    if (chkDetailUrl.Checked && b.detailPageURL == null)
                    {
                        b.detailPageURL = "-";
                    }
                }

                try
                {
                    c.SaveChanges();
                }
                catch (Exception ex)
                {
                    appendLineToLog("[error] preparing books from Db: " + ex.Message);
                    appendLineToLog("\t" + ex.StackTrace);
                    return;
                }

                request.ItemId = toProcess;
                request.IdType = ItemLookupRequestIdType.ISBN;
                //request.SearchIndex = "Books";

                request.ResponseGroup = new string[] { "ItemAttributes" };
                //request.ResponseGroup = new string[] { "Reviews", "Large", "SalesRank" };

                ItemLookup itemLookup = new ItemLookup();
                itemLookup.Request = new ItemLookupRequest[] { request };
                itemLookup.AssociateTag = "notag"; //this is a required param, so I just use a dummy value which seems to work

                // send the ItemSearch request
                ItemLookupResponse response = amazonClient.ItemLookup(itemLookup);

                if (response.Items != null && response.Items.Count() > 0 && response.Items[0].Item != null)
                {
                    AmazonCrawlerEntities context = DbUtil.getNewContext();

                    // write out the results from the ItemSearch request
                    foreach (var item in response.Items[0].Item)
                    {
                        Book toUpdate = DbUtil.getBook(context, item.ASIN);

                        if (toUpdate != null)
                        {
                            int parseOutput;
                            DateTime parseOutputDate = DateTime.MinValue;

                            if (item.ItemAttributes != null && item.ItemAttributes.Title != null)
                            {
                                toUpdate.title = item.ItemAttributes.Title;
                            }

                            //2012-10-31 21:51
                            //Not going to get sales rank from here. There are multiple ranks listed on the details page
                            //so will crawl that separately instead. 
                            //The plan is to crawl it same time as getting rating stats AND author ranks so the stats are 
                            //collected at roughly the same time.

                            //int.TryParse(item.SalesRank, out parseOutput);
                            //toUpdate.salesRank = parseOutput;

                            int.TryParse(item.ItemAttributes.NumberOfPages, out parseOutput);
                            if (parseOutput > 0)
                            {
                                toUpdate.pages = parseOutput;
                            }
                            else
                            {
                                toUpdate.pages = null;
                            }

                            toUpdate.publisher = item.ItemAttributes.Publisher;

                            DateTime.TryParse(item.ItemAttributes.PublicationDate, out parseOutputDate);
                            if (parseOutputDate.Equals(DateTime.MinValue))
                            {
                                //date format is just a year number.
                                DateTime.TryParse(item.ItemAttributes.PublicationDate + "/01/01", out parseOutputDate);
                            }
                            if (parseOutputDate > DateTime.MinValue)
                            {
                                toUpdate.publicationDate = parseOutputDate;
                            }
                            else
                            {
                                toUpdate.publicationDate = null;
                            }

                            toUpdate.detailPageURL = item.DetailPageURL.Substring(0, item.DetailPageURL.IndexOf("%3F"));

                            context.SaveChanges();
                            appendLineToLog(item.ItemAttributes.Title + " (" + item.ASIN + ") updated.");
                        }
                        else
                        {
                            appendLineToLog("[error] ISBN " + item.ASIN + " not found in database");
                        }

                    }
                    if (response.Items[0].Item.Count() != toProcess.Count())
                    {
                        appendLineToLog((toProcess.Count() - response.Items[0].Item.Count()) + " books skipped");
                    }
                }
                else
                {
                    appendLineToLog(toProcess.Count() + " books skipped.");

                    /********************
                     * Check if it's due to ItemID invalid error, if so then continue as normal
                     * ItemID invalid error just means the ISBN doesn't exist in Amazon's API.
                     * ******************/
                    if (response.Items != null && response.Items[0].Request != null && response.Items[0].Request.Errors != null)
                    {
                        var errorCode = response.Items[0].Request.Errors[0].Code;
                        if (errorCode == "AWS.InvalidParameterValue")
                        {
                            sleep();
                            continue;
                        }

                    }
                    //Otherwise there may be an API error
                    //undo the 'process' marker.
                    else
                    {
                        foreach (var isbn in toProcess)
                        {
                            Book b = DbUtil.getBook(c, isbn);

                            if (b.title == "-")
                            {
                                b.title = null;
                            }

                            if (b.detailPageURL == "-")
                            {
                                b.detailPageURL = null;
                            }
                        }

                        try
                        {
                            c.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            appendLineToLog("[error] preparing books from Db: " + ex.Message);
                            appendLineToLog("\t" + ex.StackTrace);
                            return;
                        }

                        //sleep between 3 and 10 minutes before continuing
                        TimeSpan duration = new TimeSpan(0, 0, ((3 * 60) + (RANDOM.Next(10 * 60))));
                        appendLineToLog(string.Format("Sleeping for {0}:{1} mins and secs", duration.Minutes, duration.Seconds));
                        System.Threading.Thread.Sleep(duration);
                    }
                }

                sleep(); //delay each API call
            }



        }
        #endregion

        #region 2. HTML - Get details for selected books
        private void btnUpdateBookDetailFromHtml_Click(object sender, EventArgs e)
        {
            btnUpdateBookDetailFromHtml.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                appendLineToLog("***STARTING " + btnUpdateBookDetailFromHtml.VerticalText);

                var selected = GetSelectedBooks();

                foreach (var book in selected)
                {
                    //skip if dummy detailPageUrl or already has a reviewpage. (perhaps change to avgrating)
                    if (book.detailPageURL == null || book.detailPageURL == "-" || !string.IsNullOrEmpty(book.reviewPageURL))
                    {
                        appendLineToLog("Skipped " + book.DisplayString + " - detailPageURL invalid or already has reviewPageURL");
                        continue;
                    }

                    BookHtmlCrawler.Run(this, book, getDetailsAndAuthor: true, getRanks: false); //don't update ranks/stats

                    sleep(); //delay each page visit
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("***FINISHED " + btnUpdateBookDetailFromHtml.VerticalText);
                btnUpdateBookDetailFromHtml.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }
        #endregion

        #region 3a. REVIEW - Get rating stats for selected books
        private void btnUpdateBookStatsFromReviewPage_Click(object sender, EventArgs e)
        {
            var selected = (from Book b in lstBooks.SelectedItems
                            select b).ToList();

            btnUpdateBookStatsFromReviewPage.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                appendLineToLog("***STARTING " + btnUpdateBookStatsFromReviewPage.VerticalText);

                foreach (var book in selected.ToList())
                {
                    ReviewHtmlCrawler.Run(this, book, true, false);
                    sleep(); //delay each page visit
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("***FINISHED " + btnUpdateBookStatsFromReviewPage.VerticalText);
                btnUpdateBookStatsFromReviewPage.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }
        #endregion

        #region 3b. HTML - Get rankings for selected books
        private void btnUpdateBookRankFromHTML_Click(object sender, EventArgs e)
        {
            var selected = (from Book b in lstBooks.SelectedItems
                            select b).ToList();

            btnUpdateBookRankFromHTML.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                appendLineToLog("***STARTING " + btnUpdateBookRankFromHTML.VerticalText);

                foreach (var book in selected.ToList())
                {
                    BookHtmlCrawler.Run(this, book, false, true);
                    sleep(); //delay each page visit
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("***FINISHED " + btnUpdateBookRankFromHTML.VerticalText);
                btnUpdateBookRankFromHTML.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }
        #endregion

        #region 4. REVIEW - Get reviews for selected books
        private void btnUpdateBookReviewFromHTML_Click(object sender, EventArgs e)
        {
            var selected = (from Book b in lstBooks.SelectedItems
                            select b).ToList();

            btnUpdateBookReviewFromHTML.Enabled = false;

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                appendLineToLog("***STARTING " + btnUpdateBookReviewFromHTML.VerticalText);

                foreach (var book in selected.ToList())
                {
                    ReviewHtmlCrawler.Run(this, book, false, true);
                    sleep(); //delay each page visit
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("***FINISHED " + btnUpdateBookReviewFromHTML.VerticalText);
                btnUpdateBookReviewFromHTML.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }
        #endregion

        #region 5. AUTHOR - Get rankings for authors
        private void btnUpdateAuthorRank_Click(object sender, EventArgs e)
        {
            btnUpdateAuthorRank.Enabled = false;

            var selected = (from Author a in lstBooks.SelectedItems
                            select a).ToList();

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate
            {
                appendLineToLog("***STARTING " + btnUpdateAuthorRank.VerticalText);

                foreach (var author in selected)
                {
                    AuthorHtmlCrawler.Run(this, author);
                    sleep(); //delay each page visit    
                }
            };

            bgw.RunWorkerCompleted += delegate
            {
                appendLineToLog("***FINISHED " + btnUpdateAuthorRank.VerticalText);
                btnUpdateAuthorRank.Enabled = true;
            };

            bgw.RunWorkerAsync();
        }
        #endregion

        private void btnClearBookList_Click(object sender, EventArgs e)
        {
            lstBooks.ClearSelected();
            lstBooks.Items.Clear();
        }

        //TEMP
        private static Dictionary<string, bool> d = CrawlUtil.InitUserAgentTracker();
        private void button1_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < 100; i++)
            //{
            //    int duration = ((2 * 60) + RANDOM.Next(5 * 60)) * 1000;
            //    appendLineToLog("Sleeping for " + duration / 1000 + " seconds");
            //}

            //var key = CrawlUtil.GetRandomUnblockedUserAgent(d);
            //d[key] = true; //block it
            //appendLineToLog(key);

            using (var context = DbUtil.getNewContext())
            {
                string[] isbns = System.IO.File.ReadAllLines("list.txt");

                var booksFromDb = from Book b in context.Books
                        where isbns.Contains(b.ISBN)
                        select b;

                if (booksFromDb != null)
                {
                        addToBookList(booksFromDb.ToArray());
                }
            }

        }

        internal void appendLineToLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtOutputLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(appendLineToLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtOutputLog.AppendText("[" + System.DateTime.Now.ToString() + "] " + text + Environment.NewLine);
                //this.txtLog.ScrollToCaret(); ******

                Console.Out.WriteLine("[" + System.DateTime.Now.ToString() + "] " + text);
            }
        }

        delegate void addToBookListDelegate(Book[] books);
        internal void addToBookList(Book[] books)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lstBooks.InvokeRequired)
            {
                this.Invoke(new addToBookListDelegate(addToBookList), new object[] { books });
            }
            else
            {
                this.lstBooks.Items.AddRange(books);
            }
        }

        //public static string readListBoxSelected(ListBox listbox)
        //{
        //    if (listbox.InvokeRequired)
        //    {
        //        return (string)listbox.Invoke(
        //          new Func<String>(() => readListBoxSelected(listbox))
        //        );
        //    }
        //    else
        //    {
        //        if (istbox.SelectedValue != null)

        //            return listbox.SelectedValue.ToString();
        //        else
        //            return String.Empty;
        //    }
        //}

        internal List<Book> GetSelectedBooks()
        {
            if (this.lstBooks.InvokeRequired)
            {
                return (List<Book>)this.Invoke(
                    new Func<List<Book>>(() => GetSelectedBooks())
                );
            }
            else
            {
                return (from Book b in lstBooks.SelectedItems
                        select b).ToList();
            }
        }

        internal void selectAllBooks()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lstBooks.InvokeRequired)
            {
                SetVoidCallback d = new SetVoidCallback(selectAllBooks);
                this.Invoke(d);
            }
            else
            {
                for (int i = 0; i < lstBooks.Items.Count; i++)
                {
                    this.lstBooks.SetSelected(i, true);
                }
            }
        }

        private void nudToLoad_ValueChanged(object sender, EventArgs e)
        {
            btnLoadNexToProcess.Text = "Load next " + nudToLoad.Value + " where checked is null";
        }

        private void sleep() //sleep between 1.5 and 3 seconds
        {

            System.Threading.Thread.Sleep(1499 + RANDOM.Next(3001));

            if (RANDOM.NextDouble() <= 0.01) //1% chance to sleep between 2 and 10 minutes
            {
                int duration = ((2 * 60) + (RANDOM.Next(10 * 60)) * 1000);
                appendLineToLog("Sleeping for " + duration / 1000 + " seconds");
                System.Threading.Thread.Sleep(duration);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            this.txtOutputLog.Clear();
        }

        private void btnLoadAuthors_Click(object sender, EventArgs e)
        {
            //btnLoadNexToProcess.Enabled = false;

            //BackgroundWorker bgw = new BackgroundWorker();
            //bgw.DoWork += delegate
            //{
            //    List<Author> toLoad;
            //    List<string> authorsInRanking = new List<string> { "" };

            //    var context = DbUtil.getNewContext();

            //    if (!chkAllAuthors.Checked)
            //    {
            //        authorsInRanking = (from r in context.Rankings
            //                            select r.authorId).GroupBy(r => r).ToList().Select(r => r.First()).ToList();
            //    }

            //    var toProcess = (from Author a in context.Authors
            //                     where !authorsInRanking.Contains(a.id)
            //                     select a).Take(Convert.ToInt32(nudToLoad.Value));

            //    toLoad = toProcess.ToList();

            //    foreach (var book in toLoad)
            //    {
            //        addToBookList(book); //in this case items are authors but they still get added to the listbox
            //    }

            //    selectAllBooks(); //in this case list items loaded are authors so it will select all authors
            //};

            //bgw.RunWorkerCompleted += delegate
            //{
            //    btnLoadNexToProcess.Enabled = true;
            //};

            //bgw.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form spf = new SavePageForm(this);
            spf.Show();
        }
    }

    //class VerticalButton : Button
    //{
    //    public string VerticalText { get; set; }
    //    private StringFormat fmt = new StringFormat();

    //    public VerticalButton()
    //        : base()
    //    {
    //        fmt.Alignment = StringAlignment.Center;
    //        fmt.LineAlignment = StringAlignment.Center;
    //    }

    //    protected override void OnPaint(PaintEventArgs pevent)
    //    {
    //        base.OnPaint(pevent);
    //        pevent.Graphics.TranslateTransform(Width, 0);
    //        pevent.Graphics.RotateTransform(90);
    //        pevent.Graphics.DrawString(VerticalText, Font, Brushes.Black, new Rectangle(0, 0, Height, Width), fmt);
    //    }

    //}
}
