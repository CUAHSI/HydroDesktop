using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using HydroDesktop.Database;
using System.Collections;
using System.Collections.Generic;
using HydroDesktop.Interfaces;

namespace HydroR
{
    public partial class cRCommandView : UserControl
    {

        #region privateDelaration

        [DllImport("User32")]
        private static extern int SetForegroundWindow(int hwnd);
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("User32.Dll")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, uint lParam);

        private const uint WM_CHAR = 0x102;
        private const int WM_SYSCOMMAND = 274;
        private const int SC_MAXIMIZE = 61488;
        public bool RIsRunning;
        public event REventHandler RChanged;
        public delegate void REventHandler(EventArgs e);


        #region Variables

        private Process p;
        private int pID;
        private int count = 0;
        private string pathToR;
        private ISeriesSelector _seriesSelector;

        #endregion

        #endregion

        #region Constructor


        public cRCommandView(ISeriesSelector args)
        {

            InitializeComponent();
            _seriesSelector = args;
            // Event             
            _seriesSelector.SeriesCheck += new SeriesEventHandler(SeriesSelector_SeriesCheck);
        }

        #endregion

        #region Method

        //To refresh the themes shown in the series selector
        public void RefreshView()
        {
            _seriesSelector.RefreshSelection();
        }

        //this function sends input from Hydrodesktop to R
        void sendString(string input)
        {
            char[] inputchar = input.ToCharArray();
            for (int i = 0; i < inputchar.Length; i++)
            {
                PostMessage(p.MainWindowHandle, WM_CHAR, (int)inputchar[i], 0);
            }
        }

        //void sendString(string input)
        //{
        //    char[] inputchar = input.ToCharArray();
        //    int i;

        //    for (i = 0; i < inputchar.Length; i++)
        //    {
        //        PostMessage(p.MainWindowHandle, WM_CHAR, (int)inputchar[i], 0);
        //        if (i % 400 == 0) Thread.Sleep(200);
        //    }

        //}

        private void sendLineToR(string input)
        {
            checkProcess();
            try
            {
                sendString(input);
                Thread.Sleep(50);
                PostMessage(p.MainWindowHandle, WM_CHAR, (int)Keys.Enter, 0);

            }
            catch (Exception e)
            {
                if (e.Message.Contains("Object reference not set to an instance of an object."))
                    MessageBox.Show("You must start R before you can Send Commands");
                else
                    MessageBox.Show("Input Error: " + e.Message);
                rtCommands.Line--;

            }
        }

