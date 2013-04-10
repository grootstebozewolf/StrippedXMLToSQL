namespace StrippedXMLToSQL
{
    partial class frmMain
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtInsert = new System.Windows.Forms.TextBox();
            this.lblInsert = new System.Windows.Forms.Label();
            this.txtUpdate = new System.Windows.Forms.TextBox();
            this.lblUpdate = new System.Windows.Forms.Label();
            this.pnlParseOptions = new System.Windows.Forms.Panel();
            this.rbtnByCompiledRegExType = new System.Windows.Forms.RadioButton();
            this.cboDatabase = new System.Windows.Forms.ComboBox();
            this.rbtnByCompiledRegEx = new System.Windows.Forms.RadioButton();
            this.rbtnByChunks = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlParseOptions.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 38);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtInsert);
            this.splitContainer1.Panel1.Controls.Add(this.lblInsert);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtUpdate);
            this.splitContainer1.Panel2.Controls.Add(this.lblUpdate);
            this.splitContainer1.Size = new System.Drawing.Size(620, 473);
            this.splitContainer1.SplitterDistance = 235;
            this.splitContainer1.TabIndex = 4;
            // 
            // txtInsert
            // 
            this.txtInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInsert.Location = new System.Drawing.Point(3, 19);
            this.txtInsert.Multiline = true;
            this.txtInsert.Name = "txtInsert";
            this.txtInsert.Size = new System.Drawing.Size(614, 213);
            this.txtInsert.TabIndex = 4;
            this.txtInsert.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Statement_KeyDown);
            // 
            // lblInsert
            // 
            this.lblInsert.AutoSize = true;
            this.lblInsert.Location = new System.Drawing.Point(0, 0);
            this.lblInsert.Name = "lblInsert";
            this.lblInsert.Size = new System.Drawing.Size(33, 13);
            this.lblInsert.TabIndex = 3;
            this.lblInsert.Text = "Insert";
            // 
            // txtUpdate
            // 
            this.txtUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUpdate.Location = new System.Drawing.Point(3, 18);
            this.txtUpdate.Multiline = true;
            this.txtUpdate.Name = "txtUpdate";
            this.txtUpdate.Size = new System.Drawing.Size(614, 213);
            this.txtUpdate.TabIndex = 5;
            this.txtUpdate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Statement_KeyDown);
            // 
            // lblUpdate
            // 
            this.lblUpdate.AutoSize = true;
            this.lblUpdate.Location = new System.Drawing.Point(0, 0);
            this.lblUpdate.Name = "lblUpdate";
            this.lblUpdate.Size = new System.Drawing.Size(42, 13);
            this.lblUpdate.TabIndex = 4;
            this.lblUpdate.Text = "Update";
            // 
            // pnlParseOptions
            // 
            this.pnlParseOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlParseOptions.Controls.Add(this.rbtnByCompiledRegExType);
            this.pnlParseOptions.Controls.Add(this.cboDatabase);
            this.pnlParseOptions.Controls.Add(this.rbtnByCompiledRegEx);
            this.pnlParseOptions.Controls.Add(this.rbtnByChunks);
            this.pnlParseOptions.Controls.Add(this.label1);
            this.pnlParseOptions.Location = new System.Drawing.Point(14, 8);
            this.pnlParseOptions.Name = "pnlParseOptions";
            this.pnlParseOptions.Size = new System.Drawing.Size(618, 30);
            this.pnlParseOptions.TabIndex = 5;
            // 
            // rbtnByCompiledRegExType
            // 
            this.rbtnByCompiledRegExType.AutoSize = true;
            this.rbtnByCompiledRegExType.Location = new System.Drawing.Point(288, 5);
            this.rbtnByCompiledRegExType.Name = "rbtnByCompiledRegExType";
            this.rbtnByCompiledRegExType.Size = new System.Drawing.Size(130, 17);
            this.rbtnByCompiledRegExType.TabIndex = 10;
            this.rbtnByCompiledRegExType.Text = "Compiled RegEx Type";
            this.rbtnByCompiledRegExType.UseVisualStyleBackColor = true;
            this.rbtnByCompiledRegExType.Visible = false;
            // 
            // cboDatabase
            // 
            this.cboDatabase.FormattingEnabled = true;
            this.cboDatabase.Items.AddRange(new object[] {
            "Geveke",
            "Jaski",
            "Volta",
            "FSS",
            "Worksphere",
            "Planon",
            "Nuon",
            "DHL",
            "Assa Abloy",
            "Bosch SP"});
            this.cboDatabase.Location = new System.Drawing.Point(494, 4);
            this.cboDatabase.Name = "cboDatabase";
            this.cboDatabase.Size = new System.Drawing.Size(121, 21);
            this.cboDatabase.TabIndex = 8;
            // 
            // rbtnByCompiledRegEx
            // 
            this.rbtnByCompiledRegEx.AutoSize = true;
            this.rbtnByCompiledRegEx.Checked = true;
            this.rbtnByCompiledRegEx.Location = new System.Drawing.Point(139, 5);
            this.rbtnByCompiledRegEx.Name = "rbtnByCompiledRegEx";
            this.rbtnByCompiledRegEx.Size = new System.Drawing.Size(143, 17);
            this.rbtnByCompiledRegEx.TabIndex = 7;
            this.rbtnByCompiledRegEx.TabStop = true;
            this.rbtnByCompiledRegEx.Text = "Compiled RegEx Capture";
            this.rbtnByCompiledRegEx.UseVisualStyleBackColor = true;
            this.rbtnByCompiledRegEx.CheckedChanged += new System.EventHandler(this.rbtnCompiledRegexCapture_CheckedChanged);
            // 
            // rbtnByChunks
            // 
            this.rbtnByChunks.AutoSize = true;
            this.rbtnByChunks.Location = new System.Drawing.Point(48, 5);
            this.rbtnByChunks.Name = "rbtnByChunks";
            this.rbtnByChunks.Size = new System.Drawing.Size(85, 17);
            this.rbtnByChunks.TabIndex = 5;
            this.rbtnByChunks.Text = "Split Capture";
            this.rbtnByChunks.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Options";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 501);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(644, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 523);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pnlParseOptions);
            this.Controls.Add(this.splitContainer1);
            this.Name = "frmMain";
            this.Text = "Convert";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FrmMainDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FrmMainDragEnter);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.pnlParseOptions.ResumeLayout(false);
            this.pnlParseOptions.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtInsert;
        private System.Windows.Forms.Label lblInsert;
        private System.Windows.Forms.TextBox txtUpdate;
        private System.Windows.Forms.Label lblUpdate;
        private System.Windows.Forms.Panel pnlParseOptions;
        private System.Windows.Forms.RadioButton rbtnByChunks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.RadioButton rbtnByCompiledRegEx;
        private System.Windows.Forms.ComboBox cboDatabase;
        private System.Windows.Forms.RadioButton rbtnByCompiledRegExType;
    }
}

