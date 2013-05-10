using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Search3.Settings;
using System.Diagnostics;

namespace ShaleDataNetwork.Settings.UI
{
    public partial class ShaleDataDialog : Form
    {
        #region Constructors

        public ShaleDataDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        public static DialogResult ShowDialog(KeywordsSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
               
            using (var form = new ShaleDataDialog())
            {
                var res = new List<string>();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (Control c in form.Controls)
                    {
                        if (c is CheckBox)
                        {
                            if (((CheckBox)c).Checked == true)
                            {
                                res.Add(c.Text);
                            }
                        }
                    }         
                    settings.SelectedKeywords = res;
                }

                return form.DialogResult;
            }
        }

        #endregion
    }
}
