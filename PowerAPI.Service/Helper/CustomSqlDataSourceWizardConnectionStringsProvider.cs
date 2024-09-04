using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using PowerAPI.Data.Models;
using DevExpress.DataAccess.Native;

namespace PowerAPI.Service.Helper
{
    public class CustomSqlDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        readonly EnterpriseContext reportDataContext;

        public CustomSqlDataSourceWizardConnectionStringsProvider(EnterpriseContext reportDataContext)
        {
            this.reportDataContext = reportDataContext;
        }
        Dictionary<string, string> IDataSourceWizardConnectionStringsProvider.GetConnectionDescriptions()
        {


            Dictionary<string, string> connections = AppConfigHelper.GetConnections().Keys.ToDictionary(x => x, x => x);

            // Customize the loaded connections list. 
            connections.Add("SQL Connection", "Custom SQL Connection");

            return connections;
            //return reportDataContext.SqlDataConnectionsDx.ToDictionary(x => x.Name, x => x.DisplayName);
        }

        DataConnectionParametersBase IDataSourceWizardConnectionStringsProvider.GetDataConnectionParameters(string name)
        {
            return null;
        }
    }
}
