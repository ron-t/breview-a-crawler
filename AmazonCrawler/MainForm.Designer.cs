namespace AmazonCrawler
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lstBooks = new System.Windows.Forms.ListBox();
            this.txtOutputLog = new System.Windows.Forms.TextBox();
            this.btnLoadNexToProcess = new System.Windows.Forms.Button();
            this.btnClearBookList = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.chkTitle = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkReviewsFC = new System.Windows.Forms.CheckBox();
            this.chkStatsCollected = new System.Windows.Forms.CheckBox();
            this.nudToLoad = new System.Windows.Forms.NumericUpDown();
            this.txtISBN = new System.Windows.Forms.TextBox();
            this.chkAuthor = new System.Windows.Forms.CheckBox();
            this.chkDetailUrl = new System.Windows.Forms.CheckBox();
            this.chkReviewUrl = new System.Windows.Forms.CheckBox();
            this.btnUpdateAuthorRank = new VerticalButton.VerticalButton();
            this.btnUpdateBookReviewFromHTML = new VerticalButton.VerticalButton();
            this.btnUpdateBookRankFromHTML = new VerticalButton.VerticalButton();
            this.btnUpdateBookStatsFromReviewPage = new VerticalButton.VerticalButton();
            this.btnUpdateBookDetailFromHtml = new VerticalButton.VerticalButton();
            this.btnUpdateBookDetailFromApi = new VerticalButton.VerticalButton();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnLoadAuthors = new System.Windows.Forms.Button();
            this.chkAllAuthors = new System.Windows.Forms.CheckBox();
            this.toolTipAllAuthors = new System.Windows.Forms.ToolTip(this.components);
            this.txtMinIsbn = new System.Windows.Forms.TextBox();
            this.txtMaxIsbn = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudToLoad)).BeginInit();
            this.SuspendLayout();
            // 
            // lstBooks
            // 
            this.lstBooks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstBooks.FormattingEnabled = true;
            this.lstBooks.HorizontalScrollbar = true;
            this.lstBooks.Location = new System.Drawing.Point(13, 195);
            this.lstBooks.Name = "lstBooks";
            this.lstBooks.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstBooks.Size = new System.Drawing.Size(288, 459);
            this.lstBooks.TabIndex = 0;
            // 
            // txtOutputLog
            // 
            this.txtOutputLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutputLog.Location = new System.Drawing.Point(374, 20);
            this.txtOutputLog.Multiline = true;
            this.txtOutputLog.Name = "txtOutputLog";
            this.txtOutputLog.ReadOnly = true;
            this.txtOutputLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutputLog.Size = new System.Drawing.Size(407, 634);
            this.txtOutputLog.TabIndex = 1;
            // 
            // btnLoadNexToProcess
            // 
            this.btnLoadNexToProcess.Location = new System.Drawing.Point(7, 44);
            this.btnLoadNexToProcess.Name = "btnLoadNexToProcess";
            this.btnLoadNexToProcess.Size = new System.Drawing.Size(115, 57);
            this.btnLoadNexToProcess.TabIndex = 2;
            this.btnLoadNexToProcess.UseVisualStyleBackColor = true;
            this.btnLoadNexToProcess.Click += new System.EventHandler(this.btnLoadNexToProcess_Click);
            // 
            // btnClearBookList
            // 
            this.btnClearBookList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearBookList.Location = new System.Drawing.Point(13, 771);
            this.btnClearBookList.Name = "btnClearBookList";
            this.btnClearBookList.Size = new System.Drawing.Size(111, 23);
            this.btnClearBookList.TabIndex = 5;
            this.btnClearBookList.Text = "Clear list of books";
            this.btnClearBookList.UseVisualStyleBackColor = true;
            this.btnClearBookList.Click += new System.EventHandler(this.btnClearBookList_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(257, 106);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(42, 55);
            this.button1.TabIndex = 6;
            this.button1.Text = "load from file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkTitle
            // 
            this.chkTitle.AutoSize = true;
            this.chkTitle.Location = new System.Drawing.Point(128, 14);
            this.chkTitle.Name = "chkTitle";
            this.chkTitle.Size = new System.Drawing.Size(58, 17);
            this.chkTitle.TabIndex = 7;
            this.chkTitle.Text = "1. Title";
            this.chkTitle.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkReviewsFC);
            this.groupBox1.Controls.Add(this.chkStatsCollected);
            this.groupBox1.Controls.Add(this.nudToLoad);
            this.groupBox1.Controls.Add(this.txtISBN);
            this.groupBox1.Controls.Add(this.chkAuthor);
            this.groupBox1.Controls.Add(this.chkDetailUrl);
            this.groupBox1.Controls.Add(this.chkReviewUrl);
            this.groupBox1.Controls.Add(this.chkTitle);
            this.groupBox1.Controls.Add(this.btnLoadNexToProcess);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 180);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Load books to process";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "only load this ISBN:";
            // 
            // chkReviewsFC
            // 
            this.chkReviewsFC.AutoSize = true;
            this.chkReviewsFC.Location = new System.Drawing.Point(128, 98);
            this.chkReviewsFC.Name = "chkReviewsFC";
            this.chkReviewsFC.Size = new System.Drawing.Size(94, 17);
            this.chkReviewsFC.TabIndex = 11;
            this.chkReviewsFC.Text = "Reviews (F/C)";
            this.chkReviewsFC.UseVisualStyleBackColor = true;
            // 
            // chkStatsCollected
            // 
            this.chkStatsCollected.AutoSize = true;
            this.chkStatsCollected.Location = new System.Drawing.Point(128, 65);
            this.chkStatsCollected.Name = "chkStatsCollected";
            this.chkStatsCollected.Size = new System.Drawing.Size(106, 17);
            this.chkStatsCollected.TabIndex = 10;
            this.chkStatsCollected.Text = "3. StatsCollected";
            this.chkStatsCollected.UseVisualStyleBackColor = true;
            // 
            // nudToLoad
            // 
            this.nudToLoad.Location = new System.Drawing.Point(7, 19);
            this.nudToLoad.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.nudToLoad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudToLoad.Name = "nudToLoad";
            this.nudToLoad.Size = new System.Drawing.Size(115, 20);
            this.nudToLoad.TabIndex = 9;
            this.nudToLoad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudToLoad.ValueChanged += new System.EventHandler(this.nudToLoad_ValueChanged);
            // 
            // txtISBN
            // 
            this.txtISBN.Location = new System.Drawing.Point(109, 125);
            this.txtISBN.Name = "txtISBN";
            this.txtISBN.Size = new System.Drawing.Size(100, 20);
            this.txtISBN.TabIndex = 9;
            // 
            // chkAuthor
            // 
            this.chkAuthor.AutoSize = true;
            this.chkAuthor.Location = new System.Drawing.Point(128, 82);
            this.chkAuthor.Name = "chkAuthor";
            this.chkAuthor.Size = new System.Drawing.Size(66, 17);
            this.chkAuthor.TabIndex = 9;
            this.chkAuthor.Text = "AuthorId";
            this.chkAuthor.UseVisualStyleBackColor = true;
            // 
            // chkDetailUrl
            // 
            this.chkDetailUrl.AutoSize = true;
            this.chkDetailUrl.Location = new System.Drawing.Point(128, 30);
            this.chkDetailUrl.Name = "chkDetailUrl";
            this.chkDetailUrl.Size = new System.Drawing.Size(87, 17);
            this.chkDetailUrl.TabIndex = 8;
            this.chkDetailUrl.Text = "1. DetailURL";
            this.chkDetailUrl.UseVisualStyleBackColor = true;
            // 
            // chkReviewUrl
            // 
            this.chkReviewUrl.AutoSize = true;
            this.chkReviewUrl.Location = new System.Drawing.Point(128, 47);
            this.chkReviewUrl.Name = "chkReviewUrl";
            this.chkReviewUrl.Size = new System.Drawing.Size(96, 17);
            this.chkReviewUrl.TabIndex = 8;
            this.chkReviewUrl.Text = "2. ReviewURL";
            this.chkReviewUrl.UseVisualStyleBackColor = true;
            // 
            // btnUpdateAuthorRank
            // 
            this.btnUpdateAuthorRank.Enabled = false;
            this.btnUpdateAuthorRank.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateAuthorRank.Location = new System.Drawing.Point(307, 564);
            this.btnUpdateAuthorRank.Name = "btnUpdateAuthorRank";
            this.btnUpdateAuthorRank.Size = new System.Drawing.Size(61, 87);
            this.btnUpdateAuthorRank.TabIndex = 4;
            this.btnUpdateAuthorRank.UseVisualStyleBackColor = true;
            this.btnUpdateAuthorRank.VerticalText = null;
            this.btnUpdateAuthorRank.Click += new System.EventHandler(this.btnUpdateAuthorRank_Click);
            // 
            // btnUpdateBookReviewFromHTML
            // 
            this.btnUpdateBookReviewFromHTML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateBookReviewFromHTML.Location = new System.Drawing.Point(307, 471);
            this.btnUpdateBookReviewFromHTML.Name = "btnUpdateBookReviewFromHTML";
            this.btnUpdateBookReviewFromHTML.Size = new System.Drawing.Size(61, 87);
            this.btnUpdateBookReviewFromHTML.TabIndex = 4;
            this.btnUpdateBookReviewFromHTML.UseVisualStyleBackColor = true;
            this.btnUpdateBookReviewFromHTML.VerticalText = null;
            this.btnUpdateBookReviewFromHTML.Click += new System.EventHandler(this.btnUpdateBookReviewFromHTML_Click);
            // 
            // btnUpdateBookRankFromHTML
            // 
            this.btnUpdateBookRankFromHTML.Enabled = false;
            this.btnUpdateBookRankFromHTML.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateBookRankFromHTML.Location = new System.Drawing.Point(307, 379);
            this.btnUpdateBookRankFromHTML.Name = "btnUpdateBookRankFromHTML";
            this.btnUpdateBookRankFromHTML.Size = new System.Drawing.Size(61, 87);
            this.btnUpdateBookRankFromHTML.TabIndex = 4;
            this.btnUpdateBookRankFromHTML.UseVisualStyleBackColor = true;
            this.btnUpdateBookRankFromHTML.VerticalText = null;
            this.btnUpdateBookRankFromHTML.Click += new System.EventHandler(this.btnUpdateBookRankFromHTML_Click);
            // 
            // btnUpdateBookStatsFromReviewPage
            // 
            this.btnUpdateBookStatsFromReviewPage.Enabled = false;
            this.btnUpdateBookStatsFromReviewPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateBookStatsFromReviewPage.Location = new System.Drawing.Point(307, 286);
            this.btnUpdateBookStatsFromReviewPage.Name = "btnUpdateBookStatsFromReviewPage";
            this.btnUpdateBookStatsFromReviewPage.Size = new System.Drawing.Size(61, 87);
            this.btnUpdateBookStatsFromReviewPage.TabIndex = 4;
            this.btnUpdateBookStatsFromReviewPage.UseVisualStyleBackColor = true;
            this.btnUpdateBookStatsFromReviewPage.VerticalText = null;
            this.btnUpdateBookStatsFromReviewPage.Click += new System.EventHandler(this.btnUpdateBookStatsFromReviewPage_Click);
            // 
            // btnUpdateBookDetailFromHtml
            // 
            this.btnUpdateBookDetailFromHtml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateBookDetailFromHtml.Location = new System.Drawing.Point(307, 196);
            this.btnUpdateBookDetailFromHtml.Name = "btnUpdateBookDetailFromHtml";
            this.btnUpdateBookDetailFromHtml.Size = new System.Drawing.Size(61, 87);
            this.btnUpdateBookDetailFromHtml.TabIndex = 4;
            this.btnUpdateBookDetailFromHtml.UseVisualStyleBackColor = true;
            this.btnUpdateBookDetailFromHtml.VerticalText = null;
            this.btnUpdateBookDetailFromHtml.Click += new System.EventHandler(this.btnUpdateBookDetailFromHtml_Click);
            // 
            // btnUpdateBookDetailFromApi
            // 
            this.btnUpdateBookDetailFromApi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateBookDetailFromApi.Location = new System.Drawing.Point(307, 106);
            this.btnUpdateBookDetailFromApi.Name = "btnUpdateBookDetailFromApi";
            this.btnUpdateBookDetailFromApi.Size = new System.Drawing.Size(61, 87);
            this.btnUpdateBookDetailFromApi.TabIndex = 4;
            this.btnUpdateBookDetailFromApi.UseVisualStyleBackColor = true;
            this.btnUpdateBookDetailFromApi.VerticalText = null;
            this.btnUpdateBookDetailFromApi.Click += new System.EventHandler(this.btnUpdateBookDetailFromApi_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLog.Location = new System.Drawing.Point(374, 771);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearLog.TabIndex = 10;
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnLoadAuthors
            // 
            this.btnLoadAuthors.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadAuthors.Location = new System.Drawing.Point(20, 162);
            this.btnLoadAuthors.Name = "btnLoadAuthors";
            this.btnLoadAuthors.Size = new System.Drawing.Size(77, 23);
            this.btnLoadAuthors.TabIndex = 12;
            this.btnLoadAuthors.Text = "Load authors";
            this.btnLoadAuthors.UseVisualStyleBackColor = true;
            this.btnLoadAuthors.Click += new System.EventHandler(this.btnLoadAuthors_Click);
            // 
            // chkAllAuthors
            // 
            this.chkAllAuthors.AutoSize = true;
            this.chkAllAuthors.Location = new System.Drawing.Point(103, 165);
            this.chkAllAuthors.Name = "chkAllAuthors";
            this.chkAllAuthors.Size = new System.Drawing.Size(167, 17);
            this.chkAllAuthors.TabIndex = 13;
            this.chkAllAuthors.Text = "All (even if already processed)";
            this.chkAllAuthors.UseVisualStyleBackColor = true;
            // 
            // txtMinIsbn
            // 
            this.txtMinIsbn.Location = new System.Drawing.Point(258, 27);
            this.txtMinIsbn.Name = "txtMinIsbn";
            this.txtMinIsbn.Size = new System.Drawing.Size(100, 20);
            this.txtMinIsbn.TabIndex = 14;
            // 
            // txtMaxIsbn
            // 
            this.txtMaxIsbn.Location = new System.Drawing.Point(258, 75);
            this.txtMaxIsbn.Name = "txtMaxIsbn";
            this.txtMaxIsbn.Size = new System.Drawing.Size(100, 20);
            this.txtMaxIsbn.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(258, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Min ISBN";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(258, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Max ISBN";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(261, 168);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(38, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 658);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMaxIsbn);
            this.Controls.Add(this.txtMinIsbn);
            this.Controls.Add(this.chkAllAuthors);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnLoadAuthors);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnClearBookList);
            this.Controls.Add(this.btnUpdateAuthorRank);
            this.Controls.Add(this.btnUpdateBookReviewFromHTML);
            this.Controls.Add(this.btnUpdateBookRankFromHTML);
            this.Controls.Add(this.btnUpdateBookStatsFromReviewPage);
            this.Controls.Add(this.btnUpdateBookDetailFromHtml);
            this.Controls.Add(this.btnUpdateBookDetailFromApi);
            this.Controls.Add(this.txtOutputLog);
            this.Controls.Add(this.lstBooks);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Amazon Crawler";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudToLoad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstBooks;
        private System.Windows.Forms.TextBox txtOutputLog;
        private System.Windows.Forms.Button btnLoadNexToProcess;
        private VerticalButton.VerticalButton btnUpdateBookDetailFromApi;
        private System.Windows.Forms.Button btnClearBookList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkTitle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkReviewUrl;
        private VerticalButton.VerticalButton btnUpdateBookDetailFromHtml;
        private System.Windows.Forms.CheckBox chkAuthor;
        private System.Windows.Forms.NumericUpDown nudToLoad;
        private VerticalButton.VerticalButton btnUpdateBookStatsFromReviewPage;
        private VerticalButton.VerticalButton btnUpdateBookRankFromHTML;
        private VerticalButton.VerticalButton btnUpdateBookReviewFromHTML;
        private VerticalButton.VerticalButton btnUpdateAuthorRank;
        private System.Windows.Forms.TextBox txtISBN;
        private System.Windows.Forms.CheckBox chkStatsCollected;
        private System.Windows.Forms.CheckBox chkReviewsFC;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnLoadAuthors;
        private System.Windows.Forms.CheckBox chkAllAuthors;
        private System.Windows.Forms.ToolTip toolTipAllAuthors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkDetailUrl;
        private System.Windows.Forms.TextBox txtMinIsbn;
        private System.Windows.Forms.TextBox txtMaxIsbn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
    }
}

