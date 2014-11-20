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
using System.IO;
using Ionic.Zlib;

namespace HydroDesktop.Plugins.HydroShare
{
    public partial class downloadForm : Form
    {
        string base_url = "http://dev.hydroshare.org/export.php?file=http://dev.hydroshare.org/sites/default/files/";
        List<string> list_urls = new List<string> { "http://dev.hydroshare.org/?q=my_services/node.json&api-key=581d46dd", "http://dev.hydroshare.org/?q=my_services/node.json&api-key=581d46dd&page=1", "http://dev.hydroshare.org/?q=my_services/node.json&api-key=581d46dd&page=2"};

        public downloadForm()
        {
            InitializeComponent();
        }

        public List<string> retrieveList(string filter = "")
        {
            List<string> filtered_files = new List<string>{};
            foreach (string list_url in list_urls)
            {
            var client = new RestClient(list_url);
            var json = client.MakeRequest();

            var serializer = new JavaScriptSerializer();
            var all_files = serializer.Deserialize<List<Dictionary<string,string>>>(json);

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
            }
            filtered_files.Sort();
            return filtered_files;
            }

        public List<string> populateFilterSearch()
        {
            List<string> filterOptions = new List<string> { "All" };
            foreach (string list_url in list_urls)
            {               
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
            }
            filterOptions.Sort();
            return filterOptions;
        }

        public void downloadFile(string resourceName)
        {
            resourceName = resourceName.Replace(" ", "");
            string save_loc = Application.StartupPath + resourceName + ".zip";
            string path = Path.GetTempFileName();

            WebClient fileReader = new WebClient();
            fileReader.DownloadFile(base_url + resourceName + ".zip", save_loc);

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

        private void btn_Download_Click(object sender, EventArgs e)
        {
            downloadFile(lst_AvailableItems.SelectedItem.ToString());
            this.Close();

        }
    }
}