        /*
         * Hydrodesktop's connection string contains forward slashes but R translates those into 
         *escape sequences. change slash replaces the \ with a /.
         */
        private string changeSlash(string input)
        {
            input = input.Replace(@"\", @"/");
            return input;
        }

        /*
         * starts an R process         
         */
        private void startR()
        {
            // resize panel2 and text box to be smaller
            int newheight = rtCommands.Size.Height / 3;


            //resize pnlR (larger)change location                

            spcEditor.Panel2Collapsed = false;
            spcEditor.SplitterDistance = (spcEditor.SplitterDistance - newheight) > 0 ? spcEditor.SplitterDistance - newheight : newheight * 2;
            //check to see if R was started
            if (SetupRPanel())
            {
                //check to see if this is the first time R has run
                if (isFirstTime())
                {
                    //load the required R packages
                    if (loadPackages())
                    {
                        sendLineToR("library(HydroR)");
                        p.Disposed += new System.EventHandler(this.p_Exited);
                    }
                }
                else
                {
                    sendLineToR("library(HydroR)");
                    p.Disposed += new System.EventHandler(this.p_Exited);
                }
            }
        }

        //Put R into a panel in hydroDesktop so that it is built in 
        private bool SetupRPanel()
        {
            bool foundPath = false;
            while (!foundPath)
            {
                //test to see if the process starts with the current path 
                try
                {
                    // MessageBox.Show(pathToR);
                    if (pathToR == "")
                    {
                        p = Process.Start("Rgui", "--sdi");
                    }
                    else
                    {
                        p = Process.Start(@pathToR + @"\Rgui.exe", "--sdi");//--no-save");
                    }

                    p.WaitForInputIdle();
                    pID = p.Id;
                    setRParent();
                    RIsRunning = true;
                    return true;

                }
                catch (Exception e)
                {  //if R does not start with the current path, open an openfiledialog and have the 
                    //user find the path to the R executables
                    if (e.Message.Contains("cannot find the file specified"))
                    {
                        frmInstallR frmR = new frmInstallR();
                        frmR.ShowDialog(this);
                        if (frmR.getRPathResult == HydroR.frmInstallR.buttonType.OK)
                            pathToR = frmR.getPathToR;
                        else
                        {   //resize to original panel size    
                            spcEditor.Panel2Collapsed = true;
                            spcEditor.SplitterDistance = (spcEditor.Height - 20);
                            //stop ciculating to make sure the path is correct
                            foundPath = true;
                            //stop the rest of the code from trying to start R;
                            RIsRunning = false;
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        // to start R and get info back you must redirect standard input/output
        string standardinput(string send)
        {
            try
            {
                Process tempP = new Process();
                StreamWriter sw;
                StreamReader sr;
                ProcessStartInfo psI = new ProcessStartInfo();
                if (pathToR == "")
                {
                    psI.FileName = @"Rterm.exe";
                }
                else
                {
                    //MessageBox.Show(pathToR);
                    psI.FileName = @pathToR + @"\Rterm.exe";
                }
                psI.Arguments = "--no-save";
                psI.UseShellExecute = false;
                psI.RedirectStandardInput = true;
                psI.RedirectStandardOutput = true;
                psI.CreateNoWindow = true;
                tempP.StartInfo = psI;
                tempP.Start();
                sw = tempP.StandardInput;
                sr = tempP.StandardOutput;
                sw.AutoFlush = true;
                sw.WriteLine(send);
                sw.Close();
                string data = sr.ReadToEnd();
                tempP.Dispose();
                return data;
            }
            catch (Exception e)
            {
                MessageBox.Show(@pathToR + @"\Rterm.exe: " + e.Message);
            }
            return " ";
        }

        /*
         * if R is running checks to see if they want to close window
         * same question as R asks when it is closing, then calls a 
         * function to close R. this way we have feedback from our 
         * process to know what the user has selected
         */
        bool closeWindow()
        {
            if (p != null)
            {
                DialogResult result = MessageBox.Show("Save R Workspace Image?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    closing(true);
                    return true;
                }
                else if (result == DialogResult.No)
                {
                    closing(false);
                    return true;
                }
                return false;
            }
            return true;

        }

        /*
         * closes R and simulates a button click on the R Save workspace popup.
         * after R closes resize the panels so that the Text editor takes up the entire window
         * input: wether the user has chosen to save their R workspace
         */
        void closing(bool saveWorkspace)
        {
            p.CloseMainWindow();
            Thread.Sleep(50);
            if (!saveWorkspace)
            {
                pushButton("{TAB}");
                //PostMessage(ps.MainWindowHandle, WM_CHAR, (int)Keys.Tab, 0);
            }

            pushButton("{ENTER}");
            //PostMessage(ps.MainWindowHandle, WM_CHAR, (int)Keys.Enter, 0);

            //int enter = 0x0D;
            //PostMessage(p.MainWindowHandle, WM_CHAR, enter, 0);
            Thread.Sleep(50);
            p.Dispose();
            pID = -10000000;
            p = null;
        }

        /*
         * check to see if p is still assigned to a process, if it is, 
         * is R still running or did R close itself
        */
        void checkProcess()
        {
            if (p != null)
            {
                try
                {
                    Process processRgui = Process.GetProcessById(pID);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("is not running"))
                    {
                        p.Dispose();

                    }
                }
            }
        }

        //Checks to see if the HydroR package is installed
        private bool isFirstTime()
        {
            string output = standardinput("is.element(\"HydroR\", installed.packages()[,1])");
            bool firsttime = output.Contains("FALSE");
            return firsttime;
        }

        /*
         * load RSQLite and HydroR packages. RSQlite send using sendLine, 
         * HydroR using standardInput
         * output: did the packages install 
         */
        private void pushButton(string s)
        {

            Process[] processes = Process.GetProcessesByName("Rgui");
            Process ps = new Process();
            for (int i = 0; i < processes.Length; i++)
            {
                if (processes[i].MainWindowTitle == "Question")
                {
                    ps = processes[i];
                    break;
                }
                ps = null;

            }

            if (ps != null)
            {
                SetForegroundWindow((int)ps.MainWindowHandle);
                ShowWindow((int)ps.MainWindowHandle, 1);

                SendKeys.Send(s);
            }
        }

        private bool loadPackages()
        {
            string output;
            bool installed;
            int count = 0;

            output = standardinput("is.element(\"RSQLite\", installed.packages()[,1])");
            if (!output.Contains("TRUE"))
            {
                //install RSQLite- need to allow popups
                sendLineToR("install.packages(\"RSQLite\",dependencies = TRUE) ");
                System.Threading.Thread.Sleep(200);
                pushButton("{ENTER}");
                do
                {
                    output = standardinput("is.element(\"RSQLite\", installed.packages()[,1])");
                    installed = output.Contains("TRUE");
                    Thread.Sleep(200);

                } while (!installed);
            }
            //install Hydro R package-standardinput-need feedback
            do
            {
                count++;
                //output = standardinput("install.packages(\"" + changeSlash(Path.GetDirectoryName(this.GetType().Assembly.Location)) + "/HydroR_1.1.tar.gz\",repos=NULL, type = \"source\")\n is.element(\"HydroR\", installed.packages()[,1])");
                sendLineToR("install.packages(\"" + changeSlash(Path.GetDirectoryName(this.GetType().Assembly.Location)) + "/HydroR_1.2.tar.gz\",repos=NULL, type = \"source\")");
                pushButton("{ENTER}");
                output = standardinput("is.element(\"HydroR\", installed.packages()[,1])");
                installed = output.Contains("TRUE");
                Thread.Sleep(200);
            } while (!installed && count < 15);

            //only allow the process to attempt installation 15 times so it is not stuck in an infinite loop
            if (count == 15)
            {
                MessageBox.Show("One or more of the required packages were unable to install, \nThe HydroR Plugin will not work properly until they are installed.");
                //closing(false);
                //startR();
                return false;
            }
            return true;
        }

        #endregion

        #region Event

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            //add controls to form 
            this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
            this.ParentForm.ResizeEnd += new EventHandler(pnlR_Resized);

        }

        /*
         *Checks to make sure the user actually wants to exit, if they do 
         *save the pathToR so the user does not have to find it again
         */
        void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = (!closeWindow());
            }
            catch
            {
                //if there is an error p is already closed or was never opened, so ignore request
            }
            Properties.Settings.Default.PathToR = pathToR;
            Properties.Settings.Default.Save();
        }
        void SeriesSelector_SeriesCheck(object sender, SeriesEventArgs e)
        {
        }
        // Clear the Table When Criterion Changed       
        /* private void SeriesSelector_CriterionChanged(object sender, SeriesEventArgs e)
         {
             _hydroArgs.SeriesView.SeriesSelector.CheckedIDList.Clear;
         }*/

        private void cRCommandView_Load(object sender, EventArgs e)
        {
            pathToR = Properties.Settings.Default.PathToR;
            rtCommands.Highlightlength = rtCommands.Width;
            rtCommands.Line = 0;
        }
        /*
         * if R is Running will attempt to close it,
         * if there is no instance of R it starts one  
         */
        public void btnR_Click(object sender, EventArgs e)
        {
            checkProcess();
            if (p == null)
            {
                startR();
            }
            else
            {
                try
                {
                    closeWindow();
                }
                catch
                {
                }
            }
            if (RChanged != null)
            {
                RChanged(e);
            }
        }

        //this will launch the R settings
        public void btnSettings_Click(object sender, EventArgs e)
        {
            frmInstallR frmR = new frmInstallR(true);
            frmR.getPathToR = pathToR;
            frmR.ShowDialog(this);
            if (frmR.getRPathResult == HydroR.frmInstallR.buttonType.OK)
            {
                string newPathToR = frmR.getPathToR;
                if (newPathToR != pathToR)
                {
                    pathToR = newPathToR;
                    //startR();
                }
            }
        }

        //if the process has exited resize the panels so the text box takes up the entire screen
        private void p_Exited(object sender, EventArgs e)
        {
            p = null;
            pID = -1000000;
            spcEditor.Panel2Collapsed = true;
            spcEditor.SplitterDistance = (spcEditor.Height - 5);
            RIsRunning = false;
            if (RChanged != null)
            {
                RChanged(e);
            }

        }

        //send the current line to r
        public void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                sendLineToR(rtCommands.Lines[rtCommands.Line]);
                rtCommands.Line++;
                rtCommands.SelectLine(rtCommands.Line);
                rtCommands.SelectionStart += rtCommands.Lines[rtCommands.Line].Length;
                rtCommands.ScrollToCaret();

            }
            catch { }
        }

