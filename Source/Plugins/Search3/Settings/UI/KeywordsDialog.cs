using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Search3.Settings.UI
{
    public partial class KeywordsDialog : Form
    {
        #region Constructors

        private KeywordsDialog(KeywordsSettings settings)
        {
            InitializeComponent();
            Load += delegate
                        {
                            keywordsUserControl1.SetData(settings.Keywords, settings.OntologyTree);
                            keywordsUserControl1.AddSelectedKeywords(settings.SelectedKeywords);
                        };
        }

        #endregion

        #region Public methods

        public static void ShowDialog(KeywordsSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            using(var form = new KeywordsDialog(settings.Copy()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    settings.SelectedKeywords = form.keywordsUserControl1.GetSelectedKeywords();
                }
            }
        }

        #endregion
    }
}
