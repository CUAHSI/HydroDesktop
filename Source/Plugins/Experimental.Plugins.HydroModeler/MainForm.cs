#region Copyright
/*
* Copyright (c) 2005,2006,2007, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion 
using System;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Gui.Core;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{ 
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		#region Window controls

		private System.Windows.Forms.HScrollBar compositionHScrollBar;
		private System.Windows.Forms.PictureBox compositionBox;
		private System.Windows.Forms.VScrollBar compositionVScrollBar;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.ContextMenu contextMenu;		
		private System.Windows.Forms.MenuItem menuFileNew;
		private System.Windows.Forms.MenuItem menuFileOpen;
		private System.Windows.Forms.MenuItem menuFileSave;
		private System.Windows.Forms.MenuItem menuFileSaveAs;
		private System.Windows.Forms.MenuItem menuFileExit;
		private System.Windows.Forms.MenuItem menuEditModelAdd;
		private System.Windows.Forms.MenuItem menuEditTriggerAdd;
		private System.Windows.Forms.MenuItem menuHelpAbout;
		private System.Windows.Forms.MenuItem contextConnectionAdd;
		private System.Windows.Forms.MenuItem contextModelProperties;
		private System.Windows.Forms.MenuItem contextConnectionProperties;
		private System.Windows.Forms.MenuItem contextModelRemove;
		private System.Windows.Forms.MenuItem contextConnectionRemove;
		private System.Windows.Forms.MenuItem contextModelAdd;
		private System.Windows.Forms.MenuItem menuFileReload;
		private System.Windows.Forms.MenuItem menuViewModelProperties;
		private System.Windows.Forms.MenuItem menuEditConnectionProperties;
		private System.Windows.Forms.MenuItem menuEditConnectionAdd;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuEditRunProperties;
		private System.Windows.Forms.MenuItem menuHelp;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem contextRun;
		private System.Windows.Forms.MenuItem menuComposition;

		#endregion		

		#region Member variables

		// pre-created dialogs
		ModelDialog _modelDialog;
		ConnectionDialog _connectionDialog;
		AboutBox _aboutBox;
		RunProperties _runProperties;
		RunBox _runBox;


		Cursor _sourceCursor, _targetCursor;

		bool _isAddingConnection = false;
		UIModel _sourceModel = null;
		
		bool _isMovingModel = false;
		Point _prevMouse;

		object _contextSelectedObject;
		
		CompositionManager _composition;
		
		string _compositionFilename = null;
		
		Point _compositionBoxPositionInArea;
		Rectangle _compositionArea;

		const string ApplicationTitle = "Configuration Editor";
		private System.Windows.Forms.MenuItem contextDivider;
		private System.Windows.Forms.MenuItem contextAddTrigger;
		private System.Windows.Forms.MenuItem menuHelpContents;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuOptions;
		private System.Windows.Forms.MenuItem menuRegisterExtensions;
				
		const string DefaultFilename = "NewComposition.opr";
        private ListView fileList;

        // record the culture that the application starts in
        System.Globalization.CultureInfo _cultureInfo = Application.CurrentCulture;

		#endregion
	
		/// <summary>
		/// Creates a new instance of <see cref="MainForm">MainForm</see> window.
		/// </summary>
		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			
			_compositionBoxPositionInArea = new Point(0,0);

			InitializeComponent();

			_composition = new CompositionManager();
            
			_prevMouse = new Point(0,0);

			_sourceCursor = new Cursor(GetType(), "Source.cur");
			_targetCursor = new Cursor(GetType(), "Target.cur");

			// create dialogs
			//_modelDialog = new ModelDialog();
			//_connectionDialog = new ConnectionDialog();
			//_aboutBox = new AboutBox();
			//_runProperties = new RunProperties();
			//_runBox = new RunBox();


			menuRegisterExtensions.Checked = Utils.AreFileExtensionsRegistered( Application.ExecutablePath );
		}

		
		#region Methods and properties

		/// <summary>
		/// Method is used to start application.
		/// </summary>
		/// <param name="args">Command-line arguments.</param>
		/// <remarks>Method proceeds all command-line args ("/opr %", "/reg", ...)
		/// and perform requested actions.</remarks>		
		private static void ProceedCommandLineArgs( string[] args )
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
				Control.CheckForIllegalCrossThreadCalls = false;
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
					FileInfo fileInfo = new FileInfo( oprFilename );

					mainForm.OpenOprFile( fileInfo.FullName );

					Application.Run( mainForm );
				}
				else if( omiFilename!=null )
				{
					// Create new project with one OMI model
					MainForm mainForm = new MainForm();
					FileInfo fileInfo = new FileInfo( omiFilename );

					mainForm.AddModel( fileInfo.FullName );

					Application.Run( mainForm );
				}
				else
					Application.Run( new MainForm() );
			}
			catch( Exception e )
			{
				MessageBox.Show( e.ToString(), "Exception occured", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		/// <summary>
		/// Opens composition from OPR file.
		/// </summary>
		/// <param name="fullPath">Full path to OPR file.</param>
		private void OpenOprFile( string fullPath )
		{
			try
			{				
				_compositionFilename = null;
				_composition.Release();	
				_composition.LoadFromFile( fullPath );
				_compositionFilename = fullPath;
			}
			catch( Exception ex )
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
		private void AddModel( string fullPath )
		{
			try 
			{
				_composition.AddModel( null, fullPath );
			}
			catch( Exception ex )
			{
				MessageBox.Show(
					"OMI filename: "+fullPath+"\n"+"Exception: "+ ex.ToString(),
					"Error occured while adding the model...",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );					
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
			this.Text = ApplicationTitle + (_composition.ShouldBeSaved ? " *" : "") ;
		}


		private void UpdateControls()
		{
			contextConnectionAdd.Enabled = menuEditConnectionAdd.Enabled = _composition.Models.Count > 1 ;
			
			bool hasTrigger = _composition.HasTrigger();
	
			contextAddTrigger.Enabled = menuEditTriggerAdd.Enabled = !hasTrigger;
		
			contextRun.Enabled = menuEditRunProperties.Enabled = hasTrigger && _composition.Models.Count > 1;
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
						menuFileSave_Click(null, null);
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
			// find maximum link ID of all existing links
			int maxID = 0;
			foreach( UIConnection uiLink in _composition.Connections )
				foreach( ILink iLink in uiLink.Links )
					maxID = Math.Max( int.Parse(iLink.ID), maxID );

			_connectionDialog.PopulateDialog( link, maxID+1 );
			if( _connectionDialog.ShowDialog(this) == DialogResult.OK )
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

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			MainForm_SizeChanged(sender, e);
			UpdateTitle();
			UpdateControls();
			CompositionUpdateArea();
		}


		private void MainForm_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			MessageBox.Show("form1, dragDrop");
		
		}

		
		private void MainForm_SizeChanged(object sender, System.EventArgs e)
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
		
			if( !e.Cancel )
			{
				_composition.Release();
			}
		}


		private void MainForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
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

		private void menuEditModelAdd_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			OpenFileDialog dlgFile = new OpenFileDialog();
			dlgFile.CheckFileExists = true;
			dlgFile.CheckPathExists = true;
			dlgFile.Title = "Add model...";
			dlgFile.Filter = "OpenMI models (*.omi)|*.omi|All files|*.*";
			dlgFile.Multiselect = false;

			if( dlgFile.ShowDialog( this ) == DialogResult.OK )	
				AddModel( dlgFile.FileName );

			dlgFile.Dispose();
		}


		private void menuEditTriggerAdd_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			try 
			{
				_composition.AddModel( null, CompositionManager.TriggerModelID );
			}
			catch( Exception ex )
			{
				MessageBox.Show(
					"Exception: "+ ex.ToString(),
					"Error occured while adding the trigger...",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );					
			}

			UpdateControls();	
			UpdateTitle();
			CompositionUpdateArea();
		}


		private void menuDeployRun_Click(object sender, System.EventArgs e)
		{
			StopAllActions();
			
			_runProperties.PopulateDialog( _composition, _compositionFilename==null );
			DialogResult result = _runProperties.ShowDialog( this );

			UpdateTitle();

			if( result == DialogResult.OK )
			{
				// user decided to run the composition
							
				// ### prepare listeners
				ArrayList listOfListeners = new ArrayList();

				// progress bar
				ProgressBarListener progressBarListener = new ProgressBarListener( _composition.GetSimulationTimehorizon(), _runBox.ProgressBarRun );
				listOfListeners.Add( progressBarListener );

				// log file
				if( _composition.LogToFile!=null && _composition.LogToFile!="" )
				{
					// get composition file's directory to logfile is saved in same directory
					string logFileName;
					if( _compositionFilename!=null )
					{
						FileInfo compositionFileInfo = new FileInfo(_compositionFilename);
						FileInfo logFileInfo = Utils.GetFileInfo( compositionFileInfo.DirectoryName, _composition.LogToFile ); 
						logFileName = logFileInfo.FullName;
					}
					else
						logFileName = _composition.LogToFile;

					LogFileListener logFileListener = new LogFileListener( _composition.ListenedEventTypes, logFileName );
					listOfListeners.Add( logFileListener );
				}

				// list box
				if( _composition.ShowEventsInListbox )
				{
					ListViewListener listViewListener = new ListViewListener( _composition.ListenedEventTypes, _runBox.ListViewEvents, 400 );
					listOfListeners.Add( listViewListener );
				}

				const uint actionInterval = 200; // in milliseconds

				// ### create proxy listener and register other listeners to it
				IListener proxyListener;				
				if( _composition.RunInSameThread )
				{
					// DoEvents listener
					DoEventsListener doEventsListener = new DoEventsListener( actionInterval );
					listOfListeners.Add( doEventsListener );

					ProxyListener proxySingleThreadListener = new ProxyListener();
					proxySingleThreadListener.Initialize( listOfListeners );
					proxyListener = proxySingleThreadListener;
				}
				else
				{									
					ProxyMultiThreadListener proxyMultiThreadListener = new ProxyMultiThreadListener();
					proxyMultiThreadListener.Initialize( listOfListeners, _runBox.Timer, (int)actionInterval );
					proxyListener = proxyMultiThreadListener;					
				}				

				// ### populate and show run-dialog and run simulation from it					
				Invalidate();
				_runBox.PopuplateDialog( _composition, proxyListener );
				_runBox.ShowDialog( this ); // this fires simulation					
				
				
			}				
		}


		private void menuFileNew_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			if( !CheckIfSaved() )
				return;

			_composition.Release();

			_compositionFilename = null;
			UpdateControls();
			UpdateTitle();
			CompositionUpdateArea();
		}
			

		private void menuFileOpen_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			if( !CheckIfSaved() )
				return;			

			OpenFileDialog dlgFile = new OpenFileDialog();
			dlgFile.Filter = "OmiEd projects (*.opr)|*.opr|All files|*.*";
			dlgFile.Multiselect = false;
			dlgFile.CheckFileExists = true;
			dlgFile.CheckPathExists = true;
			dlgFile.Title = "Open project...";

			if( dlgFile.ShowDialog( this ) == DialogResult.OK )
				OpenOprFile( dlgFile.FileName );					
			
			dlgFile.Dispose();

		}

		
		private void menuFileSave_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			string filename;		

			if( _compositionFilename == null )
			{
				SaveFileDialog dlgFile = new SaveFileDialog();			
				dlgFile.Filter = "OmiEd projects (*.opr)|*.opr|All files|*.*";
				dlgFile.ValidateNames = true;
				dlgFile.FileName = DefaultFilename;
				dlgFile.Title = "Save project...";
				dlgFile.AddExtension = true;
				dlgFile.OverwritePrompt = true;

				if( dlgFile.ShowDialog( this ) != DialogResult.OK )
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
				_composition.SaveToFile( filename );
				_compositionFilename = filename;
			}
			catch( System.Exception ex )
			{
				MessageBox.Show("Composition cannot be saved, make sure the file is not write-protected. Details: "+ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			UpdateTitle();			
		}


		private void menuFileSaveAs_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			SaveFileDialog dlgFile = new SaveFileDialog();			
			dlgFile.Filter = "OmiEd projects (*.opr)|*.opr|All files|*.*";
			dlgFile.ValidateNames = true;
			dlgFile.Title = "Save project As...";
			dlgFile.AddExtension = true;
			dlgFile.OverwritePrompt = true;

			if( _compositionFilename != null )			
				dlgFile.FileName = _compositionFilename;
			else
                dlgFile.FileName = DefaultFilename;

				 if( dlgFile.ShowDialog( this ) != DialogResult.OK )
			{
				dlgFile.Dispose();
				return;			
			}
			
			try
			{
				_composition.SaveToFile( dlgFile.FileName );
				_compositionFilename = dlgFile.FileName;				
			}
			catch( System.Exception ex )
			{
				MessageBox.Show("Composition cannot be saved, make sure the file is not write-protected. Details: "+ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			dlgFile.Dispose();

			UpdateTitle();
		}


		private void menuFileReload_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			_composition.Reload();		
		}


		private void menuFileExit_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			Close();		
		}


		private void menuEditConnectionAdd_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			_isAddingConnection = true;
			compositionBox.Cursor = _sourceCursor;	
			//Cursor.Current = _sourceCursor;
		}


		private void menuViewModelProperties_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			_modelDialog.PopulateDialog( _composition.Models );
			_modelDialog.ShowDialog( this );
		}


		private void menuRegisterExtensions_Click(object sender, System.EventArgs e)
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


		private void menuHelpAbout_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			_aboutBox.ShowDialog( this );			
		}


		private void menuHelpContents_Click(object sender, System.EventArgs e)
		{
			StopAllActions();

			FileInfo fileInfo = new FileInfo( Application.StartupPath + "\\Help.html" );

            if( !fileInfo.Exists )
                fileInfo = new FileInfo( Application.StartupPath + "\\HelpPage.htm" );
			
			// trick to open file in project directory (exe is in "projdir\bin\debug")
			// if not found in startup directory
			if( !fileInfo.Exists )
				fileInfo = new FileInfo( Application.StartupPath + "\\..\\..\\HelpPage.htm" );

			if( fileInfo.Exists )
			{
				ProcessStartInfo info = new ProcessStartInfo( fileInfo.FullName );
				Process.Start( info );		
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


			if( _contextSelectedObject == null )
			{
				contextDivider.Visible = false;
				contextConnectionRemove.Visible = false;
				contextConnectionProperties.Visible = false;
				contextModelProperties.Visible = false;
				contextModelRemove.Visible = false;
			}
			else if( _contextSelectedObject is UIConnection )
			{
				contextDivider.Visible = true;
				contextConnectionRemove.Visible = true;
				contextConnectionProperties.Visible = true;
				contextModelProperties.Visible = false;
				contextModelRemove.Visible = false;
			}
			else if( _contextSelectedObject is UIModel )
			{
				contextDivider.Visible = true;
				contextConnectionRemove.Visible = false;
				contextConnectionProperties.Visible = false;
				contextModelProperties.Visible = true;
				contextModelRemove.Visible = true;
			}
			else
				Debug.Assert( false );

			// Make disabled items invisible
			if( !contextConnectionRemove.Enabled  )
				contextConnectionRemove.Visible = false;
			if( !contextConnectionProperties.Enabled  )
				contextConnectionProperties.Visible = false;
			if( !contextModelProperties.Enabled  )
				contextModelProperties.Visible = false;
			if( !contextModelRemove.Enabled  )
				contextModelRemove.Visible = false;
			if( !contextAddTrigger.Enabled  )
				contextAddTrigger.Visible = false;
			if( !contextRun.Enabled  )
				contextRun.Visible = false;
			if( !contextConnectionAdd.Enabled  )
				contextConnectionAdd.Visible = false;
		}


		private void contextConnectionAdd_Click(object sender, System.EventArgs e)
		{
			menuEditConnectionAdd_Click(sender, e);	
			CompositionUpdateArea();
			UpdateControls();
			UpdateTitle();
		}

		private void contextConnectionRemove_Click(object sender, System.EventArgs e)
		{
			_composition.RemoveConnection( (UIConnection)_contextSelectedObject );
			CompositionUpdateArea();
			UpdateControls();
			UpdateTitle();
		}

		private void contextConnectionProperties_Click(object sender, System.EventArgs e)
		{
			ShowLinkDialog( (UIConnection)_contextSelectedObject );
			UpdateTitle();
		}

		private void contextModelAdd_Click(object sender, System.EventArgs e)
		{
			menuEditModelAdd_Click( sender, e );
		}

		private void contextModelRemove_Click(object sender, System.EventArgs e)
		{
			_composition.RemoveModel( (UIModel)_contextSelectedObject );
			CompositionUpdateArea();
			UpdateControls();
			UpdateTitle();
		}

		private void contextModelProperties_Click(object sender, System.EventArgs e)
		{
			_modelDialog.PopulateDialog( _composition.Models, ((UIModel)_contextSelectedObject).ModelID );
			_modelDialog.ShowDialog( this );			
		}

		private void contextRun_Click(object sender, System.EventArgs e)
		{
			menuDeployRun_Click(sender, e);			
		}


		private void contextAddTrigger_Click(object sender, System.EventArgs e)
		{
			menuEditTriggerAdd_Click( sender, e );		
		}


		#endregion

		

		#region Composition box event handlers

		private void compositionScrollBar_ValueChanged(object sender, System.EventArgs e)
		{
			_compositionBoxPositionInArea.X = compositionHScrollBar.Value;
			_compositionBoxPositionInArea.Y = compositionVScrollBar.Value;
			compositionBox.Invalidate();
		}
	
		private void compositionBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// draw OpenMI logo
			e.Graphics.DrawImage( imageList.Images[0], 0, 0 );

			foreach (UIConnection link in _composition.Connections)
				link.Draw( _compositionBoxPositionInArea, e.Graphics );
								
			foreach (UIModel model in _composition.Models)
				model.Draw( _compositionBoxPositionInArea, e.Graphics );			
			
			// Draw link currently being added (if any)
			//if( _isAddingLink && _leftMouseButtonIsDown )
			//	UIConnection.DrawLink( (float)_prevMouse.X, (float)_prevMouse.Y, (float)_currentMouse.X, (float)_currentMouse.Y, _compositionBoxPositionInArea, e.Graphics);
		}

		
		private void compositionBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
								_composition.AddConnection( _sourceModel, model );						
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

				// or show link dialog ?
				if( !actionFoundOut )
				{
					UIConnection connection = GetConnection(e.X,e.Y);
					if( connection!=null )					
						ShowLinkDialog( connection );
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

				contextMenu.Show( compositionBox, new Point(e.X,e.Y) );
			}
		}

		private void compositionBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
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
		
		private void compositionBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
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
		static void Main( string[] args ) 
		{
			try 
			{								
				ProceedCommandLineArgs( args );
			}
			catch( Exception e )
			{
				MessageBox.Show( e.ToString(), "Error occured while starting the application", MessageBoxButtons.OK, MessageBoxIcon.Error );				
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuFile = new System.Windows.Forms.MenuItem();
            this.menuFileNew = new System.Windows.Forms.MenuItem();
            this.menuItem17 = new System.Windows.Forms.MenuItem();
            this.menuFileReload = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuFileOpen = new System.Windows.Forms.MenuItem();
            this.menuFileSave = new System.Windows.Forms.MenuItem();
            this.menuFileSaveAs = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuFileExit = new System.Windows.Forms.MenuItem();
            this.menuComposition = new System.Windows.Forms.MenuItem();
            this.menuEditModelAdd = new System.Windows.Forms.MenuItem();
            this.menuEditConnectionAdd = new System.Windows.Forms.MenuItem();
            this.menuEditTriggerAdd = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuEditConnectionProperties = new System.Windows.Forms.MenuItem();
            this.menuViewModelProperties = new System.Windows.Forms.MenuItem();
            this.menuEditRunProperties = new System.Windows.Forms.MenuItem();
            this.menuOptions = new System.Windows.Forms.MenuItem();
            this.menuRegisterExtensions = new System.Windows.Forms.MenuItem();
            this.menuHelp = new System.Windows.Forms.MenuItem();
            this.menuHelpContents = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuHelpAbout = new System.Windows.Forms.MenuItem();
            this.compositionHScrollBar = new System.Windows.Forms.HScrollBar();
            this.compositionBox = new System.Windows.Forms.PictureBox();
            this.compositionVScrollBar = new System.Windows.Forms.VScrollBar();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.contextModelAdd = new System.Windows.Forms.MenuItem();
            this.contextConnectionAdd = new System.Windows.Forms.MenuItem();
            this.contextAddTrigger = new System.Windows.Forms.MenuItem();
            this.contextRun = new System.Windows.Forms.MenuItem();
            this.contextDivider = new System.Windows.Forms.MenuItem();
            this.contextConnectionRemove = new System.Windows.Forms.MenuItem();
            this.contextConnectionProperties = new System.Windows.Forms.MenuItem();
            this.contextModelRemove = new System.Windows.Forms.MenuItem();
            this.contextModelProperties = new System.Windows.Forms.MenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.fileList = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.compositionBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFile,
            this.menuComposition,
            this.menuOptions,
            this.menuHelp});
            // 
            // menuFile
            // 
            this.menuFile.Index = 0;
            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuFileNew,
            this.menuItem17,
            this.menuFileReload,
            this.menuItem18,
            this.menuFileOpen,
            this.menuFileSave,
            this.menuFileSaveAs,
            this.menuItem15,
            this.menuFileExit});
            this.menuFile.Text = "&File";
            // 
            // menuFileNew
            // 
            this.menuFileNew.Index = 0;
            this.menuFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.menuFileNew.Text = "&New";
            this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
            // 
            // menuItem17
            // 
            this.menuItem17.Index = 1;
            this.menuItem17.Text = "-";
            // 
            // menuFileReload
            // 
            this.menuFileReload.Index = 2;
            this.menuFileReload.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.menuFileReload.Text = "&Reload";
            this.menuFileReload.Click += new System.EventHandler(this.menuFileReload_Click);
            // 
            // menuItem18
            // 
            this.menuItem18.Index = 3;
            this.menuItem18.Text = "-";
            // 
            // menuFileOpen
            // 
            this.menuFileOpen.Index = 4;
            this.menuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.menuFileOpen.Text = "&Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            // 
            // menuFileSave
            // 
            this.menuFileSave.Index = 5;
            this.menuFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuFileSave.Text = "&Save";
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
            // 
            // menuFileSaveAs
            // 
            this.menuFileSaveAs.Index = 6;
            this.menuFileSaveAs.Text = "Save &As...";
            this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 7;
            this.menuItem15.Text = "-";
            // 
            // menuFileExit
            // 
            this.menuFileExit.Index = 8;
            this.menuFileExit.Text = "E&xit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuComposition
            // 
            this.menuComposition.Index = 1;
            this.menuComposition.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuEditModelAdd,
            this.menuEditConnectionAdd,
            this.menuEditTriggerAdd,
            this.menuItem1,
            this.menuEditConnectionProperties,
            this.menuViewModelProperties,
            this.menuEditRunProperties});
            this.menuComposition.Text = "&Composition";
            // 
            // menuEditModelAdd
            // 
            this.menuEditModelAdd.Index = 0;
            this.menuEditModelAdd.Text = "Add &Model";
            this.menuEditModelAdd.Click += new System.EventHandler(this.menuEditModelAdd_Click);
            // 
            // menuEditConnectionAdd
            // 
            this.menuEditConnectionAdd.Enabled = false;
            this.menuEditConnectionAdd.Index = 1;
            this.menuEditConnectionAdd.Text = "Add &Connection";
            this.menuEditConnectionAdd.Click += new System.EventHandler(this.menuEditConnectionAdd_Click);
            // 
            // menuEditTriggerAdd
            // 
            this.menuEditTriggerAdd.Index = 2;
            this.menuEditTriggerAdd.Text = "Add &Trigger";
            this.menuEditTriggerAdd.Click += new System.EventHandler(this.menuEditTriggerAdd_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 3;
            this.menuItem1.Text = "-";
            // 
            // menuEditConnectionProperties
            // 
            this.menuEditConnectionProperties.Enabled = false;
            this.menuEditConnectionProperties.Index = 4;
            this.menuEditConnectionProperties.Text = "Co&nnection properties...";
            // 
            // menuViewModelProperties
            // 
            this.menuViewModelProperties.Index = 5;
            this.menuViewModelProperties.Text = "Model &properties...";
            this.menuViewModelProperties.Click += new System.EventHandler(this.menuViewModelProperties_Click);
            // 
            // menuEditRunProperties
            // 
            this.menuEditRunProperties.Index = 6;
            this.menuEditRunProperties.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuEditRunProperties.Text = "&Run...";
            this.menuEditRunProperties.Click += new System.EventHandler(this.menuDeployRun_Click);
            // 
            // menuOptions
            // 
            this.menuOptions.Index = 2;
            this.menuOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuRegisterExtensions});
            this.menuOptions.Text = "&Options";
            // 
            // menuRegisterExtensions
            // 
            this.menuRegisterExtensions.Checked = true;
            this.menuRegisterExtensions.Index = 0;
            this.menuRegisterExtensions.Text = "&Register file extensions";
            this.menuRegisterExtensions.Click += new System.EventHandler(this.menuRegisterExtensions_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.Index = 3;
            this.menuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuHelpContents,
            this.menuItem3,
            this.menuHelpAbout});
            this.menuHelp.Text = "&Help";
            // 
            // menuHelpContents
            // 
            this.menuHelpContents.Index = 0;
            this.menuHelpContents.Shortcut = System.Windows.Forms.Shortcut.F1;
            this.menuHelpContents.Text = "Help contents";
            this.menuHelpContents.Click += new System.EventHandler(this.menuHelpContents_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "-";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Index = 2;
            this.menuHelpAbout.Text = "&About Configuration Editor ...";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
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
            this.compositionBox.Click += new System.EventHandler(this.compositionBox_Click);
            this.compositionBox.Paint += new System.Windows.Forms.PaintEventHandler(this.compositionBox_Paint);
            this.compositionBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseDown);
            this.compositionBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.compositionBox_MouseMove);
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
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
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
            // contextModelAdd
            // 
            this.contextModelAdd.Index = 0;
            this.contextModelAdd.Text = "Add Model...";
            this.contextModelAdd.Click += new System.EventHandler(this.contextModelAdd_Click);
            // 
            // contextConnectionAdd
            // 
            this.contextConnectionAdd.Index = 1;
            this.contextConnectionAdd.Text = "Add Connection";
            this.contextConnectionAdd.Click += new System.EventHandler(this.contextConnectionAdd_Click);
            // 
            // contextAddTrigger
            // 
            this.contextAddTrigger.Index = 2;
            this.contextAddTrigger.Text = "Add Trigger";
            this.contextAddTrigger.Click += new System.EventHandler(this.contextAddTrigger_Click);
            // 
            // contextRun
            // 
            this.contextRun.Index = 3;
            this.contextRun.Text = "Run...";
            this.contextRun.Click += new System.EventHandler(this.contextRun_Click);
            // 
            // contextDivider
            // 
            this.contextDivider.Index = 4;
            this.contextDivider.Text = "-";
            // 
            // contextConnectionRemove
            // 
            this.contextConnectionRemove.Index = 5;
            this.contextConnectionRemove.Text = "Remove connection";
            this.contextConnectionRemove.Click += new System.EventHandler(this.contextConnectionRemove_Click);
            // 
            // contextConnectionProperties
            // 
            this.contextConnectionProperties.Index = 6;
            this.contextConnectionProperties.Text = "Connection properties...";
            this.contextConnectionProperties.Click += new System.EventHandler(this.contextConnectionProperties_Click);
            // 
            // contextModelRemove
            // 
            this.contextModelRemove.Index = 7;
            this.contextModelRemove.Text = "Remove model";
            this.contextModelRemove.Click += new System.EventHandler(this.contextModelRemove_Click);
            // 
            // contextModelProperties
            // 
            this.contextModelProperties.Index = 8;
            this.contextModelProperties.Text = "Model properties...";
            this.contextModelProperties.Click += new System.EventHandler(this.contextModelProperties_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            // 
            // fileList
            // 
            this.fileList.Location = new System.Drawing.Point(0, 0);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(96, 236);
            this.fileList.TabIndex = 5;
            this.fileList.UseCompatibleStateImageBehavior = false;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(392, 253);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.compositionVScrollBar);
            this.Controls.Add(this.compositionBox);
            this.Controls.Add(this.compositionHScrollBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Configuration Editor";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.compositionBox)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion		

        private void compositionBox_Click(object sender, EventArgs e)
        {

        }
	

		
		#endregion			
		

	}
}
