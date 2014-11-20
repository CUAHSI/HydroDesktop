using System;
using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.TableView
{
    /// <summary>
    /// Form for deleting themes from database
    /// </summary>
    public partial class DeleteThemeForm : Form
    {
        /// <summary>
        /// Creates new instance of <see cref="DeleteThemeForm"/>
        /// </summary>
        public DeleteThemeForm()
        {
            InitializeComponent();

            FormClosing += DeleteThemeForm_FormClosing;
            bgwMain.DoWork += bgwMain_DoWork;
            bgwMain.RunWorkerCompleted += bgwMain_RunWorkerCompleted;
            bgwMain.ProgressChanged += bgwMain_ProgressChanged;
        }

        private void checkListThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = checkListThemes.CheckedItems.Count > 0;
        }

        private void DeleteThemeForm_Load(object sender, EventArgs e)
        {
            var repoManager = RepositoryFactory.Instance.Get<IDataThemesRepository>();
            var themeList = repoManager.GetAll();
            checkListThemes.DataSource = themeList;
            checkListThemes.DisplayMember = "Name";
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
            
            var numCheckedThemes = checkListThemes.CheckedItems.Count;
            var reply = MessageBox.Show("Are you sure to remove " + numCheckedThemes +
                " theme(s) with all sites, variables, time series and data values? ","Remove Theme",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reply != DialogResult.Yes) return;

            gbxDelete.Visible = false;
            gbxProgress.Visible = true;

            //get the list of checked themes to delete
            var themeIDList = new long[numCheckedThemes];
            for (var i = 0; i < checkListThemes.CheckedItems.Count; i++)
            {
                themeIDList[i] = ((Theme) checkListThemes.CheckedItems[i]).Id;
            }

            //launch the background worker..
            bgwMain.RunWorkerAsync(themeIDList);
        }

        /// <summary>
        /// When Export Form is closed, BackgroundWorker has to stop.
        /// </summary>
        private void DeleteThemeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!bgwMain.IsBusy) return;
            Cancel_worker();
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
        private static void bgwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var themeIdList = (long[])e.Argument;
            var manager = RepositoryFactory.Instance.Get<IDataThemesRepository>();
            foreach (var themeId in themeIdList)
            {
                if (manager.DeleteTheme(themeId, worker))
                {
                    e.Result = "Theme deleted successfully.";
                }
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
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Operation was cancelled.", "Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(e.Result.ToString(), "Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        #endregion
    }
}
