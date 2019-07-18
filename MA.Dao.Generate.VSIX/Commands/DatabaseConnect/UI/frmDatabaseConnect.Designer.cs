namespace MA.Dao.Generate.VSIX.Commands.DatabaseConnect.UI
{
    partial class frmDatabaseConnect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDatabaseConnect));
            this.bOk = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpTables = new System.Windows.Forms.TabPage();
            this.lvTables = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpProcedures = new System.Windows.Forms.TabPage();
            this.lvProcedures = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpViews = new System.Windows.Forms.TabPage();
            this.lvViews = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpFunctionsScalar = new System.Windows.Forms.TabPage();
            this.lvFunctionsScalar = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpFunctionsTables = new System.Windows.Forms.TabPage();
            this.lvFunctionsTable = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1.SuspendLayout();
            this.tpTables.SuspendLayout();
            this.tpProcedures.SuspendLayout();
            this.tpViews.SuspendLayout();
            this.tpFunctionsScalar.SuspendLayout();
            this.tpFunctionsTables.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOk
            // 
            this.bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOk.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bOk.Location = new System.Drawing.Point(0, 405);
            this.bOk.Margin = new System.Windows.Forms.Padding(4);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(448, 30);
            this.bOk.TabIndex = 0;
            this.bOk.Text = "Ok";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpTables);
            this.tabControl1.Controls.Add(this.tpProcedures);
            this.tabControl1.Controls.Add(this.tpViews);
            this.tabControl1.Controls.Add(this.tpFunctionsScalar);
            this.tabControl1.Controls.Add(this.tpFunctionsTables);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(448, 405);
            this.tabControl1.TabIndex = 1;
            // 
            // tpTables
            // 
            this.tpTables.Controls.Add(this.lvTables);
            this.tpTables.Location = new System.Drawing.Point(4, 26);
            this.tpTables.Margin = new System.Windows.Forms.Padding(4);
            this.tpTables.Name = "tpTables";
            this.tpTables.Padding = new System.Windows.Forms.Padding(4);
            this.tpTables.Size = new System.Drawing.Size(440, 375);
            this.tpTables.TabIndex = 0;
            this.tpTables.Text = "Tables";
            this.tpTables.UseVisualStyleBackColor = true;
            // 
            // lvTables
            // 
            this.lvTables.CheckBoxes = true;
            this.lvTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvTables.FullRowSelect = true;
            this.lvTables.GridLines = true;
            this.lvTables.HideSelection = false;
            this.lvTables.Location = new System.Drawing.Point(4, 4);
            this.lvTables.Margin = new System.Windows.Forms.Padding(4);
            this.lvTables.Name = "lvTables";
            this.lvTables.Size = new System.Drawing.Size(432, 367);
            this.lvTables.TabIndex = 0;
            this.lvTables.UseCompatibleStateImageBehavior = false;
            this.lvTables.View = System.Windows.Forms.View.Details;
            this.lvTables.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvTables_KeyDown);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Safe Name";
            this.columnHeader2.Width = 400;
            // 
            // tpProcedures
            // 
            this.tpProcedures.Controls.Add(this.lvProcedures);
            this.tpProcedures.Location = new System.Drawing.Point(4, 26);
            this.tpProcedures.Margin = new System.Windows.Forms.Padding(4);
            this.tpProcedures.Name = "tpProcedures";
            this.tpProcedures.Padding = new System.Windows.Forms.Padding(4);
            this.tpProcedures.Size = new System.Drawing.Size(440, 375);
            this.tpProcedures.TabIndex = 1;
            this.tpProcedures.Text = "Procedures";
            this.tpProcedures.UseVisualStyleBackColor = true;
            // 
            // lvProcedures
            // 
            this.lvProcedures.CheckBoxes = true;
            this.lvProcedures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.lvProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvProcedures.FullRowSelect = true;
            this.lvProcedures.GridLines = true;
            this.lvProcedures.HideSelection = false;
            this.lvProcedures.Location = new System.Drawing.Point(4, 4);
            this.lvProcedures.Margin = new System.Windows.Forms.Padding(4);
            this.lvProcedures.Name = "lvProcedures";
            this.lvProcedures.Size = new System.Drawing.Size(432, 367);
            this.lvProcedures.TabIndex = 1;
            this.lvProcedures.UseCompatibleStateImageBehavior = false;
            this.lvProcedures.View = System.Windows.Forms.View.Details;
            this.lvProcedures.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvTables_KeyDown);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Safe Name";
            this.columnHeader4.Width = 400;
            // 
            // tpViews
            // 
            this.tpViews.Controls.Add(this.lvViews);
            this.tpViews.Location = new System.Drawing.Point(4, 26);
            this.tpViews.Margin = new System.Windows.Forms.Padding(4);
            this.tpViews.Name = "tpViews";
            this.tpViews.Padding = new System.Windows.Forms.Padding(4);
            this.tpViews.Size = new System.Drawing.Size(440, 375);
            this.tpViews.TabIndex = 2;
            this.tpViews.Text = "Views";
            this.tpViews.UseVisualStyleBackColor = true;
            // 
            // lvViews
            // 
            this.lvViews.CheckBoxes = true;
            this.lvViews.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6});
            this.lvViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvViews.FullRowSelect = true;
            this.lvViews.GridLines = true;
            this.lvViews.HideSelection = false;
            this.lvViews.Location = new System.Drawing.Point(4, 4);
            this.lvViews.Margin = new System.Windows.Forms.Padding(4);
            this.lvViews.Name = "lvViews";
            this.lvViews.Size = new System.Drawing.Size(432, 367);
            this.lvViews.TabIndex = 1;
            this.lvViews.UseCompatibleStateImageBehavior = false;
            this.lvViews.View = System.Windows.Forms.View.Details;
            this.lvViews.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvTables_KeyDown);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Safe Name";
            this.columnHeader6.Width = 400;
            // 
            // tpFunctionsScalar
            // 
            this.tpFunctionsScalar.Controls.Add(this.lvFunctionsScalar);
            this.tpFunctionsScalar.Location = new System.Drawing.Point(4, 26);
            this.tpFunctionsScalar.Margin = new System.Windows.Forms.Padding(4);
            this.tpFunctionsScalar.Name = "tpFunctionsScalar";
            this.tpFunctionsScalar.Padding = new System.Windows.Forms.Padding(4);
            this.tpFunctionsScalar.Size = new System.Drawing.Size(440, 375);
            this.tpFunctionsScalar.TabIndex = 3;
            this.tpFunctionsScalar.Text = "Functions Scalar";
            this.tpFunctionsScalar.UseVisualStyleBackColor = true;
            // 
            // lvFunctionsScalar
            // 
            this.lvFunctionsScalar.CheckBoxes = true;
            this.lvFunctionsScalar.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8});
            this.lvFunctionsScalar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFunctionsScalar.FullRowSelect = true;
            this.lvFunctionsScalar.GridLines = true;
            this.lvFunctionsScalar.HideSelection = false;
            this.lvFunctionsScalar.Location = new System.Drawing.Point(4, 4);
            this.lvFunctionsScalar.Margin = new System.Windows.Forms.Padding(4);
            this.lvFunctionsScalar.Name = "lvFunctionsScalar";
            this.lvFunctionsScalar.Size = new System.Drawing.Size(432, 367);
            this.lvFunctionsScalar.TabIndex = 1;
            this.lvFunctionsScalar.UseCompatibleStateImageBehavior = false;
            this.lvFunctionsScalar.View = System.Windows.Forms.View.Details;
            this.lvFunctionsScalar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvTables_KeyDown);
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Safe Name";
            this.columnHeader8.Width = 400;
            // 
            // tpFunctionsTables
            // 
            this.tpFunctionsTables.Controls.Add(this.lvFunctionsTable);
            this.tpFunctionsTables.Location = new System.Drawing.Point(4, 26);
            this.tpFunctionsTables.Margin = new System.Windows.Forms.Padding(4);
            this.tpFunctionsTables.Name = "tpFunctionsTables";
            this.tpFunctionsTables.Padding = new System.Windows.Forms.Padding(4);
            this.tpFunctionsTables.Size = new System.Drawing.Size(440, 375);
            this.tpFunctionsTables.TabIndex = 4;
            this.tpFunctionsTables.Text = "Functions Table";
            this.tpFunctionsTables.UseVisualStyleBackColor = true;
            // 
            // lvFunctionsTable
            // 
            this.lvFunctionsTable.CheckBoxes = true;
            this.lvFunctionsTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10});
            this.lvFunctionsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFunctionsTable.FullRowSelect = true;
            this.lvFunctionsTable.GridLines = true;
            this.lvFunctionsTable.HideSelection = false;
            this.lvFunctionsTable.Location = new System.Drawing.Point(4, 4);
            this.lvFunctionsTable.Margin = new System.Windows.Forms.Padding(4);
            this.lvFunctionsTable.Name = "lvFunctionsTable";
            this.lvFunctionsTable.Size = new System.Drawing.Size(432, 367);
            this.lvFunctionsTable.TabIndex = 1;
            this.lvFunctionsTable.UseCompatibleStateImageBehavior = false;
            this.lvFunctionsTable.View = System.Windows.Forms.View.Details;
            this.lvFunctionsTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvTables_KeyDown);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Safe Name";
            this.columnHeader10.Width = 400;
            // 
            // frmDatabaseConnect
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 435);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.bOk);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmDatabaseConnect";
            this.Text = "Database Connect";
            this.Load += new System.EventHandler(this.frmDatabaseConnect_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpTables.ResumeLayout(false);
            this.tpProcedures.ResumeLayout(false);
            this.tpViews.ResumeLayout(false);
            this.tpFunctionsScalar.ResumeLayout(false);
            this.tpFunctionsTables.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpTables;
        private System.Windows.Forms.TabPage tpProcedures;
        private System.Windows.Forms.ListView lvTables;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView lvProcedures;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TabPage tpViews;
        private System.Windows.Forms.ListView lvViews;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.TabPage tpFunctionsScalar;
        private System.Windows.Forms.ListView lvFunctionsScalar;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.TabPage tpFunctionsTables;
        private System.Windows.Forms.ListView lvFunctionsTable;
        private System.Windows.Forms.ColumnHeader columnHeader10;
    }
}