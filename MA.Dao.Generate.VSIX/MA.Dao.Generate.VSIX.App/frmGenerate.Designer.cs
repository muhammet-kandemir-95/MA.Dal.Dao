namespace MA.Dao.Generate.VSIX.App
{
    partial class frmGenerate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerate));
            this.label1 = new System.Windows.Forms.Label();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.rbMSSQL = new System.Windows.Forms.RadioButton();
            this.rbPOSTGRESQL = new System.Windows.Forms.RadioButton();
            this.rbMYSQL = new System.Windows.Forms.RadioButton();
            this.bConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection String : ";
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConnectionString.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbConnectionString.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbConnectionString.Location = new System.Drawing.Point(118, 12);
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(585, 20);
            this.tbConnectionString.TabIndex = 1;
            // 
            // rbMSSQL
            // 
            this.rbMSSQL.AutoSize = true;
            this.rbMSSQL.Checked = true;
            this.rbMSSQL.Location = new System.Drawing.Point(118, 38);
            this.rbMSSQL.Name = "rbMSSQL";
            this.rbMSSQL.Size = new System.Drawing.Size(62, 17);
            this.rbMSSQL.TabIndex = 2;
            this.rbMSSQL.TabStop = true;
            this.rbMSSQL.Text = "MSSQL";
            this.rbMSSQL.UseVisualStyleBackColor = true;
            // 
            // rbPOSTGRESQL
            // 
            this.rbPOSTGRESQL.AutoSize = true;
            this.rbPOSTGRESQL.Location = new System.Drawing.Point(186, 38);
            this.rbPOSTGRESQL.Name = "rbPOSTGRESQL";
            this.rbPOSTGRESQL.Size = new System.Drawing.Size(98, 17);
            this.rbPOSTGRESQL.TabIndex = 3;
            this.rbPOSTGRESQL.Text = "POSTGRESQL";
            this.rbPOSTGRESQL.UseVisualStyleBackColor = true;
            // 
            // rbMYSQL
            // 
            this.rbMYSQL.AutoSize = true;
            this.rbMYSQL.Location = new System.Drawing.Point(290, 38);
            this.rbMYSQL.Name = "rbMYSQL";
            this.rbMYSQL.Size = new System.Drawing.Size(62, 17);
            this.rbMYSQL.TabIndex = 4;
            this.rbMYSQL.Text = "MYSQL";
            this.rbMYSQL.UseVisualStyleBackColor = true;
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(118, 61);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(77, 23);
            this.bConnect.TabIndex = 5;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // frmGenerate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 90);
            this.Controls.Add(this.bConnect);
            this.Controls.Add(this.rbMYSQL);
            this.Controls.Add(this.rbPOSTGRESQL);
            this.Controls.Add(this.rbMSSQL);
            this.Controls.Add(this.tbConnectionString);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000001, 129);
            this.MinimizeBox = false;
            this.Name = "frmGenerate";
            this.Text = "MA Connect Database";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGenerate_FormClosing);
            this.Load += new System.EventHandler(this.frmGenerate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.RadioButton rbMSSQL;
        private System.Windows.Forms.RadioButton rbPOSTGRESQL;
        private System.Windows.Forms.RadioButton rbMYSQL;
        private System.Windows.Forms.Button bConnect;
    }
}

