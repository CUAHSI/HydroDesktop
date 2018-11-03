using System;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.SeriesView
{
    public partial class DisplayOptionsForm : Form
    {
        private readonly DisplaySettings _settings;

        private DisplayOptionsForm(DisplaySettings settings)
        {
            _settings = settings;
            InitializeComponent();
            
            cmbSiteDisplayName.DataSource = Enum.GetNames(typeof (SiteDisplayColumns));
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

    public enum SiteDisplayColumns
    {
        SiteName,
        SiteCode,
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
