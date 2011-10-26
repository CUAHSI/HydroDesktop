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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Gui.Core;
using Oatc.OpenMI.Gui.Controls;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	/// <summary>
	/// Summary description for ModelDialog.
	/// </summary>
	public class ModelDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox comboBoxModel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Button buttonClose;
		private ExchangeItemSelector outputExchangeItemSelector;
		private ExchangeItemSelector inputExchangeItemSelector;
		private System.Windows.Forms.GroupBox groupBoxProperties;
		private System.Windows.Forms.Button buttonViewElementSet;
		private System.Windows.Forms.GroupBox groupBoxOutputExchnageItems;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.GroupBox groupBoxInputExchangeItems;
		private System.Windows.Forms.Splitter splitterVertical;
		private System.Windows.Forms.Splitter splitterHorizontal;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates a new instance of <see cref="ModelDialog">ModelDialog</see> dialog.
		/// </summary>
		public ModelDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		
			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			_elementSetViewer = new ElementSetViewer();
            //_elementSetViewer.ResizeEnd += new EventHandler(elementSetViewer_ResizeEnd);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ModelDialog));
			this.comboBoxModel = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.outputExchangeItemSelector = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
			this.groupBoxOutputExchnageItems = new System.Windows.Forms.GroupBox();
			this.inputExchangeItemSelector = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
			this.groupBoxProperties = new System.Windows.Forms.GroupBox();
			this.buttonViewElementSet = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.groupBoxInputExchangeItems = new System.Windows.Forms.GroupBox();
			this.splitterVertical = new System.Windows.Forms.Splitter();
			this.splitterHorizontal = new System.Windows.Forms.Splitter();
			this.groupBoxOutputExchnageItems.SuspendLayout();
			this.groupBoxProperties.SuspendLayout();
			this.panelTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.groupBoxInputExchangeItems.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxModel
			// 
			this.comboBoxModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxModel.Location = new System.Drawing.Point(60, 4);
			this.comboBoxModel.Name = "comboBoxModel";
			this.comboBoxModel.Size = new System.Drawing.Size(460, 21);
			this.comboBoxModel.TabIndex = 0;
			this.comboBoxModel.SelectedIndexChanged += new System.EventHandler(this.comboBoxModel_SelectedIndexChanged);
			this.comboBoxModel.Enter += new System.EventHandler(this.comboBoxModel_Enter);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "Model:";
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(8, 16);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(258, 272);
			this.propertyGrid.TabIndex = 28;
			this.propertyGrid.Text = "propertyGrid1";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// outputExchangeItemSelector
			// 
			this.outputExchangeItemSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.outputExchangeItemSelector.Location = new System.Drawing.Point(8, 16);
			this.outputExchangeItemSelector.Name = "outputExchangeItemSelector";
			this.outputExchangeItemSelector.Size = new System.Drawing.Size(232, 136);
			this.outputExchangeItemSelector.TabIndex = 27;
			this.outputExchangeItemSelector.SelectionChanged += new System.EventHandler(this.outputExchangeItemSelector_SelectionChanged);
			// 
			// groupBoxOutputExchnageItems
			// 
			this.groupBoxOutputExchnageItems.Controls.Add(this.outputExchangeItemSelector);
			this.groupBoxOutputExchnageItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxOutputExchnageItems.Location = new System.Drawing.Point(8, 28);
			this.groupBoxOutputExchnageItems.Name = "groupBoxOutputExchnageItems";
			this.groupBoxOutputExchnageItems.Size = new System.Drawing.Size(248, 157);
			this.groupBoxOutputExchnageItems.TabIndex = 29;
			this.groupBoxOutputExchnageItems.TabStop = false;
			this.groupBoxOutputExchnageItems.Text = " Output Exchange Items";
			// 
			// inputExchangeItemSelector
			// 
			this.inputExchangeItemSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.inputExchangeItemSelector.Location = new System.Drawing.Point(8, 16);
			this.inputExchangeItemSelector.Name = "inputExchangeItemSelector";
			this.inputExchangeItemSelector.Size = new System.Drawing.Size(232, 144);
			this.inputExchangeItemSelector.TabIndex = 1;
			this.inputExchangeItemSelector.SelectionChanged += new System.EventHandler(this.inputExchangeItemSelector_SelectionChanged);
			// 
			// groupBoxProperties
			// 
			this.groupBoxProperties.Controls.Add(this.propertyGrid);
			this.groupBoxProperties.Controls.Add(this.buttonViewElementSet);
			this.groupBoxProperties.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBoxProperties.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBoxProperties.Location = new System.Drawing.Point(260, 28);
			this.groupBoxProperties.Name = "groupBoxProperties";
			this.groupBoxProperties.Size = new System.Drawing.Size(272, 325);
			this.groupBoxProperties.TabIndex = 30;
			this.groupBoxProperties.TabStop = false;
			this.groupBoxProperties.Text = "ElementSet properties";
			// 
			// buttonViewElementSet
			// 
			this.buttonViewElementSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonViewElementSet.Enabled = false;
			this.buttonViewElementSet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonViewElementSet.Location = new System.Drawing.Point(12, 293);
			this.buttonViewElementSet.Name = "buttonViewElementSet";
			this.buttonViewElementSet.Size = new System.Drawing.Size(108, 24);
			this.buttonViewElementSet.TabIndex = 33;
			this.buttonViewElementSet.Text = "ElementSet viewer";
			this.buttonViewElementSet.Click += new System.EventHandler(this.buttonViewElementSet_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonClose.Location = new System.Drawing.Point(428, 4);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(88, 28);
			this.buttonClose.TabIndex = 31;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.label1);
			this.panelTop.Controls.Add(this.comboBoxModel);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(8, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(524, 28);
			this.panelTop.TabIndex = 32;
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.buttonClose);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(8, 353);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(524, 36);
			this.panelBottom.TabIndex = 33;
			// 
			// groupBoxInputExchangeItems
			// 
			this.groupBoxInputExchangeItems.Controls.Add(this.inputExchangeItemSelector);
			this.groupBoxInputExchangeItems.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBoxInputExchangeItems.Location = new System.Drawing.Point(8, 189);
			this.groupBoxInputExchangeItems.Name = "groupBoxInputExchangeItems";
			this.groupBoxInputExchangeItems.Size = new System.Drawing.Size(248, 164);
			this.groupBoxInputExchangeItems.TabIndex = 0;
			this.groupBoxInputExchangeItems.TabStop = false;
			this.groupBoxInputExchangeItems.Text = " Input Exchange Items";
			// 
			// splitterVertical
			// 
			this.splitterVertical.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitterVertical.Location = new System.Drawing.Point(256, 28);
			this.splitterVertical.MinExtra = 150;
			this.splitterVertical.MinSize = 150;
			this.splitterVertical.Name = "splitterVertical";
			this.splitterVertical.Size = new System.Drawing.Size(4, 325);
			this.splitterVertical.TabIndex = 34;
			this.splitterVertical.TabStop = false;
			// 
			// splitterHorizontal
			// 
			this.splitterHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitterHorizontal.Location = new System.Drawing.Point(8, 185);
			this.splitterHorizontal.MinExtra = 150;
			this.splitterHorizontal.MinSize = 150;
			this.splitterHorizontal.Name = "splitterHorizontal";
			this.splitterHorizontal.Size = new System.Drawing.Size(248, 4);
			this.splitterHorizontal.TabIndex = 35;
			this.splitterHorizontal.TabStop = false;
			// 
			// ModelDialog
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(540, 397);
			this.Controls.Add(this.groupBoxOutputExchnageItems);
			this.Controls.Add(this.splitterHorizontal);
			this.Controls.Add(this.groupBoxInputExchangeItems);
			this.Controls.Add(this.splitterVertical);
			this.Controls.Add(this.groupBoxProperties);
			this.Controls.Add(this.panelTop);
			this.Controls.Add(this.panelBottom);
			this.DockPadding.Bottom = 8;
			this.DockPadding.Left = 8;
			this.DockPadding.Right = 8;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(500, 400);
			this.Name = "ModelDialog";
			this.ShowInTaskbar = false;
			this.Text = "Model properties";
			this.Resize += new System.EventHandler(this.ModelDialog_Resize);
			this.groupBoxOutputExchnageItems.ResumeLayout(false);
			this.groupBoxProperties.ResumeLayout(false);
			this.panelTop.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.groupBoxInputExchangeItems.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private ArrayList _uiModels;
		private string _loadedModelID;

		private ElementSetViewer _elementSetViewer;

		/// <summary>
		/// Populates this dialog with models.
		/// </summary>
		/// <param name="uiModels">List of models, ie. <see cref="UIModel">UIModel</see> objects.</param>
		/// <param name="modelID">ID of the model to be selected.</param>		
		public void PopulateDialog( ArrayList uiModels, string modelID )
		{
			_uiModels = uiModels;
			_loadedModelID = null;			

			string modelIdToSelect = null;

			comboBoxModel.Items.Clear();

			for( int i=0; i<uiModels.Count; i++ )
			{
				UIModel model = (UIModel)uiModels[i];

				if( i==0 || (modelID != null && modelID == model.ModelID) )					
					modelIdToSelect = model.ModelID;
				
				comboBoxModel.Items.Add( model.ModelID );
			}			
			
			SelectModel( modelIdToSelect );
		}
		
		/// <summary>
		/// Populates this dialog with models.
		/// </summary>
		/// <param name="uiModels">List of models, ie. <see cref="UIModel">UIModel</see> objects.</param>
		public void PopulateDialog( ArrayList uiModels )
		{
			PopulateDialog( uiModels, null );			
		}


		/// <summary>
		/// Selects one model to be shown in dialog.
		/// </summary>
		/// <param name="modelID">ID of model to be selected.</param>		
		public void SelectModel( string modelID )
		{
			if( modelID == null )
			{
				outputExchangeItemSelector.PopulateExchangeItemTree( new OutputExchangeItem[0], false );
				inputExchangeItemSelector.PopulateExchangeItemTree( new InputExchangeItem[0], false );
				_loadedModelID = null;
			}
			else
			{
				// find model by ID
				int modelIndex = -1;
				for( int i=0; i<comboBoxModel.Items.Count; i++ )
					if( (string)comboBoxModel.Items[i] == modelID )
					{
						modelIndex = i;
						break;
					}				
							
				if( modelIndex < 0 || modelIndex>=_uiModels.Count )
				{
					// model with modelID wasn't found 
					Debug.Assert( false );
					SelectModel( null );
					return;
				}

				UIModel selectedModel = (UIModel)_uiModels[ modelIndex ];

				Debug.Assert( selectedModel.ModelID == modelID );

				// load exchange items (if they aren't already loaded)
				if( modelID != _loadedModelID )
				{
					IExchangeItem[] outputExchangeItems = new IExchangeItem[ selectedModel.LinkableComponent.OutputExchangeItemCount ];
					for (int i = 0; i < selectedModel.LinkableComponent.OutputExchangeItemCount; i++)
						outputExchangeItems[i] = selectedModel.LinkableComponent.GetOutputExchangeItem( i );
					outputExchangeItemSelector.PopulateExchangeItemTree( outputExchangeItems, false );

					IExchangeItem[] inputExchangeItems = new IExchangeItem[ selectedModel.LinkableComponent.InputExchangeItemCount ];
					for (int i = 0; i < selectedModel.LinkableComponent.InputExchangeItemCount; i++)
						inputExchangeItems[i] = selectedModel.LinkableComponent.GetInputExchangeItem(i);
					inputExchangeItemSelector.PopulateExchangeItemTree( inputExchangeItems, false );

					_loadedModelID = selectedModel.ModelID;
				}

				// select model also in comboBox
				// this can cause this method is reentered
				comboBoxModel.SelectedIndex = modelIndex;

				//labelInfo.Text = "Model " + selectedModel.ModelID;

				// show properties of this model
				PropertyGridSelectObject( selectedModel.LinkableComponent );
			}
		}

		private void PropertyGridSelectObject( object obj )
		{
			buttonViewElementSet.Enabled = false;

			// show properties of selected object (if any)
			if( obj!=null )
			{
				propertyGrid.SelectedObject = Oatc.OpenMI.Gui.Controls.PropertyManager.ConstructPropertyManager( obj, true );
				
				// Modify text in groupBoxProperties
				if( obj is IQuantity )
					groupBoxProperties.Text = " Quantity properties";
				else if( obj is IElementSet )
				{
					buttonViewElementSet.Enabled = true;
					groupBoxProperties.Text = " ElementSet properties";
				}
				else if( obj is IDataOperation )
					groupBoxProperties.Text = " DataOperation properties";
				else if( obj is ILinkableComponent )
					groupBoxProperties.Text = " LinkableComponent properties";
				else
				{
					groupBoxProperties.Text = " Properties";
					Debug.Assert( false );
				}
			}
			else
			{
				// no object is selected
				propertyGrid.SelectedObject = null;
				groupBoxProperties.Text = " Properties";
			}
			
		}


		private void comboBoxModel_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if( comboBoxModel.SelectedIndex < 0 )
				SelectModel( null );
			else
				SelectModel( (string)comboBoxModel.Items[comboBoxModel.SelectedIndex] );			
		}

		private void comboBoxModel_Enter(object sender, System.EventArgs e)
		{
				comboBoxModel_SelectedIndexChanged( sender, e );		
		}		


		private void outputExchangeItemSelector_SelectionChanged(object sender, System.EventArgs e)
		{
			PropertyGridSelectObject( outputExchangeItemSelector.GetSelectedObject() );		
		}
		
		private void inputExchangeItemSelector_SelectionChanged(object sender, System.EventArgs e)
		{
			PropertyGridSelectObject( inputExchangeItemSelector.GetSelectedObject() );			
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void buttonViewElementSet_Click(object sender, System.EventArgs e)
		{
			Debug.Assert( buttonViewElementSet.Enabled );

			if( propertyGrid.SelectedObject!=null )
				if( ((Oatc.OpenMI.Gui.Controls.PropertyManager)propertyGrid.SelectedObject).Tag is IElementSet )
			{
				ArrayList elementSets = new ArrayList();
				elementSets.Add( ((Oatc.OpenMI.Gui.Controls.PropertyManager)propertyGrid.SelectedObject).Tag );
				_elementSetViewer.PopulateDialog( elementSets );
				_elementSetViewer.ShowDialog();				
			}		
		}

	
		private void ModelDialog_Resize(object sender, System.EventArgs e)
		{
			if( groupBoxInputExchangeItems.Width < 100 )
				groupBoxInputExchangeItems.Width = 100;
			if( groupBoxInputExchangeItems.Height < 100 )
				groupBoxInputExchangeItems.Height = 100;

			if( groupBoxOutputExchnageItems.Width < 100 )
				groupBoxOutputExchnageItems.Width = 100;
			if( groupBoxOutputExchnageItems.Height < 100 )
				groupBoxOutputExchnageItems.Height = 100;
		
		}


        /// <summary>
        /// Forces the element set viewer to redraw.  This is triggered after the form has stopped moving. (moved into elementset viewer class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void elementSetViewer_ResizeEnd(object sender, System.EventArgs e)
        //{
        //    //force the elementset view to refresh
        //    _elementSetViewer.Refresh();

        //}
    }
}
