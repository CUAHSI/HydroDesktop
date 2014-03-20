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
using System.IO;
using System.Windows.Forms;
using Oatc.OpenMI.Gui.Core;
using OpenMI.Standard;
//using DotSpatial.Controls.RibbonControls;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	/// <summary>
	/// Summary description for RunBox.
	/// </summary>
    public class RunBox : System.Windows.Forms.Form
	{
		CompositionManager _composition;


		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.ProgressBar progressBarRun;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label labelInfo;
		private System.ComponentModel.IContainer components;

		private bool _finished;	
		private bool _started;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Timer timerProgress;
		private System.Windows.Forms.ColumnHeader _colType;
		private System.Windows.Forms.ColumnHeader _colDescription;
		private System.Windows.Forms.ColumnHeader _colSender;
		private System.Windows.Forms.ColumnHeader _colSimulationTime;
		private System.Windows.Forms.ListView listViewEvents;
		private System.Windows.Forms.ColumnHeader _colOrder;
		IListener _listener;
        public string _currentDirectory;
        mainTab hydroModelerControl;


		/// <summary>
		/// Creates a new instance of <see cref="RunBox">RunBox</see> dialog.
		/// </summary>
		public RunBox(mainTab instance)
		{
            hydroModelerControl = instance;
			InitializeComponent();
		}

		/// <summary>
		/// Progress bar showing simulation progress.
		/// </summary>
		/// <remarks>This property is used to initialize <see cref="ProgressBarListener">ProgressBarListener</see>.</remarks>
		public ProgressBar ProgressBarRun
		{
			get { return(progressBarRun); }
		}

		/// <summary>
		/// Listview showing events during simulation.
		/// </summary>
		/// <remarks>This property is used to initialize <see cref="ListViewListener">ListViewListener</see>.</remarks>
		public ListView ListViewEvents
		{
			get { return(listViewEvents); }
		}

		/// <summary>
		/// Timer used to initiate sending of events to UI listeners.
		/// </summary>
		public Timer Timer
		{
			get { return(timerProgress); }
		}

		/// <summary>
		/// Populates this dialog with specified composition and proxy listener.
		/// </summary>
		/// <param name="composition">Composition which simulation is to be run.</param>
		/// <param name="listener">Listener which is used for monitoring simulation.</param>
		/// <remarks>
		/// Simulation is fired after this dialog is showed. That's because if
		/// simulation runs in same thread we won't be able to show it another way.
		/// We determine whether simulation runs in same thread using
		/// <see cref="CompositionManager.RunInSameThread">CompositionManager.RunInSameThread</see> property.
		/// </remarks>
		public void PopuplateDialog( CompositionManager composition, IListener listener )
		{
			_composition        = composition;
			_listener           = listener;			
			_finished           = false;
			_started            = false;
			buttonClose.Enabled = !composition.RunInSameThread;
			buttonStop.Enabled  = !composition.RunInSameThread;			

			progressBarRun.Value = 0;
			progressBarRun.Enabled = true;

			labelInfo.Text = "Running...";

			listViewEvents.Items.Clear();
		}


		private void RunBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            
			// this event is fired when this dialog is shown
			StartSimulation();		
		}


		/// <summary>
		/// Method fires simulation if not already running.
		/// </summary>
		private void StartSimulation()
		{
			// this method  is called from repaint handler

			// we start simulation here because now we are sure,
			// dialog is already visible on the screen.
			// Thats because if running in same thread, we won't be able to show it another way.
			if( !_started )
			{
				_started = true;
				Invalidate(); // next call may block this repaint handler, so another repaint event should be generated
				_composition.Run( _listener, _composition.RunInSameThread );				
			}
		}


		private void buttonStop_Click(object sender, System.EventArgs e)
		{
			if( buttonStop.Enabled )
			{
				_composition.Stop();
				buttonStop.Enabled = false;
			}
		}


		private void progressBarRun_EnabledChanged(object sender, System.EventArgs e)
		{
			// This event is fired by ProgressBarListener ( called from Proxy(MultiThread)Listener )
			// when simulation finishes	
			if( !progressBarRun.Enabled ) 
			{
				progressBarRun.Value = progressBarRun.Maximum;
				buttonStop.Enabled = false;
				buttonClose.Enabled = true;
				labelInfo.Text = "Finished...";
				_finished = true;
			}
		}
		

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}


		private void RunBox_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// if running in same thread and simulation hasn't finished yet,
			// we cannot close dialog
			if( !_finished )
			{	
				if( _composition.RunInSameThread )
				{
					e.Cancel = true; 
					return;
				}

				switch( MessageBox.Show("Simulation hasn't finished yet, do you want to stop it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) )
				{
					case DialogResult.Yes:
						buttonStop_Click( null, null );						
						break;

					default:
						e.Cancel = true;
						return;
				}
			}
			
			switch( MessageBox.Show("All models have been finished their simulation run. Would you like to reload project?\n\nNote: Models must be reloaded prior to simulation.  If \"No\" is selected, all models will be removed from the composition.", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) )
			{
                case DialogResult.Yes:
                    {

                        #region old reload method
                        //////_composition.Reload();


                        //////temporarily save file
                        ////_composition.SaveToFile(this._currentDirectory + "\\temp.opr");

                        //////clear composition window
                        ////_composition.RemoveAllModels();

                        //////HACK: fix to fix opr file
                        //////make sure that the trigger does not contain any extra characters
                        ////string filename = this._currentDirectory + "\\temp.opr";
                        ////StreamReader sr = new StreamReader(filename);
                        ////string contents = sr.ReadToEnd();
                        ////sr.Close();

                        ////if (contents.Contains("Oatc.OpenMI.Gui.Trigger"))
                        ////{
                        ////    int end = contents.IndexOf("Oatc.OpenMI.Gui.Trigger");
                        ////    int index = end - 1;
                        ////    int count = 0;
                        ////    while (contents[index] != '\"')
                        ////    {
                        ////        count++;
                        ////        index--;
                        ////    }

                        ////    contents = contents.Remove(end - count, count);
                        ////}

                        ////StreamWriter sw = new StreamWriter(filename);
                        ////sw.Write(contents);
                        ////sw.Close();

                        //////reload temp opr
                        ////_composition.LoadFromFile(this._currentDirectory + "\\temp.opr");

                        ////System.IO.File.Delete(this._currentDirectory + "\\temp.opr");

                        //////foreach (UIModel model in models)
                        //////_composition.AddModel(model.
                        //////    if (model.ModelID.Contains("Oatc"))
                        //////    {
                        //////        _composition.RemoveModel(model);
                        //////        break;
                        //////    }
                        //////_composition.Reload();
                        #endregion

                        //---- reload the composition ---

                        //-- get the current file path
                        string path = _composition.FilePath;

                        //-- overwrite the original file
                        _composition.SaveToFile(path);

                        //-- clear the composition window
                        _composition.RemoveAllModels();

                        //-- remove extra characters (in path) from the trigger
                        StreamReader sr = new StreamReader(path);
                        string contents = sr.ReadToEnd();
                        sr.Close();

                        if (contents.Contains("Oatc.OpenMI.Gui.Trigger"))
                        {
                            int end = contents.IndexOf("Oatc.OpenMI.Gui.Trigger");
                            int index = end - 1;
                            int count = 0;
                            while (contents[index] != '\"')
                            { count++; index--; }
                            contents = contents.Remove(end - count, count);
                        }

                        //-- rewrite the opr with revised trigger info
                        StreamWriter sw = new StreamWriter(path);
                        sw.Write(contents);
                        sw.Close();

                        //-- reopen the opr file
                        _composition.LoadFromFile(_composition.FilePath);

                        break;
                    }
				default:
                    hydroModelerControl.composition_clear();
					break;
			}
		
			// clean-up
			listViewEvents.Items.Clear();
		}

		

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunBox));
            this.labelInfo = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.progressBarRun = new System.Windows.Forms.ProgressBar();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timerProgress = new System.Windows.Forms.Timer(this.components);
            this.listViewEvents = new System.Windows.Forms.ListView();
            this._colOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colSender = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colSimulationTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInfo.Location = new System.Drawing.Point(4, 4);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(507, 16);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "Running...";
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonStop.Location = new System.Drawing.Point(327, 569);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(84, 24);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop !!!";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // progressBarRun
            // 
            this.progressBarRun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarRun.Location = new System.Drawing.Point(4, 24);
            this.progressBarRun.Maximum = 256;
            this.progressBarRun.Name = "progressBarRun";
            this.progressBarRun.Size = new System.Drawing.Size(511, 24);
            this.progressBarRun.TabIndex = 2;
            this.progressBarRun.EnabledChanged += new System.EventHandler(this.progressBarRun_EnabledChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Enabled = false;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonClose.Location = new System.Drawing.Point(419, 569);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(84, 24);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(4, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(511, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Events:";
            // 
            // listViewEvents
            // 
            this.listViewEvents.AllowColumnReorder = true;
            this.listViewEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewEvents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._colOrder,
            this._colType,
            this._colDescription,
            this._colSender,
            this._colSimulationTime});
            this.listViewEvents.FullRowSelect = true;
            this.listViewEvents.GridLines = true;
            this.listViewEvents.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.listViewEvents.Location = new System.Drawing.Point(4, 76);
            this.listViewEvents.Name = "listViewEvents";
            this.listViewEvents.Size = new System.Drawing.Size(511, 483);
            this.listViewEvents.TabIndex = 6;
            this.listViewEvents.UseCompatibleStateImageBehavior = false;
            this.listViewEvents.View = System.Windows.Forms.View.Details;
            // 
            // _colOrder
            // 
            this._colOrder.Text = "Order";
            this._colOrder.Width = 48;
            // 
            // _colType
            // 
            this._colType.Text = "Type";
            this._colType.Width = 82;
            // 
            // _colDescription
            // 
            this._colDescription.Text = "Description";
            this._colDescription.Width = 147;
            // 
            // _colSender
            // 
            this._colSender.Text = "Sender";
            this._colSender.Width = 82;
            // 
            // _colSimulationTime
            // 
            this._colSimulationTime.Text = "Simulation Time";
            this._colSimulationTime.Width = 155;
            // 
            // RunBox
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(519, 600);
            this.Controls.Add(this.listViewEvents);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.progressBarRun);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.labelInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 200);
            this.Name = "RunBox";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Simulation progress";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.RunBox_Closing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RunBox_Paint);
            this.ResumeLayout(false);

		}
		#endregion

		
	
		

	}
}
