using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Common;
using HydroShare.Properties;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using HydroShare.Forms;

namespace HydroShare
{
    public class HydroSharePlugin : Extension
    {
        #region Fields

        //private SimpleActionItem test1;
        //private SimpleActionItem test2;
        private SimpleActionItem hydroShareDownload;
        private SimpleActionItem hydroShareUpload;
        private SimpleActionItem csHydroShareDownload;
        private SimpleActionItem csHydroShareUpload;
        private SimpleActionItem addUser;
        private SimpleActionItem editUser;

        private readonly string _hydroShareKey = SharedConstants.HydroShareRootkey;

        #endregion

        #region Plugin operations

        public override void Activate()
        {
           AddHydroShareRibbon();
           base.Activate();

           //App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;   
        }

        public override void Deactivate()
        {
       
           //App.HeaderControl.RootItemSelected -= HeaderControl_RootItemSelected;
         

           App.HeaderControl.RemoveAll();
           base.Deactivate();
        }

        #endregion

        private void AddHydroShareRibbon()
        {
            var head = App.HeaderControl;
            head.Add(new RootItem(_hydroShareKey, Resources.HydroShare) { SortOrder = 180 });

            hydroShareDownload = new SimpleActionItem(_hydroShareKey, "Download", hydroShareDownload_Click) { LargeImage = Resources.Download_32x32, GroupCaption = "Python", Visible = true };
            hydroShareUpload = new SimpleActionItem(_hydroShareKey, "Upload", hydroShareUpload_Click) { LargeImage = Resources.Upload_32x32, GroupCaption = "Python", Visible = true };
            csHydroShareDownload = new SimpleActionItem(_hydroShareKey, "Download", csHydroShareDownload_Click) { LargeImage = Resources.Download_32x32, GroupCaption = "C#", Visible = true };
            csHydroShareUpload = new SimpleActionItem(_hydroShareKey, "Upload", csHydroShareUpload_Click) { LargeImage = Resources.Upload_32x32, GroupCaption = "C#", Visible = true };
            addUser = new SimpleActionItem(_hydroShareKey, "Add account", addUser_Click) { LargeImage = Resources.addUser_32x32, GroupCaption = "HydroShare account", Visible = true };
            editUser = new SimpleActionItem(_hydroShareKey, "Edit account", editUser_Click) { LargeImage = Resources.editUser_32x32, GroupCaption = "HydroShare account", Visible = true };


            head.Add(hydroShareDownload);
            head.Add(hydroShareUpload);
            head.Add(csHydroShareDownload);
            head.Add(csHydroShareUpload);
            head.Add(addUser);
            head.Add(editUser);
        }

        private void csHydroShareUpload_Click(object sender, EventArgs e)
        {
            logIn l = new logIn();
            l.StartPosition = FormStartPosition.CenterScreen;
            l.Visible = true;
           
            //uploadForm1 form1 = new uploadForm1();
            //form1.StartPosition = FormStartPosition.CenterScreen;
            //form1.Visible = true;
            //throw new NotImplementedException();
        }

        private void csHydroShareDownload_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void hydroShareUpload_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This button is only implemented in c#", "No Python", MessageBoxButtons.OK);
        }

        private void addUser_Click(object sender, EventArgs e)
        {
            addUserForm addUser = new addUserForm();
            addUser.StartPosition = FormStartPosition.CenterScreen;
            addUser.Visible = true;
           
        }

        private void editUser_Click(object sender, EventArgs e)
        {
            editUserForm editUser = new editUserForm();
            editUser.StartPosition = FormStartPosition.CenterScreen;
            editUser.Visible = true;
        }


