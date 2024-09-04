using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace PowerAPI.Service.Reports
{
    public partial class RmaReport : DevExpress.XtraReports.UI.XtraReport
    {
        public RmaReport()
        {
            InitializeComponent();
        }

        private void sqlDataSource1_ConfigureDataConnection(object sender, DevExpress.DataAccess.Sql.ConfigureDataConnectionEventArgs e)
        {

        }
    }
}
