using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SeriesView
{
    public partial class DisplayOptionsForm : Form
    {
        private readonly DisplaySettings _settings;

        private DisplayOptionsForm(DisplaySettings settings)
        {
            _settings = settings;
            InitializeComponent();

            cmbSiteDisplayName.Items.Clear();
            cmbSiteDisplayName.Items.Add("SiteName");
            cmbSiteDisplayName.Items.Add("SiteCode");
            cmbSiteDisplayName.DataBindings.Clear();
            cmbSiteDisplayName.DataBindings.Add("SelectedItem", _settings, "SiteDisplayColumn");
        }

        public static DialogResult ShowDialog(DisplaySettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            using(var form = new DisplayOptionsForm(settings.Copy()))
            {
                var dialogResult = form.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    settings.Copy(form._settings);
                }

                return dialogResult;
            }
        }
    }

    public class DisplaySettings
    {
        public string SiteDisplayColumn { get; set; }


        /// <summary>
        /// Create deep copy of current instance.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public DisplaySettings Copy()
        {
            var result = new DisplaySettings();
            result.Copy(this);
            return result;

        }

        /// <summary>
        /// Create deep from source into current instance.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>must be not null.</exception>
        public void Copy(DisplaySettings source)
        {
            if (source == null) throw new ArgumentNullException("source");

            SiteDisplayColumn = source.SiteDisplayColumn;
        }
    }
}