        /// <summary>
        /// Should be called with all filepaths returned from the HydroShareDownload Python script.
        /// As of now, all .shp files are found inside these filepaths and opened in HydroDesktop.
        /// This will need to be expanded to support more data types.
        /// </summary>
        /// <param name="filePath">The FilePath to the downloaded data files.</param>
        private void openResult(String filePath)
        {
            //Open result is the method that gets called for each filepath returned from the Python script

            //We look for all files in all directories of the returned folder that are .shp.
            //TODO: This will need to be expanded to look for TimeSeries and other filetypes.
            String[] files = Directory.GetFiles(filePath, "*.shp", SearchOption.AllDirectories);

            //Iterate through the list of .shp files and try to open them in the map.
            foreach (String f in files)
            {
                try
                {
                    App.Map.AddLayer(f);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error opening file: " + f);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Returns the location that the downloaded data files will be saved.
        /// The CurrentProjectDirectory will be used if it is not null,
        /// otherwise we will show a "Choose Folder" dialog and force the user
        /// to decide where their data will be downloaded.
        /// </summary>
        /// <returns>The filepath to which the downloaded data will be returned.</returns>
        private String retreiveSavePath()
        {
            //TODO: Need to make a more elegant way to decide save path.
            //Also, if the user has a sample project open then the data will just get saved with the sample data files
            //with the sample project files, this may or may not be desired.

            String save_path = null;
            //Only ask user to choose a save location if the CurrentProjectDirectory is null
            if (App.SerializationManager.CurrentProjectDirectory == null)
            {
                //shows choose folder dialog and sets it to save_path if the user did not cancel.
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();
                if (result.Equals(DialogResult.OK))
                {
                    save_path = fbd.SelectedPath;
                }
            }
            else
            {
                save_path = App.SerializationManager.CurrentProjectDirectory;
            }
            return save_path;
        }

        /// <summary>
        /// This should probably be renamed. When clicked, open the HydroShareDownload dialog (python script)
        /// and allow user to select which files to download. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hydroShareDownload_Click(object sender, EventArgs e)
        {
            //Set the save_path to which we will save the downloaded files.
            String save_path = retreiveSavePath();
            
            //Only open the Python script if we know where we will be storing the files.
            if (save_path != null)
            {
                //Setup the parameters for starting the cmd process.
                ProcessStartInfo start = new ProcessStartInfo("python");
                //The python script is located in Plugins\HydroShare\Lib relative to the HydroDesktop executable. Pass this script to Python when we start the process.
                String cmd = AppDomain.CurrentDomain.BaseDirectory + Path.Combine("Plugins", "HydroShare", "Lib", "HydroShareDownloadDialog.py");
                String args = save_path;
                //Make sure we run the following command: python "path\to\HydroShareDownloadDialog.py" "path\to\savelocation"
                start.Arguments = string.Format("\"{0}\" \"{1}\"", cmd, args);
                start.UseShellExecute = false;
                start.CreateNoWindow = true;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;

                //Start the python script as if from a command prompt.
                Process process = new Process();
                process.StartInfo = start;
                process.Start();

                //Declare err and s. err holds Error output data from Python and s holds standard python output.
                string err = null;
                string s = null;

                //This while loop puts data from Python into s and err one line at a time. 
                //A single | is needed to make sure that BOTH statements below are evaluated before 
                //moving into the While loop.
                while (((err = process.StandardError.ReadLine()) != null)
                    | ((s = process.StandardOutput.ReadLine()) != null))
                {
                    //If s is not null it means our Python script has returned some values.
                    if (s != null)
                    {
                        openResult(s);
                    }

                    //If err is not null it means we have errors and should print those to the console for debugging.
                    if (err != null)
                    {
                        Console.WriteLine(err);
                    }
                }
            }
        }

    /*ScriptRuntime ipy = Python.CreateRuntime();
    ScriptEngine engine = ipy.GetEngine("Python");
    var paths = engine.GetSearchPaths();
    paths.Add(@"C:\Program Files\IronPython 2.7");
    paths.Add(@"C:\Users\Student\Documents\HD-3\Binaries\Plugins\HydroShare\Lib");
    engine.SetSearchPaths(paths);


    dynamic test = ipy.ExecuteFile(System.IO.Path.Combine("Plugins", "HydroShare", "Lib", "GetShapefilesFromHydroShareMyFrame1.py"));

    test.test();
    //dynamic test = ipy.UseFile(@"C:\Users\Cuyler Frisby\Documents\Python\bagit-1.2.1\bagit.py");

    /*dynamic test = ipy.ExecuteFile(System.IO.Path.Combine("Plugins", "HydroShare", "Lib", "hydrosharedownload.py"));
    IList<object> originalResult = (IList<object>)test.retrieveList();
    List<string> typeSafeResult = new List<string>();
    foreach (object element in originalResult)
    {
        typeSafeResult.Add((string)element);
    }
    String x = typeSafeResult[0];*/


        /*public class dynamic_demo
        {
            static void Main()
            {
            //    var ipy = Python.CreateRuntime();
               // dynamic test = ipy.UseFile("Test.py");
                //test.Simple();
            }
        }*/
       
    }
}
