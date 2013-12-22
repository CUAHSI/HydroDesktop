using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web.Script.Serialization;

namespace HydroShare
{
    public partial class downloadForm : Form
    {
        string base_url = "http://dev.hydroshare.org/export.php?file=http://dev.hydroshare.org/sites/default/files/{0}.zip";
        string list_url = "http://dev.hydroshare.org/?q=my_services/node.json&api-key=581d46dd";

        public downloadForm()
        {
            InitializeComponent();
        }

        public List<string> retrieveList(string filter = "")
        {
            var client = new RestClient(list_url);
            var json = client.MakeRequest();

            var serializer = new JavaScriptSerializer();
            var all_files = serializer.Deserialize<List<Dictionary<string,string>>>(json);
            List<string> filtered_files = new List<string>{};

            if (filter == "" || filter == "All")
            {
                foreach (Dictionary<string, string> file in all_files)
                {
                        filtered_files.Add(file["title"]);
                }
            }
            else
            {
                foreach (Dictionary<string, string> file in all_files)
                {
                    if (filter == file["type"])
                    {
                        filtered_files.Add(file["title"]);
                    }
                }
            }
            return filtered_files;
            }

        public List<string> populateFilterSearch()
        {
            List<string> filterOptions = new List<string> { "All" };
            //List<string> usableResourceTypes = new List<string> { "hydroshare_geoanalytics", "hydroshare_time_series", "Time Series", "Geonanalytics" };

            var client = new RestClient(list_url);
            var json = client.MakeRequest();

            var serializer = new JavaScriptSerializer();
            var all_files = serializer.Deserialize<List<Dictionary<string, string>>>(json);

            foreach (Dictionary<string, string> file in all_files)
            {
                if (filterOptions.Contains(file["type"]) == false/* && usableResourceTypes.Contains(file["type"])*/)
                {
                    filterOptions.Add(file["type"]);
                }
            }

            return filterOptions;
        }

        private void downloadForm_Load(object sender, EventArgs e)
        {
            foreach (string s in retrieveList())
            {
                lst_AvailableItems.Items.Add(s);
            }

            foreach (string s in populateFilterSearch())
            {
                cmb_FilterSearch.Items.Add(s);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lst_AvailableItems_Click(object sender, EventArgs e)
        {

        }

        private void cmb_FilterSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lst_AvailableItems.Items.Clear();
            foreach (string s in retrieveList(cmb_FilterSearch.SelectedItem.ToString()))
            {
                lst_AvailableItems.Items.Add(s);
            }

        }
    }
}
