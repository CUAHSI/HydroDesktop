using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

using HydroDesktop.Database;

namespace HydroDesktop.ImportExport
{
    /// <summary>
    /// Pass users' options to the clients to export data.
    /// </summary>
    public partial class GetExportOptionsDialog : Form
    {
        #region Variables

        // These variables are read by the client after closing the dialog
        public List<string> FieldsToExport { get; set; }
        public string Delimiter { get; set; }
        public string OutputFilename { get; set; }

        private DataTable _dataToExport;
        SaveFileDialog _saveFileDlg = new SaveFileDialog();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the GetExportOptionsDialog form, and pass the users' datatable to the form
        /// </summary>
        /// <param name="dataToExport"> DataTable used to pass data from the users' datatable.</param>
        public GetExportOptionsDialog(DataTable dataToExport)
        {
            if (dataToExport.Columns.Count == 0)
            {
                throw new ArgumentException("Data table for export must have at least one column.");
            }
            else if (dataToExport.Rows.Count == 0)
            {
                throw new ArgumentException("Data table for export must have at least one row.");
            }
            
            InitializeComponent();
            _dataToExport = dataToExport;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize the ChecklistBox to show all the fields that were passed in from the client's DataTable.
        /// </summary>
        private void ListBox_load(object sender, EventArgs e)
        {
            //Headers shown in CheckListBox
            string[] headers = new string[_dataToExport.Columns.Count];
            for (int i = 0; i < _dataToExport.Columns.Count; i++)
            {
                headers[i] = _dataToExport.Columns[i].ToString();
            }

            //Initialize items in CheckedlistBox
            clbFieldsToExport.Items.AddRange(headers);
            for (int h = 0; h < clbFieldsToExport.Items.Count; h++)
            {
                clbFieldsToExport.SetItemChecked(h, true);
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Specify the location and file name to export.
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            _saveFileDlg.Title = "Select file";
			_saveFileDlg.OverwritePrompt = false;

			if ( rdoComma.Checked == true )
			{
				_saveFileDlg.Filter = "CSV (Comma delimited) (*.csv)|*.csv|Text (*.txt)|*.txt";
			}
			else
			{
				_saveFileDlg.Filter = "Text (*.txt)|*.txt";
			}

            if (_saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                tbOutputFilename.Text = _saveFileDlg.FileName;
            }
        }

        /// <summary>
        /// Validate export options and return the result.
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            //Check the desired fields for export.  There should be at least one item selected.
            List<string> fields = new List<string>();

            if (clbFieldsToExport.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please choose at least one field to export", "Choose Export Options");
                return;
            }
            else
            {
                foreach (string item in clbFieldsToExport.CheckedItems)
                {
                    fields.Add(item);
                }

                FieldsToExport = fields;
            }

            // Get the delimiter chosen by the user
            if (rdoComma.Checked) Delimiter = ",";
            if (rdoTab.Checked) Delimiter = "\t";
            if (rdoSpace.Checked) Delimiter = "\0";
            if (rdoPipe.Checked) Delimiter = "|";
            if (rdoSemicolon.Checked) Delimiter = ";";

            if (rdoOthers.Checked == true)
            {
                if (tbOther.Text.Length != 0)
                {
                    Delimiter = tbOther.Text.ToString();
                }
                else
                {
                    MessageBox.Show("Please input delimiter", "Choose Export Options");
                    return;
                }
            }

            // Check the output file path
            string outputFilename = tbOutputFilename.Text.Trim();
            if (outputFilename == String.Empty)
            {
                MessageBox.Show("Please specify output filename", "Choose Export Options");
                return;
            }
            else if (Directory.Exists(Path.GetDirectoryName(outputFilename)) == false)
            {
                MessageBox.Show("The directory for the output filename does not exist", "Choose Export Options");
                return;
            }

			// Check for overwrite
			if ( File.Exists ( outputFilename ) == true )
			{
				string message = "File " + outputFilename + " already exists.\nDo you want to replace it?";
				if ( MessageBox.Show ( message, "Choose Export Options", MessageBoxButtons.YesNo ) == DialogResult.No )
				{
					return;
				}
			}

            OutputFilename = outputFilename;

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Close the form when "Cancel" button is clicked.
        /// </summary>
        private void btncancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #region CheckListBox Selection Events

        /// <summary>
        /// Checks all items in the list of fields to export.
        /// </summary>
        private void SelectAll_Click(object sender, EventArgs e)
        {
            for (int c = 0; c < clbFieldsToExport.Items.Count; c++)
            {
                clbFieldsToExport.SetItemChecked(c, true);
            }
        }

        /// <summary>
        /// Unchecks all items in the list of fields to export.
        /// </summary>
        private void SelectNone_Click(object sender, EventArgs e)
        {
            for (int c = 0; c < clbFieldsToExport.Items.Count; c++)
            {
                clbFieldsToExport.SetItemChecked(c, false);
            }
        }

        #endregion

        /// <summary>
        /// Set the "Others" radiobutton be selected automatically when the textbox is changed.
        /// </summary>        
        private void other_TextChanged(object sender, EventArgs e)
        {
            if (tbOther.Text.Length != 0)
            {
                rdoOthers.Checked = true;
            }
        }

        #endregion
    }
}

