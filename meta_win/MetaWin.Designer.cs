namespace meta_win
{
    partial class MetaWin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.srcTxt = new System.Windows.Forms.RichTextBox();
            this.desTxt = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.srcTxt);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.desTxt);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 481;
            this.splitContainer1.TabIndex = 0;
            // 
            // srcTxt
            // 
            this.srcTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.srcTxt.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.srcTxt.Location = new System.Drawing.Point(0, 0);
            this.srcTxt.Name = "srcTxt";
            this.srcTxt.Size = new System.Drawing.Size(481, 450);
            this.srcTxt.TabIndex = 0;
            this.srcTxt.Text = "user = {\n  name: {\n    first: \'phat\',\n    last: \'dam\'\n  },\n  age: 100\n  1/1: 11\n " +
    " 1/2: 12\n  2/1: 21\n  2/2: 22\n}";
            this.srcTxt.TextChanged += new System.EventHandler(this.srcTxt_TextChanged);
            // 
            // desTxt
            // 
            this.desTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.desTxt.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.desTxt.Location = new System.Drawing.Point(0, 0);
            this.desTxt.Name = "desTxt";
            this.desTxt.Size = new System.Drawing.Size(315, 450);
            this.desTxt.TabIndex = 0;
            this.desTxt.Text = "";
            this.desTxt.TextChanged += new System.EventHandler(this.srcTxt_TextChanged);
            // 
            // MetaWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MetaWin";
            this.Text = "MetaWin";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private RichTextBox srcTxt;
        private RichTextBox desTxt;
    }
}