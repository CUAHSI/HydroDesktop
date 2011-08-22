using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Interfaces;

namespace TableView
{
    public partial class DeleteThemeForm : Form
    {
        private DbOperations _db = null;
        //private bool _formIsClosing = false;

        //used as a lookup - which themes are deleted
        private Dictionary<string, Theme> _themeLookup = new Dictionary<string, Theme>();
        
        public DeleteThemeForm(IHydroDbOperations db)
        {
            InitializeComponent();

            _db = db as DbOperations;

            this.FormClosing +=new FormClosingEventHandler(DeleteThemeForm_FormClosing);
            //checkListThemes.ItemCheck += new ItemCheckEventHandler(checkListThemes_ItemCheck);
            bgwMain.DoWork +=new DoWorkEventHandler(bgwMain_DoWork);
            bgwMain.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bgwMain_RunWorkerCompleted);
            bgwMain.ProgressChanged +=new ProgressChangedEventHandler(bgwMain_ProgressChanged);
        }

        //void checkListThemes_ItemCheck(object sender, ItemCheckEventArgs e)
        //{
        //    if (checkListThemes.CheckedItems.Count > 0)
        //    {
        //        btnOK.Enabled = true;
        //    }
        //    else
        //    {
        //        btnOK.Enabled = false;
        //    }
        //}

        private void checkListThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkListThemes.CheckedItems.Count > 0)
            {
                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        private void DeleteThemeForm_Load(object sender, EventArgs e)
        {
            //_db = new DbOperations(Config.DataRepositoryConnectionString, DatabaseTypes.SQLite);


            RepositoryManagerSQL repoManager = new RepositoryManagerSQL(_db);
            IList<Theme> themeList = repoManager.GetAllThemes();

            foreach (Theme theme in themeList)
            {
                _themeLookup.Add(theme.Name, theme);
                checkListThemes.Items.Add(theme.Name);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Make sure we aren't still working on a previous task
            if (bgwMain.IsBusy == true)
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
                int[] themeIDList = new int[numCheckedThemes];
                int index = 0;
                foreach (object checkedItem in checkListThemes.CheckedItems)
                {  
                    string name = checkedItem.ToString();
                    int id = Convert.ToInt32(_themeLookup[name].Id);
                    themeIDList[index] = id;
                    index++;
                }

                RepositoryManagerSQL manager = new RepositoryManagerSQL(_db);
                object[] parameters = new object[2];
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
                return;
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
            object[] parameters = e.Argument as object[];
            BackgroundWorker worker = sender as BackgroundWorker;

            int[] themeIdList = parameters[0] as int[];
            //int themeId = Convert.ToInt32(parameters[0]);
            RepositoryManagerSQL manager = parameters[1] as RepositoryManagerSQL;

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
            this.pgsBar.Value = e.ProgressPercentage;
            this.gbxProgress.Text = e.UserState.ToString();
        }

        /// <summary>
        /// Enable all the buttons again when BackgroundWorker complete working.
        /// </summary>
        private void bgwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;

            // Restore controls to their regular state
            
            pgsBar.Value = 0;
            gbxProgress.Text = "Processing...";
            gbxProgress.Enabled = false;
            gbxProgress.Visible = false;
            this.btnPgsCancel.Enabled = true;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

            else if (e.Cancelled == true || e.Result.ToString() == "Data Export Cancelled.")
            {
                MessageBox.Show("Operation was cancelled.");
                this.DialogResult = DialogResult.OK;
                this.Close();
                
                // Close the form if the user clicked the X to close it.

                //if (_formIsClosing == true)
                //{
                //    this.DialogResult = DialogResult.Cancel;
                //}
            }
            else
            {
                MessageBox.Show(e.Result.ToString());
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #endregion

        
    }
}
