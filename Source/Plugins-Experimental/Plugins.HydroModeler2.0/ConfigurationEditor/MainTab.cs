﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using Oatc.OpenMI.Gui.Core;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;
using System.Collections;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    public partial class MainTab : UserControl
    {
        #region Window controls

		private HScrollBar compositionHScrollBar;
		private PictureBox compositionBox;
		//private VScrollBar compositionVScrollBar;

		private MainMenu mainMenu1;
        private ToolStripMenuItem menuItem15;
        private ToolStripMenuItem menuItem17;
        private ToolStripMenuItem menuItem18;

        private ToolStripMenuItem menuFileNew;
        private ToolStripMenuItem menuFileOpen;
        private ToolStripMenuItem menuFileSave;
        private ToolStripMenuItem menuFileSaveAs;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuEditModelAdd;
        private ToolStripMenuItem menuModelAttachTrigger;
        private ToolStripMenuItem menuHelpAbout;

        private ToolStripMenuItem menuFileReload;
        private ToolStripMenuItem menuViewModelProperties;
        private ToolStripMenuItem menuEditConnectionLinks;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuEditRunProperties;
        private ToolStripMenuItem menuHelp;
		private ImageList imageList;
        private ToolStripMenuItem menuCompositionSpacer;
        private ToolStripMenuItem menuComposition;

        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem contextConnectionEditLinks;
        private System.Windows.Forms.MenuItem contextModelAttachTrigger;
        private System.Windows.Forms.MenuItem contextRun;
        private System.Windows.Forms.MenuItem contextConnectionAdd;
        private System.Windows.Forms.MenuItem contextModelProperties;
        private System.Windows.Forms.MenuItem contextConnectionProperties;
        private System.Windows.Forms.MenuItem contextModelRemove;
        private System.Windows.Forms.MenuItem contextConnectionRemove;
        private System.Windows.Forms.MenuItem contextModelAdd;
		#endregion		

		#region Member variables

        readonly Cursor _sourceCursor;
        readonly Cursor _targetCursor;

	    bool _isAddingConnection = false;
		UIModel _sourceModel = null;
		
		bool _isMovingModel = false;
		Point _prevMouse;

		object _contextSelectedObject;

	    readonly CompositionManager _composition;
				
		Point _compositionBoxPositionInArea;
		Rectangle _compositionArea;

        const string ApplicationTitle = "OATC OpenMI Editor 2.0";

        private ToolStripMenuItem menuHelpContents;
        private ToolStripMenuItem menuItem3;
        private ToolStripMenuItem menuOptions;
        private ToolStripMenuItem menuRegisterExtensions;

        private ToolStripMenuItem menuItem2;
        private ToolStripMenuItem menuEditConnectionAdd;
        private ToolStripMenuItem menuItem1;
        private ToolStripMenuItem menuItem4;
        private ToolStripMenuItem menuExamples;
        private ToolStripMenuItem menuExample1;
        private ToolStripMenuItem menuExample2;
        private ToolStripMenuItem menuExample3;
        private ToolStripMenuItem menuExample4;
        private ToolStripMenuItem menuItem5;
        private ToolStripMenuItem popup;

        // pre-created dialogs
        ModelDialog _modelDialog;
        ConnectionDialog _connectionDialog;
        AboutBox _aboutBox;
        RunProperties _runProperties;
        RunBox _runBox;

        //a toolbar button added by the plugin
        private ToolStripButton btnHydroModelerPlugin = null;
        //a menu item added by the plugin
        private ToolStripMenuItem mnuHydroModelerPlugin = null;
        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;

        // record the culture that the application starts in
	    readonly System.Globalization.CultureInfo _cultureInfo = Application.CurrentCulture;

		#endregion
	
		/// <summary>
		/// Creates a new instance of <see cref="MainForm">MainForm</see> window.
		/// </summary>
		public MainTab(IMapPluginArgs args)
		{
			//
			// Required for Windows Form Designer support
			//

            _mapArgs = args;

            _prevMouse = new Point(0, 0);

            // create dialogs
            _modelDialog = new ModelDialog();
            //_connectionDialog = new ConnectionDialog();
            _aboutBox = new AboutBox();
            _runProperties = new RunProperties();
            _runBox = new RunBox();


            _compositionBoxPositionInArea = new Point(0, 0);

            InitializeComponent();

            _composition = new CompositionManager();

            _prevMouse = new Point(0, 0);

            //_sourceCursor = new Cursor(GetType(), "Source.cur");
            //_targetCursor = new Cursor(GetType(), "Target.cur");

            menuRegisterExtensions.Checked = Utils.AreFileExtensionsRegistered(Application.ExecutablePath);


		}

		


		#region Methods and properties

		/// <summary>
		/// Method is used to start application.
		/// </summary>
		/// <param name="args">Command-line arguments.</param>
		/// <remarks>Method proceeds all command-line args ("/opr %", "/reg", ...)
		/// and perform requested actions.</remarks>		
		private static void ProcessCommandLineArgs( string[] args )
		{
			// read commad-line args
			string oprFilename = null;
			string omiFilename = null;
			bool mta = false;

			for( int i=0; i<args.Length; i++ )
				switch( args[i].ToLower() )
				{
					case "/opr":
					case "-opr":
						if( oprFilename!=null )
							throw( new Exception("-opr can be used only once.") );

						if( omiFilename!=null )
							throw( new Exception("-opr cannot be used together with -omi option.") );

						if( args.Length <= i+1 )
							throw( new Exception("-opr option must be followed by filename.") );

						oprFilename = args[i+1];
						i++;
						break;

					case "/omi":
					case "-omi":
						if( omiFilename!=null )
							throw( new Exception("-omi can be used only once.") );

						if( oprFilename!=null )
							throw( new Exception("-omi cannot be used together with -opr option.") );

						if( args.Length <= i+1 )
							throw( new Exception("-omi option must be followed by filename.") );

						omiFilename = args[i+1];
						i++;
						break;

					case "/reg":
					case "-reg":						
						Utils.RegisterFileExtensions( Application.ExecutablePath );
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
						MessageBox.Show( help, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;

					default:
						throw( new Exception("Unknown command-line option: "+args[i]) );
				}

			// do actions...
			
			// VS2005 fix
			// In VS2005 the main thread uses MTA by default, so we have to create new thread,
			// which will run the message loop, and set it's appartment state before it's started
			
			Thread thread = new Thread( new ParameterizedThreadStart( StartApplication) );
			thread.IsBackground = false;

			if( mta )
			{
				thread.SetApartmentState( ApartmentState.MTA );

				// NOTE: when using MTA, the OpenFileDialog (and maybe other things)
				// throws ThreadStateException ("Current thread must be set to single thread
				// apartment (STA) mode before OLE calls can be made. Ensure that your Main
				// function has STAThreadAttribute marked on it. This exception is only raised
				// if a debugger is attached to the process.")
				//
				// MTA is used only if really needed (we provide it as feature),
				// thus this statement is perfectly correct
				CheckForIllegalCrossThreadCalls = false;
			}
			else
			{
				thread.SetApartmentState( ApartmentState.STA );
			}

			thread.Start( new string[] { oprFilename, omiFilename } );			
		}

		private static void StartApplication( object data )
		{
			try
			{
				string oprFilename = ( (string[]) data )[0];
				string omiFilename = ( (string[]) data )[1];

				if( oprFilename!=null )
				{
					// Open OPR project from file
					MainForm mainForm = new MainForm();
					FileInfo oprFile = new FileInfo( oprFilename );

					mainForm.OpenComposition(oprFile);
                    
					Application.Run( mainForm );
				}
				else if( omiFilename!=null )
				{
					// Create new project with one OMI model
					MainForm mainForm = new MainForm();
					FileInfo fileInfo = new FileInfo( omiFilename );

					mainForm.AddModel( fileInfo );

					Application.Run( mainForm );
				}
				else
					Application.Run( new MainForm() );
            }
            catch (Exception ex)
            {
                Trace.TraceError("Start Application: " + ex.Message);
            }
        }

		/// <summary>
		/// Opens composition from OPR file.
		/// </summary>
		/// <param name="fullPath">Full path to OPR file.</param>
		private void OpenComposition(FileInfo oprFile)
		{
			try
			{				
				_composition.Open(oprFile);

                UpdateTitle();

                CompositionUpdateArea();
                CompositionCenterView();
            }
			catch( Exception ex )
			{
                FinalCatchAndDisplay("Open Composition", ex);				
			}				
		}


		/// <summary>
		/// Adds one model to composition.
		/// </summary>
		/// <param name="fullPath">Full path to OMI file.</param>
		private void AddModel(FileInfo omiFile)
		{
			try 
			{
				UIModel model = new UIModel();
				model.OmiDeserializeAndInitialize(omiFile);

				_composition.ModelAdd(model);

				if (_composition.Models.Count == 1)
					_composition.Models[0].IsTrigger = true;

			}
			catch( Exception ex )
			{
				FinalCatchAndDisplay("Cannot add " + omiFile.FullName, ex);
			}

            // Reset the culture every time a new model is added.
            // The new model may be of a different culture, we want to retain the original culture of the application, 
            // which will be that of the User's computer.
            Application.CurrentCulture = _cultureInfo;

			CompositionUpdateArea();
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
		private void CompositionUpdateArea()
		{
			Point topLeft = new Point( 0, 0 ),
				bottomRight = new Point( 0, 0 );

			foreach( UIModel model in _composition.Models )
			{
				topLeft.X = Math.Min( topLeft.X, model.Rect.X );
				topLeft.Y = Math.Min( topLeft.Y, model.Rect.Y );

				bottomRight.X = Math.Max( bottomRight.X, model.Rect.X + model.Rect.Width );
				bottomRight.Y = Math.Max( bottomRight.Y, model.Rect.Y + model.Rect.Height );
			}

			// increase size of area
			topLeft.X -= compositionBox.Width / 2;
			topLeft.Y -= compositionBox.Height / 2;
			bottomRight.X += compositionBox.Width - compositionBox.Width / 2;
			bottomRight.Y += compositionBox.Height - compositionBox.Height / 2;

			_compositionArea = new Rectangle( topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y );

			// update scrollbars

			compositionHScrollBar.Minimum = _compositionArea.X;
			compositionHScrollBar.Maximum = _compositionArea.X + _compositionArea.Width;
			compositionHScrollBar.LargeChange = compositionBox.Width;
			//compositionHScrollBar.Value = compositionHScrollBar.Value; // don't change Value, but call ValueChange event
		
			
			compositionVScrollBar.Minimum = _compositionArea.Y;
			compositionVScrollBar.Maximum = _compositionArea.Y + _compositionArea.Height;
			compositionVScrollBar.LargeChange = compositionBox.Height;
			//compositionVScrollBar.Value = compositionVScrollBar.Value; // todo

			compositionScrollBar_ValueChanged(null, null);

			compositionBox.Invalidate();
		}
		

		/// <summary>
		/// Sets composition box to center.
		/// </summary>
		private void CompositionCenterView()
		{
			// todo...
		}

		private Point CompositionWindowPointToAreaPoint( Point point )
		{
			return( new Point(_compositionBoxPositionInArea.X+point.X, _compositionBoxPositionInArea.Y+point.Y) ); 
		}

		private Point CompositionAreaPointToWindowPoint( Point point )
		{
			return( new Point(point.X - _compositionBoxPositionInArea.X, point.Y - _compositionBoxPositionInArea.Y) );
		}


		private void UpdateTitle()
		{
            string path = "?.opr";
            string readOnly = string.Empty;

            if (_composition != null && _composition.FileOpr != null)
            {
				if (_composition.FileOpr.FullName.Length < 40)
					path = _composition.FileOpr.FullName;
                else
					path = _composition.FileOpr.FullName.Substring(
						_composition.FileOpr.FullName.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                if (_composition.FileOpr.IsReadOnly)
                    readOnly = ", READ ONLY";
            }

            Text = string.Format("{0}: {1}{2}{3}",
                ApplicationTitle, path, 
                _composition.ShouldBeSaved ? " *" : "",
                readOnly);
		}
		
		/// <summary>
		/// If composition should be saved, this method shows message box, where the user can do it, can
		/// ignore it or can cancel current operation.
		/// </summary>
		/// <returns>Returns <c>true</c> if current operation can continue, or <c>false</c>
		/// if user pressed cancel button.</returns>
		private bool CheckIfSaved()
		{
			if( _composition.ShouldBeSaved )
			{
				switch( MessageBox.Show("The composition has been changed.\n\nDo you want to save the changes?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) )
				{
					case DialogResult.Yes:
						OnSave(null, null);
						return( !_composition.ShouldBeSaved );
					case DialogResult.No:
						return( true );
					default:
						return( false );
				}
			}
			return( true );
		}
		

		private void ShowLinkDialog( UIConnection link )
		{
            ConnectionDialog dlg = new ConnectionDialog(_composition.Models);
            dlg.PopulateDialog(link);
            if (dlg.ShowDialog(this) == DialogResult.OK)
				_composition.ShouldBeSaved = true;
			
			UpdateTitle();
		}


		private UIModel GetModel( int x, int y )
		{
			Point areaPoint = CompositionWindowPointToAreaPoint( new Point(x,y) );

			// search from last model to first for case some models are overlapping
			for( int i=_composition.Models.Count-1; i>=0; i-- )
			{
				UIModel model = (UIModel)_composition.Models[i];

				if( model.IsPointInside(areaPoint) )
					return( model );				
			}

			return( null );
		}


		private UIConnection GetConnection( int x, int y )
		{
			Point areaPoint = CompositionWindowPointToAreaPoint( new Point(x,y) );

			for( int i=_composition.Connections.Count-1; i>=0; i-- )
			{
				UIConnection connection = (UIConnection)_composition.Connections[i];

				if( connection.IsOnConnectionLine(areaPoint) )
					return( connection );				
			}

			return( null );
		}


		private void StopAddingConnection()
		{
			_isAddingConnection = false;
			compositionBox.Cursor = Cursors.Default;
			_sourceModel = null;
		}

		private void StopMovingModel()
		{
			_isMovingModel = false;
			foreach( UIModel model in _composition.Models )
				model.IsMoving = false;
			compositionBox.Invalidate();
		}


		private void StopAllActions()
		{
			StopAddingConnection();
			StopMovingModel();
		}


		#endregion

		#region MainForm event handlers

		private void MainForm_Load(object sender, EventArgs e)
		{
			MainForm_SizeChanged(sender, e);
			UpdateTitle();
			CompositionUpdateArea();
		}


		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			MessageBox.Show("form1, dragDrop");
		
		}

		
		private void MainForm_SizeChanged(object sender, EventArgs e)
		{
			/*// resize all elements so they fit to window
			const int border = 5;
			const int scrollBarWidth = 16;			
			
			// listBoxOutput
			listBoxOutput.Height = (ClientRectangle.Height * 3) / 10; // 30%
			listBoxOutput.Width = ClientRectangle.Width - 2*border;
			listBoxOutput.Top = ClientRectangle.Height - (listBoxOutput.Height+border);
			listBoxOutput.Left = border;
			
			//compositionBox.BackColor = Color.Brown; // todo

			// compositionBox
			compositionBox.Top = border;
			compositionBox.Left = border;
			compositionBox.Width = listBoxOutput.Width - scrollBarWidth;
			compositionBox.Height = ClientRectangle.Height - listBoxOutput.Height - scrollBarWidth - 3*border;

			// compositionVScrollBar
			compositionVScrollBar.Width = scrollBarWidth;
			compositionVScrollBar.Height = compositionBox.Height;
			compositionVScrollBar.Top = compositionBox.Top;
			compositionVScrollBar.Left = border + compositionBox.Width;

			// compositionHScrollBar
			compositionHScrollBar.Width = compositionBox.Width;
			compositionHScrollBar.Height = scrollBarWidth;
			compositionHScrollBar.Top = border + compositionBox.Height;
			compositionHScrollBar.Left = border;*/

			CompositionUpdateArea();
		}


		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// if composition isn't saved, show message box, and maybe stop the closing			
			e.Cancel = !CheckIfSaved();
		}


		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			// ESC cancels adding connection
			if( _isAddingConnection && e.KeyChar == 27 )
			{
				StopAddingConnection();
				e.Handled = true;
				Invalidate();
			}		
		}
		

		#endregion	

		#region Main menu event handlers

		private void OnAddModels(object sender, EventArgs e)
		{
            try
            {
			    StopAllActions();

			    OpenFileDialog dlg = new OpenFileDialog();
			    dlg.CheckFileExists = true;
			    dlg.CheckPathExists = true;
			    dlg.Title = "Add model(s)...";
			    dlg.Filter = "OpenMI models (*.omi)|*.omi|All files|*.*";
			    dlg.Multiselect = true;

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                string[] files = new string[dlg.FileNames.Length];
                dlg.FileNames.CopyTo(files, 0);

                dlg.Dispose();

                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Refresh();

                    foreach (string filename in files)
                        AddModel(new FileInfo(filename));
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
			}
			catch( Exception ex )
			{
                FinalCatchAndDisplay("AddModels", ex);				
			}
        }


		private void OnAttachTrigger(object sender, EventArgs e)
		{
            try
            {
			    StopAllActions();

			    if (_contextSelectedObject != null
				    && _contextSelectedObject is UIModel)
			    {
				    foreach (UIModel model in _composition.Models)
					    model.IsTrigger = false;

				    ((UIModel)_contextSelectedObject).IsTrigger = true;
			    }

			    CompositionUpdateArea();
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("Attach Trigger", ex);
            }
		}

		private void OnRun(object sender, EventArgs e)
		{
			try
			{
				StopAllActions();

				if (_composition.ShouldBeSaved)
					_composition.Save();

				Run run = new Run();
				run.Initialise(_composition.FileOpr.FullName);
				run.ShowDialog();
			}
			catch( Exception ex )
			{
				FinalCatchAndDisplay("Run", ex);				
			}
		}

		private void OnNew(object sender, EventArgs e)
		{
			try
			{
				StopAllActions();

				if( !CheckIfSaved() )
					return;

				_composition.Initialize();

				UpdateTitle();
				CompositionUpdateArea();
			}
			catch (Exception ex)
			{
				FinalCatchAndDisplay("New", ex);
			}
		}
			
		private void OnOpen(object sender, EventArgs e)
		{
			try
			{
				StopAllActions();

				if( !CheckIfSaved() )
					return;			

				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Filter = "OmiEd projects (*.opr)|*.opr|All files|*.*";
				dlg.Multiselect = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.Title = "Open project...";

				if (dlg.ShowDialog(this) != DialogResult.OK)
					return;

				FileInfo oprFile = new FileInfo(dlg.FileName);	

				dlg.Dispose();

                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Refresh();

                    OpenComposition(oprFile);

                    UpdateTitle();
                    CompositionUpdateArea();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
			}
			catch (Exception ex)
			{
				FinalCatchAndDisplay("Open", ex);
			}					
		}
	
		private void OnSave(object sender, EventArgs e)
		{
			if (_composition.FileOpr == null 
                || _composition.FileOpr.IsReadOnly)
			{
				OnSaveAs(sender, e);
				return;
			}

			try
			{
				StopAllActions();

				_composition.Save();

				UpdateTitle();
			}
			catch (Exception ex)
			{
				FinalCatchAndDisplay("Save", ex);
			}
		}

		private void OnSaveAs(object sender, EventArgs e)
		{
			try
			{
				SaveFileDialog dlg = new SaveFileDialog();			
				dlg.Filter = "Compositions (*.opr)|*.opr|All files|*.*";
				dlg.ValidateNames = true;
				dlg.Title = "Save As ...";
				dlg.AddExtension = true;
				dlg.OverwritePrompt = true;
				dlg.FileName = _composition.FileOpr != null
					? _composition.FileOpr.FullName : "";

				if( dlg.ShowDialog( this ) != DialogResult.OK )
					return;

				FileInfo fi = new FileInfo(dlg.FileName);

				dlg.Dispose();

				StopAllActions();

				_composition.SaveAs(fi);
				 
				UpdateTitle();
			}
			catch (Exception ex)
			{
				FinalCatchAndDisplay("SaveAs", ex);
			}
		}

		private void OnReOpen(object sender, EventArgs e)
		{
			try
			{
				StopAllActions();

				if (!CheckIfSaved())
					return;			

				_composition.ReOpen();

				UpdateTitle();
			}
			catch (Exception ex)
			{
				FinalCatchAndDisplay("ReOpen", ex);
			}
		}

		void FinalCatchAndDisplay(string task, Exception e)
		{
			string s = string.Format("Task:\n\n{0}\n\nReason:\n\n{1}", 
				task, Utils.ToString(e));

            Trace.TraceError(s);

			MessageBox.Show(s, "Operation Failed", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);

			if (_composition != null)
				_composition.Initialize();
		}

		private void menuFileExit_Click(object sender, EventArgs e)
		{
			StopAllActions();

			//Close();		
		}


		private void OnMenuConnectionAdd(object sender, EventArgs e)
		{
            try
            {
			    StopAllActions();

			    _isAddingConnection = true;
			    compositionBox.Cursor = _sourceCursor;	
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("View Model Properties", ex);
            }
        }


		private void OnViewModelProperties(object sender, EventArgs e)
		{
            try
            {
			    StopAllActions();

                ModelDialog modelDialog = new ModelDialog();
			    modelDialog.PopulateDialog( _composition.Models );
			    modelDialog.ShowDialog( this );
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("View Model Properties", ex);
            }
		}


		private void menuRegisterExtensions_Click(object sender, EventArgs e)
		{
			if( menuRegisterExtensions.Checked )
			{
				Utils.UnregisterFileExtensions();
				menuRegisterExtensions.Checked = false;
			}
			else
			{
				Utils.RegisterFileExtensions( Application.ExecutablePath );
				menuRegisterExtensions.Checked = true;
			}		
		}


		private void OnHelpAbout(object sender, EventArgs e)
		{
            try
            {
                StopAllActions();

                AboutBox aboutBox = new AboutBox();
                aboutBox.ShowDialog(this);
            }
			catch (Exception ex)
			{
				FinalCatchAndDisplay("Help About", ex);
			}
		}

		private void OnHelp(object sender, EventArgs e)
		{
            try
            {
                ShopHelp(HelpFormat.pdf);
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("Help", ex);
            }
		}

        enum HelpFormat { chm = 0, pdf, };

        void ShopHelp(HelpFormat fmt)
        {
            FileInfo file = new FileInfo(Path.Combine(Application.StartupPath,
                @"OpenMIEditorHelp_2_0." + fmt.ToString()));

            if (file.Exists)
                Help.ShowHelp(this, file.FullName, HelpNavigator.TableOfContents);
        }

        void ShopUri(Uri uri)
        {
            StopAllActions();

            Help.ShowHelp(this, uri.AbsoluteUri, HelpNavigator.TableOfContents);
        }

		#endregion

		#region Context menu event handlers

        private void OnMenuCompositionOpen(object sender, EventArgs e)
        {
            menuEditModelAdd.Enabled = true;

            if (_composition != null)
            {
                if (_contextSelectedObject != null
                    && _contextSelectedObject is UIModel)
                {
                    menuModelAttachTrigger.Enabled = true;
                    menuViewModelProperties.Enabled = true;
                }
                else
                {
                    menuModelAttachTrigger.Enabled = false;
                    menuViewModelProperties.Enabled = false;
                }

                menuEditConnectionAdd.Enabled
                    = _composition.Models.Count > 1;
                menuEditConnectionLinks.Enabled
                    = _contextSelectedObject != null
                    && _contextSelectedObject is UIConnection;

                menuEditRunProperties.Enabled
                    = !_composition.ShouldBeSaved
                    && _composition.Models.Count > 0;
            }
            else
            {
                menuCompositionSpacer.Enabled = false;
                menuViewModelProperties.Enabled = false;
                menuEditConnectionAdd.Enabled = false;
                menuEditConnectionLinks.Enabled = false;
                menuEditRunProperties.Enabled = false;
            }
        }

		private void OnContextMenu(object sender, EventArgs e)
		{
            try
            {
			    StopAllActions();

                contextModelAdd.Visible = false;
                contextModelRemove.Visible = false;
                contextModelProperties.Visible = false;
                contextModelAttachTrigger.Visible = false;
                contextConnectionAdd.Visible = false;
                contextConnectionRemove.Visible = false;
                contextConnectionEditLinks.Visible = false;
                contextRun.Visible = false;

			    if( _contextSelectedObject == null )
			    {
                    contextModelAdd.Visible = true;
                    contextRun.Visible = true;
                    contextConnectionAdd.Visible = true;

                    contextModelAdd.Enabled = true;

                    if (_composition != null)
                    {
                        contextRun.Enabled
                            = !_composition.ShouldBeSaved
                            && _composition.Models.Count > 0;
                        contextConnectionAdd.Enabled 
                            = _composition.Models.Count > 1;
                    }
                }
			    else if( _contextSelectedObject is UIConnection )
			    {
                    contextConnectionRemove.Visible = true;
                    contextConnectionEditLinks.Visible = true;

                    contextConnectionRemove.Enabled = true;
                    contextConnectionEditLinks.Enabled = true;
                }
			    else if( _contextSelectedObject is UIModel )
			    {
                    contextModelRemove.Visible = true;
                    contextModelProperties.Visible = true;
                    contextModelAttachTrigger.Visible = true;

                    contextModelRemove.Enabled = true;
                    contextModelProperties.Enabled = true;
                    contextModelAttachTrigger.Enabled 
                        = !((UIModel)_contextSelectedObject).IsTrigger;
                }
			    else
				    Debug.Assert( false );
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("Context Menu", ex);
            }
		}


		private void contextConnectionAdd_Click(object sender, EventArgs e)
		{
			OnMenuConnectionAdd(sender, e);	
			CompositionUpdateArea();
			UpdateTitle();
		}

		private void contextConnectionRemove_Click(object sender, EventArgs e)
		{
			_composition.RemoveConnection( (UIConnection)_contextSelectedObject );
			CompositionUpdateArea();
			UpdateTitle();
		}

		private void contextConnectionProperties_Click(object sender, EventArgs e)
		{
            ShowSelectedObjectProperties();
		}

		private void contextModelAdd_Click(object sender, EventArgs e)
		{
			OnAddModels( sender, e );
		}

		private void contextModelRemove_Click(object sender, EventArgs e)
		{
			_composition.ModelRemove( (UIModel)_contextSelectedObject );
			CompositionUpdateArea();
			UpdateTitle();
		}

		private void contextModelProperties_Click(object sender, EventArgs e)
		{
            ShowSelectedObjectProperties();
		}

		private void contextRun_Click(object sender, EventArgs e)
		{
			OnRun(sender, e);			
		}


		private void contextIsTrigger_Click(object sender, EventArgs e)
		{
			OnAttachTrigger( sender, e );		
		}


		#endregion

		

		#region Composition box event handlers

		private void compositionScrollBar_ValueChanged(object sender, EventArgs e)
		{
			_compositionBoxPositionInArea.X = compositionHScrollBar.Value;
			_compositionBoxPositionInArea.Y = compositionVScrollBar.Value;
			compositionBox.Invalidate();
		}
	
		private void OnCanvasPaint(object sender, PaintEventArgs e)
		{
            try
            {
                // draw OpenMI logo
                e.Graphics.DrawImage(imageList.Images[0], 0, 0);

                //foreach (UIConnection link in _composition.Connections)
                //    link.Draw(true,_compositionBoxPositionInArea, e.Graphics);

                //foreach (UIModel model in _composition.Models)
                //    model.Draw(true,_compositionBoxPositionInArea, e.Graphics);

                foreach (UIConnection connection in _composition.Connections)
                    connection.Draw(_contextSelectedObject == connection,
                        _compositionBoxPositionInArea, e.Graphics);

                foreach (UIModel model in _composition.Models)
                    model.Draw(_contextSelectedObject == model,
                        _compositionBoxPositionInArea, e.Graphics);
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("Canvas Paint", ex);
            }
		}
		
		private void compositionBox_MouseDown(object sender, MouseEventArgs e)
		{
			StopMovingModel();
			compositionBox.Invalidate();	

			bool actionFoundOut = false;			

			// Left mouse button
			if( e.Button == MouseButtons.Left )
			{
				// if adding a connection
				if( _isAddingConnection )
				{
					UIModel model = GetModel( e.X, e.Y );
                    
					// if some model selected
					if( model!=null )
					{
						// if source model selected
						if( _sourceModel == null )
						{
							_sourceModel = model;							
							compositionBox.Cursor = _targetCursor;
						}
						else
						{
							// target model selected => add connection to composition
							if( _sourceModel != model )							
								_composition.AddConnection(
									new UIConnection(_sourceModel, model));						
							StopAddingConnection();
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
				if( !actionFoundOut )
				{
					UIModel model = GetModel( e.X, e.Y );

					if( model != null )
					{
						_prevMouse.X = e.X;
						_prevMouse.Y = e.Y;

						_isMovingModel = true;
						model.IsMoving = true;

						actionFoundOut = true;
					}
				}				
			}
			else if( e.Button == MouseButtons.Right )
			{
				// right button => show context menu

				// stop other actions
				StopAddingConnection();
				StopMovingModel();

				// get model under cursor
				_contextSelectedObject = GetModel(e.X,e.Y);
				if( _contextSelectedObject == null )
                    _contextSelectedObject = GetConnection(e.X,e.Y);

                //contextMenu.Show(popup, new Point(e.X, e.Y));
				contextMenu.Show( compositionBox, new Point(e.X,e.Y) );
			}
		}

		private void compositionBox_MouseMove(object sender, MouseEventArgs e)
		{
			// moving model ?
			if( _isMovingModel )
			{				
				foreach( UIModel model in _composition.Models )
					if( model.IsMoving )
					{
						model.Rect.X += e.X -_prevMouse.X;
						model.Rect.Y += e.Y -_prevMouse.Y;

						_prevMouse.X = e.X;
						_prevMouse.Y = e.Y;
						
						_composition.ShouldBeSaved = true;
						CompositionUpdateArea();
						UpdateTitle();
						compositionBox.Invalidate();
					}					
			}
		
		}
		
		private void compositionBox_MouseUp(object sender, MouseEventArgs e)
		{
			StopMovingModel();		
		}

		
		#endregion

		#region .NET generated members

		private System.ComponentModel.IContainer components;		
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>		
        static void Main(string[] args)
        {
            try
            {
                ProcessCommandLineArgs(args);
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
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();

            ////InitializeMyMenu();
            //InitializeOldMenu();
            //InitializeContext();
            //InitializeComposition();
            // 
            // mainTab
            // 
 
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileReload = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem18 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuComposition = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditModelAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewModelProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModelAttachTrigger = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCompositionSpacer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditConnectionAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditConnectionLinks = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditRunProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRegisterExtensions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExamples = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExample1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExample2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExample3 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExample4 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.compositionHScrollBar = new System.Windows.Forms.HScrollBar();
            this.compositionBox = new System.Windows.Forms.PictureBox();
            this.compositionVScrollBar = new System.Windows.Forms.VScrollBar();

            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.contextModelAdd = new System.Windows.Forms.MenuItem();
            this.contextModelRemove = new System.Windows.Forms.MenuItem();
            this.contextModelProperties = new System.Windows.Forms.MenuItem();
            this.contextModelAttachTrigger = new System.Windows.Forms.MenuItem();
            this.contextConnectionAdd = new System.Windows.Forms.MenuItem();
            this.contextConnectionRemove = new System.Windows.Forms.MenuItem();
            this.contextConnectionEditLinks = new System.Windows.Forms.MenuItem();
            this.contextRun = new System.Windows.Forms.MenuItem();

            this.imageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.compositionBox)).BeginInit();
            this.SuspendLayout();
            //// 
            //// mainMenu1
            //// 
            //this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.menuFile,
            //this.menuComposition,
            //this.menuOptions,
            //this.menuHelp});
            // 
            this.menuFile = new ToolStripMenuItem();
            this.menuFile.Text = "HydroModeler 2.0";
            this.menuFile.Name = "hydroModelerMenuItem";
            _mapArgs.MainMenu.Items.Add(this.menuFile);
            

            // menuFile
            // 
            //this.menuFile.Index = 0;

            //this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItemCollection {
            //this.menuFileNew,
            //this.menuItem17,
            //this.menuFileReload,
            //this.menuItem18,
            //this.menuFileOpen,
            //this.menuFileSave,
            //this.menuFileSaveAs,
            //this.menuItem15,
            //this.menuFileExit});
            // 
            // menuFileNew
            // 
            this.menuFileNew.Text = "&New";
            this.menuFileNew.Click += new System.EventHandler(this.OnNew);
            this.menuFile.DropDownItems.Add(this.menuFileNew);
            // 
            // menuItem17
            // 
            //this.menuItem17.Text = "-";
            //this.menuFile.DropDownItems.Add(this.menuItem17);
            // 
            // menuFileReload
            // 
            this.menuFileReload.Text = "&Reload";
            this.menuFileReload.Click += new System.EventHandler(this.OnReOpen);
            this.menuFile.DropDownItems.Add(this.menuFileReload);
            // 
            // menuItem18
            // 
            //this.menuItem18.Text = "-";
            //this.menuFile.DropDownItems.Add(this.menuItem18);
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Text = "&Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.OnOpen);
            this.menuFile.DropDownItems.Add(this.menuFileOpen);
            // 
            // menuFileSave
            // 
            this.menuFileSave.Text = "&Save";
            this.menuFileSave.Click += new System.EventHandler(this.OnSave);
            this.menuFile.DropDownItems.Add(this.menuFileSave);
            // 
            // menuFileSaveAs
            // 
            this.menuFileSaveAs.Text = "Save &As...";
            this.menuFileSaveAs.Click += new System.EventHandler(this.OnSaveAs);
            this.menuFile.DropDownItems.Add(this.menuFileSaveAs);
            // 
            // menuItem15
            // 
            //this.menuItem15.Text = "-";
            //this.menuFile.DropDownItems.Add(this.menuItem15);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Text = "E&xit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            this.menuFile.DropDownItems.Add(this.menuFileExit);

            this.menuFile.DropDownItems.Add(new ToolStripSeparator());
            // 
            // menuComposition
            // 

            //this.menuComposition.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.menuEditModelAdd,
            //this.menuItem4,
            //this.menuViewModelProperties,
            //this.menuModelAttachTrigger,
            //this.menuCompositionSpacer,
            //this.menuEditConnectionAdd,
            //this.menuEditConnectionLinks,
            //this.menuItem1,
            //this.menuEditRunProperties});

            //this.menuComposition.Text = "&Composition";
            //this.menuComposition.Popup += new System.EventHandler(this.OnMenuCompositionOpen);
            // 
            // menuEditModelAdd
            // 
            this.menuEditModelAdd.Text = "Add &Model(s) ...";
            this.menuEditModelAdd.Click += new System.EventHandler(this.OnAddModels);
            this.menuFile.DropDownItems.Add(this.menuEditModelAdd);
            // 
            // menuItem4
            // 
            //this.menuItem4.Index = 1;
            //this.menuItem4.Text = "-";
            // 
            // menuViewModelProperties
            // 
            //this.menuViewModelProperties.Index = 2;
            this.menuViewModelProperties.Text = "Model &Properties...";
            this.menuViewModelProperties.Click += new System.EventHandler(this.OnViewModelProperties);
            this.menuFile.DropDownItems.Add(this.menuViewModelProperties);
            // 
            // menuModelAttachTrigger
            // 
            //this.menuModelAttachTrigger.Index = 3;
            this.menuModelAttachTrigger.Text = "Model Attach &Trigger";
            this.menuModelAttachTrigger.Click += new System.EventHandler(this.OnAttachTrigger);
            this.menuFile.DropDownItems.Add(this.menuModelAttachTrigger);
            // 
            // menuCompositionSpacer
            // 
            //this.menuCompositionSpacer.Index = 4;
            //this.menuCompositionSpacer.Text = "-";
            //this.menuFile.DropDownItems.Add(this.
            // 
            // menuEditConnectionAdd
            // 
            this.menuEditConnectionAdd.Enabled = false;
            //this.menuEditConnectionAdd.Index = 5;
            this.menuEditConnectionAdd.Text = "Add &Connection";
            this.menuEditConnectionAdd.Click += new System.EventHandler(this.OnMenuConnectionAdd);
            this.menuFile.DropDownItems.Add(this.menuEditConnectionAdd);
            // 
            // menuEditConnectionLinks
            // 
            this.menuEditConnectionLinks.Enabled = false;
            //this.menuEditConnectionLinks.Index = 6;
            this.menuEditConnectionLinks.Text = "Edit Connection &Links...";
            this.menuFile.DropDownItems.Add(this.menuEditConnectionLinks);
            // 
            // menuItem1
            // 
            //this.menuItem1.Index = 7;
            //this.menuItem1.Text = "-";
            // 
            // menuEditRunProperties
            // 
            //this.menuEditRunProperties.Index = 8;
            //this.menuEditRunProperties.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuEditRunProperties.Text = "&Run...";
            this.menuEditRunProperties.Click += new System.EventHandler(this.OnRun);
            this.menuFile.DropDownItems.Add(this.menuEditRunProperties);
            // 
            // menuOptions
            // 
            //this.menuOptions.Index = 2;
            //this.menuOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.menuItem5,
            //this.menuRegisterExtensions});
            //this.menuOptions.Text = "&Options";

            this.menuFile.DropDownItems.Add(new ToolStripSeparator());

            // 
            // menuItem5
            // 
            //this.menuItem5.Index = 0;
            this.menuItem5.Text = "View Exception details";
            this.menuItem5.Click += new System.EventHandler(this.OnViewExceptionDetails);
            this.menuFile.DropDownItems.Add(this.menuItem5);
            // 
            // menuRegisterExtensions
            // 
            this.menuRegisterExtensions.Checked = true;
            //this.menuRegisterExtensions.Index = 1;
            this.menuRegisterExtensions.Text = "&Register file extensions";
            this.menuRegisterExtensions.Click += new System.EventHandler(this.menuRegisterExtensions_Click);
            this.menuFile.DropDownItems.Add(this.menuRegisterExtensions);
            // 
            // menuHelp
            // 
            //this.menuHelp.Index = 3;
            //this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.menuHelpContents,
            //this.menuItem2,
            //this.menuExamples,
            //this.menuItem3,
            //this.menuHelpAbout});
            //this.menuHelp.Text = "&Help";
            //this.menuHelp.Popup += new System.EventHandler(this.OnMenuHelpOpen);
            // 
            // menuHelpContents
            // 
            //this.menuHelpContents.Index = 0;
            //this.menuHelpContents.Shortcut = System.Windows.Forms.Shortcut.F1;
            this.menuHelpContents.Text = "&Help ...";
            this.menuHelpContents.Click += new System.EventHandler(this.OnHelp);
            this.menuFile.DropDownItems.Add(this.menuHelpContents);
            // 
            // menuItem2
            // 
            //this.menuItem2.Index = 1;
            this.menuItem2.Text = "&Wiki ...";
            this.menuItem2.Click += new System.EventHandler(this.OnMenuHelpWiki);
            this.menuFile.DropDownItems.Add(this.menuItem2);
            // 
            // menuExamples
            // 
            //this.menuExamples.Index = 2;
            //this.menuExamples.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.menuExample1,
            //this.menuExample2,
            //this.menuExample3,
            //this.menuExample4});
            //this.menuExamples.Text = "Examples";
            // 
            // menuExample1
            // 
            //this.menuExample1.Index = 0;
            //this.menuExample1.Text = "1. One river reach";
            //this.menuExample1.Click += new System.EventHandler(this.OnExample1);
            //// 
            //// menuExample2
            //// 
            //this.menuExample2.Index = 1;
            //this.menuExample2.Text = "2. Two river reaches";
            //this.menuExample2.Click += new System.EventHandler(this.OnExample2);
            //// 
            //// menuExample3
            //// 
            //this.menuExample3.Index = 2;
            //this.menuExample3.Text = "3. Two reaches + 1 adapter";
            //this.menuExample3.Click += new System.EventHandler(this.OnExample3);
            //// 
            //// menuExample4
            //// 
            //this.menuExample4.Index = 3;
            //this.menuExample4.Text = "4. Two reaches + 2 adapters";
            //this.menuExample4.Click += new System.EventHandler(this.OnExample4);
            //// 
            //// menuItem3
            //// 
            //this.menuItem3.Index = 3;
            //this.menuItem3.Text = "-";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Text = "&About ...";
            this.menuHelpAbout.Click += new System.EventHandler(this.OnHelpAbout);
            this.menuFile.DropDownItems.Add(this.menuHelpAbout);
            // 
            // compositionHScrollBar
            // 
            this.compositionHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.compositionHScrollBar.Location = new System.Drawing.Point(0, 236);
            this.compositionHScrollBar.Maximum = 20;
            this.compositionHScrollBar.Minimum = -10;
            this.compositionHScrollBar.Name = "compositionHScrollBar";
            this.compositionHScrollBar.Size = new System.Drawing.Size(376, 16);
            this.compositionHScrollBar.TabIndex = 2;
            this.compositionHScrollBar.ValueChanged += new System.EventHandler(this.compositionScrollBar_ValueChanged);
            // 
            // compositionBox
            // 
            this.compositionBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.compositionBox.BackColor = System.Drawing.Color.White;
            this.compositionBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.compositionBox.Location = new System.Drawing.Point(0, 0);
            this.compositionBox.Name = "compositionBox";
            this.compositionBox.Size = new System.Drawing.Size(376, 236);
            this.compositionBox.TabIndex = 3;
            this.compositionBox.TabStop = false;
            this.compositionBox.DoubleClick += new System.EventHandler(this.OnCanvasDoubleClick);
            this.compositionBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseMove);
            this.compositionBox.Click += new System.EventHandler(this.OnCanvasClick);
            this.compositionBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseDown);
            this.compositionBox.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCanvasPaint);
            this.compositionBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseUp);
            // 
            // compositionVScrollBar
            // 
            this.compositionVScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.compositionVScrollBar.Location = new System.Drawing.Point(376, 0);
            this.compositionVScrollBar.Name = "compositionVScrollBar";
            this.compositionVScrollBar.Size = new System.Drawing.Size(16, 236);
            this.compositionVScrollBar.TabIndex = 4;
            this.compositionVScrollBar.ValueChanged += new System.EventHandler(this.compositionScrollBar_ValueChanged);
            // 
            // contextMenu
            // 
            //this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.contextModelAdd,
            //this.contextModelRemove,
            //this.contextModelProperties,
            //this.contextModelAttachTrigger,
            //this.contextConnectionAdd,
            //this.contextConnectionRemove,
            //this.contextConnectionEditLinks,
            //this.contextRun});
            this.contextMenu.Popup += new System.EventHandler(this.OnContextMenu);
            // 
            // contextModelAdd
            // 
            //this.contextModelAdd.Index = 0;
            this.contextModelAdd.Text = "Add Model(s)...";
            this.contextModelAdd.Click += new System.EventHandler(this.contextModelAdd_Click);
            this.contextMenu.MenuItems.Add(this.contextModelAdd);
            // 
            // contextModelRemove
            // 
            //this.contextModelRemove.Index = 1;
            this.contextModelRemove.Text = "Model Remove";
            this.contextModelRemove.Click += new System.EventHandler(this.contextModelRemove_Click);
            this.contextMenu.MenuItems.Add(this.contextModelRemove);
            // 
            // contextModelProperties
            // 
            //this.contextModelProperties.Index = 2;
            this.contextModelProperties.Text = "Model Properties...";
            this.contextModelProperties.Click += new System.EventHandler(this.contextModelProperties_Click);
            this.contextMenu.MenuItems.Add(this.contextModelProperties);
            // 
            // contextModelAttachTrigger
            // 
            //this.contextModelAttachTrigger.Index = 3;
            this.contextModelAttachTrigger.Text = "Attach Trigger";
            this.contextModelAttachTrigger.Click += new System.EventHandler(this.contextIsTrigger_Click);
            this.contextMenu.MenuItems.Add(this.contextModelAttachTrigger);
            // 
            // contextConnectionAdd
            // 
            //this.contextConnectionAdd.Index = 4;
            this.contextConnectionAdd.Text = "Add Connection";
            this.contextConnectionAdd.Click += new System.EventHandler(this.contextConnectionAdd_Click);
            this.contextMenu.MenuItems.Add(this.contextConnectionAdd);
            // 
            // contextConnectionRemove
            // 
            //this.contextConnectionRemove.Index = 5;
            this.contextConnectionRemove.Text = "Connection Delete";
            this.contextConnectionRemove.Click += new System.EventHandler(this.contextConnectionRemove_Click);
            this.contextMenu.MenuItems.Add(this.contextConnectionRemove);
            // 
            // contextConnectionEditLinks
            // 
            //this.contextConnectionEditLinks.Index = 6;
            this.contextConnectionEditLinks.Text = "Edit Links...";
            this.contextConnectionEditLinks.Click += new System.EventHandler(this.contextConnectionProperties_Click);
            this.contextMenu.MenuItems.Add(this.contextConnectionEditLinks);
            // 
            // contextRun
            // 
            //this.contextRun.Index = 7;
            this.contextRun.Text = "Run...";
            this.contextRun.Click += new System.EventHandler(this.contextRun_Click);
            this.contextMenu.MenuItems.Add(this.contextRun);
            // 
            //// imageList
            //// 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(392, 253);
            this.Controls.Add(this.compositionVScrollBar);
            this.Controls.Add(this.compositionBox);
            this.Controls.Add(this.compositionHScrollBar);
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            //this.Menu = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "MainForm";
            this.Text = "OpenMI Editor 2";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            //this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.compositionBox)).EndInit();
            this.ResumeLayout(false);

		}

        private void InitializeOldMenu()
        {
        //    this.menuFile = new ToolStripMenuItem();
        //    this.menuFile.Text = "HydroModeler";
        //    this.menuFile.Name = "hydroModelerMenuItem";
        //    _mapArgs.MainMenu.Items.Add(this.menuFile);

        //    this.menuFileNew = new ToolStripMenuItem();
        //    this.menuFileNew.Text = "&New composition";
        //    this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
        //    this.menuFile.DropDownItems.Add(this.menuFileNew);

        //    this.menuFileReload = new ToolStripMenuItem();
        //    this.menuFileReload.Text = "&Reload composition";
        //    this.menuFileReload.Click += new System.EventHandler(this.menuFileReload_Click);
        //    this.menuFile.DropDownItems.Add(this.menuFileReload);

        //    this.menuFileOpen = new ToolStripMenuItem();
        //    this.menuFileOpen.Text = "&Open composition...";
        //    this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
        //    this.menuFile.DropDownItems.Add(this.menuFileOpen);

        //    this.menuFileSave = new ToolStripMenuItem();
        //    this.menuFileSave.Text = "&Save composition";
        //    this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
        //    this.menuFile.DropDownItems.Add(this.menuFileSave);

        //    this.menuFileSaveAs = new ToolStripMenuItem();
        //    this.menuFileSaveAs.Text = "Save composition &As...";
        //    this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
        //    this.menuFile.DropDownItems.Add(this.menuFileSaveAs);

        //    this.menuFileExit = new ToolStripMenuItem();
        //    this.menuFileExit.Text = "E&xit";
        //    this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
        //    //this.menuFile.DropDownItems.Add(this.menuFileExit);

        //    this.menuFile.DropDownItems.Add(new ToolStripSeparator());

        //    this.menuEditModelAdd = new ToolStripMenuItem();
        //    this.menuEditModelAdd.Text = "Add &Model";
        //    this.menuEditModelAdd.Click += new System.EventHandler(this.menuEditModelAdd_Click);
        //    this.menuFile.DropDownItems.Add(this.menuEditModelAdd);

        //    this.menuEditConnectionAdd = new ToolStripMenuItem();
        //    this.menuEditConnectionAdd.Enabled = false;
        //    this.menuEditConnectionAdd.Text = "Add &Connection";
        //    this.menuEditConnectionAdd.Click += new System.EventHandler(this.menuEditConnectionAdd_Click);
        //    this.menuFile.DropDownItems.Add(this.menuEditConnectionAdd);

        //    //this.menuEditTriggerAdd = new ToolStripMenuItem();
        //    //this.menuEditTriggerAdd.Text = "Add &Trigger";
        //    //this.menuEditTriggerAdd.Click += new System.EventHandler(this.menuEditTriggerAdd_Click);
        //    //this.menuFile.DropDownItems.Add(this.menuEditTriggerAdd);

        //    //this.menuEditConnectionProperties = new ToolStripMenuItem();
        //    //this.menuEditConnectionProperties.Enabled = false;
        //    //this.menuEditConnectionProperties.Text = "Co&nnection properties...";
        //    //this.menuFile.DropDownItems.Add(this.menuEditConnectionProperties);

        //    this.menuViewModelProperties = new ToolStripMenuItem();
        //    this.menuViewModelProperties.Text = "Model &properties...";
        //    this.menuViewModelProperties.Click += new System.EventHandler(this.menuViewModelProperties_Click);
        //    this.menuFile.DropDownItems.Add(this.menuViewModelProperties);

        //    this.menuEditRunProperties = new ToolStripMenuItem();
        //    this.menuEditRunProperties.Text = "&Run...";
        //    this.menuEditRunProperties.Click += new System.EventHandler(this.menuDeployRun_Click);
        //    this.menuFile.DropDownItems.Add(this.menuEditRunProperties);

        //    this.menuRegisterExtensions = new ToolStripMenuItem();
        //    this.menuRegisterExtensions.Checked = true;
        //    this.menuRegisterExtensions.Text = "&Register file extensions";
        //    this.menuRegisterExtensions.Click += new System.EventHandler(this.menuRegisterExtensions_Click);

        //    this.menuOptions = new ToolStripMenuItem();
        //    this.menuOptions.Text = "&Options";
        //    this.menuOptions.DropDownItems.Add(this.menuRegisterExtensions);
        //    this.menuFile.DropDownItems.Add(this.menuOptions);

        //    this.menuFile.DropDownItems.Add(new ToolStripSeparator());

        //    this.menuHelp = new ToolStripMenuItem();
        //    this.menuHelp.Text = "&Help";
        //    //_ mapArgs.MainMenu.Items.Add(this.menuHelp); don't include for now.

        //    this.menuHelpAbout = new ToolStripMenuItem();
        //    this.menuHelpAbout.Text = "&About Configuration Editor ...";
        //    this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
        //    this.menuFile.DropDownItems.Add(this.menuHelpAbout);

        //    this.menuHelpContents = new ToolStripMenuItem();
        //    this.menuHelpContents.Text = "Help contents";
        //    this.menuHelpContents.Click += new System.EventHandler(this.menuHelpContents_Click);
        //    this.menuFile.DropDownItems.Add(this.menuHelpContents);
        }
        private void InitializeContext()
        {
            //this.contextMenu = new System.Windows.Forms.ContextMenu();
            //this.contextConfigurationAdd = new System.Windows.Forms.MenuItem();
            //this.contextModelAdd = new System.Windows.Forms.ToolStripMenuItem();
            //this.contextConnectionAdd = new System.Windows.Forms.ToolStripMenuItem();
            ////this.contextAddTrigger = new System.Windows.Forms.MenuItem();
            //this.contextRun = new System.Windows.Forms.ToolStripMenuItem();
            ////this.contextDivider = new System.Windows.Forms.MenuItem();
            //this.contextConnectionRemove = new System.Windows.Forms.ToolStripMenuItem();
            //this.contextConnectionProperties = new System.Windows.Forms.ToolStripMenuItem();
            //this.contextModelRemove = new System.Windows.Forms.ToolStripMenuItem();
            //this.contextModelProperties = new System.Windows.Forms.ToolStripMenuItem();

            //// 
            //// contextMenu
            //// 
            //this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            //this.contextConfigurationAdd,
            //this.contextModelAdd,
            //this.contextConnectionAdd,
            ////this.contextAddTrigger,
            //this.contextRun,
            ////this.contextDivider,
            //this.contextConnectionRemove,
            //this.contextConnectionProperties,
            //this.contextModelRemove,
            //this.contextModelProperties});
            //this.contextMenu.Popup += new System.EventHandler(this.contextMenu_Popup);
            //// 
            //// contextConfigurationAdd
            //// 
            //this.contextConfigurationAdd.Index = 0;
            //this.contextConfigurationAdd.Text = "Add Configuration";
            //this.contextConfigurationAdd.Click += new System.EventHandler(this.contextConfigurationAdd_Click);
            //// 
            //// contextModelAdd
            //// 
            //this.contextModelAdd.Index = 1;
            //this.contextModelAdd.Text = "Add Model";
            //this.contextModelAdd.Click += new System.EventHandler(this.contextModelAdd_Click);
            //// 
            //// contextConnectionAdd
            //// 
            //this.contextConnectionAdd.Index = 2;
            //this.contextConnectionAdd.Text = "Add Connection";
            //this.contextConnectionAdd.Click += new System.EventHandler(this.contextConnectionAdd_Click);
            //// 
            //// contextAddTrigger
            //// 
            ////this.contextAddTrigger.Index = 3;
            ////this.contextAddTrigger.Text = "Add Trigger";
            ////this.contextAddTrigger.Click += new System.EventHandler(this.contextAddTrigger_Click);
            //// 
            //// contextRun
            //// 
            ////this.contextRun.Index = 4;
            //this.contextRun.Text = "Run";
            //this.contextRun.Click += new System.EventHandler(this.contextRun_Click);
            //// 
            //// contextDivider
            //// 
            ////this.contextDivider.Index = 5;
            ////this.contextDivider.Text = "-";
            //// 
            //// contextConnectionRemove
            //// 
            //this.contextConnectionRemove.Index = 6;
            //this.contextConnectionRemove.Text = "Remove connection";
            //this.contextConnectionRemove.Click += new System.EventHandler(this.contextConnectionRemove_Click);
            //// 
            //// contextConnectionProperties
            //// 
            //this.contextConnectionProperties.Index = 7;
            //this.contextConnectionProperties.Text = "Connection properties";
            //this.contextConnectionProperties.Click += new System.EventHandler(this.contextConnectionProperties_Click);
            //// 
            //// contextModelRemove
            //// 
            //this.contextModelRemove.Index = 8;
            //this.contextModelRemove.Text = "Remove model";
            //this.contextModelRemove.Click += new System.EventHandler(this.contextModelRemove_Click);
            //// 
            //// contextModelProperties
            //// 
            //this.contextModelProperties.Index = 9;
            //this.contextModelProperties.Text = "Model properties";
            //this.contextModelProperties.Click += new System.EventHandler(this.contextModelProperties_Click);
        }
        private void InitializeComposition()
        {
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainTab));

            //this.compositionHScrollBar = new System.Windows.Forms.HScrollBar();
            //this.compositionBox = new System.Windows.Forms.PictureBox();

            //this.imageList = new System.Windows.Forms.ImageList(this.components);
            //this.compositionVScrollBar = new System.Windows.Forms.VScrollBar();
            //((System.ComponentModel.ISupportInitialize)(this.compositionBox)).BeginInit();
            //this.SuspendLayout();

            //// 
            //// compositionHScrollBar
            //// 
            //this.compositionHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
            //this.compositionHScrollBar.Location = new System.Drawing.Point(0, -525);
            //this.compositionHScrollBar.Maximum = 20;
            //this.compositionHScrollBar.Minimum = -10;
            //this.compositionHScrollBar.Name = "compositionHScrollBar";
            //this.compositionHScrollBar.Size = new System.Drawing.Size(586, 16);
            //this.compositionHScrollBar.TabIndex = 2;
            //this.compositionHScrollBar.ValueChanged += new System.EventHandler(this.compositionScrollBar_ValueChanged);
            //// 
            //// compositionBox
            //// 
            //this.compositionBox.BackColor = System.Drawing.Color.White;
            //this.compositionBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //this.compositionBox.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.compositionBox.Location = new System.Drawing.Point(0, 0);
            //this.compositionBox.Name = "compositionBox";
            //this.compositionBox.Size = new System.Drawing.Size(602, 288);
            //this.compositionBox.TabIndex = 3;
            //this.compositionBox.TabStop = false;
            //this.compositionBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseMove);
            //this.compositionBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseDown);
            //this.compositionBox.Paint += new System.Windows.Forms.PaintEventHandler(this.compositionBox_Paint);
            //this.compositionBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseUp);

            //// 
            //// imageList
            //// 
            //this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            //this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            //this.imageList.Images.SetKeyName(0, "");
            //// 
            //// compositionVScrollBar
            //// 
            //this.compositionVScrollBar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            //this.compositionVScrollBar.Location = new System.Drawing.Point(586, -41);
            //this.compositionVScrollBar.Name = "compositionVScrollBar";
            //this.compositionVScrollBar.Size = new System.Drawing.Size(19, 340);
            //this.compositionVScrollBar.TabIndex = 4;
            //this.compositionVScrollBar.Visible = false;
            //this.compositionVScrollBar.ValueChanged += new System.EventHandler(this.compositionScrollBar_ValueChanged);
        }
		#endregion		

		#endregion			

        private void OnCanvasClick(object sender, EventArgs e)
        {
            try
            {
                MouseEventArgs args = e as MouseEventArgs;

                _contextSelectedObject = null;

                if (args != null)
                {
                    _contextSelectedObject = GetModel(args.X, args.Y);

                    if (_contextSelectedObject == null)
                        _contextSelectedObject = GetConnection(args.X, args.Y);
                }
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("Canvas Click", ex);
            }
        }

        private void OnCanvasDoubleClick(object sender, EventArgs e)
        {
            try
            {
                OnCanvasClick(sender, e);
                ShowSelectedObjectProperties();
            }
            catch (Exception ex)
            {
                FinalCatchAndDisplay("Canvas Double Click", ex);
            }
        }

        void ShowSelectedObjectProperties()
        {
            if (_contextSelectedObject is UIConnection)
            {
                ShowLinkDialog((UIConnection)_contextSelectedObject);
                UpdateTitle();
            }
            else if (_contextSelectedObject is UIModel)
            {
                ModelDialog modelDialog = new ModelDialog();
                modelDialog.PopulateDialog(_composition.Models, ((UIModel)_contextSelectedObject).InstanceCaption);
                modelDialog.ShowDialog(this);
            }
        }

        private void OnMenuHelpWiki(object sender, EventArgs e)
        {
            ShopUri(new Uri(@"http://public.wldelft.nl/display/OPENMI/OpenMI+AssociationTechnical+Committee"));
        }

        string[] _exampleOprs = new string[] {
            "SimpleCSharpRiver2_RiverReach1.opr",
            "SimpleCSharpRiver2_RiverReachs1and2.opr",
            "SimpleCSharpRiver2_Decorators01.opr",
            "SimpleCSharpRiver2_Decorators02.opr",
        };

        DirectoryInfo ExamplesLocation()
        {
            object location = Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\OpenMI\OATC_Editor",
                "Examples_Location", null);

            DirectoryInfo examples = null;

            if (location != null)
                examples = new DirectoryInfo((string)location);

            if (examples == null || !examples.Exists)
            {
                // try develepment location

                FileInfo ass = new FileInfo(Assembly.GetExecutingAssembly().Location);

                examples = new DirectoryInfo(
                    Path.Combine(ass.DirectoryName, @"..\help\examples"));
            }

            return examples;
        }

        private void OnMenuHelpOpen(object sender, EventArgs e)
        {
            DirectoryInfo examples = ExamplesLocation();

            menuExamples.Enabled = examples != null && examples.Exists;

            if (menuExamples.Enabled)
            {
                menuExample1.Enabled = File.Exists(Path.Combine(examples.FullName,
                    _exampleOprs[0]));
                menuExample2.Enabled = File.Exists(Path.Combine(examples.FullName,
                    _exampleOprs[1]));
                menuExample3.Enabled = File.Exists(Path.Combine(examples.FullName,
                    _exampleOprs[2]));
                menuExample4.Enabled = File.Exists(Path.Combine(examples.FullName,
                    _exampleOprs[3]));
            }
        }

        void OpenExample(int index)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Refresh();

                if (index < 0 || index >= _exampleOprs.Length)
                throw new IndexOutOfRangeException();

                DirectoryInfo examples = ExamplesLocation();

                if (examples == null || !examples.Exists)
                    return;

                OpenComposition(new FileInfo(
                    Path.Combine(examples.FullName, _exampleOprs[index])));

                Cursor.Current = Cursors.WaitCursor;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                Refresh();
            }
        }

        private void OnExample1(object sender, EventArgs e)
        {
            OpenExample(0);
        }

        private void OnExample2(object sender, EventArgs e)
        {
            OpenExample(1);
        }

        private void OnExample3(object sender, EventArgs e)
        {
            OpenExample(2);
        }

        private void OnExample4(object sender, EventArgs e)
        {
            OpenExample(3);
        }

        private void OnViewExceptionDetails(object sender, EventArgs e)
        {

        }

        #region mainTab event handlers

        private void mainTab_Load(object sender, System.EventArgs e)
        {
            mainTab_SizeChanged(sender, e);
            UpdateTitle();
            UpdateControls();
            CompositionUpdateArea();
        }

        private void UpdateControls()
        {
            contextConnectionAdd.Enabled = menuEditConnectionAdd.Enabled = _composition.Models.Count > 1;

            //bool hasTrigger = _composition.HasTrigger();

            //contextAddTrigger.Enabled = menuEditTriggerAdd.Enabled = !hasTrigger;

            //contextRun.Enabled = menuEditRunProperties.Enabled = hasTrigger && _composition.Models.Count > 1;
        }

        private void mainTab_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            MessageBox.Show("form1, dragDrop");

        }


        private void mainTab_SizeChanged(object sender, System.EventArgs e)
        {
            /*// resize all elements so they fit to window
            const int border = 5;
            const int scrollBarWidth = 16;			
			
            // listBoxOutput
            listBoxOutput.Height = (ClientRectangle.Height * 3) / 10; // 30%
            listBoxOutput.Width = ClientRectangle.Width - 2*border;
            listBoxOutput.Top = ClientRectangle.Height - (listBoxOutput.Height+border);
            listBoxOutput.Left = border;
			
            //compositionBox.BackColor = Color.Brown; // todo

            // compositionBox
            compositionBox.Top = border;
            compositionBox.Left = border;
            compositionBox.Width = listBoxOutput.Width - scrollBarWidth;
            compositionBox.Height = ClientRectangle.Height - listBoxOutput.Height - scrollBarWidth - 3*border;

            // compositionVScrollBar
            compositionVScrollBar.Width = scrollBarWidth;
            compositionVScrollBar.Height = compositionBox.Height;
            compositionVScrollBar.Top = compositionBox.Top;
            compositionVScrollBar.Left = border + compositionBox.Width;

            // compositionHScrollBar
            compositionHScrollBar.Width = compositionBox.Width;
            compositionHScrollBar.Height = scrollBarWidth;
            compositionHScrollBar.Top = border + compositionBox.Height;
            compositionHScrollBar.Left = border;*/

            CompositionUpdateArea();
        }


        private void mainTab_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // if composition isn't saved, show message box, and maybe stop the closing			
            e.Cancel = !CheckIfSaved();

            if (!e.Cancel)
            {
                //_composition.Release();
            }
        }


        private void mainTab_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // ESC cancels adding connection
            if (_isAddingConnection && e.KeyChar == 27)
            {
                StopAddingConnection();
                e.Handled = true;
                Invalidate();
            }
        }


        #endregion

        private void contextConfigurationAdd_Click(object sender, EventArgs e)
        {
            menuFileOpen_Click(sender, e);
        }
        private void menuFileOpen_Click(object sender, System.EventArgs e)
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
                //OpenOprFile(dlgFile.FileName);

            dlgFile.Dispose();

        }
        /// <summary>
        /// Opens composition from OPR file.
        /// </summary>
        /// <param name="fullPath">Full path to OPR file.</param>
        private void OpenOprFile(string fullPath)
        {
            //try
            //{
            //    _compositionFilename = null;
            //    _composition.Release();
            //    _composition.LoadFromFile(fullPath);
            //    _compositionFilename = fullPath;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString(), "Error occured while loading the file...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    //_composition.Release();
            //}

            //UpdateControls();
            //UpdateTitle();

            //CompositionUpdateArea();
            //CompositionCenterView();
        }

        private void contextMenu_Popup(object sender, System.EventArgs e)
        {
            StopAllActions();

            contextConnectionRemove.Visible = true;
            contextConnectionProperties.Visible = true;
            contextModelProperties.Visible = true;
            contextModelRemove.Visible = true;
            //contextAddTrigger.Visible = true;
            contextRun.Visible = true;
            contextConnectionAdd.Visible = true;


            if (_contextSelectedObject == null)
            {
                //contextDivider.Visible = false;
                contextConnectionRemove.Visible = false;
                contextConnectionProperties.Visible = false;
                contextModelProperties.Visible = false;
                contextModelRemove.Visible = false;
            }
            else if (_contextSelectedObject is UIConnection)
            {
                //contextDivider.Visible = true;
                contextConnectionRemove.Visible = true;
                contextConnectionProperties.Visible = true;
                contextModelProperties.Visible = false;
                contextModelRemove.Visible = false;
            }
            else if (_contextSelectedObject is UIModel)
            {
                //contextDivider.Visible = true;
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
            //if (!contextAddTrigger.Enabled)
            //    contextAddTrigger.Visible = false;
            if (!contextRun.Enabled)
                contextRun.Visible = false;
            if (!contextConnectionAdd.Enabled)
                contextConnectionAdd.Visible = false;
        }
    }
}
