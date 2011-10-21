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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Oatc.OpenMI.Gui.Controls;
using Oatc.OpenMI.Gui.Core;
using OpenMI.Standard2;

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
		private System.Windows.Forms.Button btnViewer;
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

        ExchangeItemSelector.TreeOptions _treeOptionsSources;
        ExchangeItemSelector.TreeOptions _treeOptionsTargets;

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

            _treeOptionsSources.IsSource = true;
            _treeOptionsTargets.IsTarget = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelDialog));
            this.comboBoxModel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.outputExchangeItemSelector = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
            this.groupBoxOutputExchnageItems = new System.Windows.Forms.GroupBox();
            this.inputExchangeItemSelector = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
            this.groupBoxProperties = new System.Windows.Forms.GroupBox();
            this.btnViewer = new System.Windows.Forms.Button();
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
            this.comboBoxModel.Size = new System.Drawing.Size(485, 21);
            this.comboBoxModel.TabIndex = 0;
            this.comboBoxModel.SelectedIndexChanged += new System.EventHandler(this.comboBoxModel_SelectedIndexChanged);
            this.comboBoxModel.Enter += new System.EventHandler(this.comboBoxModel_Enter);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.propertyGrid.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(8, 16);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(258, 311);
            this.propertyGrid.TabIndex = 28;
            // 
            // outputExchangeItemSelector
            // 
            this.outputExchangeItemSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputExchangeItemSelector.Location = new System.Drawing.Point(8, 16);
            this.outputExchangeItemSelector.Name = "outputExchangeItemSelector";
            this.outputExchangeItemSelector.Size = new System.Drawing.Size(257, 142);
            this.outputExchangeItemSelector.TabIndex = 27;
            this.outputExchangeItemSelector.SelectionChanged += new System.EventHandler(this.outputExchangeItemSelector_SelectionChanged);
            // 
            // groupBoxOutputExchnageItems
            // 
            this.groupBoxOutputExchnageItems.Controls.Add(this.outputExchangeItemSelector);
            this.groupBoxOutputExchnageItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOutputExchnageItems.Location = new System.Drawing.Point(8, 28);
            this.groupBoxOutputExchnageItems.Name = "groupBoxOutputExchnageItems";
            this.groupBoxOutputExchnageItems.Size = new System.Drawing.Size(273, 163);
            this.groupBoxOutputExchnageItems.TabIndex = 29;
            this.groupBoxOutputExchnageItems.TabStop = false;
            this.groupBoxOutputExchnageItems.Text = "Sources";
            // 
            // inputExchangeItemSelector
            // 
            this.inputExchangeItemSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.inputExchangeItemSelector.Location = new System.Drawing.Point(8, 16);
            this.inputExchangeItemSelector.Name = "inputExchangeItemSelector";
            this.inputExchangeItemSelector.Size = new System.Drawing.Size(257, 144);
            this.inputExchangeItemSelector.TabIndex = 1;
            this.inputExchangeItemSelector.SelectionChanged += new System.EventHandler(this.inputExchangeItemSelector_SelectionChanged);
            // 
            // groupBoxProperties
            // 
            this.groupBoxProperties.Controls.Add(this.propertyGrid);
            this.groupBoxProperties.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBoxProperties.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxProperties.Location = new System.Drawing.Point(285, 28);
            this.groupBoxProperties.Name = "groupBoxProperties";
            this.groupBoxProperties.Size = new System.Drawing.Size(272, 331);
            this.groupBoxProperties.TabIndex = 30;
            this.groupBoxProperties.TabStop = false;
            this.groupBoxProperties.Text = "Properties";
            // 
            // btnViewer
            // 
            this.btnViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewer.Enabled = false;
            this.btnViewer.Location = new System.Drawing.Point(0, 6);
            this.btnViewer.Name = "btnViewer";
            this.btnViewer.Size = new System.Drawing.Size(108, 24);
            this.btnViewer.TabIndex = 33;
            this.btnViewer.Text = "&Viewer";
            this.btnViewer.Click += new System.EventHandler(this.buttonViewElementSet_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(441, 6);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(108, 24);
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
            this.panelTop.Size = new System.Drawing.Size(549, 28);
            this.panelTop.TabIndex = 32;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonClose);
            this.panelBottom.Controls.Add(this.btnViewer);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(8, 359);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(549, 30);
            this.panelBottom.TabIndex = 33;
            // 
            // groupBoxInputExchangeItems
            // 
            this.groupBoxInputExchangeItems.Controls.Add(this.inputExchangeItemSelector);
            this.groupBoxInputExchangeItems.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxInputExchangeItems.Location = new System.Drawing.Point(8, 195);
            this.groupBoxInputExchangeItems.Name = "groupBoxInputExchangeItems";
            this.groupBoxInputExchangeItems.Size = new System.Drawing.Size(273, 164);
            this.groupBoxInputExchangeItems.TabIndex = 0;
            this.groupBoxInputExchangeItems.TabStop = false;
            this.groupBoxInputExchangeItems.Text = "Targets";
            // 
            // splitterVertical
            // 
            this.splitterVertical.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterVertical.Location = new System.Drawing.Point(281, 28);
            this.splitterVertical.MinExtra = 150;
            this.splitterVertical.MinSize = 150;
            this.splitterVertical.Name = "splitterVertical";
            this.splitterVertical.Size = new System.Drawing.Size(4, 331);
            this.splitterVertical.TabIndex = 34;
            this.splitterVertical.TabStop = false;
            // 
            // splitterHorizontal
            // 
            this.splitterHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterHorizontal.Location = new System.Drawing.Point(8, 191);
            this.splitterHorizontal.MinExtra = 150;
            this.splitterHorizontal.MinSize = 150;
            this.splitterHorizontal.Name = "splitterHorizontal";
            this.splitterHorizontal.Size = new System.Drawing.Size(273, 4);
            this.splitterHorizontal.TabIndex = 35;
            this.splitterHorizontal.TabStop = false;
            // 
            // ModelDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(565, 397);
            this.Controls.Add(this.groupBoxOutputExchnageItems);
            this.Controls.Add(this.splitterHorizontal);
            this.Controls.Add(this.groupBoxInputExchangeItems);
            this.Controls.Add(this.splitterVertical);
            this.Controls.Add(this.groupBoxProperties);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "ModelDialog";
            this.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
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


        private List<UIModel> _uiModels;
		private string _loadedModelID;

		private ElementSetViewer _elementSetViewer;

		/// <summary>
		/// Populates this dialog with models.
		/// </summary>
		/// <param name="uiModels">List of models, ie. <see cref="UIModel">UIModel</see> objects.</param>
		/// <param name="modelID">ID of the model to be selected.</param>		
		public void PopulateDialog( List<UIModel> uiModels, string modelID )
		{
			_uiModels = uiModels;
			_loadedModelID = null;			

			string modelIdToSelect = null;

			comboBoxModel.Items.Clear();

			for( int i=0; i<uiModels.Count; i++ )
			{
				UIModel model = (UIModel)uiModels[i];

				if( i==0 || (modelID != null && modelID == model.InstanceCaption) )					
					modelIdToSelect = model.InstanceCaption;
				
				comboBoxModel.Items.Add( model.InstanceCaption );
			}			
			
			SelectModel( modelIdToSelect );
		}
		
		/// <summary>
		/// Populates this dialog with models.
		/// </summary>
		/// <param name="uiModels">List of models, ie. <see cref="UIModel">UIModel</see> objects.</param>
        public void PopulateDialog(List<UIModel> uiModels)
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
				outputExchangeItemSelector.TreePopulate(_treeOptionsSources);
                inputExchangeItemSelector.TreePopulate(_treeOptionsTargets);
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

				Debug.Assert( selectedModel.InstanceCaption == modelID );

				// load exchange items (if they aren't already loaded)
				if( modelID != _loadedModelID )
				{
                    outputExchangeItemSelector.TreePopulate(selectedModel.LinkableComponent, _treeOptionsSources);
                    inputExchangeItemSelector.TreePopulate(selectedModel.LinkableComponent, _treeOptionsTargets);

					_loadedModelID = selectedModel.InstanceCaption;
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
            propertyGrid.SelectedObject = Oatc.OpenMI.Gui.Controls.PropertyPane.Selection(obj);
/*
            string text = "";
		    propertyGrid.SelectedObject = Oatc.OpenMI.Gui.Controls.PropertyManager.ConstructPropertyManager( obj, true, ref text );
			groupBoxProperties.Text = text;			
 * */
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
            UIExchangeItem item = outputExchangeItemSelector.GetSelectedObject();

            PropertyGridSelectObject(item);

            btnViewer.Enabled = item != null && item is IOutput;
        }
		
		private void inputExchangeItemSelector_SelectionChanged(object sender, System.EventArgs e)
		{
            object item = inputExchangeItemSelector.GetSelectedObject();

            PropertyGridSelectObject(item);

            btnViewer.Enabled = item != null && item is IInput;
        }

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void buttonViewElementSet_Click(object sender, System.EventArgs e)
		{
			Debug.Assert(btnViewer.Enabled);

            ArrayList elementSets = new ArrayList();

            UIExchangeItem item = inputExchangeItemSelector.GetSelectedObject();

            if (item != null && item.ElementSet != null)
                elementSets.Add(item.ElementSet);

            item = outputExchangeItemSelector.GetSelectedObject();

            if (item != null && item.ElementSet != null)
                elementSets.Add(item.ElementSet);

            if (elementSets.Count > 0)
            {
                _elementSetViewer.PopulateDialog(elementSets);
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
	}
}