        //Send the lines that are currently selected to R
        public void btnSendSel_Click(object sender, EventArgs e)
        {
            try
            {
                sendLineToR(rtCommands.SelectedText);
            }
            catch { }
        }

        //Send all the data in the TextBox to R
        public void btnSendAll_Click(object sender, EventArgs e)
        {
            try
            {
                sendLineToR(rtCommands.Text);
            }
            catch { }
        }

        //creat the command to send to R 
        public void txtGenR_Click(object sender, EventArgs e)
        {
            rtCommands.GenerateCode = true;
            //for each series that is selected
            if (_seriesSelector.CheckedIDList.Length < 1)
            { MessageBox.Show("A Series Must Be Selected to Generate R Code"); }
            else
            {
                //nrs = nrs.Distinct().ToArray();
                //int[] ids = GetDistinctValues(Convert.seriesSelector31.CheckedIDList.ToArray());
                for (int i = 0; i < _seriesSelector.CheckedIDList.Length; i++)
                {
                    //get the DB connection from HydroDesktop
                    string fileLoc = (HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString.Split(';'))[0].Substring(12);
                    DbOperations dbCall = new DbOperations(HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
                    //get the begin and end dates from the database for the current series
                    DateTime begin = Convert.ToDateTime(dbCall.ExecuteSingleOutput("Select BeginDateTime FROM DataSeries WHERE SeriesID = " + _seriesSelector.CheckedIDList[i]));
                    DateTime end = Convert.ToDateTime(dbCall.ExecuteSingleOutput("Select EndDateTime FROM DataSeries WHERE SeriesID = " + _seriesSelector.CheckedIDList[i]));
                    if (fileLoc.Contains(" "))
                        rtCommands.AppendText("data" + count + " <- getDataSeries(connectionString=" + changeSlash(fileLoc).Trim() + "," + "\n");

                    else
                    {
                        rtCommands.AppendText("data" + count + " <- getDataSeries(connectionString=\"" + changeSlash(fileLoc).Trim() + "\"," + "\n");                        
                    }
                    rtCommands.AppendText("\tseriesID=" + _seriesSelector.CheckedIDList[i] + "," + "\n"
                                            + "\tSQLite=TRUE," + "\n"
                                            + "\tstartDate= \"" + begin.ToString("yyyy-MM-dd") + "\"," + "\n"
                                            + "\tendDate=\"" + end.ToString("yyyy-MM-dd") + "\")" + "\n");
                    //set the current line to the start of the DB request
                    rtCommands.Line = rtCommands.Lines.Length - 6;
                    rtCommands.TextChange(rtCommands.Line, rtCommands.Line + 6);
                    count++;

                }
                rtCommands.SelectLine(rtCommands.Line);
            }
            rtCommands.GenerateCode = false;
        }

        //open a saved file to fill the textbox
        public void btnOpen_Click(object sender, EventArgs e)
        {
            rtCommands.GenerateCode = true;
            ofdOpen.ShowDialog();
            if (ofdOpen.FileName != "")
                rtCommands.LoadFile(ofdOpen.FileName, RichTextBoxStreamType.PlainText);
            rtCommands.TextChange();
            rtCommands.Line = 0;
            rtCommands.SelectLine(rtCommands.Line);
            rtCommands.GenerateCode = false;

        }

        public void btnSetRPath_Click(object sender, EventArgs e) 
        {
            frmInstallR frmR = new frmInstallR();
            frmR.ShowDialog(this);
            if (frmR.getRPathResult == HydroR.frmInstallR.buttonType.OK)
                pathToR = frmR.getPathToR;
        }

        //save textbox to file
        public void btnSave_Click(object sender, EventArgs e)
        {
            sfdSave.ShowDialog();
            if (sfdSave.FileName != "")
                rtCommands.SaveFile(sfdSave.FileName, RichTextBoxStreamType.PlainText);
        }

        private void cRCommandView_MouseMove(object sender, MouseEventArgs e)
        {
            checkProcess();
        }

        /*
         * Resizes R to fit into the panel. only resizes when the parent 
         * form is done resizing
         */
        private void setRParent()
        {
            if (p != null)
            {
                SetParent(p.MainWindowHandle, spcEditor.Panel2.Handle);
                SendMessage(p.MainWindowHandle, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
            }
        }

        private void pnlR_Resized(object sender, EventArgs e)
        {
            setRParent();
        }

        private void spcEditor_SplitterMoved(object sender, SplitterEventArgs e)
        {
            setRParent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            rtCommands.SelectLine(rtCommands.Line);
        }

        #endregion


        

       

        
    
    }
}


