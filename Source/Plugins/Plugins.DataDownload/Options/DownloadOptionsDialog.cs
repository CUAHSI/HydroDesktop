using System;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;

namespace HydroDesktop.Plugins.DataDownload.Options
{
    public partial class DownloadOptionsDialog : Form
    {
        private readonly DownloadOptions _options;

        public DownloadOptionsDialog(DownloadOptions options)
        {
            _options = options;
            InitializeComponent();

            nudNumberOfValues.AddBinding(d => d.Value, options, d => d.NumberOfValuesPerRequest);
            chbUseSingleThread.AddBinding(d => d.Checked, options, d => d.UseSingleThread);
            chbGetAllValuesInOneRequest.AddBinding(d => d.Checked, options, d => d.GetAllValuesInOneRequest);
            checkBox1_CheckedChanged(this, EventArgs.Empty);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            nudNumberOfValues.Enabled = !chbGetAllValuesInOneRequest.Checked;
        }  
    }

    public class DownloadOptions
    {
        public DownloadOptions()
        {
            
        }

        public DownloadOptions(DownloadOptions source)
        {
            NumberOfValuesPerRequest = source.NumberOfValuesPerRequest;
            UseSingleThread = source.UseSingleThread;
            GetAllValuesInOneRequest = source.GetAllValuesInOneRequest;
        }
        
// ReSharper disable MemberCanBePrivate.Global
        public int NumberOfValuesPerRequest { get; set; }
        public bool UseSingleThread { get; set; }
        public bool GetAllValuesInOneRequest { get; set; }
// ReSharper restore MemberCanBePrivate.Global
    }
}
