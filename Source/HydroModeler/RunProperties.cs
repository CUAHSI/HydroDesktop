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
using System.Windows.Forms;
using Oatc.OpenMI.Gui.Core;
using Oatc.OpenMI.Sdk.DevelopmentSupport;
using OpenMI.Standard;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	/// <summary>
	/// Summary description for RunProperties.
	/// </summary>
	public class RunProperties : System.Windows.Forms.Form
	{
		#region Window controls

		private System.Windows.Forms.CheckBox cbEventTypeWarning;
		private System.Windows.Forms.CheckBox cbEventTypeValOutOfRange;
		private System.Windows.Forms.CheckBox cbEventTypeTimeStepProgress;
		private System.Windows.Forms.CheckBox cbEventTypeTargetBefore;
		private System.Windows.Forms.CheckBox cbEventTypeTargetAfter;
		private System.Windows.Forms.CheckBox cbEventTypeSourceBefore;
		private System.Windows.Forms.CheckBox cbEventTypeSourceAfter;
		private System.Windows.Forms.CheckBox cbEventTypeOther;
		private System.Windows.Forms.CheckBox cbEventTypeInformative;
		private System.Windows.Forms.CheckBox cbEventTypeGlobalProgress;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbEventTypeDataChanged;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonTimeLatestOverlapping;
		private System.Windows.Forms.Button buttonRun;
		private System.Windows.Forms.TextBox textTriggerInvokeTime;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.TextBox textLogToFile;
		private System.Windows.Forms.CheckBox checkBoxLogToFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		CheckBox[] checkboxesEventTypes;
		CompositionManager _composition;
		private System.Windows.Forms.Button buttonSetAll;
		private System.Windows.Forms.Button buttonClearAll;
		private System.Windows.Forms.Button buttonBrowseLogFile;
		private System.Windows.Forms.CheckBox checkBoxNoMultithreading;
		private System.Windows.Forms.CheckBox checkBoxEventsToListbox;
		bool runIt;

		/// <summary>
		/// Creates a new instance of <see cref="RunProperties">RunProperties</see> dialog.
		/// </summary>
		public RunProperties()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			checkboxesEventTypes = new CheckBox[ (int)EventType.NUM_OF_EVENT_TYPES ];			
			checkboxesEventTypes[0] = cbEventTypeWarning;
			checkboxesEventTypes[1] = cbEventTypeInformative;
			checkboxesEventTypes[2] = cbEventTypeValOutOfRange;
			checkboxesEventTypes[3] = cbEventTypeGlobalProgress;
			checkboxesEventTypes[4] = cbEventTypeTimeStepProgress;
			checkboxesEventTypes[5] = cbEventTypeDataChanged;
			checkboxesEventTypes[6] = cbEventTypeTargetBefore;
			checkboxesEventTypes[7] = cbEventTypeSourceAfter;
			checkboxesEventTypes[8] = cbEventTypeSourceBefore;
			checkboxesEventTypes[9] = cbEventTypeTargetAfter;
			checkboxesEventTypes[10] = cbEventTypeOther;

			this.DialogResult = DialogResult.OK;
		}


		/// <summary>
		/// Populates this dialog with specific composition.
		/// </summary>
		/// <param name="composition">Composition to be used for dialog.</param>
		/// <param name="initialTriggerInvokeTime">
		/// If <c>true</c>, the <see cref="CompositionManager.TriggerInvokeTime">CompositionManager.TriggerInvokeTime</see>
		/// is set to latest overlapping time of time horizons of all models. Typically this is used
		/// when this dialog is showed for the first time.</param>
		public void PopulateDialog( CompositionManager composition, bool initialTriggerInvokeTime )
		{
			_composition = composition;			

			Debug.Assert( _composition.HasTrigger() );
			Debug.Assert( _composition.Models.Count > 1 );

			// fill dialog according to composition
			if( _composition.LogToFile == null )
			{
				checkBoxLogToFile.Checked = false;
				textLogToFile.Text = "CompositionRun.log";
			}
			else
			{
				checkBoxLogToFile.Checked = true;
				textLogToFile.Text = _composition.LogToFile;
			}

			for( int i=0; i<(int)EventType.NUM_OF_EVENT_TYPES; i++ )
				checkboxesEventTypes[i].Checked = _composition.ListenedEventTypes[i];

			if( initialTriggerInvokeTime )
			{
				buttonTimeLatestOverlapping_Click(null, null);
			}
			else
			{
				textTriggerInvokeTime.Text = _composition.TriggerInvokeTime.ToString( );
			}

			checkBoxEventsToListbox.Checked = _composition.ShowEventsInListbox;

			checkBoxNoMultithreading.Checked = _composition.RunInSameThread;

			runIt = false;
		}


		private void SaveStateToComposition()
		{	
			_composition.LogToFile =  checkBoxLogToFile.Checked ? textLogToFile.Text : null;

			for( int i=0; i<(int)EventType.NUM_OF_EVENT_TYPES; i++ )
				if( _composition.ListenedEventTypes[i] != checkboxesEventTypes[i].Checked )
				{
					_composition.ListenedEventTypes[i] = checkboxesEventTypes[i].Checked;
					_composition.ShouldBeSaved = true;
				}			

			_composition.TriggerInvokeTime = DateTime.Parse( textTriggerInvokeTime.Text );

			_composition.RunInSameThread = checkBoxNoMultithreading.Checked;

			_composition.ShowEventsInListbox = checkBoxEventsToListbox.Checked;
		}


		private bool CheckControlsFormat()
		{
			// check format of date and time
			try 
			{
				DateTime.Parse(textTriggerInvokeTime.Text);
			}
			catch( FormatException )
			{
                switch (MessageBox.Show("Text you have entered is not valid date and time, please use format specific for " + Application.CurrentCulture.Name + " culture, i.e. " + Application.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + Application.CurrentCulture.DateTimeFormat.LongTimePattern + ", for example " + System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToLongTimeString(), "Invalid format", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
                {
					case DialogResult.Cancel:
						textTriggerInvokeTime.Text = _composition.TriggerInvokeTime.ToString();
						break;
					default:
						break;
				}
				textTriggerInvokeTime.Focus();
				return( false );
			}

			// check log filename
			if( checkBoxLogToFile.Checked )
				if( textLogToFile.Text==null || textLogToFile.Text=="" )
				{
					switch( MessageBox.Show("Text you have entered is not file name.", "Invalid format", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) )
					{
						case DialogResult.Cancel:
							checkBoxLogToFile.Checked = false;
							break;
						default:
							break;
					}
					textLogToFile.Focus();
					return( false );					
				}			


			return( true );
		}
		

		private void checkBoxLogToFile_CheckedChanged(object sender, System.EventArgs e)
		{
			textLogToFile.Enabled = checkBoxLogToFile.Checked;
			buttonBrowseLogFile.Enabled = checkBoxLogToFile.Checked;
		}


		private void RunProperties_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if( CheckControlsFormat() )
			{
				SaveStateToComposition();
				DialogResult = runIt ? DialogResult.OK : DialogResult.No;
			}
			else
			{
				e.Cancel = true;
			}
		}


		private void textTriggerInvokeTime_Leave(object sender, System.EventArgs e)
		{
			CheckControlsFormat();
		}


		private void buttonClose_Click(object sender, System.EventArgs e)
		{			
			Close();
		}


		private void buttonRun_Click(object sender, System.EventArgs e)
		{
			runIt = true;
			Close();
		}


		private void buttonTimeLatestOverlapping_Click(object sender, System.EventArgs e)
		{
			double start = double.MinValue,
				end = double.MaxValue;

			foreach( UIModel model in _composition.Models )
			{
				start = Math.Max( model.LinkableComponent.TimeHorizon.Start.ModifiedJulianDay, start );
				end =  Math.Min( model.LinkableComponent.TimeHorizon.End.ModifiedJulianDay, end );
			}

			if( start > end )
			{
				MessageBox.Show("Model timehorizons don't overlap.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			else
			{
				textTriggerInvokeTime.Text = CalendarConverter.ModifiedJulian2Gregorian( end ).ToString();
			}
		
		}


		private void buttonSetAll_Click(object sender, System.EventArgs e)
		{
			foreach( CheckBox checkBox in checkboxesEventTypes )
				checkBox.Checked = true;		
		}


		private void buttonClearAll_Click(object sender, System.EventArgs e)
		{
			foreach( CheckBox checkBox in checkboxesEventTypes )
				checkBox.Checked = false;		
		}


		private void buttonBrowseLogFile_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dlgFile = new SaveFileDialog();			
			dlgFile.Filter = "Run log (*.log)|*.log|All files|*.*";
			dlgFile.ValidateNames = true;
			dlgFile.Title = "Select log file...";
			dlgFile.AddExtension = true;
			dlgFile.OverwritePrompt = true;

			if( textLogToFile.Text.Length>0 )
				dlgFile.FileName = textLogToFile.Text;			

			if( dlgFile.ShowDialog() == DialogResult.OK )
				textLogToFile.Text = dlgFile.FileName;
			
			dlgFile.Dispose();		
		}

		#region .NET generated

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RunProperties));
			this.cbEventTypeWarning = new System.Windows.Forms.CheckBox();
			this.cbEventTypeValOutOfRange = new System.Windows.Forms.CheckBox();
			this.cbEventTypeTimeStepProgress = new System.Windows.Forms.CheckBox();
			this.cbEventTypeTargetBefore = new System.Windows.Forms.CheckBox();
			this.cbEventTypeTargetAfter = new System.Windows.Forms.CheckBox();
			this.cbEventTypeSourceBefore = new System.Windows.Forms.CheckBox();
			this.cbEventTypeSourceAfter = new System.Windows.Forms.CheckBox();
			this.cbEventTypeOther = new System.Windows.Forms.CheckBox();
			this.cbEventTypeInformative = new System.Windows.Forms.CheckBox();
			this.cbEventTypeGlobalProgress = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonClearAll = new System.Windows.Forms.Button();
			this.cbEventTypeDataChanged = new System.Windows.Forms.CheckBox();
			this.buttonSetAll = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBoxEventsToListbox = new System.Windows.Forms.CheckBox();
			this.checkBoxNoMultithreading = new System.Windows.Forms.CheckBox();
			this.textTriggerInvokeTime = new System.Windows.Forms.TextBox();
			this.buttonTimeLatestOverlapping = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonBrowseLogFile = new System.Windows.Forms.Button();
			this.textLogToFile = new System.Windows.Forms.TextBox();
			this.checkBoxLogToFile = new System.Windows.Forms.CheckBox();
			this.buttonRun = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbEventTypeWarning
			// 
			this.cbEventTypeWarning.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeWarning.Location = new System.Drawing.Point(16, 36);
			this.cbEventTypeWarning.Name = "cbEventTypeWarning";
			this.cbEventTypeWarning.Size = new System.Drawing.Size(120, 20);
			this.cbEventTypeWarning.TabIndex = 23;
			this.cbEventTypeWarning.Text = "Warning";
			// 
			// cbEventTypeValOutOfRange
			// 
			this.cbEventTypeValOutOfRange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeValOutOfRange.Location = new System.Drawing.Point(16, 84);
			this.cbEventTypeValOutOfRange.Name = "cbEventTypeValOutOfRange";
			this.cbEventTypeValOutOfRange.Size = new System.Drawing.Size(120, 20);
			this.cbEventTypeValOutOfRange.TabIndex = 22;
			this.cbEventTypeValOutOfRange.Text = "Value out of range";
			// 
			// cbEventTypeTimeStepProgress
			// 
			this.cbEventTypeTimeStepProgress.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeTimeStepProgress.Location = new System.Drawing.Point(16, 52);
			this.cbEventTypeTimeStepProgress.Name = "cbEventTypeTimeStepProgress";
			this.cbEventTypeTimeStepProgress.Size = new System.Drawing.Size(120, 20);
			this.cbEventTypeTimeStepProgress.TabIndex = 21;
			this.cbEventTypeTimeStepProgress.Text = "Time step progress";
			// 
			// cbEventTypeTargetBefore
			// 
			this.cbEventTypeTargetBefore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeTargetBefore.Location = new System.Drawing.Point(144, 68);
			this.cbEventTypeTargetBefore.Name = "cbEventTypeTargetBefore";
			this.cbEventTypeTargetBefore.Size = new System.Drawing.Size(192, 20);
			this.cbEventTypeTargetBefore.TabIndex = 20;
			this.cbEventTypeTargetBefore.Text = "Target before GetValues() call";
			// 
			// cbEventTypeTargetAfter
			// 
			this.cbEventTypeTargetAfter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeTargetAfter.Location = new System.Drawing.Point(144, 52);
			this.cbEventTypeTargetAfter.Name = "cbEventTypeTargetAfter";
			this.cbEventTypeTargetAfter.Size = new System.Drawing.Size(192, 20);
			this.cbEventTypeTargetAfter.TabIndex = 19;
			this.cbEventTypeTargetAfter.Text = "Target after GetValues() return";
			// 
			// cbEventTypeSourceBefore
			// 
			this.cbEventTypeSourceBefore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeSourceBefore.Location = new System.Drawing.Point(144, 36);
			this.cbEventTypeSourceBefore.Name = "cbEventTypeSourceBefore";
			this.cbEventTypeSourceBefore.Size = new System.Drawing.Size(192, 20);
			this.cbEventTypeSourceBefore.TabIndex = 18;
			this.cbEventTypeSourceBefore.Text = "Source before GetValues() return";
			// 
			// cbEventTypeSourceAfter
			// 
			this.cbEventTypeSourceAfter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeSourceAfter.Location = new System.Drawing.Point(144, 20);
			this.cbEventTypeSourceAfter.Name = "cbEventTypeSourceAfter";
			this.cbEventTypeSourceAfter.Size = new System.Drawing.Size(192, 20);
			this.cbEventTypeSourceAfter.TabIndex = 17;
			this.cbEventTypeSourceAfter.Text = "Source after GetValues() call";
			// 
			// cbEventTypeOther
			// 
			this.cbEventTypeOther.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeOther.Location = new System.Drawing.Point(144, 84);
			this.cbEventTypeOther.Name = "cbEventTypeOther";
			this.cbEventTypeOther.Size = new System.Drawing.Size(60, 20);
			this.cbEventTypeOther.TabIndex = 16;
			this.cbEventTypeOther.Text = "Other";
			// 
			// cbEventTypeInformative
			// 
			this.cbEventTypeInformative.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeInformative.Location = new System.Drawing.Point(16, 20);
			this.cbEventTypeInformative.Name = "cbEventTypeInformative";
			this.cbEventTypeInformative.Size = new System.Drawing.Size(120, 20);
			this.cbEventTypeInformative.TabIndex = 15;
			this.cbEventTypeInformative.Text = "Informative";
			// 
			// cbEventTypeGlobalProgress
			// 
			this.cbEventTypeGlobalProgress.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeGlobalProgress.Location = new System.Drawing.Point(16, 68);
			this.cbEventTypeGlobalProgress.Name = "cbEventTypeGlobalProgress";
			this.cbEventTypeGlobalProgress.Size = new System.Drawing.Size(120, 20);
			this.cbEventTypeGlobalProgress.TabIndex = 14;
			this.cbEventTypeGlobalProgress.Text = "Global progress";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.buttonClearAll);
			this.groupBox1.Controls.Add(this.cbEventTypeDataChanged);
			this.groupBox1.Controls.Add(this.buttonSetAll);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.groupBox1.Location = new System.Drawing.Point(4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(348, 124);
			this.groupBox1.TabIndex = 24;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Events listened during calculation";
			// 
			// buttonClearAll
			// 
			this.buttonClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonClearAll.Location = new System.Drawing.Point(276, 96);
			this.buttonClearAll.Name = "buttonClearAll";
			this.buttonClearAll.Size = new System.Drawing.Size(64, 20);
			this.buttonClearAll.TabIndex = 29;
			this.buttonClearAll.Text = "Clear all";
			this.buttonClearAll.Click += new System.EventHandler(this.buttonClearAll_Click);
			// 
			// cbEventTypeDataChanged
			// 
			this.cbEventTypeDataChanged.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cbEventTypeDataChanged.Location = new System.Drawing.Point(12, 96);
			this.cbEventTypeDataChanged.Name = "cbEventTypeDataChanged";
			this.cbEventTypeDataChanged.Size = new System.Drawing.Size(120, 20);
			this.cbEventTypeDataChanged.TabIndex = 0;
			this.cbEventTypeDataChanged.Text = "Data changed";
			// 
			// buttonSetAll
			// 
			this.buttonSetAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonSetAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonSetAll.Location = new System.Drawing.Point(212, 96);
			this.buttonSetAll.Name = "buttonSetAll";
			this.buttonSetAll.Size = new System.Drawing.Size(60, 20);
			this.buttonSetAll.TabIndex = 28;
			this.buttonSetAll.Text = "Set all";
			this.buttonSetAll.Click += new System.EventHandler(this.buttonSetAll_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxEventsToListbox);
			this.groupBox2.Controls.Add(this.checkBoxNoMultithreading);
			this.groupBox2.Controls.Add(this.textTriggerInvokeTime);
			this.groupBox2.Controls.Add(this.buttonTimeLatestOverlapping);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.buttonBrowseLogFile);
			this.groupBox2.Controls.Add(this.textLogToFile);
			this.groupBox2.Controls.Add(this.checkBoxLogToFile);
			this.groupBox2.Location = new System.Drawing.Point(4, 132);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(348, 108);
			this.groupBox2.TabIndex = 25;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Run properties";
			// 
			// checkBoxEventsToListbox
			// 
			this.checkBoxEventsToListbox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.checkBoxEventsToListbox.Location = new System.Drawing.Point(12, 64);
			this.checkBoxEventsToListbox.Name = "checkBoxEventsToListbox";
			this.checkBoxEventsToListbox.Size = new System.Drawing.Size(328, 16);
			this.checkBoxEventsToListbox.TabIndex = 29;
			this.checkBoxEventsToListbox.Text = "Show events in list-box";
			// 
			// checkBoxNoMultithreading
			// 
			this.checkBoxNoMultithreading.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.checkBoxNoMultithreading.Location = new System.Drawing.Point(12, 84);
			this.checkBoxNoMultithreading.Name = "checkBoxNoMultithreading";
			this.checkBoxNoMultithreading.Size = new System.Drawing.Size(328, 16);
			this.checkBoxNoMultithreading.TabIndex = 28;
			this.checkBoxNoMultithreading.Text = "Don\'t use separate thread";
			// 
			// textTriggerInvokeTime
			// 
			this.textTriggerInvokeTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textTriggerInvokeTime.Location = new System.Drawing.Point(96, 16);
			this.textTriggerInvokeTime.MaxLength = 30;
			this.textTriggerInvokeTime.Name = "textTriggerInvokeTime";
			this.textTriggerInvokeTime.Size = new System.Drawing.Size(132, 20);
			this.textTriggerInvokeTime.TabIndex = 3;
			this.textTriggerInvokeTime.Text = "12/31/1999 11:59:59 PM";
			this.textTriggerInvokeTime.Leave += new System.EventHandler(this.textTriggerInvokeTime_Leave);
			// 
			// buttonTimeLatestOverlapping
			// 
			this.buttonTimeLatestOverlapping.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonTimeLatestOverlapping.Location = new System.Drawing.Point(232, 16);
			this.buttonTimeLatestOverlapping.Name = "buttonTimeLatestOverlapping";
			this.buttonTimeLatestOverlapping.Size = new System.Drawing.Size(108, 20);
			this.buttonTimeLatestOverlapping.TabIndex = 1;
			this.buttonTimeLatestOverlapping.Text = "Latest overlapping";
			this.buttonTimeLatestOverlapping.Click += new System.EventHandler(this.buttonTimeLatestOverlapping_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Invoke trigger at:";
			// 
			// buttonBrowseLogFile
			// 
			this.buttonBrowseLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonBrowseLogFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonBrowseLogFile.Location = new System.Drawing.Point(312, 40);
			this.buttonBrowseLogFile.Name = "buttonBrowseLogFile";
			this.buttonBrowseLogFile.Size = new System.Drawing.Size(28, 20);
			this.buttonBrowseLogFile.TabIndex = 4;
			this.buttonBrowseLogFile.Text = "...";
			this.buttonBrowseLogFile.Click += new System.EventHandler(this.buttonBrowseLogFile_Click);
			// 
			// textLogToFile
			// 
			this.textLogToFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textLogToFile.Enabled = false;
			this.textLogToFile.Location = new System.Drawing.Point(96, 40);
			this.textLogToFile.Name = "textLogToFile";
			this.textLogToFile.Size = new System.Drawing.Size(212, 20);
			this.textLogToFile.TabIndex = 27;
			this.textLogToFile.Text = "CompositionRun.log";
			// 
			// checkBoxLogToFile
			// 
			this.checkBoxLogToFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.checkBoxLogToFile.Location = new System.Drawing.Point(12, 40);
			this.checkBoxLogToFile.Name = "checkBoxLogToFile";
			this.checkBoxLogToFile.Size = new System.Drawing.Size(80, 20);
			this.checkBoxLogToFile.TabIndex = 1;
			this.checkBoxLogToFile.Text = "Log to file:";
			this.checkBoxLogToFile.CheckedChanged += new System.EventHandler(this.checkBoxLogToFile_CheckedChanged);
			// 
			// buttonRun
			// 
			this.buttonRun.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonRun.Location = new System.Drawing.Point(168, 248);
			this.buttonRun.Name = "buttonRun";
			this.buttonRun.Size = new System.Drawing.Size(84, 24);
			this.buttonRun.TabIndex = 4;
			this.buttonRun.Text = "RUN !!!";
			this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonClose.Location = new System.Drawing.Point(260, 248);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(84, 24);
			this.buttonClose.TabIndex = 28;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// RunProperties
			// 
			this.AcceptButton = this.buttonRun;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(358, 279);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.cbEventTypeWarning);
			this.Controls.Add(this.cbEventTypeValOutOfRange);
			this.Controls.Add(this.cbEventTypeTimeStepProgress);
			this.Controls.Add(this.cbEventTypeTargetBefore);
			this.Controls.Add(this.cbEventTypeTargetAfter);
			this.Controls.Add(this.cbEventTypeSourceBefore);
			this.Controls.Add(this.cbEventTypeSourceAfter);
			this.Controls.Add(this.cbEventTypeOther);
			this.Controls.Add(this.cbEventTypeInformative);
			this.Controls.Add(this.cbEventTypeGlobalProgress);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonRun);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RunProperties";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Run properties";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.RunProperties_Closing);
			this.Load += new System.EventHandler(this.RunProperties_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void RunProperties_Load(object sender, System.EventArgs e)
		{
		
		}


		

		

		


		#endregion

		
	}
}
