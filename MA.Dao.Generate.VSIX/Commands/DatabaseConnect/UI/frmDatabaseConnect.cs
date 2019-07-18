using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MA.Dao.Generate.VSIX.Commands.DatabaseConnect.UI
{
    public partial class frmDatabaseConnect : Form
    {
        public frmDatabaseConnect()
        {
            InitializeComponent();
        }

        #region Variables
        public Models.GenerateSQL Generate { get; set; }
        #endregion

        #region Methods
        public ListViewItem GetNewItem(Models.GenerateSQLItem item)
        {
            ListViewItem lvItem = new ListViewItem();
            lvItem.Text = item.SafeName;
            lvItem.Tag = item;
            return lvItem;
        }
        public List<Models.GenerateSQLItem> GetList(ListView lv)
        {
            List<Models.GenerateSQLItem> list = new List<Models.GenerateSQLItem>();
            foreach (ListViewItem item in lv.Items)
            {
                if (item.Checked)
                {
                    list.Add((Models.GenerateSQLItem)item.Tag);
                }
            }
            return list;
        }
        #endregion

        private void frmDatabaseConnect_Load(object sender, EventArgs e)
        {
            lvTables.Items.AddRange(
                Generate.Tables.OrderBy(o => o.SafeName).Select(o => GetNewItem(o)).ToArray()
                );
            lvProcedures.Items.AddRange(
                Generate.Procedures.OrderBy(o => o.SafeName).Select(o => GetNewItem(o)).ToArray()
                );
            lvViews.Items.AddRange(
                Generate.Views.OrderBy(o => o.SafeName).Select(o => GetNewItem(o)).ToArray()
                );
            lvFunctionsScalar.Items.AddRange(
                Generate.FunctionsScalar.OrderBy(o => o.SafeName).Select(o => GetNewItem(o)).ToArray()
                );
            lvFunctionsTable.Items.AddRange(
                Generate.FunctionsTable.OrderBy(o => o.SafeName).Select(o => GetNewItem(o)).ToArray()
                );
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            Generate.Tables = GetList(lvTables);
            Generate.Procedures = GetList(lvProcedures);
            Generate.Views = GetList(lvViews);
            Generate.FunctionsScalar = GetList(lvFunctionsScalar);
            Generate.FunctionsTable = GetList(lvFunctionsTable);
        }

        private void lvTables_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                var listview = ((ListView)sender);
                foreach (ListViewItem item in listview.Items)
                    item.Selected = true;
            }
        }
    }
}
