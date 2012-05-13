using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace TableView
{
    public partial class DeleteThemeForm : Form
    {
        private readonly Dictionary<string, Theme> _themeLookup = new Dictionary<string, Theme>();
        
        public DeleteThemeForm()
        {
            InitializeComponent();

            FormClosing +=DeleteThemeForm_FormClosing;
            //checkListThemes.ItemCheck += new ItemCheckEventHandler(checkListThemes_ItemCheck);
            bgwMain.DoWork +=bgwMain_DoWork;
            bgwMain.RunWorkerCompleted +=bgwMain_RunWorkerCompleted;
            bgwMain.ProgressChanged +=bgwMain_ProgressChanged;
        }

        private void checkListThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = checkListThemes.CheckedItems.Count > 0;
        }

        private void DeleteThemeForm_Load(object sender, EventArgs e)
        {
            var repoManager = RepositoryFactory.Instance.Get<IDataThemesRepository>();
            var themeList = repoManager.GetAll();

            foreach (var theme in themeList)
            {
                _themeLookup.Add(theme.Name, theme);
                checkListThemes.Items.Add(theme.Name);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Make sure we aren't still working on a previous task
            if (bgwMain.IsBusy)
            {
                MessageBox.Show("The background worker is busy now, please try later.");
                return;
            }
            
            int numCheckedThemes = checkListThemes.CheckedItems.Count;

            
            DialogResult reply = MessageBox.Show("Are you sure to delete " + numCheckedThemes +
                " theme(s) with all sites, variables, time series and data values? ","Delete Theme",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (reply == DialogResult.Yes)
            {
                gbxDelete.Visible = false;
                gbxProgress.Visible = true;

                //get the list of checked themes to delete
                var themeIDList = new int[numCheckedThemes];
                int index = 0;
                foreach (object checkedItem in checkListThemes.CheckedItems)
                {  
                    string name = checkedItem.ToString();
                    int id = Convert.ToInt32(_themeLookup[name].Id);
                    themeIDList[index] = id;
                    index++;
                }

                var manager = RepositoryFactory.Instance.Get<IDataThemesRepository>();
                var parameters = new object[2];
                parameters[0] = themeIDList;
                parameters[1] = manager;
                
                //launch the background worker..
                bgwMain.RunWorkerAsync(parameters);
            }
        }

        /// <summary>
        /// When Export Form is closed, BackgroundWorker has to stop.
        /// </summary>
        private void DeleteThemeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwMain.IsBusy)
            {
                Cancel_worker();
                //_formIsClosing = true;
                e.Cancel = true;
            }
        }

        #region BackgroundWorker

        

        /// <summary>
        /// Call "Cancel_worker" when button click happens.
        /// </summary>
        private void btnPgsCancel_Click_1(object sender, EventArgs e)
        {
            Cancel_worker();
        }

        /// <summary>
        /// When "Cancel" button is clicked during the exporting process, BackgroundWorker stops.
        /// </summary>
        private void Cancel_worker()
        {
            bgwMain.CancelAsync();
            gbxProgress.Text = "Cancelling...";
            btnPgsCancel.Enabled = false;
        }

        /// <summary>
        /// BackgroundWorker Do event, used to call for the BackgroundWorker method.
        /// </summary>
        private void bgwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            var parameters = (object[])e.Argument;
            var worker = sender as BackgroundWorker;

            var themeIdList = (int[])parameters[0];
            var manager = (IDataThemesRepository)parameters[1];

            foreach (int themeId in themeIdList)
            {
                manager.DeleteTheme(themeId, worker, e);
            }
        }

        /// <summary>
        /// BackgroundWorker Progress event, used to report the progress when doing BackgroundWorker.
        /// </summary>
        private void bgwMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgsBar.Value = e.ProgressPercentage;
            gbxProgress.Text = e.UserState.ToString();
        }

        /// <summary>
        /// Enable all the buttons again when BackgroundWorker complete working.
        /// </summary>
        private void bgwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;

            // Restore controls to their regular state
            
            pgsBar.Value = 0;
            gbxProgress.Text = "Processing...";
            gbxProgress.Enabled = false;
            gbxProgress.Visible = false;
            btnPgsCancel.Enabled = true;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

            else if (e.Cancelled || e.Result.ToString() == "Data Export Cancelled.")
            {
                MessageBox.Show("Operation was cancelled.");
                DialogResult = DialogResult.OK;
                Close();
                
                // Close the form if the user clicked the X to close it.

                //if (_formIsClosing == true)
                //{
                //    this.DialogResult = DialogResult.Cancel;
                //}
            }
            else
            {
                MessageBox.Show(e.Result.ToString());
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        #endregion

        
    }
}
