using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.WebServices.WaterOneFlow;
using HydroDesktop.Interfaces.ObjectModel;
using System.Net;
using System.IO;

namespace Search3.Settings.UI
{
    public partial class AddSitesLayerDialog : Form
    {
        public AddSitesLayerDialog()
        {
            InitializeComponent();
        }

        WaterOneFlowClient waterOneFlowClient;

        private void button1_Click(object sender, EventArgs e)
        {
           waterOneFlowClient = new WaterOneFlowClient(urlConnectionTextbox.Text);
    
           // Update service info in the metadata database
           var waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;
           // Get all sites for this service
           IList<Site> siteList;
           siteList = waterOneFlowClient.GetSites();
           var variableList = new List<Tuple<string, string>>();
           foreach (var site in siteList)
           {
               // Get series for this site
               IList<SeriesMetadata> currentSeriesList;
               try
               {
                   currentSeriesList = waterOneFlowClient.GetSiteInfo(site.Code);
               }
               catch (WebException ex)
               {
                   if (ex.Response != null)
                   {
                       var rdr = new StreamReader(ex.Response.GetResponseStream());
                       rdr.ReadToEnd();
                   }

                   continue;
               }
               // Save series info to metadata cache database
               foreach (var series in currentSeriesList)
               {
                   var variable = new Tuple<string, string>(series.Variable.Code, series.Variable.Name);
                   variableList.Add(variable);
               }
           }
           foreach (var variable in variableList)
           {
               this.variablesListBox.Items.Add(variable.Item2, false);
               
           }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
   
    }
}
