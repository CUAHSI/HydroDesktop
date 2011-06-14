using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using OpenMI.Standard;
using Oatc.OpenMI.Gui.Core;
using Oatc.OpenMI.Sdk.Backbone;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using System.Drawing.Imaging;
using System.Timers;
using System.Reflection;
using System.Xml;
using HydroDesktop.Help;
using System.Runtime.InteropServices;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    public partial class mainTab : UserControl
    {

        #region Window controls
        //private System.Windows.Forms.HScrollBar compositionHScrollBar;
        private System.Windows.Forms.PictureBox compositionBox;
        private System.Windows.Forms.ListView fileList;
        private System.Windows.Forms.ListView properties;
        private System.Windows.Forms.Panel ListPanel;
        private System.Windows.Forms.SplitContainer container;
        //private TextBox output_box;
        private SplitContainer container2;
        private SplitContainer comp_container;
        private TextBox t;
        private System.Windows.Forms.FolderBrowserDialog dirDialog;
        private System.Windows.Forms.Button AddModelButton;
        private ImageList _hmImage;
        //private Image _HMimage;
        private Button changeDir;
        private Button _Save;
        private Label currentDir;
        private Label emptylabel1;
        private Label emptylabel2;
        private TextBox tb_navigate;
        private ToolStripMenuItem menuFileNew;
        private ToolStripMenuItem menuFileOpen;
        private ToolStripMenuItem menuFileSave;
        private ToolStripMenuItem menuFileSaveAs;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuEditModelAdd;
        private ToolStripMenuItem menuEditTriggerAdd;
        private ToolStripMenuItem menuHelpAbout;
        private ToolStripMenuItem menuFileReload;
        private ToolStripMenuItem menuViewModelProperties;
        private ToolStripMenuItem menuEditConnectionProperties;
        private ToolStripMenuItem menuEditConnectionAdd;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuEditRunProperties;
        private ToolStripMenuItem menuHelp;  
        private ContextMenu filelist_context;
        private MenuItem add_model;
        private MenuItem add_comp;
        private MenuItem delete;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem contextRun;
        private System.Windows.Forms.MenuItem contextConnectionAdd;
        private System.Windows.Forms.MenuItem contextModelProperties;
        private System.Windows.Forms.MenuItem contextConnectionProperties;
        private System.Windows.Forms.MenuItem contextModelRemove;
        private System.Windows.Forms.MenuItem contextConnectionRemove;
        private System.Windows.Forms.MenuItem contextModelAdd;
        private System.Windows.Forms.ImageList imageList;
        private string listviewImagesPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).CodeBase).Remove(0, 6) + "/icons";
        private bool _isdragging = false;
        private int initialX, initialY, currentX, currentY;
        private List<RibbonPanel> rps;
        private string _currentFile = null;
        #endregion

        #region Member variables

        // pre-created dialogs
        bool _isMovingModel = false;
        bool _isAddingConnection = false;
        private bool ispan = false;
        string _compositionFilename = null;
        string image_path;
        const string ApplicationTitle = "Configuration Editor";
        const string DefaultFilename = "NewComposition.opr";
        object _contextSelectedObject;
        AboutBox _aboutBox;
        CompositionManager _composition;
        ConnectionDialog _connectionDialog;
        Cursor _sourceCursor, _targetCursor, _panCursor;
        private IMapPluginArgs _mapArgs;
        ModelDialog _modelDialog;
        Point _prevMouse;
        Point _compositionBoxPositionInArea;
        Rectangle _compositionArea;
        RunProperties _runProperties;
        RunBox _runBox;
        UIModel _sourceModel = null;
        

        private System.Windows.Forms.MenuItem contextDivider;
        private System.Windows.Forms.MenuItem contextAddTrigger;
        private System.Windows.Forms.ToolStripMenuItem menuHelpContents;
      
        private System.Windows.Forms.ToolStripMenuItem menuOptions;
        private System.Windows.Forms.ToolStripMenuItem menuRegisterExtensions;
        private readonly string _localHelpUri = HydroModeler.Settings.Default.localHelpUri;

        // record the culture that the application starts in
        System.Globalization.CultureInfo _cultureInfo = Application.CurrentCulture;

        private Dictionary<string, string> openmiFiles = new Dictionary<string, string>();
        private Dictionary<string, string> Folders = new Dictionary<string, string>();
        private ListViewItem _currentLvi = new ListViewItem();
        private ListViewItem _currentFileItem = new ListViewItem();
        private bool _hasChanged = false;
        private string _oldText;
        bool _allowEdit = true;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="mainTab">mainTab</see> window.
        /// </summary>
        public mainTab(IMapPluginArgs args, List<RibbonPanel> rp, string currentDirectory)
        {
            //get ribbon panel object
            rps = rp;

            //set mapwindow args
            _mapArgs = args;

            _compositionBoxPositionInArea = new Point(0, 0);

            fileList = new ListView();
            
            //initialize the composition
            InitializeComponentRibbon();
            _composition = new CompositionManager();
            
            //define mouse variables
            _prevMouse = new Point(0, 0);

            //set cursors from resources file
            using (var memoryStream = new MemoryStream(HydroModeler.Properties.Resources.Source))
            {
                _sourceCursor = new Cursor(memoryStream);
            }

            using (var memoryStream = new MemoryStream(HydroModeler.Properties.Resources.Target))
            {
                _targetCursor = new Cursor(memoryStream);
            }
            Image panBit = HydroModeler.Properties.Resources.pan1.GetThumbnailImage(32, 32, null, IntPtr.Zero);            
            Bitmap b = new Bitmap(panBit);
            _panCursor = CreateCursor(b, 3, 3);

            // create dialogs
            _modelDialog = new ModelDialog();
            _connectionDialog = new ConnectionDialog();
            _aboutBox = new AboutBox();
            _runProperties = new RunProperties();
            _runBox = new RunBox(this);

            //set current directory
            _runBox._currentDirectory = currentDirectory;
        }


        #region Methods and properties

        /// <summary>
        /// Sets the ispan field.  This is used to determine if panning will be performed when the user clicks in the composition box.
        /// </summary>
        public bool Ispan
        {
            get { return ispan; }
            set { ispan = value; }
        }

        public string Image_Path
        {
            get { return image_path; }
            set {  image_path = value; }
        }

        #region Icon Creation
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
        #endregion

        /// <summary>
        /// Method is used to start application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <remarks>Method proceeds all command-line args ("/opr %", "/reg", ...)
        /// and perform requested actions.</remarks>		
        private static void ProceedCommandLineArgs(string[] args)
        {
            // read commad-line args
            string oprFilename = null;
            string omiFilename = null;
            bool mta = false;

            for (int i = 0; i < args.Length; i++)
                switch (args[i].ToLower())
                {
                    case "/opr":
                    case "-opr":
                        if (oprFilename != null)
                            throw (new Exception("-opr can be used only once."));

                        if (omiFilename != null)
                            throw (new Exception("-opr cannot be used together with -omi option."));

                        if (args.Length <= i + 1)
                            throw (new Exception("-opr option must be followed by filename."));

                        oprFilename = args[i + 1];
                        i++;
                        break;

                    case "/omi":
                    case "-omi":
                        if (omiFilename != null)
                            throw (new Exception("-omi can be used only once."));

                        if (oprFilename != null)
                            throw (new Exception("-omi cannot be used together with -opr option."));

                        if (args.Length <= i + 1)
                            throw (new Exception("-omi option must be followed by filename."));

                        omiFilename = args[i + 1];
                        i++;
                        break;

                    case "/reg":
                    case "-reg":
                        Utils.RegisterFileExtensions(Application.ExecutablePath);
                        return;

                    case "/unreg":
                    case "-unreg":
                        Utils.UnregisterFileExtensions();
                        return;

                    case "-mta":
                    case "/mta":
                        mta = true;
                        break;

                    case "-help":
                    case "/help":
                    case "-?":
                    case "/?":
                    case "--help":
                    case "-h":
                    case "/h":
                        string help =
                            "OmiEd command-line options:\n\n" +
                            "Syntax: OmiEd.exe [-opr OPRFILE | -omi OMIFILE | -reg | -unreg | -help] [-mta]\n\n" +
                            "Options:\n" +
                            "-opr OPRFILE\tOpens OmiEd project from specific OPRFILE\n" +
                            "-omi OMIFILE\tCreates a new composition and adds model from OMIFILE into it.\n" +
                            "-reg\t\tRegisters OPR and OMI file extensions in Windows registry to be opened with this OmiEd executable.\n" +
                            "-unreg\t\tDiscards all OPR and OMI file extension registrations from Windows registry.\n" +
                            "-help\t\tShows this help.\n" +
                            "-mta\t\tApplication creates and enters a multi-threaded apartment COM model at start.\n";
                        MessageBox.Show(help, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;

                    default:
                        throw (new Exception("Unknown command-line option: " + args[i]));
                }

            // do actions...

            // VS2005 fix
            // In VS2005 the main thread uses MTA by default, so we have to create new thread,
            // which will run the message loop, and set it's appartment state before it's started

            Thread thread = new Thread(new ParameterizedThreadStart(StartApplication));
            thread.IsBackground = false;

            if (mta)
            {
                thread.SetApartmentState(ApartmentState.MTA);

                // NOTE: when using MTA, the OpenFileDialog (and maybe other things)
                // throws ThreadStateException ("Current thread must be set to single thread
                // apartment (STA) mode before OLE calls can be made. Ensure that your Main
                // function has STAThreadAttribute marked on it. This exception is only raised
                // if a debugger is attached to the process.")
                //
                // MTA is used only if really needed (we provide it as feature),
                // thus this statement is perfectly correct
                Control.CheckForIllegalCrossThreadCalls = false;
            }
            else
            {
                thread.SetApartmentState(ApartmentState.STA);
            }

            thread.Start(new string[] { oprFilename, omiFilename });
        }

        private static void StartApplication(object data)
        {
            try
            {
                string oprFilename = ((string[])data)[0];
                string omiFilename = ((string[])data)[1];

                if (oprFilename != null)
                {
                    // Open OPR project from file                    
                    FileInfo fileInfo = new FileInfo(oprFilename);

                    //this.OpenOprFile(fileInfo.FullName);

                    //Application.Run( mainTab );
                }
                else if (omiFilename != null)
                {
                    FileInfo fileInfo = new FileInfo(omiFilename);

                    //AddModel(fileInfo.FullName);

                    //Application.Run( mainTab );
                }
                //else
                //Application.Run( new mainTab() );
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens composition from OPR file.
        /// </summary>
        /// <param name="fullPath">Full path to OPR file.</param>
        private void OpenOprFile(string fullPath)
        {
            try
            {
                _compositionFilename = null;
                _composition.Release();
                _composition.LoadFromFile(fullPath);
                _compositionFilename = fullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error occured while loading the file...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _composition.Release();
            }

            UpdateControls();
            UpdateTitle();

            CompositionUpdateArea();
            CompositionCenterView();
        }


        /// <summary>
        /// Adds one model to composition.
        /// </summary>
        /// <param name="fullPath">Full path to OMI file.</param>
        private void AddModel(string fullPath)
        {
            try
            {
                _composition.AddModel(null, fullPath);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "OMI filename: " + fullPath + "\n" + "Exception: " + ex.ToString(),
                    "Error occured while adding the model...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Reset the culture every time a new model is added.
            // The new model may be of a different culture, we want to retain the original culture of the application, 
            // which will be that of the User's computer.
            Application.CurrentCulture = _cultureInfo;

            CompositionUpdateArea();
            UpdateControls();
            UpdateTitle();
            Invalidate();
        }


        /// <summary>
        /// Method calculates size of composition area and it's scroll-bars according to 
        /// position of models' rectangles and size of the window.
        /// </summary>
        /// <remarks>
        /// This method is called if some model has moved, main window has resized or if new file was opened.
        /// </remarks>
        public void CompositionUpdateArea()
        {
            Point topLeft = new Point(0, 0),
                bottomRight = new Point(0, 0);

            int minx = 999;
            int miny = 999;
            int maxx = 0;
            int maxy = 0;
            foreach (UIModel model in _composition.Models)
            {
                topLeft.X = Math.Min(topLeft.X, model.Rect.X);
                if (topLeft.X < minx)
                    minx = topLeft.X;

                topLeft.Y = Math.Min(topLeft.Y, model.Rect.Y);
                if (topLeft.Y < miny)
                    miny = topLeft.Y;

                bottomRight.X = Math.Max(bottomRight.X, model.Rect.X + model.Rect.Width);
                if (bottomRight.X > maxx)
                    maxx = bottomRight.X;

                bottomRight.Y = Math.Max(bottomRight.Y, model.Rect.Y + model.Rect.Height);
                if (bottomRight.Y > maxy)
                    maxy = bottomRight.Y;
            }
            
            //get the container's rectangle
            Rectangle extents = new Rectangle(new Point(0,0),container.Panel2.ClientSize);
            
            //get the rectangle needed to show the model composition
            if (minx < extents.X)
                extents.X = minx;
            if (miny < extents.Y)
                extents.Y = miny;
            if (maxx > extents.Right)
                extents.Width = maxx + 10 - extents.X;
            if (maxy > extents.Bottom)
                extents.Height = maxy + 10 - extents.Y;

            //set the composition area
            _compositionArea = extents;
            
            //resize the compositionBox
            this.compositionBox.Size = new System.Drawing.Size(extents.Width, extents.Height);

            compositionBox.Invalidate();
        }

        //TODO: Remove this method because its out-of-date
        public void CompositionUpdateArea(int x, int y)
        {
            //get the top left corner of panel2 in screen coordinates 
            Point p = this.container.Panel2.PointToScreen(new Point(0, 0));

            Point TopLeft = new Point(x - p.X, y - p.Y),
                            bottomRight = new Point(0, 0);

            Point topLeft = CompositionAreaPointToWindowPoint(TopLeft);

            //Point topLeft = new Point(x-left, y-top),
            //    bottomRight = new Point(0, 0);

            //get the last entered model
            UIModel model = (UIModel)_composition.Models[_composition.Models.Count - 1];

            //set its draw properties
            bottomRight.X = topLeft.X + model.Rect.Width;
            bottomRight.Y = topLeft.Y + model.Rect.Height;
            model.Rect = new Rectangle(topLeft.X, topLeft.Y, model.Rect.Width, model.Rect.Height);

            Rectangle extents = container.Panel2.DisplayRectangle;



            _compositionArea = extents;
            _compositionArea = new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

            compositionBox.Invalidate();
        }
        /// <summary>
        /// Sets composition box to center.
        /// </summary>
        private void CompositionCenterView()
        {
            // todo...
        }

        private Point CompositionWindowPointToAreaPoint(Point point)
        {
            return (new Point(_compositionBoxPositionInArea.X + point.X, _compositionBoxPositionInArea.Y + point.Y));
        }

        private Point CompositionAreaPointToWindowPoint(Point point)
        {
            return (new Point(point.X - _compositionBoxPositionInArea.X, point.Y - _compositionBoxPositionInArea.Y));
        }


        private void UpdateTitle()
        {
            this.Text = ApplicationTitle + (_composition.ShouldBeSaved ? " *" : "");
        }


        private void UpdateControls()
        {
            contextConnectionAdd.Enabled = menuEditConnectionAdd.Enabled = _composition.Models.Count  > 1;
              
            //_composition.Models.Count > 1;
            bool hasTrigger = _composition.HasTrigger();

            contextAddTrigger.Enabled = menuEditTriggerAdd.Enabled = !hasTrigger;

            contextRun.Enabled = menuEditRunProperties.Enabled = hasTrigger && (_composition.Models.Count - _composition.Connections.Count <= 1) && (_composition.Models.Count > 1);

            //enable or disable the Run ribbon button
            //enableRun = contextRun.Enabled;

            //enable or disable the Connection ribbon button
            //enableConn = contextConnectionAdd.Enabled;
        }


        /// <summary>
        /// If composition should be saved, this method shows message box, where the user can do it, can
        /// ignore it or can cancel current operation.
        /// </summary>
        /// <returns>Returns <c>true</c> if current operation can continue, or <c>false</c>
        /// if user pressed cancel button.</returns>
        private bool CheckIfSaved()
        {
            if (_composition.ShouldBeSaved)
            {
                switch (MessageBox.Show("The composition has been changed.\n\nDo you want to save the changes?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.Yes:
                        menuFileSave_Click(null, null);
                        return (!_composition.ShouldBeSaved);
                    case DialogResult.No:
                        return (true);
                    default:
                        return (false);
                }
            }
            return (true);
        }


        private void ShowLinkDialog(UIConnection link)
        {
            // find maximum link ID of all existing links
            int maxID = 0;
            foreach (UIConnection uiLink in _composition.Connections)
                foreach (ILink iLink in uiLink.Links)
                    maxID = Math.Max(int.Parse(iLink.ID), maxID);

            _connectionDialog.PopulateDialog(link, maxID + 1);
            if (_connectionDialog.ShowDialog(this) == DialogResult.OK)
                _composition.ShouldBeSaved = true;

            UpdateTitle();
        }


        private UIModel GetModel(int x, int y)
        {
            Point areaPoint = CompositionWindowPointToAreaPoint(new Point(x, y));

            // search from last model to first for case some models are overlapping
            for (int i = _composition.Models.Count - 1; i >= 0; i--)
            {
                UIModel model = (UIModel)_composition.Models[i];

                if (model.IsPointInside(areaPoint))
                    return (model);
            }

            return (null);
        }


        private UIConnection GetConnection(int x, int y)
        {
            Point areaPoint = CompositionWindowPointToAreaPoint(new Point(x, y));

            for (int i = _composition.Connections.Count - 1; i >= 0; i--)
            {
                UIConnection connection = (UIConnection)_composition.Connections[i];

                if (connection.IsOnConnectionLine(areaPoint))
                    return (connection);
            }

            return (null);
        }


        private void StopAddingConnection()
        {
            _isAddingConnection = false;
            //compositionBox.Cursor = Cursors.Default;
            _sourceModel = null;
        }

        private void StopMovingModel()
        {
            _isMovingModel = false;
            foreach (UIModel model in _composition.Models)
                model.IsMoving = false;
            compositionBox.Invalidate();
        }


        private void StopAllActions()
        {
            StopAddingConnection();
            StopMovingModel();
        }


        #endregion

        #region mainTab event handlers

        private void mainTab_Load(object sender, System.EventArgs e)
        {
            mainTab_SizeChanged(sender, e);
            UpdateTitle();
            UpdateControls();
            CompositionUpdateArea();

            //load the file list too
            LoadFileList();
            
        }
        private void LoadFileList()
        {
            int height = compositionBox.Height;
            int width = 100;
            fileList.Invalidate(new Rectangle(0,0,width,height));

        }

        private void mainTab_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            MessageBox.Show("form1, dragDrop");

        }


        private void mainTab_SizeChanged(object sender, System.EventArgs e)
        {
            // resize all elements so they fit to window
            //const int border = 5;
            //const int scrollBarWidth = 16;			
            CompositionUpdateArea();
        }


        private void mainTab_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // if composition isn't saved, show message box, and maybe stop the closing			
            e.Cancel = !CheckIfSaved();

            if (!e.Cancel)
            {
                _composition.Release();
            }
        }


        public void mainTab_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // ESC cancels adding connection
            if (_isAddingConnection && e.KeyChar == 27)
            {
                StopAddingConnection();
                e.Handled = true;
                Invalidate();
            }
            else if (e.KeyChar == (char)System.Windows.Forms.Keys.LButton)
            {
                e.Handled = true;
                Invalidate();
            }
        }


        #endregion

        #region Main menu event handlers

        public void menuEditModelAdd_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            OpenFileDialog dlgFile = new OpenFileDialog();
            dlgFile.CheckFileExists = true;
            dlgFile.CheckPathExists = true;
            dlgFile.Title = "Add model...";
            dlgFile.Filter = "OpenMI models (*.omi)|*.omi|All files|*.*";
            dlgFile.Multiselect = false;

            if (dlgFile.ShowDialog(this) == DialogResult.OK)
                AddModel(dlgFile.FileName);

            dlgFile.Dispose();
        }


        public void menuEditTriggerAdd_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            try
            {
                _composition.AddModel(null, CompositionManager.TriggerModelID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Exception: " + ex.ToString(),
                    "Error occured while adding the trigger...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            UpdateControls();
            UpdateTitle();
            CompositionUpdateArea();
        }


        public void menuDeployRun_Click(object sender, System.EventArgs e)
        {
            StopAllActions();




            _runProperties.PopulateDialog(_composition, _compositionFilename == null);
            DialogResult result = _runProperties.ShowDialog(this);

            UpdateTitle();

            if (result == DialogResult.OK)
            {
                // user decided to run the composition

                // ### prepare listeners
                ArrayList listOfListeners = new ArrayList();

                // progress bar
                ProgressBarListener progressBarListener = new ProgressBarListener(_composition.GetSimulationTimehorizon(), _runBox.ProgressBarRun);
                listOfListeners.Add(progressBarListener);

                // log file
                if (_composition.LogToFile != null && _composition.LogToFile != "")
                {
                    // get composition file's directory to logfile is saved in same directory
                    string logFileName;
                    if (_compositionFilename != null)
                    {
                        FileInfo compositionFileInfo = new FileInfo(_compositionFilename);
                        FileInfo logFileInfo = Utils.GetFileInfo(compositionFileInfo.DirectoryName, _composition.LogToFile);
                        logFileName = logFileInfo.FullName;
                    }
                    else
                        logFileName = _composition.LogToFile;

                    LogFileListener logFileListener = new LogFileListener(_composition.ListenedEventTypes, logFileName);
                    listOfListeners.Add(logFileListener);
                }

                // list box
                if (_composition.ShowEventsInListbox)
                {
                    ListViewListener listViewListener = new ListViewListener(_composition.ListenedEventTypes, _runBox.ListViewEvents, 400);
                    listOfListeners.Add(listViewListener);
                }

                const uint actionInterval = 200; // in milliseconds

                // ### create proxy listener and register other listeners to it
                IListener proxyListener;
                if (_composition.RunInSameThread)
                {
                    // DoEvents listener
                    DoEventsListener doEventsListener = new DoEventsListener(actionInterval);
                    listOfListeners.Add(doEventsListener);

                    ProxyListener proxySingleThreadListener = new ProxyListener();
                    proxySingleThreadListener.Initialize(listOfListeners);
                    proxyListener = proxySingleThreadListener;
                }
                else
                {
                    ProxyMultiThreadListener proxyMultiThreadListener = new ProxyMultiThreadListener();
                    proxyMultiThreadListener.Initialize(listOfListeners, _runBox.Timer, (int)actionInterval);
                    proxyListener = proxyMultiThreadListener;
                }

                // ### populate and show run-dialog and run simulation from it					
                Invalidate();
                _runBox.PopuplateDialog(_composition, proxyListener);
                _runBox.ShowDialog(this); // this fires simulation					


            }
        }


        public void menuFileNew_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            if (!CheckIfSaved())
                return;

            _composition.Release();

            _compositionFilename = null;
            UpdateControls();
            UpdateTitle();
            CompositionUpdateArea();
        }


        public void menuFileOpen_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            if (!CheckIfSaved())
                return;

            OpenFileDialog dlgFile = new OpenFileDialog();
            dlgFile.Filter = "OmiEd projects (*.opr)|*.opr|All files|*.*";
            dlgFile.Multiselect = false;
            dlgFile.CheckFileExists = true;
            dlgFile.CheckPathExists = true;
            dlgFile.Title = "Open project...";

            if (dlgFile.ShowDialog(this) == DialogResult.OK)
                OpenOprFile(dlgFile.FileName);

            dlgFile.Dispose();

        }


        public void menuFileSave_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            string filename;

            if (_compositionFilename == null)
            {
                SaveFileDialog dlgFile = new SaveFileDialog();
                dlgFile.Filter = "OmiEd projects (*.opr)|*.opr|All files|*.*";
                dlgFile.ValidateNames = true;
                dlgFile.FileName = DefaultFilename;
                dlgFile.Title = "Save project...";
                dlgFile.AddExtension = true;
                dlgFile.OverwritePrompt = true;

                if (dlgFile.ShowDialog(this) != DialogResult.OK)
                {
                    dlgFile.Dispose();
                    return;
                }

                filename = dlgFile.FileName;

                dlgFile.Dispose();
            }
            else
                filename = _compositionFilename;

            try
            {
                _composition.SaveToFile(filename);
                _compositionFilename = filename;

                //HACK:
                //make sure that the trigger does not contain any extra characters
                StreamReader sr = new StreamReader(filename);
                string contents = sr.ReadToEnd();
                sr.Close();

                if (contents.Contains("Oatc.OpenMI.Gui.Trigger"))
                {
                    int end = contents.IndexOf("Oatc.OpenMI.Gui.Trigger");
                    int index = end-1;
                    int count = 0;
                    while (contents[index] != '\"')
                    {
                        count++;
                        index--;
                    }

                    contents = contents.Remove(end - count, count);
                }

                StreamWriter sw = new StreamWriter(filename);
                sw.Write(contents);
                sw.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Composition cannot be saved, make sure the file is not write-protected. Details: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //refresh filelist
            this.UpdateFileList(((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText);
        }


        public void menuFileSaveAs_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            SaveFileDialog dlgFile = new SaveFileDialog();
            dlgFile.Filter = "OmiEd projects (*.opr)|*.opr|All files|*.*";
            dlgFile.ValidateNames = true;
            dlgFile.Title = "Save project As...";
            dlgFile.AddExtension = true;
            dlgFile.OverwritePrompt = true;

            if (_compositionFilename != null)
                dlgFile.FileName = _compositionFilename;
            else
                dlgFile.FileName = DefaultFilename;

            if (dlgFile.ShowDialog(this) != DialogResult.OK)
            {
                dlgFile.Dispose();
                return;
            }

            try
            {
                _composition.SaveToFile(dlgFile.FileName);
                _compositionFilename = dlgFile.FileName;

                //HACK:
                //make sure that the trigger does not contain any extra characters
                string filename = dlgFile.FileName;
                StreamReader sr = new StreamReader(filename);
                string contents = sr.ReadToEnd();
                sr.Close();

                if (contents.Contains("Oatc.OpenMI.Gui.Trigger"))
                {
                    int end = contents.IndexOf("Oatc.OpenMI.Gui.Trigger");
                    int index = end - 1;
                    int count = 0;
                    while (contents[index] != '\"')
                    {
                        count++;
                        index--;
                    }

                    contents = contents.Remove(end - count, count);
                }

                StreamWriter sw = new StreamWriter(filename);
                sw.Write(contents);
                sw.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Composition cannot be saved, make sure the file is not write-protected. Details: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            dlgFile.Dispose();

            //refresh filelist
            this.UpdateFileList(((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText);
            UpdateTitle();
        }


        public void menuFileReload_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            _composition.Reload();

        }


        private void menuFileExit_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            //Close();		
        }


        public void menuEditConnectionAdd_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            _isAddingConnection = true;
            compositionBox.Cursor = _sourceCursor;
            Cursor.Current = _sourceCursor;

        }


        private void menuViewModelProperties_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            _modelDialog.PopulateDialog(_composition.Models);
            _modelDialog.ShowDialog(this);
        }


        private void menuRegisterExtensions_Click(object sender, System.EventArgs e)
        {
            if (menuRegisterExtensions.Checked)
            {
                Utils.UnregisterFileExtensions();
                menuRegisterExtensions.Checked = false;
            }
            else
            {
                Utils.RegisterFileExtensions(Application.ExecutablePath);
                menuRegisterExtensions.Checked = true;
            }
        }


        private void menuHelpAbout_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            _aboutBox.ShowDialog(this);
        }


        private void menuHelpContents_Click(object sender, System.EventArgs e)
        {
            StopAllActions();

            FileInfo fileInfo = new FileInfo(Application.StartupPath + "\\Help.html");

            if (!fileInfo.Exists)
                fileInfo = new FileInfo(Application.StartupPath + "\\HelpPage.htm");

            // trick to open file in project directory (exe is in "projdir\bin\debug")
            // if not found in startup directory
            if (!fileInfo.Exists)
                fileInfo = new FileInfo(Application.StartupPath + "\\..\\..\\HelpPage.htm");

            if (fileInfo.Exists)
            {
                ProcessStartInfo info = new ProcessStartInfo(fileInfo.FullName);
                Process.Start(info);
            }
        }


        #endregion

        #region Context menu event handlers

        private void contextMenu_Popup(object sender, System.EventArgs e)
        {
            StopAllActions();

            contextConnectionRemove.Visible = true;
            contextConnectionProperties.Visible = true;
            contextModelProperties.Visible = true;
            contextModelRemove.Visible = true;
            contextAddTrigger.Visible = true;
            contextRun.Visible = true;
            contextConnectionAdd.Visible = true;


            if (_contextSelectedObject == null)
            {
                contextDivider.Visible = false;
                contextConnectionRemove.Visible = false;
                contextConnectionProperties.Visible = false;
                contextModelProperties.Visible = false;
                contextModelRemove.Visible = false;
            }
            else if (_contextSelectedObject is UIConnection)
            {
                contextDivider.Visible = true;
                contextConnectionRemove.Visible = true;
                contextConnectionProperties.Visible = true;
                contextModelProperties.Visible = false;
                contextModelRemove.Visible = false;
            }
            else if (_contextSelectedObject is UIModel)
            {
                contextDivider.Visible = true;
                contextConnectionRemove.Visible = false;
                contextConnectionProperties.Visible = false;
                contextModelProperties.Visible = true;
                contextModelRemove.Visible = true;
            }
            else
                Debug.Assert(false);

            // Make disabled items invisible
            if (!contextConnectionRemove.Enabled)
                contextConnectionRemove.Visible = false;
            if (!contextConnectionProperties.Enabled)
                contextConnectionProperties.Visible = false;
            if (!contextModelProperties.Enabled)
                contextModelProperties.Visible = false;
            if (!contextModelRemove.Enabled)
                contextModelRemove.Visible = false;
            if (!contextAddTrigger.Enabled)
                contextAddTrigger.Visible = false;
            if (!contextRun.Enabled)
                contextRun.Visible = false;
            if (!contextConnectionAdd.Enabled)
                contextConnectionAdd.Visible = false;
        }


        private void contextConnectionAdd_Click(object sender, System.EventArgs e)
        {
            menuEditConnectionAdd_Click(sender, e);
            CompositionUpdateArea();
            //UpdateControls();
            UpdateTitle();
        }

        private void contextConnectionRemove_Click(object sender, System.EventArgs e)
        {
            _composition.RemoveConnection((UIConnection)_contextSelectedObject);
            CompositionUpdateArea();
            UpdateControls();
            UpdateTitle();

        }

        private void contextConnectionProperties_Click(object sender, System.EventArgs e)
        {
            ShowLinkDialog((UIConnection)_contextSelectedObject);
            UpdateTitle();
        }

        private void contextModelAdd_Click(object sender, System.EventArgs e)
        {
            menuEditModelAdd_Click(sender, e);
        }



        private void contextModelRemove_Click(object sender, System.EventArgs e)
        {
            _composition.RemoveModel((UIModel)_contextSelectedObject);

            //--- update ribbon controls ---
            //disable "add connection"
            if (_composition.Models.Count - 1 <= _composition.Connections.Count)
                ((RibbonButton)rps[1].Items[2]).Enabled = false;
            //disable "run"
            if(!_composition.HasTrigger())
                ((RibbonButton)rps[1].Items[3]).Enabled = false;

            CompositionUpdateArea();
            UpdateControls();
            UpdateTitle();
        }

        private void contextModelProperties_Click(object sender, System.EventArgs e)
        {
            _modelDialog.PopulateDialog(_composition.Models, ((UIModel)_contextSelectedObject).ModelID);
            _modelDialog.ShowDialog(this);
        }

        private void contextRun_Click(object sender, System.EventArgs e)
        {
            menuDeployRun_Click(sender, e);
        }


        private void contextAddTrigger_Click(object sender, System.EventArgs e)
        {
            menuEditTriggerAdd_Click(sender, e);

            //enable run button
            if(_composition.Models.Count - _composition.Connections.Count <= 1)
                if(_composition.HasTrigger())
                    ((RibbonButton)rps[1].Items[3]).Enabled = true;

        }


        #endregion



        #region Composition box event handlers

        private void compositionScrollBar_ValueChanged(object sender, System.EventArgs e)
        {
            compositionBox.Invalidate();
        }

        public void compositionBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            // draw OpenMI logo
            //e.Graphics.DrawImage(_HMimage, 30, 30);
            List<Point[]> points = new List<Point[]>();
            //draw links
            foreach (UIConnection link in _composition.Connections)
            {
                try
                {
                    //link.Draw(_compositionBoxPositionInArea, e.Graphics, DotSpatial.Tools.ModelShapes.Arrow);
                    Point[] p = link.Draw(_compositionBoxPositionInArea, e.Graphics);
                

                //save triangle points if length is > 0
                if (p.Length > 0)
                    points.Add(p);

                }
                catch (OutOfMemoryException) { }

            }
            //fill in arrows
            foreach (UIConnection link in _composition.Connections)
            {
                //link.Draw(_compositionBoxPositionInArea, e.Graphics, DotSpatial.Tools.ModelShapes.Arrow);
                link.FillArrows(points, e.Graphics);
            }
            





            foreach (UIModel model in _composition.Models)
            {
                //model.Draw(_compositionBoxPositionInArea, e.Graphics);
                model.Draw(_compositionBoxPositionInArea, e.Graphics, model.Shape);
            }


            //enable / disable "trigger"
            if(!_composition.HasTrigger())
                ((RibbonButton)rps[1].Items[1]).Enabled = true;         
            else
                ((RibbonButton)rps[1].Items[1]).Enabled = false;

            //enable / disable "add connection"
            if(_composition.Models.Count >= 2)
                ((RibbonButton)rps[1].Items[2]).Enabled = true;
            else
                ((RibbonButton)rps[1].Items[2]).Enabled = false;

            //enable / disable "run"

            if (_composition.Models.Count - _composition.Connections.Count <= 1)
            {
                if (_composition.HasTrigger())
                    ((RibbonButton)rps[1].Items[3]).Enabled = true;
            }
            else
            {
                ((RibbonButton)rps[1].Items[3]).Enabled = false;
            }



            // Draw link currently being added (if any)
            //if( _isAddingLink && _leftMouseButtonIsDown )
            //	UIConnection.DrawLink( (float)_prevMouse.X, (float)_prevMouse.Y, (float)_currentMouse.X, (float)_currentMouse.Y, _compositionBoxPositionInArea, e.Graphics);
        }

        //TODO: zoom in/out with mouse wheel
        private void compositionBox_mousewheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            //int newWidth = compositionBox.Width, newHeight = compositionBox.Height, newX = compositionBox.Location.X, newY = compositionBox.Location.Y;

            //if (e.Delta > 0)
            //{
            //    newWidth = compositionBox.Size.Width + (compositionBox.Size.Width / 10);
            //    newHeight = compositionBox.Size.Height + (compositionBox.Size.Height / 10);
            //    newX = compositionBox.Location.X - ((compositionBox.Size.Width / 10) / 2);
            //    newY = compositionBox.Location.Y - ((compositionBox.Size.Height / 10) / 2);
            //}

            //else if (e.Delta < 0)
            //{
            //    newWidth = compositionBox.Size.Width - (compositionBox.Size.Width / 10);
            //    newHeight = compositionBox.Size.Height - (compositionBox.Size.Height / 10);
            //    newX = compositionBox.Location.X + ((compositionBox.Size.Width / 10) / 2);
            //    newY = compositionBox.Location.Y + ((compositionBox.Size.Height / 10) / 2);

            //    // Prevent image from zooming out beyond original size
            //    if (newWidth < compositionBox.Width)
            //    {
            //        newWidth = compositionBox.Width;
            //        newHeight = compositionBox.Height;
            //        newX = 0;
            //        newY = 0;
            //    }
            //}
            //compositionBox.Size = new Size(newWidth, newHeight);
            //compositionBox.Location = new Point(newX, newY);
        }

        private void compositionBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            StopMovingModel();
            compositionBox.Invalidate();

            bool actionFoundOut = false;

            // Left mouse button
            if (e.Button == MouseButtons.Left)
            {

                // if adding a connection
                if (_isAddingConnection)
                {
                    UIModel model = GetModel(e.X, e.Y);

                    // if some model selected
                    if (model != null)
                    {
                        // if source model selected
                        if (_sourceModel == null)
                        {
                            _sourceModel = model;
                            compositionBox.Cursor = _targetCursor;
                        }
                        else
                        {
                            // target model selected => add connection to composition
                            if (_sourceModel != model)
                                _composition.AddConnection(_sourceModel, model);
                            StopAddingConnection();
                            UpdateControls();

                            //reset cursor
                            compositionBox.Cursor = Cursors.Default;

                        }
                    }
                    else
                    {
                        // no model selected
                        StopAddingConnection();
                    }

                    actionFoundOut = true;
                }

                // move model ?
                if (!actionFoundOut)
                {
                    UIModel model = GetModel(e.X, e.Y);

                    if (model != null)
                    {
                        _prevMouse.X = e.X;
                        _prevMouse.Y = e.Y;

                        _isMovingModel = true;
                        model.IsMoving = true;

                        actionFoundOut = true;
                    }
                    //if in pan mode
                    else if (Ispan)
                    {
                        _prevMouse.X = e.X;
                        _prevMouse.Y = e.Y;
                        actionFoundOut = true;
                    }
                }

                // or show link dialog ?
                if (!actionFoundOut)
                {
                    UIConnection connection = GetConnection(e.X, e.Y);
                    if (connection != null)
                        ShowLinkDialog(connection);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // right button => show context menu

                // stop other actions
                StopAddingConnection();
                StopMovingModel();

                // get model under cursor
                _contextSelectedObject = GetModel(e.X, e.Y);
                if (_contextSelectedObject == null)
                    _contextSelectedObject = GetConnection(e.X, e.Y);

                contextMenu.Show(compositionBox, new Point(e.X, e.Y));
            }
        }

        private void compositionBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //get the model under the cursor
            UIModel M = GetModel(e.X, e.Y);

            // moving model ?
            if (_isMovingModel)
            {
                foreach (UIModel model in _composition.Models)
                    if (model.IsMoving)
                    {
                        model.Rect.X += e.X - _prevMouse.X;
                        model.Rect.Y += e.Y - _prevMouse.Y;

                        _prevMouse.X = e.X;
                        _prevMouse.Y = e.Y;

                        _composition.ShouldBeSaved = true;
                        CompositionUpdateArea();
                        UpdateTitle();
                        compositionBox.Invalidate();
                    }
            }
            //if pan is selected, and no models are
            else if (Ispan && e.Button == System.Windows.Forms.MouseButtons.Left && M == null)
            {
                foreach (UIModel model in _composition.Models)
                {
                    model.Rect.X += e.X - _prevMouse.X;
                    model.Rect.Y += e.Y - _prevMouse.Y;
                    _composition.ShouldBeSaved = true;
                    CompositionUpdateArea();
                    UpdateTitle();
                    compositionBox.Invalidate();
                }
                _prevMouse.X = e.X;
                _prevMouse.Y = e.Y;
            }

        }

        /// <summary>
        /// Stops the selected model from moving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compositionBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            StopMovingModel();
        }

        /// <summary>
        /// Change the cursor into pan mode when it leaves the composition box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compositionBox_MouseEnter(object sender, EventArgs e)
        {
            if (Ispan)
            {
                compositionBox.Cursor = _panCursor;
            }
        }

        /// <summary>
        /// change the cursor back to default mode when it leaves the composition box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compositionBox_MouseLeave(object sender, EventArgs e)
        {
            compositionBox.Cursor = Cursors.Default;
        }
        #endregion

        #region .NET generated members

        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>		
        static void Main(string[] args)
        {
            try
            {
                ProceedCommandLineArgs(args);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error occured while starting the application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        /// 


        /// <summary>
        /// Initializes the HydoDesktop Ribbon.  Also calls the three methods below.
        /// </summary>
        private void InitializeComponentRibbon()
        {
            this.components = new System.ComponentModel.Container();

            InitializeOldMenu();
            InitializeContext();
            InitializeComposition();

            // mainTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            //add controls



            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "mainTab";
            this.Size = new System.Drawing.Size(602, 288);

            //add event handelers to the maintab
            this.Load += new System.EventHandler(this.mainTab_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.mainTab_DragDrop);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mainTab_KeyPress);
            this.SizeChanged += new System.EventHandler(this.mainTab_SizeChanged);

            //finish initialization
            ((System.ComponentModel.ISupportInitialize)(this.compositionBox)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.fileList)).EndInit();

            this.container.ResumeLayout(false);
            
            //container.PerformAutoScale();
            container.PerformLayout();
            this.ResumeLayout(false);

            
        }
        /// <summary>
        /// Initializes the orignal OpenMI Editor menu and variables
        /// </summary>
        private void InitializeOldMenu()
        {
            this.menuFile = new ToolStripMenuItem();
            this.menuFile.Text = "HydroModeler";
            this.menuFile.Name = "hydroModelerMenuItem";

            this.menuFileNew = new ToolStripMenuItem();
            this.menuFileNew.Text = "&New composition";
            this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
            this.menuFile.DropDownItems.Add(this.menuFileNew);

            this.menuFileReload = new ToolStripMenuItem();
            this.menuFileReload.Text = "&Reload composition";
            this.menuFileReload.Click += new System.EventHandler(this.menuFileReload_Click);
            this.menuFile.DropDownItems.Add(this.menuFileReload);

            this.menuFileOpen = new ToolStripMenuItem();
            this.menuFileOpen.Text = "&Open composition...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            this.menuFile.DropDownItems.Add(this.menuFileOpen);

            this.menuFileSave = new ToolStripMenuItem();
            this.menuFileSave.Text = "&Save composition";
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
            this.menuFile.DropDownItems.Add(this.menuFileSave);

            this.menuFileSaveAs = new ToolStripMenuItem();
            this.menuFileSaveAs.Text = "Save composition &As...";
            this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
            this.menuFile.DropDownItems.Add(this.menuFileSaveAs);

            this.menuFileExit = new ToolStripMenuItem();
            this.menuFileExit.Text = "E&xit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);

            this.menuFile.DropDownItems.Add(new ToolStripSeparator());

            this.menuEditModelAdd = new ToolStripMenuItem();
            this.menuEditModelAdd.Text = "Add &Model";
            this.menuEditModelAdd.Click += new System.EventHandler(this.menuEditModelAdd_Click);
            this.menuFile.DropDownItems.Add(this.menuEditModelAdd);

            this.menuEditConnectionAdd = new ToolStripMenuItem();
            this.menuEditConnectionAdd.Enabled = false;
            this.menuEditConnectionAdd.Text = "Add &Connection";
            this.menuEditConnectionAdd.Click += new System.EventHandler(this.menuEditConnectionAdd_Click);
            this.menuFile.DropDownItems.Add(this.menuEditConnectionAdd);

            this.menuEditTriggerAdd = new ToolStripMenuItem();
            this.menuEditTriggerAdd.Text = "Add &Trigger";
            this.menuEditTriggerAdd.Click += new System.EventHandler(this.menuEditTriggerAdd_Click);
            this.menuFile.DropDownItems.Add(this.menuEditTriggerAdd);

            this.menuEditConnectionProperties = new ToolStripMenuItem();
            this.menuEditConnectionProperties.Enabled = false;
            this.menuEditConnectionProperties.Text = "Co&nnection properties...";
            this.menuFile.DropDownItems.Add(this.menuEditConnectionProperties);

            this.menuViewModelProperties = new ToolStripMenuItem();
            this.menuViewModelProperties.Text = "Model &properties...";
            this.menuViewModelProperties.Click += new System.EventHandler(this.menuViewModelProperties_Click);
            this.menuFile.DropDownItems.Add(this.menuViewModelProperties);

            this.menuEditRunProperties = new ToolStripMenuItem();
            this.menuEditRunProperties.Text = "&Run...";
            this.menuEditRunProperties.Click += new System.EventHandler(this.menuDeployRun_Click);
            this.menuFile.DropDownItems.Add(this.menuEditRunProperties);

            this.menuRegisterExtensions = new ToolStripMenuItem();
            this.menuRegisterExtensions.Checked = true;
            this.menuRegisterExtensions.Text = "&Register file extensions";
            this.menuRegisterExtensions.Click += new System.EventHandler(this.menuRegisterExtensions_Click);

            this.menuOptions = new ToolStripMenuItem();
            this.menuOptions.Text = "&Options";
            this.menuOptions.DropDownItems.Add(this.menuRegisterExtensions);
            this.menuFile.DropDownItems.Add(this.menuOptions);

            this.menuFile.DropDownItems.Add(new ToolStripSeparator());

            this.menuHelp = new ToolStripMenuItem();
            this.menuHelp.Text = "&Help";
            //_ mapArgs.MainMenu.Items.Add(this.menuHelp); don't include for now.

            this.menuHelpAbout = new ToolStripMenuItem();
            this.menuHelpAbout.Text = "&About Configuration Editor ...";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            this.menuFile.DropDownItems.Add(this.menuHelpAbout);

            this.menuHelpContents = new ToolStripMenuItem();
            this.menuHelpContents.Text = "Help contents";
            this.menuHelpContents.Click += new System.EventHandler(this.menuHelpContents_Click);
            this.menuFile.DropDownItems.Add(this.menuHelpContents);

            //add an event handler for launching the Help file via F1
            this.HelpRequested += new HelpEventHandler(mainTab_HelpRequested);

        }

        void mainTab_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            LocalHelp.OpenHelpFile(_localHelpUri);
            hlpevent.Handled = true;
        }

        /// <summary>
        /// Initializes the context menus for the HydroModeler Model window and File Browser window
        /// </summary>
        private void InitializeContext()
        {
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.contextConfigurationAdd = new System.Windows.Forms.MenuItem();
            this.contextModelAdd = new System.Windows.Forms.MenuItem();
            this.contextConnectionAdd = new System.Windows.Forms.MenuItem();
            this.contextAddTrigger = new System.Windows.Forms.MenuItem();
            this.contextRun = new System.Windows.Forms.MenuItem();
            this.contextDivider = new System.Windows.Forms.MenuItem();
            this.contextConnectionRemove = new System.Windows.Forms.MenuItem();
            this.contextConnectionProperties = new System.Windows.Forms.MenuItem();
            this.contextModelRemove = new System.Windows.Forms.MenuItem();
            this.contextModelProperties = new System.Windows.Forms.MenuItem();
            // 
            // contextMenu
            // 
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.contextConfigurationAdd,
            this.contextModelAdd,
            this.contextConnectionAdd,
            this.contextAddTrigger,
            this.contextRun,
            this.contextDivider,
            this.contextConnectionRemove,
            this.contextConnectionProperties,
            this.contextModelRemove,
            this.contextModelProperties});
            this.contextMenu.Popup += new System.EventHandler(this.contextMenu_Popup);
            // 
            // contextConfigurationAdd
            // 
            this.contextConfigurationAdd.Index = 0;
            this.contextConfigurationAdd.Text = "Add Configuration";
            this.contextConfigurationAdd.Click += new System.EventHandler(this.contextConfigurationAdd_Click);
            // 
            // contextModelAdd
            // 
            this.contextModelAdd.Index = 1;
            this.contextModelAdd.Text = "Add Model";
            this.contextModelAdd.Click += new System.EventHandler(this.contextModelAdd_Click);
            // 
            // contextConnectionAdd
            // 
            this.contextConnectionAdd.Index = 2;
            this.contextConnectionAdd.Text = "Add Connection";
            this.contextConnectionAdd.Click += new System.EventHandler(this.contextConnectionAdd_Click);
            // 
            // contextAddTrigger
            // 
            this.contextAddTrigger.Index = 3;
            this.contextAddTrigger.Text = "Add Trigger";
            this.contextAddTrigger.Click += new System.EventHandler(this.contextAddTrigger_Click);
            // 
            // contextRun
            // 
            //this.contextRun.Index = 4;
            this.contextRun.Text = "Run";
            this.contextRun.Click += new System.EventHandler(this.contextRun_Click);
            // 
            // contextDivider
            // 
            this.contextDivider.Index = 5;
            this.contextDivider.Text = "-";
            // 
            // contextConnectionRemove
            // 
            this.contextConnectionRemove.Index = 6;
            this.contextConnectionRemove.Text = "Remove connection";
            this.contextConnectionRemove.Click += new System.EventHandler(this.contextConnectionRemove_Click);
            // 
            // contextConnectionProperties
            // 
            this.contextConnectionProperties.Index = 7;
            this.contextConnectionProperties.Text = "Connection properties";
            this.contextConnectionProperties.Click += new System.EventHandler(this.contextConnectionProperties_Click);
            // 
            // contextModelRemove
            // 
            this.contextModelRemove.Index = 8;
            this.contextModelRemove.Text = "Remove model";
            this.contextModelRemove.Click += new System.EventHandler(this.contextModelRemove_Click);
            // 
            // contextModelProperties
            // 
            this.contextModelProperties.Index = 9;
            this.contextModelProperties.Text = "Model properties";
            this.contextModelProperties.Click += new System.EventHandler(this.contextModelProperties_Click);
            //
            // FileList Context Menu
            //
            this.filelist_context = new ContextMenu();
            this.add_comp = new MenuItem();
            this.add_model = new MenuItem();
            this.delete = new MenuItem();
            this.filelist_context.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.add_comp,
            this.add_model,
            this.delete});
            // 
            // filelist_context Add Configuration
            // 
            this.add_model.Index = 0;
            this.add_model.Text = "Add Model";
            this.add_model.Click += new System.EventHandler(this.ClickModel);
            // 
            // filelist_context Add Model
            // 
            this.add_comp.Index = 1;
            this.add_comp.Text = "Add Component";
            this.add_comp.Click += new System.EventHandler(this.ClickModel);
            //
            // filelist_context Delete
            //
            this.delete.Index = 2;
            this.delete.Text = "Delete";
            this.delete.Click += new EventHandler(delete_Click);
        }


        /// <summary>
        /// Initializes the HydroModeler composition items, including the modeler window, file browser, and properties window.
        /// </summary>
        private void InitializeComposition()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainTab));

            this.compositionBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.compositionBox)).BeginInit();
            this.container = new SplitContainer();
            this.container2 = new SplitContainer();
            this.comp_container = new SplitContainer();

            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.ListPanel = new Panel();
            this.fileList = new ListView();
            this.properties = new ListView();
            this.tb_navigate = new TextBox();
            this.changeDir = new Button();
            this.currentDir = new Label();
            this.emptylabel1 = new Label();
            this.emptylabel2 = new Label();
            this._hmImage = new ImageList();
            this.dirDialog = new FolderBrowserDialog();
            this.AddModelButton = new Button();
            this._Save = new Button();
            this.t = new TextBox();
            this.container.SuspendLayout();
            this.ListPanel.SuspendLayout();
            this.compositionBox.SuspendLayout();
            this.SuspendLayout();
            
            //
            // SplitContainer1
            //
            this.container.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(0, 0);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(602, 288);
            this.container.SplitterDistance = 200;
            this.container.SplitterMoved += new SplitterEventHandler(container_SplitterMoved);
            this.Controls.Add(container);   //add container as a form control
            // SplitContainer1.Panel2
            this.container.Panel2.AutoScroll = true;
            //this.container.Panel2.Controls.Add(this.compositionBox);
            //
            // SplitContainer2
            //
            this.container2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.container2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container2.Location = new System.Drawing.Point(0, 0);
            this.container2.Name = "container2";
            this.container2.Size = new System.Drawing.Size(602, 288);
            this.container2.Orientation = Orientation.Horizontal;
            this.container2.SplitterDistance = (int)((2.0/3.0)*(double)container.Panel1.Height);
            this.container2.SplitterMoved += new SplitterEventHandler(container2_SplitterMoved);
            this.container.Panel1.Controls.Add(container2);
            //
            // Composition Split Container
            //
            //this.comp_container.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //this.comp_container.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.comp_container.Location = new System.Drawing.Point(0, 0);
            //this.comp_container.Name = "comp_container";
            //this.comp_container.Size = new System.Drawing.Size(602, 288);
            //this.comp_container.Orientation = Orientation.Horizontal;
            //this.comp_container.SplitterDistance = (int)((2.0 / 3.0) * (double)container.Panel1.Height);
            ////this.comp_container.SplitterMoved += new SplitterEventHandler(container2_SplitterMoved);
            //this.container.Panel2.Controls.Add(comp_container);
            //
            // compositionBox
            // 
            this.compositionBox.BackColor = System.Drawing.Color.White;
            this.compositionBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.compositionBox.Dock = DockStyle.Fill;
            this.compositionBox.Location = new System.Drawing.Point(0, 0);
            this.compositionBox.Name = "compositionBox";
            this.compositionBox.SizeMode = PictureBoxSizeMode.AutoSize;
            this.compositionBox.TabIndex = 3;
            this.compositionBox.TabStop = false;
            this.compositionBox.AllowDrop = true;
            this.compositionBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseMove);
            this.compositionBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseDown);
            this.compositionBox.Paint += new System.Windows.Forms.PaintEventHandler(this.compositionBox_Paint);
            this.compositionBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseUp);
            this.compositionBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragEnterModel);
            this.compositionBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropModel);
            this.compositionBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.compositionBox_mousewheel);
            this.compositionBox.MouseEnter += new EventHandler(compositionBox_MouseEnter);
            this.compositionBox.MouseLeave += new EventHandler(compositionBox_MouseLeave);
            this.container.Panel2.Controls.Add(this.compositionBox);
            //this.comp_container.Panel1.Controls.Add(this.compositionBox);
            // 
            // imageList
            // 
            //set opacity of _HMImage
            //Image hm_image = Bitmap.FromFile(listviewImagesPath + _HmLogo);
            //Bitmap temp = new Bitmap(hm_image.Width,hm_image.Height);
            //Graphics gfx = Graphics.FromImage(temp);
            //ColorMatrix cmx = new ColorMatrix();
            //cmx.Matrix33 = 0.2f;
            //ImageAttributes ia = new ImageAttributes();
            //ia.SetColorMatrix(cmx, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //gfx.DrawImage(hm_image, new Rectangle(0, 0, temp.Width, temp.Height), 0, 0, hm_image.Width, hm_image.Height, GraphicsUnit.Pixel, ia);
            //gfx.Dispose();
            //this._HMimage = temp;
            //this.imageList.Images.Add(temp);
            //this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            //this.imageList.Images.SetKeyName(0, "");
            //
            // List View
            //
            this.fileList.BackColor = System.Drawing.Color.GhostWhite;
            this.fileList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fileList.Dock = DockStyle.None;
            this.fileList.Location = new System.Drawing.Point(0, 37);
            this.fileList.Size = new Size(245, this.container2.Panel1.Height - 2);
            this.fileList.Name = "fileList";

            ImageList _imagesList = new ImageList();
            //_imagesList.Images.Add(HydroModeler.Properties.Resources.component_image.GetThumbnailImage(10, 10, null, IntPtr.Zero));
            //_imagesList.Images.Add(HydroModeler.Properties.Resources.model_image.GetThumbnailImage(10,10,null,IntPtr.Zero));
            _imagesList.Images.Add(HydroModeler.Properties.Resources.component_image.ToBitmap());
            _imagesList.Images.Add(HydroModeler.Properties.Resources.model_image.ToBitmap());
            _imagesList.Images.Add(HydroModeler.Properties.Resources.Folder.ToBitmap());
            //_imagesList.Images.Add(Bitmap.FromFile(listviewImagesPath + _xmlIcon));
            fileList.SmallImageList = _imagesList;

            ColumnHeader col = new ColumnHeader();
            col.Text = "Name";
            col.Width = 145;
            col.TextAlign = HorizontalAlignment.Left;
            this.fileList.Columns.Add(col);

            col = new ColumnHeader();
            col.Text = "Type";
            col.Width = 50;
            col.TextAlign = HorizontalAlignment.Left;
            this.fileList.Columns.Add(col);

            col = new ColumnHeader();
            col.Text = "Details";
            col.Width = 50;
            col.TextAlign = HorizontalAlignment.Left;
            this.fileList.Columns.Add(col);

            this.fileList.AllowColumnReorder = true;
            //this.fileList.GridLines = true;
            this.fileList.View = View.Details;
            this.fileList.FullRowSelect = true;
            this.fileList.MultiSelect = false;
            this.fileList.DoubleClick += new System.EventHandler(this.ClickModel);
            this.fileList.MouseDown += new MouseEventHandler(fileList_Click);
            this.fileList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.filelist_itemDrag);
            this.fileList.SizeChanged += new EventHandler(fileList_SizeChanged);
            this.fileList.SelectedIndexChanged += new EventHandler(fileList_SelectedIndexChanged);
            this.container2.Panel1.Controls.Add(this.fileList); //add to container2 panel 1
            
            //
            // Label: Double Click or Drag
            //
            this.emptylabel2.Location = new Point(0, 3);
            this.emptylabel2.Text = "Double click, or drag the desired model to add it to the HydroModeler composition window.";
            this.emptylabel2.Size = new Size(245, 35);
            this.emptylabel2.Font = new Font(emptylabel2.Font, FontStyle.Italic);
            this.container2.Panel1.Controls.Add(this.emptylabel2);
            //
            // Add Model Button
            //
            this.AddModelButton.Text = "Add Item >>";
            this.AddModelButton.Size = new Size(75, 30);
            this.AddModelButton.Location = new Point(135, this.container2.Panel2.Height-35);
            this.AddModelButton.Click += new EventHandler(ClickModel);
            this.container2.Panel2.Controls.Add(this.AddModelButton);
            //
            // Save Properties Button
            //
            this._Save.Text = "Save";
            this._Save.Size = new Size(75, 30);
            this._Save.Location = new Point(50, this.container2.Panel2.Height - 35);
            this._Save.Enabled = false;
            this._Save.Visible = true;
            this._Save.Click += new EventHandler(_Save_Click);
            this.container2.Panel2.Controls.Add(this._Save);
            //
            // Dialog Directory
            //
            this.dirDialog.ShowNewFolderButton = false;
            //
            // textbox
            //
            this.t.Location = new Point(0, this.container2.Panel2.Height - 35);
            this.t.Size = new Size(75, 30);
            this.t.Text = "Test";
            this.t.Visible = false;
            this.t.KeyPress += new KeyPressEventHandler(textbox_KeyPress);
            this.t.LostFocus += new EventHandler(tb_LostFocus);
            this.container2.Panel2.Controls.Add(this.t);
            //
            // Properties Window
            //
            this.properties.BackColor = System.Drawing.Color.GhostWhite;
            this.properties.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.properties.Dock = DockStyle.None;
            this.properties.Height = (int)(0.75 * (double)container2.Panel2.Height);
            this.properties.Width = container2.Panel2.Width;
            this.properties.Location = new System.Drawing.Point(2, 0);
            this.properties.Name = "properties";

            col = new ColumnHeader();
            col.Text = "Property";
            col.Width = 125;
            col.TextAlign = HorizontalAlignment.Left;
            this.properties.Columns.Add(col);

            col = new ColumnHeader();
            col.Text = "Value";
            col.Width = 70;
            col.TextAlign = HorizontalAlignment.Left;
            this.properties.Columns.Add(col);

            this.properties.AllowColumnReorder = true;
            this.properties.GridLines = true;
            this.properties.View = View.Details;
            this.properties.FullRowSelect = true;
            this.properties.MultiSelect = false;
            this.properties.LabelEdit = false;
            this.properties.MouseDown += new MouseEventHandler(properties_MouseDown);
            this.properties.ColumnWidthChanging += new ColumnWidthChangingEventHandler(properties_ColumnWidthChanging);

            this.container2.Panel2.Controls.Add(this.properties); //add to container2 panel 2

            //
            // Output text box
            //
            //this.output_box = new TextBox();
            //this.output_box.AcceptsReturn = true;
            //this.output_box.Dock = DockStyle.Fill;
            //this.output_box.BackColor = System.Drawing.Color.Black; ;
            //this.output_box.ForeColor = System.Drawing.Color.White;
            //this.output_box.Multiline = true;
            //this.output_box.ReadOnly = true;
            //this.output_box.WordWrap = true;
            ////this.output_box.f
            //this.comp_container.Panel2.Controls.Add(this.output_box);
            
        }

        /*
        private void InitializeComponent()
        {


            this.components = new System.ComponentModel.Container();            

            //InitializeMyMenu();
            InitializeOldMenu(); 
            InitializeContext();
            InitializeComposition();
// 
            // mainTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.compositionVScrollBar);
            this.Controls.Add(this.compositionBox);
            this.Controls.Add(this.compositionHScrollBar);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "mainTab";
            this.Size = new System.Drawing.Size(602, 288);
            this.Load += new System.EventHandler(this.mainTab_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.mainTab_DragDrop);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mainTab_KeyPress);
            this.SizeChanged += new System.EventHandler(this.mainTab_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.compositionBox)).EndInit();
            this.ResumeLayout(false);

            // Add UI features
            //btnHydroModelerPlugin.DisplayStyle = ToolStripItemDisplayStyle.Text;
            //btnSamplePlugin.Image = Resources.MySampleIcon1.ToBitmap();
            //btnHydroModelerPlugin.Name = "HydroModeler";
            //btnHydroModelerPlugin.ToolTipText = "Launch HydroModeler";
            //btnHydroModelerPlugin.Click += new EventHandler(btnSamplePlugin_Click);

        }
        */
        /*
        private void InitializeMyMenu()
        {
            ////create the menu, add the items and attatch the EventHandelers
            mnuHydroModelerPlugin = new ToolStripMenuItem("HydroModeler");

            ToolStripMenuItem mnuNew = new ToolStripMenuItem("New Composition");
            mnuHydroModelerPlugin.DropDownItems.Add(mnuNew);

            mnuHydroModelerPlugin.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem mnuReload = new ToolStripMenuItem("Reload Composition");
            mnuHydroModelerPlugin.DropDownItems.Add(mnuReload);
            ToolStripMenuItem mnuOpen = new ToolStripMenuItem("Open Composition");
            mnuHydroModelerPlugin.DropDownItems.Add(mnuOpen);
            ToolStripMenuItem mnuSave = new ToolStripMenuItem("Save Composition");
            mnuHydroModelerPlugin.DropDownItems.Add(mnuSave);
            ToolStripMenuItem mnuSaveAs = new ToolStripMenuItem("Save Composition As ...");
            mnuHydroModelerPlugin.DropDownItems.Add(mnuSaveAs);

            mnuHydroModelerPlugin.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem mnuClose = new ToolStripMenuItem("Close");
            mnuHydroModelerPlugin.DropDownItems.Add(mnuClose);

            if (_mapArgs.MainMenu != null)
            {
                _mapArgs.MainMenu.Items.Add(mnuHydroModelerPlugin);
                mnuNew.Click += new EventHandler(menuFileNew_Click);
                mnuReload.Click += new EventHandler(menuFileReload_Click);
                mnuOpen.Click += new EventHandler(menuFileOpen_Click);
                mnuSave.Click += new EventHandler(menuFileSave_Click);
                mnuSaveAs.Click += new EventHandler(menuFileSave_Click);
                //mnuClose.Click += new EventHandler(btnClose_Click);
            }
        }
        */
        #endregion




        #endregion

        #region HydroModeler Event Handelers

        private void contextConfigurationAdd_Click(object sender, EventArgs e)
        {
            menuFileOpen_Click(sender, e);
        }
        public void UpdateFileList(string path)
        {
            //pass the new directory to the runBox dialog
            _runBox._currentDirectory = path;

            //clearItems
            this.fileList.Items.Clear();

            //clear stored items
            openmiFiles.Clear();

            //clear stored folders
            Folders.Clear();

            this.fileList.BeginUpdate();
            //add new items
            if (System.IO.Directory.Exists(path))
            {
                try
                {
                    string[] omifiles = Directory.GetFiles(path, "*.omi");
                    string[] oprfiles = Directory.GetFiles(path, "*.opr");
                    //string[] xmlfiles = Directory.GetFiles(path, "*.xml");


                    string[] folders = Directory.GetDirectories(path);

                    //add omi files to dictionary
                    for (int i = 0; i <= omifiles.Length - 1; i++)
                    {
                        int omilength = omifiles[i].Split('\\').Length;
                        openmiFiles.Add(omifiles[i].Split('\\')[omilength - 1], omifiles[i]);
                    }
                    //add opr files to dictionary
                    for (int i = 0; i <= oprfiles.Length - 1; i++)
                    {
                        int oprlength = oprfiles[i].Split('\\').Length;
                        openmiFiles.Add(oprfiles[i].Split('\\')[oprlength - 1], oprfiles[i]);
                    }
                    ////add xml files to dictionary
                    //for (int i = 0; i <= xmlfiles.Length - 1; i++)
                    //{
                    //    int xmllength = xmlfiles[i].Split('\\').Length;
                    //    openmiFiles.Add(xmlfiles[i].Split('\\')[xmllength - 1], xmlfiles[i]);
                    //}

                    //
                    //--- add "up dir" to dictionary ---
                    //
                    path = path.Replace('/', '\\');

                    //remove end '\\' if there is one
                    path = path.TrimEnd('\\');

                    //convert path into array
                    string[] path_array = path.Split('\\');
                    int path_array_length = path_array.Length - 1;

                    //create new path
                    string newPath;
                    if (path_array[path_array_length].Contains(':'))
                        newPath = path_array[path_array_length] + "\\";
                    else
                    {
                        Array.Resize<string>(ref path_array, path_array_length);
                        newPath = string.Join("\\", path_array);
                    }

                    if (newPath.Split('\\').Length == 1)
                        newPath += "\\";

                    Folders.Add("...", newPath);

                    //add folders to dictionary
                    for (int i = 0; i <= folders.Length - 1; i++)
                    {
                        int folder_length = folders[i].Split('\\').Length;
                        Folders.Add(folders[i].Split('\\')[folder_length - 1], folders[i]);
                    }


                    //sort folders by name
                    var fldrs = from key in Folders.Keys
                                //            orderby Folders[key] ascending
                                select key;


                    ListViewItem li;
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem();
                    //add all other folders to listview
                    foreach (string key in fldrs)
                    {
                        //set folder name
                        li = new ListViewItem(key, 2);

                        //set type
                        lsi = new ListViewItem.ListViewSubItem();
                        lsi.Text = "Folder";
                        li.SubItems.Add(lsi);

                        //set details
                        lsi = new ListViewItem.ListViewSubItem();
                        lsi.Text = " ";
                        li.SubItems.Add(lsi);
                        fileList.Items.Add(li);
                    }

                    //sort files by name
                    var items = from k in openmiFiles.Keys
                                orderby openmiFiles[k] ascending
                                select k;

                    //add files to listview
                    foreach (string k in items)
                    {
                        //get file extension
                        string ext = k.Split('.')[1];

                        //set type and image
                        lsi = new ListViewItem.ListViewSubItem();
                        if (ext == "omi")
                        {
                            //set file name
                            li = new ListViewItem(k.Split('.')[0], 0);
                            lsi.Text = "Component";
                        }
                        else
                        {
                            //set file name
                            li = new ListViewItem(k.Split('.')[0], 1);
                            lsi.Text = "Model";
                        }
                        //else
                        //{
                        //    //set file name
                        //    li = new ListViewItem(k.Split('.')[0], 3);
                        //    lsi.Text = "Xml";
                        //}
                        li.SubItems.Add(lsi);

                        //set details
                        lsi = new ListViewItem.ListViewSubItem();
                        lsi.Text = k.Split('.')[1];
                        li.SubItems.Add(lsi);
                        fileList.Items.Add(li);
                    }
                }
                catch (Exception) { }

            }

            this.fileList.EndUpdate();
        }
        private void filelist_mouseMoving(object sender, MouseEventArgs e)
        {
            if (_isdragging)
            {
                currentX = e.X;
                currentY = e.Y;
            }
        }
        private void filelist_itemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            fileList.DoDragDrop(fileList.SelectedItems[0], DragDropEffects.Copy);
        }
        private void SelectModel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //check that a single left click was performed
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                //make sure an item has been selected
                if (fileList.SelectedItems.Count > 0)
                {
                    //make sure the item is not a folder
                    if (fileList.SelectedItems[0].SubItems[2].Text != " ")
                    {
                        if (fileList.SelectedItems[0].SubItems[2].Text == ".omi")
                        {
                            string key = fileList.SelectedItems[0].SubItems[0].Text + "." + fileList.SelectedItems[0].SubItems[2].Text;
                            //get the item path
                            string path = openmiFiles[key];
                            PopulateProperties(path);
                        }

                        _isdragging = true;
                        initialX = e.X;
                        initialY = e.Y;
                        fileList.DoDragDrop(fileList.SelectedItems[0], DragDropEffects.Copy);

                    }
                }
            }
            else if (e.Clicks > 1)
            {
            }
            else if (e.Button == MouseButtons.Right)
            {
                filelist_context_display(e);
            }
            //else if (fileList.SelectedItems.Count > 0)
            //{
            //    ClickModel(this, e);
            //}


        }
        private void fileList_Click(object sender, MouseEventArgs e)
        {

            ListViewItem lvi = fileList.GetItemAt(e.X, e.Y);
            this.fileList.Refresh();



            //make sure the item is not a folder
            if (lvi != null)
            {
                //set textbox invisable

                if (_currentFileItem != lvi)
                {
                    if (_hasChanged)
                    {
                        try
                        {
                            string path = openmiFiles[_currentFileItem.SubItems[0].Text + "." + _currentFileItem.SubItems[2].Text];
                            saveChanges(path);
                        }
                        catch (Exception) { }
                    }

                    if (t.Visible)
                        t.Visible = false;

                }

                //clear items from the properties window
                this.properties.Items.Clear();

                if (lvi.SubItems[2].Text != " ")
                {
                    //check that it is an omi file
                    if (lvi.SubItems[2].Text == "omi")
                    {
                        //set current file item
                        _currentFileItem = lvi;

                        //create a key
                        string key = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

                        //get the item path from dictionary
                        string path = openmiFiles[key];

                        //populate the properties window
                        PopulateProperties(path);

                        //set the current file
                        _currentFile = path;
                    }
                    else if (lvi.SubItems[2].Text == "opr")
                    {
                        //set current file item
                        _currentFileItem = lvi;

                        //create a key
                        string key = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

                        //get the item path from dictionary
                        string path = openmiFiles[key];

                        //populate the properties window
                        PopulateOPRProperties(path);

                        //set the current file
                        _currentFile = path;
                    }
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                filelist_context_display(e);
            }

        }
        private void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileList.SelectedItems.Count > 0)
            {
                //get the selected item
                ListViewItem lvi = fileList.SelectedItems[0];

                //clear items from the properties window
                this.properties.Items.Clear();

                if (lvi.SubItems[2].Text != " ")
                {
                    //check that it is an omi file
                    if (lvi.SubItems[2].Text == "omi")
                    {
                        //allow editing
                        _allowEdit = true;

                        //set current file item
                        _currentFileItem = lvi;

                        //create a key
                        string key = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

                        //get the item path from dictionary
                        string path = openmiFiles[key];

                        //populate the properties window
                        PopulateProperties(path);

                        //set the current file
                        _currentFile = path;
                    }
                    else if (lvi.SubItems[2].Text == "opr")
                    {
                        //dont allow editing
                        _allowEdit = false;

                        //set current file item
                        _currentFileItem = lvi;

                        //create a key
                        string key = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

                        //get the item path from dictionary
                        string path = openmiFiles[key];

                        //populate the properties window
                        PopulateOPRProperties(path);

                        //set the current file
                        _currentFile = path;
                    }
                }
            }
        }
        private void filelist_context_display(MouseEventArgs e)
        {

            //get item from list view
            ListViewItem lvi = this.fileList.GetItemAt(e.X, e.Y);

            if (lvi != null)
            {
                //highlight the selected item
                this.fileList.FocusedItem = lvi;

                //get the file extension
                string ext = lvi.SubItems[2].Text;

                if (ext == "omi")
                {
                    add_model.Enabled = false;
                    add_comp.Enabled = true;
                    delete.Enabled = true;
                }
                else if (ext == "opr")
                {
                    add_model.Enabled = true;
                    add_comp.Enabled = false;
                    delete.Enabled = true;

                }
                else if (ext == "xml")
                {
                    add_model.Enabled = false;
                    add_comp.Enabled = false;
                    delete.Enabled = false;
                }
                else
                {
                    add_model.Enabled = false;
                    add_comp.Enabled = false;
                    delete.Enabled = false;
                }

                filelist_context.Show(fileList, new Point(e.X, e.Y));
            }

        }
        private void LeaveFileList(object sender, EventArgs e)
        {

            //get item from list view
            ListViewItem lvi = this.fileList.SelectedItems[0];

            //create the dictionary key
            string key = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

            //get the item path
            string path = openmiFiles[key];

            //add model or composition to the composition window
            StopAllActions();
            if (lvi.SubItems[2].Text == "omi")
                this.AddModel(path);
            else
                this.OpenOprFile(path);
        }
        private void DragEnterModel(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void DragDropModel(object sender, DragEventArgs e)
        {
            //change to wait cursor
            compositionBox.Cursor = Cursors.WaitCursor;

            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                //get item from list view
                ListViewItem lvi = this.fileList.SelectedItems[0];

                //create the dictionary key
                string key = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

                //make sure that the selected file is either an .opr or .omi
                if (lvi.SubItems[2].Text == "omi" || lvi.SubItems[2].Text == "opr")
                {

                    //get the item path
                    string path = openmiFiles[key];


                    //get x location
                    int x = e.X;

                    //get y location
                    int y = e.Y;


                    //add model or composition to the composition window
                    StopAllActions();
                    if (lvi.SubItems[2].Text == "omi")
                    {
                        try
                        {
                            _composition.AddModel(null, path);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                "OMI filename: " + path + "\n" + "Exception: " + ex.ToString(),
                                "Error occured while adding the model...",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }


                        //update the Enable Ribbon Add Connection
                        if (_composition.Models.Count - 1 > _composition.Connections.Count)
                            ((RibbonButton)rps[1].Items[2]).Enabled = true;


                        Application.CurrentCulture = _cultureInfo;

                        CompositionUpdateArea(x, y);


                        UpdateControls();
                        UpdateTitle();
                        Invalidate();
                    }
                    else if (lvi.SubItems[2].Text == "opr")
                        this.OpenOprFile(path);
                }
                else
                {
                    ToolTip tip = new ToolTip();

                    Point topLeft = this.PointToScreen(new Point(this.container.Panel2.Left, this.container.Panel2.Top));

                    int y = e.Y - topLeft.Y + 32;
                    int x = e.X;
                    tip.Show("Sorry, cannot add a folder to the composition window", this, x, y, 1000);


                }

                //reset cursor
                compositionBox.Cursor = Cursors.Default;

                _isdragging = false;

            }
        }
        public string changeDir_Click(object sender, EventArgs e)
        {
            DialogResult result = this.dirDialog.ShowDialog();
            string path = null;
            if (result == DialogResult.OK)
            {
                path = dirDialog.SelectedPath;
                UpdateFileList(path);
                //tb_navigate.Text = path;
            }

            return path;

        }
        private void container_SplitterMoved(object sender, EventArgs e)
        {
            //get splitter width
            int newWidth = this.container.Panel1.Width;


            //set new filelist width
            this.fileList.Width = newWidth;

            //set column widths
            this.fileList.Columns[0].Width = Convert.ToInt32(3 * newWidth / 5) - 2;
            this.fileList.Columns[1].Width = Convert.ToInt32(newWidth / 5);
            this.fileList.Columns[2].Width = Convert.ToInt32(newWidth / 5);

            this.fileList.Invalidate();

            // Move the AddItem box


            //Resize the label
            this.emptylabel2.Width = newWidth;

            //
            //resize properties window
            //
            this.properties.Width = newWidth;                                                   //set the new width for the properties window
            int oldWidth = this.properties.Columns[0].Width + this.properties.Columns[1].Width; //get the old width
            double ratio = (Double)(this.properties.Columns[0].Width) / oldWidth;               //calculate the percentage of total width for column 1
            this.properties.Columns[0].Width = (Int32)(newWidth * ratio - 2);                     //set column 1 width
            this.properties.Columns[1].Width = (Int32)(newWidth * (1 - ratio) - 2);             //set column 2 width
            this.AddModelButton.Location = new Point(newWidth - this.AddModelButton.Width - 15, this.container2.Panel2.Height - 35);    //set button loc
            this._Save.Location = new Point(newWidth - this._Save.Width - 100, this.container2.Panel2.Height - 35);    //set button loc

        }
        private void container2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            //get splitter height
            int height1 = this.container2.Panel1.Height;
            int height2 = this.container2.Panel2.Height;

            //get splitter width
            int width = this.container2.Panel1.Width;

            //set new heights in panel 1
            this.fileList.Height = height1 - 2;


            //set new heights in panel 2
            this.properties.Height = height2 - 45;
            this.AddModelButton.Location = new Point(width - this.AddModelButton.Width - 15, height2 - 35);
            this._Save.Location = new Point(width - this._Save.Width - 100, height2 - 35);

        }
        public void clear()
        {
            _composition.RemoveAllModels();

            //disable "add connection"
            ((RibbonButton)rps[1].Items[2]).Enabled = false;

            //disable "run"
            ((RibbonButton)rps[1].Items[3]).Enabled = false;

            CompositionUpdateArea();
            UpdateControls();
            UpdateTitle();

        }
        private void filelist_context_edit(object sender, EventArgs e)
        {
            ////HydroModeler.XmlViewer viewer = new HydroModeler.XmlViewer();

            ////ListViewItem lvi = lvi = this.fileList.SelectedItems[0];

            ////string path = ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText;
            ////string file = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

            ////viewer.populate(path + "\\" + file);



        }
        private void fileList_SizeChanged(object sender, EventArgs e)
        {

        }
        private void PopulateProperties(string file)
        {

            //define some colors
            Color headerColor = Color.Gray;
            Color groupColor = Color.Silver;
            Color itemColor = Color.WhiteSmoke;
            //Color headerColor = Color.Salmon;
            //Color groupColor = Color.LightGreen;
            //Color itemColor = Color.LightBlue;

            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            Dictionary<string, string> args = new Dictionary<string, string>();

            //get root element
            XmlElement root = doc.DocumentElement;

            //get root children
            XmlNodeList elements = root.ChildNodes;


            //get the linkable component node
            XmlNode linkableComponent = root.SelectSingleNode("/LinkableComponent");
            if (linkableComponent == null)
                linkableComponent = (XmlNode)root;

            //get the class name and assembly path
            string className = linkableComponent.OuterXml.Split('=')[1].Split('\"')[1];
            string assembly = linkableComponent.OuterXml.Split('=')[2].Split('\"')[1];

            //loop through the root children
            foreach (XmlNode Arguments in elements)
            {
                //get arguments node
                if (Arguments.Name == "Arguments")
                {
                    //get arguments children
                    XmlNodeList Argument = Arguments.ChildNodes;

                    //loop through args
                    foreach (XmlNode arg in Argument)
                    {
                        string Key = arg.OuterXml.Split(' ')[1].Split('=')[1];
                        Key = Key.Remove(0, 1).Remove(Key.Length - 2, 1);
                        string Value = arg.OuterXml.Split(' ')[3].Split('=')[1];
                        for (int i = 4; i <= arg.OuterXml.Split(' ').Length - 1; i++)
                            Value += arg.OuterXml.Split(' ')[i];
                        Value = Value.Replace("/>", "");
                        Value = Value.Replace(">", "");
                        Value = Value.Replace(".\\", "");
                        Value = Value.Replace("\\", "");
                        Value = Value.Replace("\"", "");
                        ////remove /> characters if there isnt a space after this element
                        //if(arg.OuterXml.Split(' ').Length == 3)        
                        //    Value = Value.Remove(0, 1).Remove(Value.Length - 2, 1);

                        try
                        {
                            args.Add(Key, Value);
                        }
                        catch (Exception)
                        {
                            while (args.ContainsKey(Key))
                                Key += " ";

                            args.Add(Key, Value);
                        }
                    }
                    break;
                }
            }



            this.properties.BeginUpdate();

            ListViewItem li;
            ListViewItem.ListViewSubItem lsi;

            li = new ListViewItem("Omi Arguments");
            li.UseItemStyleForSubItems = true;
            li.BackColor = headerColor;
            li.Font = new Font(li.Font, FontStyle.Bold);
            this.properties.Items.Add(li);

            //set the class name 
            li = new ListViewItem("Class");
            li.UseItemStyleForSubItems = false;
            li.BackColor = itemColor;
            lsi = new ListViewItem.ListViewSubItem();
            lsi.Text = className;
            li.SubItems.Add(lsi);
            this.properties.Items.Add(li);
            //set the assembly name
            li = new ListViewItem("Assembly");
            li.UseItemStyleForSubItems = false;
            li.BackColor = itemColor;
            lsi = new ListViewItem.ListViewSubItem();
            lsi.Text = assembly;
            li.SubItems.Add(lsi);
            this.properties.Items.Add(li);

            foreach (KeyValuePair<string, string> kvp in args)
            {
                //
                // populate the properties window
                //
                li = new ListViewItem(kvp.Key);
                li.UseItemStyleForSubItems = false;
                li.BackColor = itemColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = kvp.Value;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);
            }


            //loop through the arguments, and see if any of the xml data can be loaded
            foreach (KeyValuePair<string, string> kvp in args)
            {
                if (kvp.Value.Contains(".xml"))
                {
                    try
                    {
                        //load the document
                        doc = new XmlDocument();
                        int characters = file.Split('\\')[file.Split('\\').Length - 1].Length;
                        string path = System.IO.Path.GetFullPath(file.Remove(file.Length - characters) + kvp.Value);
                        doc.Load(path);
                        //get root element
                        root = doc.DocumentElement;

                        //get input and output exchangeitems
                        XmlNodeList outputs = root.SelectNodes("/Configuration/ExchangeItems/OutputExchangeItem");
                        XmlNodeList inputs = root.SelectNodes("/Configuration/ExchangeItems/InputExchangeItem");
                        //get timehorizon
                        XmlNode timeHorizon = root.SelectSingleNode("/Configuration/TimeHorizon");
                        //get model info
                        XmlNode modelInfo = root.SelectSingleNode("/Configuration/ModelInfo");

                        #region Add Output Exchange Items
                        //loop through output and input exchange items
                        foreach (XmlNode output in outputs)
                        {
                            //add Exchange Item Property
                            li = new ListViewItem("Output Exchange Item");
                            li.UseItemStyleForSubItems = true;
                            li.BackColor = headerColor;
                            li.Font = new Font(li.Font, FontStyle.Bold);
                            this.properties.Items.Add(li);

                            //add ElementSet Property
                            li = new ListViewItem("Element Set");
                            li.UseItemStyleForSubItems = false;
                            li.BackColor = groupColor;
                            li.Font = new Font(li.Font, FontStyle.Bold);
                            this.properties.Items.Add(li);

                            //Add Element Set Items
                            foreach (XmlNode e in output.FirstChild.ChildNodes)
                            {
                                li = new ListViewItem(e.Name);
                                li.UseItemStyleForSubItems = false;
                                li.BackColor = itemColor;
                                lsi = new ListViewItem.ListViewSubItem();
                                lsi.Text = e.FirstChild.Value;
                                li.SubItems.Add(lsi);
                                this.properties.Items.Add(li);
                            }

                            //add Quantity Property
                            li = new ListViewItem("Quantity");
                            li.UseItemStyleForSubItems = false;
                            li.BackColor = groupColor;
                            li.Font = new Font(li.Font, FontStyle.Bold);
                            this.properties.Items.Add(li);

                            //Add Quantity Items
                            foreach (XmlNode e in output.FirstChild.NextSibling.ChildNodes)
                            {
                                li = new ListViewItem(e.Name);
                                li.UseItemStyleForSubItems = false;
                                li.BackColor = itemColor;
                                lsi = new ListViewItem.ListViewSubItem();

                                if (e.Name == "Dimensions")
                                {
                                    XmlNodeList dims = e.ChildNodes;
                                    string dimension = null;
                                    foreach (XmlNode dim in dims)
                                    {
                                        dimension += "[" + dim.FirstChild.InnerText + " ^" + dim.FirstChild.NextSibling.InnerText + "]";
                                    }
                                    lsi.Text = dimension;
                                    li.SubItems.Add(lsi);
                                    this.properties.Items.Add(li);
                                }
                                else if (e.Name == "Unit")
                                {
                                    XmlNodeList units = e.ChildNodes;
                                    foreach (XmlNode unit in units)
                                    {
                                        li = new ListViewItem("Unit: " + unit.Name);
                                        li.UseItemStyleForSubItems = false;
                                        li.BackColor = itemColor;
                                        lsi = new ListViewItem.ListViewSubItem();
                                        lsi.Text = unit.FirstChild.Value;
                                        li.SubItems.Add(lsi);
                                        this.properties.Items.Add(li);

                                    }
                                }
                                else
                                {
                                    li = new ListViewItem(e.Name);
                                    li.UseItemStyleForSubItems = false;
                                    li.BackColor = itemColor;
                                    lsi = new ListViewItem.ListViewSubItem();
                                    lsi.Text = e.FirstChild.Value;
                                    li.SubItems.Add(lsi);
                                    this.properties.Items.Add(li);
                                }



                            }


                        }
                        #endregion

                        #region Add Input Exchange Items
                        foreach (XmlNode input in inputs)
                        {
                            //add Exchange Item Property
                            li = new ListViewItem("Input Exchange Item");
                            li.UseItemStyleForSubItems = true;
                            li.BackColor = headerColor;
                            li.Font = new Font(li.Font, FontStyle.Bold);
                            this.properties.Items.Add(li);

                            //add ElementSet Property
                            li = new ListViewItem("Element Set");
                            li.UseItemStyleForSubItems = false;
                            li.BackColor = groupColor;
                            li.Font = new Font(li.Font, FontStyle.Bold);
                            this.properties.Items.Add(li);

                            //Add Element Set Items
                            foreach (XmlNode e in input.FirstChild.ChildNodes)
                            {
                                li = new ListViewItem(e.Name);
                                li.UseItemStyleForSubItems = false;
                                li.BackColor = itemColor;
                                lsi = new ListViewItem.ListViewSubItem();
                                lsi.Text = e.FirstChild.Value;
                                li.SubItems.Add(lsi);
                                this.properties.Items.Add(li);
                            }

                            //add Quantity Property
                            li = new ListViewItem("Quantity");
                            li.UseItemStyleForSubItems = false;
                            li.BackColor = groupColor;
                            li.Font = new Font(li.Font, FontStyle.Bold);
                            this.properties.Items.Add(li);

                            //Add Quantity Items
                            foreach (XmlNode e in input.FirstChild.NextSibling.ChildNodes)
                            {
                                li = new ListViewItem(e.Name);
                                li.UseItemStyleForSubItems = false;
                                li.BackColor = itemColor;
                                lsi = new ListViewItem.ListViewSubItem();

                                if (e.Name == "Dimensions")
                                {
                                    XmlNodeList dims = e.ChildNodes;
                                    string dimension = null;
                                    foreach (XmlNode dim in dims)
                                    {
                                        dimension += "[" + dim.FirstChild.InnerText + "^" + dim.FirstChild.NextSibling.InnerText + "]";
                                    }
                                    lsi.Text = dimension;
                                    li.SubItems.Add(lsi);
                                    this.properties.Items.Add(li);
                                }
                                else if (e.Name == "Unit")
                                {
                                    XmlNodeList units = e.ChildNodes;
                                    foreach (XmlNode unit in units)
                                    {
                                        li = new ListViewItem("Unit: " + unit.Name);
                                        li.UseItemStyleForSubItems = false;
                                        li.BackColor = itemColor;
                                        lsi = new ListViewItem.ListViewSubItem();
                                        lsi.Text = unit.FirstChild.Value;
                                        li.SubItems.Add(lsi);
                                        this.properties.Items.Add(li);

                                    }
                                }
                                else
                                {
                                    li = new ListViewItem(e.Name);
                                    li.UseItemStyleForSubItems = false;
                                    li.BackColor = itemColor;
                                    lsi = new ListViewItem.ListViewSubItem();
                                    lsi.Text = e.FirstChild.Value;
                                    li.SubItems.Add(lsi);
                                    this.properties.Items.Add(li);
                                }



                            }
                        }
                        #endregion

                        //read time horizion

                        //add TimeHorizon Property
                        li = new ListViewItem("Time Horizon");
                        li.UseItemStyleForSubItems = true;
                        li.BackColor = headerColor;
                        li.Font = new Font(li.Font, FontStyle.Bold);
                        this.properties.Items.Add(li);
                        foreach (XmlNode child in timeHorizon.ChildNodes)
                        {
                            li = new ListViewItem(child.Name);
                            li.UseItemStyleForSubItems = false;
                            li.BackColor = itemColor;
                            lsi = new ListViewItem.ListViewSubItem();
                            lsi.Text = child.FirstChild.Value;
                            li.SubItems.Add(lsi);
                            this.properties.Items.Add(li);

                        }



                        //read model info

                        //add Model Info Poroperty
                        li = new ListViewItem("Model Info");
                        li.UseItemStyleForSubItems = true;
                        li.BackColor = headerColor;
                        li.Font = new Font(li.Font, FontStyle.Bold);
                        this.properties.Items.Add(li);
                        foreach (XmlNode child in modelInfo.ChildNodes)
                        {
                            li = new ListViewItem(child.Name);
                            li.UseItemStyleForSubItems = false;
                            li.BackColor = itemColor;
                            lsi = new ListViewItem.ListViewSubItem();
                            lsi.Text = child.FirstChild.Value;
                            li.SubItems.Add(lsi);
                            this.properties.Items.Add(li);

                        }
                    }
                    catch (Exception) { }
                }
            }
            this.properties.EndUpdate();
        }
        private void PopulateOPRProperties(string file)
        {
            //define some colors
            Color headerColor = Color.Gray;
            Color groupColor = Color.Silver;
            Color itemColor = Color.WhiteSmoke;
            //Color headerColor = Color.Salmon;
            //Color groupColor = Color.LightGreen;
            //Color itemColor = Color.LightBlue;

            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            Dictionary<string, string> args = new Dictionary<string, string>();

            //get root element
            XmlElement root = doc.DocumentElement;

            //get root children
            XmlNodeList elements = root.ChildNodes;

            List<UiLink> links = new List<UiLink>();


            foreach (XmlNode children in elements)
            {
                if (children.Name == "links")
                {
                    //loop through the links
                    foreach (XmlNode child in children)
                    {
                        /*TODO:  This catches the instance when a link has yet to be formed between two components.
                                 In the future it should also load this information, right now its just omitting it.base */
                        try
                        {
                            //get some data from the opr file and store it in the UiLink class
                            string provider = child.OuterXml.Split('=')[1].Split('\"')[1];
                            string accepter = child.OuterXml.Split('=')[2].Split('\"')[1];
                            int id = Convert.ToInt32(child.InnerXml.Split('=')[1].Split('\"')[1]);
                            string p_element = child.InnerXml.Split('=')[2].Split('\"')[1];
                            string p_quantity = child.InnerXml.Split('=')[3].Split('\"')[1];
                            string a_element = child.InnerXml.Split('=')[4].Split('\"')[1];
                            string a_quantity = child.InnerXml.Split('=')[5].Split('\"')[1];
                            string dataop = "none";
                            if (child.ChildNodes[0].HasChildNodes)
                            {
                                dataop = (child.InnerXml.Split('=')[6].Split('\"')[1]);

                            }

                            links.Add(new UiLink(id, provider, accepter, p_element, p_quantity, a_element, a_quantity, dataop));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }


            this.properties.BeginUpdate();

            ListViewItem li;
            ListViewItem.ListViewSubItem lsi;

            for (int i = 0; i <= links.Count - 1; i++)
            {
                li = new ListViewItem("Link id=" + links[i].linkID.ToString());
                li.UseItemStyleForSubItems = true;
                li.BackColor = headerColor;
                li.Font = new Font(li.Font, FontStyle.Bold);
                this.properties.Items.Add(li);

                //set the providing model name 
                li = new ListViewItem("Providing Model");
                li.UseItemStyleForSubItems = false;
                li.BackColor = groupColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = links[i].provider;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);

                //set the providing model name 
                li = new ListViewItem("Quantity");
                li.UseItemStyleForSubItems = false;
                li.BackColor = itemColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = links[i].provider_quantity;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);

                //set the accepting model name
                li = new ListViewItem("Element Set");
                li.UseItemStyleForSubItems = false;
                li.BackColor = itemColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = links[i].provider_elementset;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);

                //set the accepting model name
                li = new ListViewItem("Accepting Model");
                li.UseItemStyleForSubItems = false;
                li.BackColor = groupColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = links[i].accepter;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);

                //set the providing model name 
                li = new ListViewItem("Quantity");
                li.UseItemStyleForSubItems = false;
                li.BackColor = itemColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = links[i].accepter_quantity;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);

                //set the accepting model name
                li = new ListViewItem("Element Set");
                li.UseItemStyleForSubItems = false;
                li.BackColor = itemColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = links[i].accepter_elementset;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);

                //set the accepting model name
                li = new ListViewItem("Data Operation");
                li.UseItemStyleForSubItems = false;
                li.BackColor = groupColor;
                lsi = new ListViewItem.ListViewSubItem();
                lsi.Text = links[i].dataoperation;
                li.SubItems.Add(lsi);
                this.properties.Items.Add(li);

            }
            this.properties.EndUpdate();

        }
        private void properties_MouseDown(object sender, MouseEventArgs e)
        {
            //make sure a file has been selected
            if (_currentFile != null)
            {
                //get the selected item
                ListViewItem lvi = properties.GetItemAt(e.X, e.Y);

                //make sure any changes that have been made are saved
                if (t.Visible == true)
                    setProperty();

                //make sure the item is not a folder
                if (lvi != null)
                {

                    //check if the item is already highlighted
                    if (this.properties.Items[lvi.Index].Selected == true)
                    {
                        if (!_allowEdit)
                        {
                            ToolTip tip = new ToolTip();
                            int y = this.container2.Panel2.Top + properties.Location.Y + e.Y + 32;
                            int x = e.X;
                            tip.Show("Editing is not available for this file", this, x, y, 1000);
                        }
                        else
                        {
                            try
                            {
                                //disable scrolling
                                this.properties.Scrollable = false;

                                //get the sub item
                                ListViewItem.ListViewSubItem lvsi = lvi.SubItems[1];

                                //create a text box in that exact location
                                t.Location = new Point(lvsi.Bounds.X + 5, lvsi.Bounds.Y + 2);
                                t.Size = new Size(lvsi.Bounds.Width, lvsi.Bounds.Height + 1);
                                t.Font = lvi.Font;
                                t.BorderStyle = BorderStyle.None;
                                t.Text = lvsi.Text;
                                t.Visible = true;

                                //save the old text
                                _oldText = t.Text;
                                _currentLvi = lvi;

                                //highlight the text
                                t.SelectAll();
                                t.HideSelection = false;
                            }
                            catch (ArgumentOutOfRangeException) { }
                        }
                    }
                    else
                    {
                        //highlight the selected item
                        this.properties.Items[lvi.Index].Selected = true;
                        this.properties.Refresh();
                    }
                }
            }
        }
        private void textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                setProperty();
            }
        }
        private void setProperty()
        {
            try
            {
                ListViewItem.ListViewSubItem sub = _currentLvi.SubItems[1];
                if (t.Text != _oldText)
                {
                    this.properties.BeginUpdate();
                    sub.Text = t.Text;
                    this.properties.EndUpdate();
                    _hasChanged = true;
                    _Save.Enabled = true;

                }
                t.Visible = false;
                //re enable scrolling
                this.properties.Scrollable = true;
            }
            catch (Exception) { }
        }
        private void saveTheseChanges(string file)
        {
            Dictionary<string, string> newVals = new Dictionary<string, string>();
            List<string> output = new List<string>();
            List<string> input = new List<string>();
            List<string> time = new List<string>();
            List<string> model = new List<string>();

            #region Save Properties By XPath
            string config = null;
            string headerItem = "";
            bool elementsAdded = false;
            int outputItems = 0;
            int inputItems = 0;

            //read properties window to get new values
            for (int i = 0; i < this.properties.Items.Count; i++)
            {
                //get row info
                ListViewItem lvi = this.properties.Items[i];
                //configRoot.SelectSingleNode("/Configuration/ExchangeItems/OutputExchangeItem/Quantity[ID='Excess Rainfall']")
                if (lvi.SubItems.Count > 1)
                {
                    if (headerItem.Contains("Omi Arguments"))
                    {
                        newVals.Add(headerItem + ":" + lvi.Text, lvi.SubItems[1].Text);
                    }
                    else if (headerItem.Contains("Output Exchange Item"))
                    {
                        if (lvi.Text.Contains("Dimensions"))
                            output.Add("Dimension:" + lvi.SubItems[1].Text);
                        else
                            output.Add(lvi.SubItems[1].Text);
                    }
                    else if (headerItem.Contains("Input Exchange Item"))
                    {
                        if (lvi.Text.Contains("Dimensions"))
                            input.Add("Dimension:" + lvi.SubItems[1].Text);
                        else
                            input.Add(lvi.SubItems[1].Text);
                    }
                    else if (headerItem.Contains("Time Horizon"))
                        time.Add(lvi.SubItems[1].Text);
                    else if (headerItem.Contains("Model Info"))
                        model.Add(lvi.SubItems[1].Text);

                    //newVals.Add(headerItem+":"+lvi.SubItems[0].Text, lvi.SubItems[1].Text);
                    elementsAdded = true;
                }
                else
                {

                    if (lvi.Text == "Omi Arguments" || lvi.Text == "Output Exchange Item" || lvi.Text == "Input Exchange Item"
                        || lvi.Text == "Time Horizon" || lvi.Text == "Model Info")
                    {
                        if (lvi.Text == "Output Exchange Item")
                        {
                            outputItems++;
                            headerItem = lvi.Text + outputItems;
                            //output.Add(lvi.Text);
                        }
                        else if (lvi.Text == "Input Exchange Item")
                        {
                            inputItems++;
                            headerItem = lvi.Text + inputItems;
                            //input.Add(lvi.Text);
                        }
                        else
                        {
                            headerItem = lvi.Text;

                        }
                        elementsAdded = false;
                    }
                    else
                    {
                        if (elementsAdded)
                        {
                            string[] headeritems = headerItem.Split(':');
                            int length = headeritems.Length - 1;
                            Array.Resize(ref headeritems, length);
                            headerItem = String.Join(":", headeritems);
                            elementsAdded = false;
                        }
                        headerItem += ":" + lvi.Text;
                    }
                }


            }
            #endregion

            List<List<string>> values = new List<List<string>>();
            values.Add(output);
            values.Add(input);
            values.Add(time);
            values.Add(model);


            #region Edit the OMI file
            //Open Omi file
            XmlDocument omi = new XmlDocument();
            omi.Load(file);
            XmlElement omiRoot = omi.DocumentElement;

            //get the linkable component node
            XmlNode linkableComponent = omiRoot.SelectSingleNode("/LinkableComponent");

            //get the class name and assembly path
            string className = linkableComponent.OuterXml.Split('=')[1].Split('\"')[1];
            string assembly = linkableComponent.OuterXml.Split('=')[2].Split('\"')[1];



            //
            // Read the arguments from the omi file
            //
            XmlNodeList args = omiRoot.SelectNodes("/LinkableComponent/Arguments/Argument");
            foreach (XmlNode arg in args)
            {
                if (arg.OuterXml.Contains("ConfigFile"))
                {
                    //get the length of the omi filename
                    int characters = file.Split('\\')[file.Split('\\').Length - 1].Length;

                    //get the config filename
                    int configLength = arg.OuterXml.Split('=')[3].Length;
                    string configFileName = arg.OuterXml.Split('=')[3].Remove(0, 1).Remove(configLength - 5, 4);

                    //get full path to config file
                    config = System.IO.Path.GetFullPath(file.Remove(file.Length - characters) + configFileName);
                    break;
                }
            }

            //open the omi file to read its contents
            StreamReader srOmi = new StreamReader(file);
            //read all contents
            string[] contents = srOmi.ReadToEnd().Split('\n');
            //close the stream reader
            srOmi.Close();

            for (int i = 0; i <= contents.Length - 1; i++)
            {
                //replace class name and assembly pat
                if (contents[i].Contains("Type"))
                {
                    string type = newVals["Omi Arguments:Class"];
                    string assemblyName = newVals["Omi Arguments:Assembly"];
                    contents[i] = "<LinkableComponent Type=\"" + type + "\" Assembly=\"" + assemblyName + "\">\r";

                    //remove class and assembly from dictionary
                    newVals.Remove("Omi Arguments:Class");
                    newVals.Remove("Omi Arguments:Assembly");

                    break;
                }
            }

            //replace aruments
            foreach (KeyValuePair<string, string> kvp in newVals)
            {
                if (kvp.Key.Split(':')[0] == "Omi Arguments")
                {
                    for (int i = 0; i <= contents.Length - 1; i++)
                    {
                        //Argument Key=\"ConfigFile\"
                        if (contents[i].Contains("Argument Key=\""+kvp.Key.Split(':')[1]+"\""))
                        {
                            contents[i] = "    <Argument Key=\"" + kvp.Key.Split(':')[1] + "\" ReadOnly=\"true\" Value=\"" + kvp.Value + "\" />\r";
                            break;
                        }

                    }
                }
            }
            #endregion

            string configStream = null;
            if (config != null)
                configStream = ReWriteConfig(values, config);

            #region Update OMI and Config Values

            //save changes made to the omi file
            StreamWriter swOmi = new StreamWriter(file, false);
            string omiOutput = String.Join("\n", contents);
            swOmi.Write(omiOutput);
            swOmi.Close();

            //write new config if stream was created successfully
            if (configStream != null)
            {
                StreamWriter swConfig = new StreamWriter(config, false);
                swConfig.Write(configStream);
                swConfig.Close();
            }

            #endregion


            _hasChanged = false;
            _Save.Enabled = false;


        }
        private string ReWriteConfig(List<List<string>> values, string path)
        {
            string id = "";
            string configStream = null;
            configStream += "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>" + "\n";
            configStream += "<Configuration>" + "\n";
            configStream += "\t<ExchangeItems>" + "\n";

            for (int i = 0; i <= values.Count - 1; i++)
            {
                //output exchange item or input exchange item
                if (i == 0 || i == 1)
                {
                    for (int j = 0; j <= values[i].Count - 1; j++)
                    {
                        if (i == 0)
                            configStream += "\t\t<OutputExchangeItem>" + "\n";
                        else
                            configStream += "\t\t<InputExchangeItem>" + "\n";

                        configStream += "\t\t\t<ElementSet>" + "\n";
                        configStream += "\t\t\t\t<ID>" + values[i][j] + "</ID>" + "\n"; j++;
                        configStream += "\t\t\t\t<Description>" + values[i][j] + "</Description>" + "\n"; j++;

                        //BROKEN:  File Exists is not working!!!!! It must be relatice to the OMI!!!!!
                        //check to see if a shapefile path was given
                        string currPath = ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText + "//";
                        //check to see if the file exists by absolute path
                        if (File.Exists(Path.GetFullPath(values[i][j])))
                        { configStream += "\t\t\t\t<ShapefilePath>" + values[i][j] + "</ShapefilePath>" + "\n"; j++; }
                        //check to see if the file exists by relative path
                        else if (File.Exists(Path.GetFullPath(currPath + values[i][j])))
                        { configStream += "\t\t\t\t<ShapefilePath>" + values[i][j] + "</ShapefilePath>" + "\n"; j++; }

                        configStream += "\t\t\t\t<Version>" + values[i][j] + "</Version>" + "\n"; j++;
                        configStream += "\t\t\t</ElementSet>" + "\n";

                        configStream += "\t\t\t<Quantity>" + "\n";
                        configStream += "\t\t\t\t<ID>" + values[i][j] + "</ID>" + "\n"; j++;
                        configStream += "\t\t\t\t<Description>" + values[i][j] + "</Description>" + "\n"; j++;

                        configStream += "\t\t\t\t<Dimensions>" + "\n";
                        if (values[i][j].Contains("Dimension"))
                        {
                            string dim = values[i][j].Split(':')[1];
                            for (int k = 0; k <= dim.Split(']').Length - 2; k++)
                            {

                                configStream += "\t\t\t\t\t<Dimension>" + "\n";
                                configStream += "\t\t\t\t\t\t<Base>" + dim.Split(']')[k].Split('^')[0].Replace("[", "").Replace("]", "").Trim() + "</Base>" + "\n";
                                configStream += "\t\t\t\t\t\t<Power>" + dim.Split('^')[k + 1].Split(']')[0] + "</Power>" + "\n";
                                configStream += "\t\t\t\t\t</Dimension>" + "\n";
                            }
                            j++;
                        }

                        configStream += "\t\t\t\t</Dimensions>" + "\n";

                        configStream += "\t\t\t\t<Unit>" + "\n";
                        configStream += "\t\t\t\t\t<ID>" + values[i][j] + "</ID>" + "\n"; j++;
                        configStream += "\t\t\t\t\t<Description>" + values[i][j] + "</Description>" + "\n"; j++;
                        configStream += "\t\t\t\t\t<ConversionFactorToSI>" + values[i][j] + "</ConversionFactorToSI>" + "\n"; j++;
                        configStream += "\t\t\t\t\t<OffSetToSI>" + values[i][j] + "</OffSetToSI>" + "\n"; j++;
                        configStream += "\t\t\t\t</Unit>" + "\n";

                        configStream += "\t\t\t\t<ValueType>" + values[i][j] + "</ValueType>" + "\n";

                        configStream += "\t\t\t</Quantity>" + "\n";

                        if (i == 0)
                            configStream += "\t\t</OutputExchangeItem>" + "\n";
                        else
                            configStream += "\t\t</InputExchangeItem>" + "\n";
                    }
                }
                //time horizon
                else if (i == 2)
                {
                    for (int j = 0; j <= values[i].Count - 1; j++)
                    {
                        configStream += "\t</ExchangeItems>" + "\n";

                        configStream += "\t<TimeHorizon>" + "\n";

                        // Model must have a start time defined 
                        configStream += "\t\t<StartDateTime>" + values[i][j] + "</StartDateTime>" + "\n"; j++;

                        // This assumes that a if only one date-time is given, then it is the StartDateTime
                        DateTime dt;
                        if (DateTime.TryParse(values[i][j], out dt))
                        { configStream += "\t\t<EndDateTime>" + values[i][j] + "</EndDateTime>" + "\n"; j++; }

                        configStream += "\t\t<TimeStepInSeconds>" + values[i][j] + "</TimeStepInSeconds>" + "\n"; j++;

                        configStream += "\t</TimeHorizon>" + "\n";

                    }

                }
                //model info
                else if (i == 3)
                {
                    for (int j = 0; j <= values[i].Count - 1; j++)
                    {
                        configStream += "\t<ModelInfo>" + "\n";
                        configStream += "\t\t<ID>" + values[i][j] + "</ID>" + "\n"; id = values[i][j]; j++;
                        configStream += "\t\t<Description>" + values[i][j] + "</Description>" + "\n"; j++;
                        configStream += "\t</ModelInfo>" + "\n";
                    }
                }

            }
            configStream += "\t</Configuration>" + "\n";

            return configStream;
        }
        private void saveChanges(string filename)
        {
            DialogResult result = MessageBox.Show("Would you like to save the changes made to " + filename + " ?", "Important Question", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                saveTheseChanges(filename);

                int length = filename.Split('\\').Length;
                //string id = ((Oatc.OpenMI.Gui.Core.UIModel)_composition.Models[0]).OmiFilename;

                //check to see if an omi file has been modified
                if (filename.Split('\\')[length - 1].Contains(".omi"))
                {
                    for (int i = 0; i <= _composition.Models.Count - 1; i++)
                    {
                        //checl to see of the edited model is loaded in the composition
                        if (((Oatc.OpenMI.Gui.Core.UIModel)_composition.Models[i]).OmiFilename == filename)
                        {

                            //--- reload the composition ---

                            //save the currently loaded opr file path
                            string currentOPR = _composition.FilePath;

                            if (currentOPR == null)
                            {
                                menuFileSaveAs_Click(this, new EventArgs());
                                currentOPR = _composition.FilePath;
                            }

                            //clear the composition window
                            this.clear();

                            //reload the currentOpr
                            this.OpenOprFile(currentOPR);

                            break;
                        }
                    }
                }

            }
            if (result == DialogResult.No)
            {
                t.Visible = false;
                _hasChanged = false;
            }

        }
        private void _Save_Click(object sender, EventArgs e)
        {
            string path = openmiFiles[_currentFileItem.SubItems[0].Text + "." + _currentFileItem.SubItems[2].Text];

            try
            {
                saveChanges(path);
            }
            catch (SystemException)
            {
                DialogResult failed = MessageBox.Show("HydroModeler encountered an error while saving. Please make sure that the values entered are in the correct format, and file exist at the paths specified. ", "Error", MessageBoxButtons.OK);
            }

        }
        private void tb_LostFocus(object sender, EventArgs e)
        {
            setProperty();
        }
        private void properties_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            //get column index
            int index = e.ColumnIndex;

            if (index == 0)
            {
                //get new width for col 0
                int Column0Width = e.NewWidth;

                //get total width of the properties window
                int TotalWidth = this.properties.Width;

                //get new column 1 width
                int Column1Width = TotalWidth - Column0Width;

                //set new column 0 width
                //this.properties.Columns[0].Width = Column0Width;

                //set column 1 width
                this.properties.Columns[1].Width = Column1Width - 4;

            }
        }

        #region Context Menu Handelers
        private void ClickModel(object sender, EventArgs e)
        {
            string key = null;
            ListViewItem lvi = null;
            try
            {
                //get item from list view
                lvi = this.fileList.SelectedItems[0];

                //create the dictionary key
                if (lvi.SubItems[2].Text == " ")
                    key = lvi.SubItems[0].Text;
                else
                    key = lvi.SubItems[0].Text + "." + lvi.SubItems[2].Text;

                //check to see if the user clicked on a folder
                if (Folders.ContainsKey(key))
                {
                    //get the folder path
                    string path = Folders[key];

                    //update the filelist
                    UpdateFileList(path);

                    //update the ribbon textbox text
                    ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText = path;
                    //give the path to the run dialog box
                    _runBox._currentDirectory = path;
                }
                // if folder == ...
                else if (Folders.ContainsKey(key.Substring(0, 3)))
                {
                    //get the folder path
                    string path = Folders[key.Substring(0, 3)];

                    //update the filelist
                    UpdateFileList(path);

                    //update the ribbon textbox text
                    ((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText = path;
                    _runBox._currentDirectory = path;
                }
                else if (lvi.SubItems[2].Text == "omi" || lvi.SubItems[2].Text == "opr")
                {
                    //get the item path
                    string path = openmiFiles[key];

                    //add model or composition to the composition window
                    StopAllActions();
                    if (lvi.SubItems[2].Text == "omi")
                        this.AddModel(path);
                    else
                        this.OpenOprFile(path);
                }
            }
            catch (Exception) { }
        }
        void delete_Click(object sender, EventArgs e)
        {
            //authenticate the deletion
            DialogResult result = MessageBox.Show("Are you sure you want to permanently remove " + _currentFileItem.SubItems[0].Text + "." + _currentFileItem.SubItems[2].Text + "?", "Important Question", MessageBoxButtons.YesNo);


            if (result == DialogResult.Yes)
            {
                //delete file
                File.Delete(openmiFiles[_currentFileItem.SubItems[0].Text + "." + _currentFileItem.SubItems[2].Text]);

                //clear the composition window if the deleted item is currently loaded
                if (openmiFiles[_currentFileItem.SubItems[0].Text + "." + _currentFileItem.SubItems[2].Text] == _composition.FilePath)
                    clear();

                //refresh the file browser
                this.UpdateFileList(((RibbonTextBox)((RibbonItemGroup)rps[2].Items[0]).Items[0]).TextBoxText);
            }
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // mainTab
            // 
            this.Name = "mainTab";
            this.Size = new System.Drawing.Size(229, 211);
            this.ResumeLayout(false);

        }

        #endregion
       
    }

    public class UiLink
    {
        public int linkID;
        public string provider;
        public string accepter;
        public string provider_elementset;
        public string provider_quantity;
        public string accepter_elementset;
        public string accepter_quantity;
        public string dataoperation;

        public UiLink(  int linkID, string provider, string accepter, string provider_elementset, 
                        string provider_quantity, string accepter_elementset, 
                        string accepter_quantity, string dataoperation)
        {
            //set internal variables
            this.linkID = linkID;
            this.provider = provider;
            this.accepter = accepter;
            this.provider_elementset = provider_elementset;
            this.provider_quantity = provider_quantity;
            this.accepter_elementset = accepter_elementset;
            this.accepter_quantity = accepter_quantity;
            this.dataoperation = dataoperation ;

        }

    }

}