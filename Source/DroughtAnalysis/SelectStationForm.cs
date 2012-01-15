using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace DroughtAnalysis
{
    public partial class SelectStationForm : Form
    {
        public SelectStationForm()
        {
            InitializeComponent();
        }

        public SelectStationForm(DroughtSettings settings)
        {
            InitializeComponent();
            Settings = settings;
        }

        public DroughtSettings Settings { get; set; }

        private string defaultOutputFolder;

        private void SelectStationForm_Load(object sender, EventArgs e)
        {
            bindingSource1.DataSource = Settings.SuitableSites;

            listBoxStations.SelectedIndex = -1;

            TryGuessPathToR();

            TryCreateDefaultOutputFolder();
            defaultOutputFolder = Settings.OutputDirectory;
        }

        private void TryGuessPathToR()
        {
            try
            {
                string programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                if (programFilesDir.EndsWith(" (x86)"))
                {
                    programFilesDir = programFilesDir.Remove(programFilesDir.IndexOf(" (x86)"));
                }
                string baseRPath1 = "R\\R-{0}\\bin\\i386\\R.exe";
                string[] possibleRVersionsNew = new string[] { "2.14.0", "2.13.2", "2.13.1", "2.13.0", "2.12.2", "2.12.1", "2.12.0", };
                string baseRPathOld = "R\\R-{0}\\bin\\R.exe";
                string[] possibleRVersionsOld = new string[] { "2.11.1", "2.11.0", "2.10.1", "2.10.0" };
                foreach (string rVersion in possibleRVersionsNew)
                {
                    string rPath = Path.Combine(programFilesDir, string.Format(baseRPath1, rVersion));
                    if (File.Exists(rPath))
                    {
                        Settings.PathToR = rPath;
                        txtPathToR.Text = rPath;
                        return;
                    }
                }
                foreach (string rVersion in possibleRVersionsOld)
                {
                    string rPath = Path.Combine(programFilesDir, string.Format(baseRPathOld, rVersion));
                    if (File.Exists(rPath))
                    {
                        Settings.PathToR = rPath;
                        txtPathToR.Text = rPath;
                        return;
                    }
                }

            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
        }

        private void TryCreateDefaultOutputFolder()
        {
            try
            {
                string documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string hydroDocsDir = Path.Combine(documentsDir, "HydroDesktop");
                if (!Directory.Exists(hydroDocsDir))
                {
                    Directory.CreateDirectory(hydroDocsDir);
                }
                if (Directory.Exists(hydroDocsDir))
                {
                    string droughtDir = Path.Combine(hydroDocsDir, "Drought");
                    if (!Directory.Exists(droughtDir))
                    {
                        Directory.CreateDirectory(droughtDir);
                    }

                    if (Directory.Exists(droughtDir))
                    {
                        Settings.OutputDirectory = droughtDir;
                        txtOutputFolder.Text = Settings.OutputDirectory;
                        defaultOutputFolder = droughtDir;
                    }
                }

                

            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
            catch (IOException)
            {
                return;
            }

        }


        private void btnPathToR_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog fd = new OpenFileDialog();
            string initialDir = @"C:\Program Files\R";
            if (Directory.Exists(initialDir))
            {
                fd.InitialDirectory = initialDir;
            }
            fd.Filter = "R.exe file|*.exe";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                Settings.PathToR = fd.FileName;
                txtPathToR.Text = Settings.PathToR;
            }
        }

        private void TextBoxR_TextChanged(object sender, EventArgs e)
        {
            if (txtPathToR.Text.ToLower().EndsWith("r.exe"))
            {
                Settings.PathToR = txtPathToR.Text;
            }
        }

        private void btnOutputFolder_Click(object sender, EventArgs e)
        {
           FolderBrowserDialog fd = new FolderBrowserDialog();
           if (Settings.OutputDirectory != null)
               if (Directory.Exists(Settings.OutputDirectory))
                   fd.SelectedPath = Settings.OutputDirectory;

            if (fd.ShowDialog() == DialogResult.OK)
            {
                fd.ShowNewFolderButton = true;
                Settings.OutputDirectory = fd.SelectedPath;
                txtOutputFolder.Text = Settings.OutputDirectory;
            }
        }

        private void TextBoxOutputFolder_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txtOutputFolder.Text))
            {
                Settings.OutputDirectory = txtOutputFolder.Text;
            }
        }

        public static string RemoveDiacritism(string Text)
        {
            string stringFormD = Text.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder retVal = new System.Text.StringBuilder();
            for (int index = 0; index < stringFormD.Length; index++)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stringFormD[index]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    retVal.Append(stringFormD[index]);
            }
            return retVal.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        //analyze drought button
        private void button3_Click(object sender, EventArgs e)
        {
            
            
            if (Settings.SelectedSite == null)
            {
                MessageBox.Show("Please select a station.");
                return;
            }
            if (!Directory.Exists(Settings.OutputDirectory))
            {
                MessageBox.Show("Please specify a valid output directory.");
                return;
            }

            //try creating output directory
            Settings.OutputDirectory = txtOutputFolder.Text;
            if (checkBoxNewDirectory.Checked)
            {
                try
                {
                    string stationNameEnglish = RScriptModifier.RemoveDiacritism(Settings.SelectedSite.Name);
                    string newOutDir = Path.Combine(Settings.OutputDirectory, stationNameEnglish);
                    if (!Directory.Exists(newOutDir))
                    {
                        Directory.CreateDirectory(newOutDir);
                        Settings.OutputDirectory = newOutDir;
                    }
                }
                catch { }
            }

            //format the site name
            string siteName1 = Settings.SelectedSite.Name;
            string siteName = RScriptModifier.RemoveDiacritism(siteName1.ToLower().Replace(" ", "_"));

            string dataFile = Path.Combine(Settings.OutputDirectory, string.Format("{0}_data.dat",siteName));

            //process the selected site
            Cursor = Cursors.WaitCursor;
            lblProgress.Text = "Creating drought data file..";
            Application.DoEvents();

            DataExporter exp = new DataExporter();
            exp.ExportDataForStation(Settings.SelectedSite, dataFile);

            if (!File.Exists(dataFile))
            {
                MessageBox.Show("Error creating drought data file.");
                Cursor = Cursors.Default;
                return;
            }

            lblProgress.Text = "Passing parameters to R script..";
            Application.DoEvents();

            //pass the parameters to the R-script by modifying the script
            string rScriptTemplatePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "msr-vysl_11.r");
            string newScriptFullPath = Path.Combine(Settings.OutputDirectory, String.Format("{0}_R_script", siteName));
            RScriptParameterInfo param = new RScriptParameterInfo();
            param.InputFilePath = dataFile;
            param.OutputDirectory = Settings.OutputDirectory;
            param.StationName = siteName;

            try
            {
                RScriptModifier.ModifyRScript(rScriptTemplatePath, newScriptFullPath, param);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error passing parameters to R script. " + ex.Message);
                return;
            }

            //RUN the R-SCRIPT
            lblProgress.Text = "Executing R-script '" + newScriptFullPath + "'";
            Application.DoEvents();
            string rScriptExe = Path.Combine(Path.GetDirectoryName(Settings.PathToR), "RScript.exe");
            ProcessStartInfo rScriptInfo = new ProcessStartInfo(rScriptExe);
            
            if (!File.Exists(newScriptFullPath))
            {
                MessageBox.Show("The R script file '" + newScriptFullPath + "' was not found. Cannot calculate drought analysis.");
                return;
            }
            //the only argument passed to RSCRIPT.exe is the script file name.
            rScriptInfo.Arguments = String.Format(@"""{0}""", newScriptFullPath);

            try
            {
                using (Process p = Process.Start(rScriptInfo))
                {
                    p.WaitForExit();
                }
                Cursor = Cursors.Default;
                lblProgress.Text = "Operation Completed!";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error running R-script." + ex.Message);
                return;
            }

            MessageBox.Show("Created Report files in directory: " + Settings.OutputDirectory);
            btnViewResults.Enabled = true;
        }

        private void Stations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxStations.SelectedIndex < 0) Settings.SelectedSite = null;
            Settings.SelectedSite = (Site)listBoxStations.SelectedItem;

            

            //if (String.IsNullOrEmpty(defaultOutputFolder))
            //{
            //    defaultOutputFolder = txtOutputFolder.Text;
            //}

            ////try to change path..
            //string sitePath = txtOutputFolder.Text;
            //string siteNameEnglish = RScriptModifier.RemoveDiacritism(Settings.SelectedSite.Name);
            //try
            //{
            //    sitePath = Path.Combine(defaultOutputFolder, siteNameEnglish);
            //    if (!Directory.Exists(sitePath))
            //    {
            //        Directory.CreateDirectory(sitePath);
            //    }
            //    if (Directory.Exists(sitePath))
            //    {
            //        txtOutputFolder.Text = sitePath;
            //        Settings.OutputDirectory = sitePath;
            //    }
            //}
            //catch { }
        }

        private void btnViewResults_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = Settings.OutputDirectory,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open results folder. " + ex.Message);
            }

        }
    }
}
