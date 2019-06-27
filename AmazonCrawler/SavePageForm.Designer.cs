namespace AmazonCrawler
{
    partial class SavePageForm
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
            this.textBoxUrls = new System.Windows.Forms.TextBox();
            this.buttonSavePages = new System.Windows.Forms.Button();
            this.textLog = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxUrls
            // 
            this.textBoxUrls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUrls.Location = new System.Drawing.Point(13, 13);
            this.textBoxUrls.Multiline = true;
            this.textBoxUrls.Name = "textBoxUrls";
            this.textBoxUrls.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxUrls.Size = new System.Drawing.Size(575, 280);
            this.textBoxUrls.TabIndex = 0;
            this.textBoxUrls.WordWrap = false;
            // 
            // buttonSavePages
            // 
            this.buttonSavePages.Location = new System.Drawing.Point(13, 300);
            this.buttonSavePages.Name = "buttonSavePages";
            this.buttonSavePages.Size = new System.Drawing.Size(239, 23);
            this.buttonSavePages.TabIndex = 1;
            this.buttonSavePages.Text = "Download selected to HtmlDump";
            this.buttonSavePages.UseVisualStyleBackColor = true;
            this.buttonSavePages.Click += new System.EventHandler(this.HtmlDump_Click);
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textLog.BackColor = System.Drawing.SystemColors.Window;
            this.textLog.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.textLog.Location = new System.Drawing.Point(13, 330);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textLog.Size = new System.Drawing.Size(575, 155);
            this.textLog.TabIndex = 2;
            this.textLog.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(258, 299);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(239, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Process from HtmlRead";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.HtmlRead_Click);
            // 
            // SavePageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 497);
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonSavePages);
            this.Controls.Add(this.textBoxUrls);
            this.Name = "SavePageForm";
            this.Text = "SavePageForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SavePageForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUrls;
        private System.Windows.Forms.Button buttonSavePages;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.Button button1;
    }
}