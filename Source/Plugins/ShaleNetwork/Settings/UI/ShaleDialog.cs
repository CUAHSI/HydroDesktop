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

namespace ShaleNetwork.Settings.UI
{
    public partial class ShaleDialog : Form
    {
        #region Constructors

        public ShaleDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        public static DialogResult ShowDialog(KeywordsSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
               
            using (var form = new ShaleDialog())
            {
                var res = new List<string>();

                if (SearchSettings.AndSearch == false)
                {
                    form.radioButton1.Checked = true;
                }
                else if (SearchSettings.AndSearch == true)
                {
                    form.radioButton2.Checked = true;
                }

                foreach (Control c in form.groupBox1.Controls)
                {
                    if (c is CheckBox)
                    {
                 
                        if (settings.SelectedKeywords.Contains(c.Text))
                        {
                            ((CheckBox)c).Checked = true;
                        }
                        else
                        {
                            ((CheckBox)c).Checked = false;
                        }


                        if (c.Text == "Discharge")
                        {
                            if (settings.SelectedKeywords.Contains("Discharge, stream"))
                            {
                                ((CheckBox)c).Checked = true;
                            }
                        }
                        if (c.Text == "Total Dissolved Solids")
                        {
                            if (settings.SelectedKeywords.Contains("Solids, total dissolved"))
                            {
                                ((CheckBox)c).Checked = true;
                            }
                        }
                        if (c.Text == "Total Suspended Solids")
                        {
                            if (settings.SelectedKeywords.Contains("Solids, total suspended"))
                            {
                                ((CheckBox)c).Checked = true;
                            }
                        }
                    }
                }

                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (Control c in form.groupBox1.Controls)
                    {
                        if (c is CheckBox)
                        {
                            if (((CheckBox)c).Checked == true)
                            {
                                if (c.Text == "Discharge")
                                {
                                    res.Add("Discharge, stream");
                                }
                                else
                                {
                                    res.Add(c.Text);
                                }
                            }
                        }
                    }        
 
                    settings.SelectedKeywords = res;

                    if (form.radioButton1.Checked == true)
                    {
                        SearchSettings.AndSearch = false;
                    }
                    else if (form.radioButton2.Checked == true)
                    {
                        SearchSettings.AndSearch = true;
                    }

                }

                return form.DialogResult;
            }
        }

        #endregion

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ShaleDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
