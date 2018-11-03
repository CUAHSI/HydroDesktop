using System;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.Search.Settings.UI
{
    public partial class KeywordsDialog : Form
    {
        #region Constructors

        private KeywordsDialog(KeywordsSettings settings)
        {
            InitializeComponent();
            
            Load += delegate { keywordsUserControl1.BindKeywordsSettings(settings); };
        }

        #endregion

        #region Public methods

        public static DialogResult ShowDialog(KeywordsSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            using(var form = new KeywordsDialog(settings.Copy()))
            {
                if (SearchSettings.AndSearch == false)
                {
                    form.keywordsUserControl1.radioButton1.Checked = true;
                }
                else if (SearchSettings.AndSearch == true)
                {
                    form.keywordsUserControl1.radioButton2.Checked = true;
                }

                if (form.ShowDialog() == DialogResult.OK)
                {
                    settings.SelectedKeywords = form.keywordsUserControl1.GetSelectedKeywords();

                    if (form.keywordsUserControl1.radioButton1.Checked == true)
                    {
                        SearchSettings.AndSearch = false;
                    }
                    else if (form.keywordsUserControl1.radioButton2.Checked == true)
                    {
                        SearchSettings.AndSearch = true;
                    }
                }

                return form.DialogResult;
            }
        }

        #endregion
    }
}
