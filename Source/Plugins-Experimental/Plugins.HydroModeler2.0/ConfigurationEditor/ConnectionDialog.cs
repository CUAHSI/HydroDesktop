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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Oatc.OpenMI.Gui.Controls;
using Oatc.OpenMI.Gui.Core;
using OpenMI.Standard2;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    /// <summary>
    /// Summary description for ConnectionDialog.
    /// </summary>
    public class ConnectionDialog : Form
    {
        #region Form controls

        private ListBox listLinks;
        private ExchangeItemSelector _treeSource;
        private ExchangeItemSelector _treeTarget;
        private CheckBox DimensionFilterCheckBox;
        private CheckBox ElementTypeFilterCheckBox;
        private PropertyGrid propertyGrid;
        private Button btnLinkAdd;
        private Button btnLinkRemove;
        private Label labelWarning;
        private Button buttonClose;

        private Button btnViewer;
        private GroupBox groupBoxOutputExchnageItems;
        private GroupBox groupBoxLinks;
        private GroupBox groupBoxProperties;
        private GroupBox groupBoxInputExchangeItems;
        private Panel panelBottom;
        private Splitter splitterHorizontal;
        private Splitter splitterVertical2;
        private Splitter splitterVertical1;

        #endregion

        #region Member variables

        /// <summary>
        /// This Hashtable is used to store already constructed PropertyManagers of 
        /// selected object. Its key is selected object, value is corresponding PropertyManager
        /// or <c>null</c> if it not exists at the time.
        /// </summary>
        private Hashtable _propertyManagerCache;

        private UIConnection _uilink;
        private bool _shouldBeSaved;

        private readonly ElementSetViewer _elementSetViewer;

        ExchangeItemSelector.TreeOptions _treeOptionsSources;
        ExchangeItemSelector.TreeOptions _treeOptionsTargets;

        private Label label1;
        private Button btnAddSources;
        private Button btnArgEdit;
        ToolTip _tooltips;
        List<UIModel> _models;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionDialog">ConnectionDialog</see> dialog.
        /// </summary>
        public ConnectionDialog(List<UIModel> models)
        {
            _models = models;

            InitializeComponent();

            _uilink = null;
            _propertyManagerCache = new Hashtable();

            _shouldBeSaved = false;

            _elementSetViewer = new ElementSetViewer();

            _treeOptionsSources.IsSource = true;
            _treeOptionsSources.ShowCheckboxs = true;

            _treeOptionsTargets.IsTarget = true;
            _treeOptionsTargets.ShowCheckboxs = true;

            _tooltips = new ToolTip();
            _tooltips.AutoPopDelay = 5000;
            _tooltips.InitialDelay = 1000;
            _tooltips.ReshowDelay = 500;
            _tooltips.ShowAlways = true;
            _tooltips.IsBalloon = true;
            _tooltips.ToolTipIcon = ToolTipIcon.Info;

            string active = "\r\nOnly active when both source and target items are ticked";

            _tooltips.SetToolTip(btnLinkAdd, "Create a new link from ticked source and target" + active);
            _tooltips.SetToolTip(btnLinkRemove, "Remove a link\r\nOnly active when a link is selected");

            _tooltips.SetToolTip(btnViewer, "View spatial representation of source and target" + active);
           
            _tooltips.SetToolTip(btnAddSources, "Add additional sources from 'Source Model' or '3rd party dll'" + active);
            _tooltips.SetToolTip(btnArgEdit, "Edit adapted source arguments\r\nOnly active when an adapted source is ticked");
        }

        #region Methods

        /// <summary>
        /// Populates this <see cref="ConnectionDialog">ConnectionDialog</see> with specific connection.
        /// </summary>
        /// <param name="uilink"></param>
        public void PopulateDialog(UIConnection uilink)
        {
            _uilink = uilink;
            _propertyManagerCache = new Hashtable();

            _shouldBeSaved = false;
     
            _treeSource.TreePopulate(
                uilink, _treeOptionsSources);
            _treeTarget.TreePopulate(
                uilink, _treeOptionsTargets);

            ElementTypeFilterCheckBox.Checked = false;
            DimensionFilterCheckBox.Checked = false;

            UpdateListLinks();

            Text = "Connection: " + uilink.SourceModel.InstanceCaption + " => " + uilink.TargetModel.InstanceCaption;
        }


        private void UpdateListLinks()
        {
            UIConnection.Link selected = (UIConnection.Link)listLinks.SelectedItem;
            
            listLinks.Items.Clear();
            listLinks.Items.AddRange(_uilink.Links.ToArray());

            foreach (object item in listLinks.Items)
                if (item == selected)
                {
                    listLinks.SelectedItem = item;
                    break;
                }
        }

        /// <summary>
        // show properties of selected object (if any)					
        /// </summary>
        /// <param name="obj"></param>
        private void PropertyGridSelectObject(object obj)
        {
            // first look into cache if propertyManager wasn't already constructed for this object,
            // if not, construct new one
            if (obj == null)
                propertyGrid.SelectedObject = null;
            else if (_propertyManagerCache.Contains(obj))
                propertyGrid.SelectedObject = _propertyManagerCache[obj];
            else
            {
                propertyGrid.SelectedObject = Oatc.OpenMI.Gui.Controls.PropertyPane.Selection(obj);
                
                // store propertyManager of actually showing properties into cache,
                // so next time user selects same object the properties won't be newly constructed.
                // It's most useful for making changes of DataOperation arguments persistent
                // between selection of other items
                if (propertyGrid.SelectedObject != null)
                    _propertyManagerCache.Add(obj, propertyGrid.SelectedObject);
            }
        }

        UIOutputItem SelectedSource
        {
            get
            {
                List<UIExchangeItem> sources = _treeSource.GetCheckedExchangeItems();
                
                return sources.Count > 0 ? (UIOutputItem)sources[sources.Count - 1] : null;
            }
        }

        UIInputItem SelectedTarget
        {
            get
            {
                List<UIExchangeItem> targets = _treeTarget.GetCheckedExchangeItems();

                return targets.Count > 0 ? (UIInputItem)targets[targets.Count - 1] : null;
            }
        }
        

        private void UpdateButtonsEnabledStatus()
        {
            UIOutputItem source = SelectedSource;
            UIInputItem target = SelectedTarget;

            List<UIExchangeItem> sources = _treeSource.GetCheckedExchangeItems();

            bool checkedSourceAndTarget = source != null && target != null;

            // Make it look as if inactive rather than do it as
            // tooltips will not work on inactive buttons

            btnLinkAdd.ForeColor = checkedSourceAndTarget
                ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaptionText;
            btnLinkRemove.ForeColor = listLinks.SelectedItem != null
                ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaptionText;

            btnViewer.ForeColor = checkedSourceAndTarget
                ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaptionText;

            btnAddSources.ForeColor = checkedSourceAndTarget
                ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaptionText;
            btnArgEdit.ForeColor = source != null && source.IExchangeItem is IAdaptedOutput
                ? SystemColors.ActiveCaptionText : SystemColors.InactiveCaptionText;
		}

        #endregion

        #region Event handlers

        private void OnSelectionChanged(object sender, System.EventArgs e)
        {
            if (listLinks.SelectedIndex < 0)
                return;
            
            PropertyGridSelectObject(_uilink.Links[listLinks.SelectedIndex]);
            _treeSource.CheckLink(_uilink.Links[listLinks.SelectedIndex]);
            _treeTarget.CheckLink(_uilink.Links[listLinks.SelectedIndex]);

            UpdateButtonsEnabledStatus();
        }
        
        private void OnClose(object sender, System.EventArgs e)
        {
            Close();
        }


        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // if DialogResult is DialogResult.OK, dialog changed something, so
            // composition should be saved
            DialogResult = _shouldBeSaved ? DialogResult.OK : DialogResult.No;

            // clean-up
            _propertyManagerCache.Clear();
        }


        private void OnLinkAdd(object sender, System.EventArgs e)
        {
            if (_treeSource.GetCheckedExchangeItems().Count < 1
                || _treeTarget.GetCheckedExchangeItems().Count < 1)
                return;

            List<UIExchangeItem> providers = _treeSource.GetCheckedExchangeItems();
			List<UIExchangeItem> acceptors = _treeTarget.GetCheckedExchangeItems();

            if (providers.Count == 0)
            {
                // ADH: should realy disable apply button and provide tooltip as to why
                MessageBox.Show("No Output Exchange Item selected.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }
            
            if (acceptors.Count == 0)
            {
                // ADH: should realy disable apply button and provide tooltip as to why
                MessageBox.Show("No Input Exchange Item selected.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }

            // Are the connection dialogs check boxes screwed up?
            Debug.Assert(acceptors.Count == 1);

            _uilink.Links.Add(new UIConnection.Link(
                (UIOutputItem)providers[providers.Count - 1], (UIInputItem)acceptors[0]));

            UpdateListLinks();
            UpdateButtonsEnabledStatus();

            _shouldBeSaved = true;
        }


        private void OnLinkRemove(object sender, System.EventArgs e)
        {
            if (listLinks.SelectedItem == null)
                return;

            _uilink.Links.Remove((UIConnection.Link)listLinks.SelectedItem);

            UpdateListLinks();
            UpdateButtonsEnabledStatus();

            _shouldBeSaved = true;
        }


        private void DimensionFilterCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            _treeOptionsTargets.FilterTargetByDimension = ((CheckBox)sender).Checked;

            _treeTarget.TreePopulate(
                _uilink.SourceModel.LinkableComponent,
                _uilink.TargetModel.LinkableComponent,
                _treeOptionsTargets);

            UpdateButtonsEnabledStatus();
        }

        private void ElementTypeFilterCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            _treeOptionsTargets.FilterTargetByElementType = ((CheckBox)sender).Checked;

            _treeTarget.TreePopulate(
                _uilink.SourceModel.LinkableComponent,
                _uilink.TargetModel.LinkableComponent,
                _treeOptionsTargets);
 
            UpdateButtonsEnabledStatus();
        }


        private void providerExchangeItemSelector_SelectionChanged(object sender, System.EventArgs e)
        {
            UpdateButtonsEnabledStatus();
            PropertyGridSelectObject(_treeSource.GetSelectedObject());
        }


        private void acceptorExchangeItemSelector_SelectionChanged(object sender, System.EventArgs e)
        {
            UpdateButtonsEnabledStatus();
            PropertyGridSelectObject(_treeTarget.GetSelectedObject());
        }


        private void providerExchangeItemSelector_CheckboxesChanged(object sender, System.EventArgs e)
        {
            UpdateButtonsEnabledStatus();
        }


        private void acceptorExchangeItemSelector_CheckboxesChanged(object sender, System.EventArgs e)
        {
            UpdateButtonsEnabledStatus();
        }


        private void OnViewElementSet(object sender, System.EventArgs e)
        {
            UIOutputItem source = SelectedSource;
            UIInputItem target = SelectedTarget;

            ArrayList elementSets = new ArrayList();

            if (source != null)
                elementSets.Add(source.ElementSet);
            if (target != null)
                elementSets.Add(target.ElementSet);

            if (elementSets.Count > 0)
            {
                _elementSetViewer.PopulateDialog(elementSets);
                _elementSetViewer.ShowDialog();
            }
        }

        #endregion

        #region .NET generated members

        private readonly System.ComponentModel.Container components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionDialog));
            this.listLinks = new System.Windows.Forms.ListBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.DimensionFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.ElementTypeFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.groupBoxOutputExchnageItems = new System.Windows.Forms.GroupBox();
            this.btnArgEdit = new System.Windows.Forms.Button();
            this.btnAddSources = new System.Windows.Forms.Button();
            this._treeSource = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
            this.labelWarning = new System.Windows.Forms.Label();
            this.groupBoxLinks = new System.Windows.Forms.GroupBox();
            this.btnLinkAdd = new System.Windows.Forms.Button();
            this.btnLinkRemove = new System.Windows.Forms.Button();
            this.groupBoxProperties = new System.Windows.Forms.GroupBox();
            this.btnViewer = new System.Windows.Forms.Button();
            this.groupBoxInputExchangeItems = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this._treeTarget = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.splitterHorizontal = new System.Windows.Forms.Splitter();
            this.splitterVertical2 = new System.Windows.Forms.Splitter();
            this.splitterVertical1 = new System.Windows.Forms.Splitter();
            this.groupBoxOutputExchnageItems.SuspendLayout();
            this.groupBoxLinks.SuspendLayout();
            this.groupBoxProperties.SuspendLayout();
            this.groupBoxInputExchangeItems.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // listLinks
            // 
            this.listLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listLinks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listLinks.HorizontalExtent = 3000;
            this.listLinks.HorizontalScrollbar = true;
            this.listLinks.Location = new System.Drawing.Point(8, 18);
            this.listLinks.Name = "listLinks";
            this.listLinks.Size = new System.Drawing.Size(552, 80);
            this.listLinks.TabIndex = 5;
            this.listLinks.SelectedIndexChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(564, 119);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(64, 24);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.OnClose);
            // 
            // DimensionFilterCheckBox
            // 
            this.DimensionFilterCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DimensionFilterCheckBox.Checked = true;
            this.DimensionFilterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DimensionFilterCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DimensionFilterCheckBox.Location = new System.Drawing.Point(53, 277);
            this.DimensionFilterCheckBox.Name = "DimensionFilterCheckBox";
            this.DimensionFilterCheckBox.Size = new System.Drawing.Size(143, 16);
            this.DimensionFilterCheckBox.TabIndex = 2;
            this.DimensionFilterCheckBox.Text = "&Dimension";
            this.DimensionFilterCheckBox.CheckedChanged += new System.EventHandler(this.DimensionFilterCheckBox_CheckedChanged);
            // 
            // ElementTypeFilterCheckBox
            // 
            this.ElementTypeFilterCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ElementTypeFilterCheckBox.Checked = true;
            this.ElementTypeFilterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ElementTypeFilterCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ElementTypeFilterCheckBox.Location = new System.Drawing.Point(53, 261);
            this.ElementTypeFilterCheckBox.Name = "ElementTypeFilterCheckBox";
            this.ElementTypeFilterCheckBox.Size = new System.Drawing.Size(143, 16);
            this.ElementTypeFilterCheckBox.TabIndex = 3;
            this.ElementTypeFilterCheckBox.Text = "&Element Type";
            this.ElementTypeFilterCheckBox.CheckedChanged += new System.EventHandler(this.ElementTypeFilterCheckBox_CheckedChanged);
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
            this.propertyGrid.Size = new System.Drawing.Size(209, 272);
            this.propertyGrid.TabIndex = 4;
            // 
            // groupBoxOutputExchnageItems
            // 
            this.groupBoxOutputExchnageItems.Controls.Add(this.btnArgEdit);
            this.groupBoxOutputExchnageItems.Controls.Add(this.btnAddSources);
            this.groupBoxOutputExchnageItems.Controls.Add(this._treeSource);
            this.groupBoxOutputExchnageItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBoxOutputExchnageItems.Location = new System.Drawing.Point(8, 0);
            this.groupBoxOutputExchnageItems.Name = "groupBoxOutputExchnageItems";
            this.groupBoxOutputExchnageItems.Size = new System.Drawing.Size(204, 294);
            this.groupBoxOutputExchnageItems.TabIndex = 23;
            this.groupBoxOutputExchnageItems.TabStop = false;
            this.groupBoxOutputExchnageItems.Text = "Sources";
            // 
            // btnArgEdit
            // 
            this.btnArgEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnArgEdit.Location = new System.Drawing.Point(116, 265);
            this.btnArgEdit.Name = "btnArgEdit";
            this.btnArgEdit.Size = new System.Drawing.Size(80, 23);
            this.btnArgEdit.TabIndex = 4;
            this.btnArgEdit.Text = "Edit Args ...";
            this.btnArgEdit.UseVisualStyleBackColor = true;
            this.btnArgEdit.Click += new System.EventHandler(this.OnEditArgs);
            // 
            // btnAddSources
            // 
            this.btnAddSources.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddSources.Location = new System.Drawing.Point(8, 265);
            this.btnAddSources.Name = "btnAddSources";
            this.btnAddSources.Size = new System.Drawing.Size(101, 23);
            this.btnAddSources.TabIndex = 3;
            this.btnAddSources.Text = "Add  Sources ...";
            this.btnAddSources.UseVisualStyleBackColor = true;
            this.btnAddSources.Click += new System.EventHandler(this.OnAddSources);
            // 
            // _treeSource
            // 
            this._treeSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._treeSource.Location = new System.Drawing.Point(8, 16);
            this._treeSource.Name = "_treeSource";
            this._treeSource.Size = new System.Drawing.Size(188, 239);
            this._treeSource.TabIndex = 0;
            this._treeSource.SelectionChanged += new System.EventHandler(this.providerExchangeItemSelector_SelectionChanged);
            this._treeSource.ExchangeItemChanged += new System.EventHandler(this.providerExchangeItemSelector_CheckboxesChanged);
            // 
            // labelWarning
            // 
            this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWarning.ForeColor = System.Drawing.Color.Red;
            this.labelWarning.Location = new System.Drawing.Point(210, 115);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(348, 28);
            this.labelWarning.TabIndex = 2;
            this.labelWarning.Text = "Warning: Selected combination  is invalid !";
            this.labelWarning.Visible = false;
            // 
            // groupBoxLinks
            // 
            this.groupBoxLinks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLinks.Controls.Add(this.btnLinkAdd);
            this.groupBoxLinks.Controls.Add(this.btnLinkRemove);
            this.groupBoxLinks.Controls.Add(this.listLinks);
            this.groupBoxLinks.Location = new System.Drawing.Point(8, 302);
            this.groupBoxLinks.Name = "groupBoxLinks";
            this.groupBoxLinks.Size = new System.Drawing.Size(636, 109);
            this.groupBoxLinks.TabIndex = 25;
            this.groupBoxLinks.TabStop = false;
            this.groupBoxLinks.Text = " Links";
            // 
            // btnLinkAdd
            // 
            this.btnLinkAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkAdd.Location = new System.Drawing.Point(564, 22);
            this.btnLinkAdd.Name = "btnLinkAdd";
            this.btnLinkAdd.Size = new System.Drawing.Size(64, 24);
            this.btnLinkAdd.TabIndex = 6;
            this.btnLinkAdd.Text = "&Add";
            this.btnLinkAdd.Click += new System.EventHandler(this.OnLinkAdd);
            // 
            // btnLinkRemove
            // 
            this.btnLinkRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinkRemove.Location = new System.Drawing.Point(564, 57);
            this.btnLinkRemove.Name = "btnLinkRemove";
            this.btnLinkRemove.Size = new System.Drawing.Size(64, 24);
            this.btnLinkRemove.TabIndex = 7;
            this.btnLinkRemove.Text = "&Remove";
            this.btnLinkRemove.Click += new System.EventHandler(this.OnLinkRemove);
            // 
            // groupBoxProperties
            // 
            this.groupBoxProperties.Controls.Add(this.propertyGrid);
            this.groupBoxProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProperties.Location = new System.Drawing.Point(422, 0);
            this.groupBoxProperties.Name = "groupBoxProperties";
            this.groupBoxProperties.Size = new System.Drawing.Size(222, 294);
            this.groupBoxProperties.TabIndex = 26;
            this.groupBoxProperties.TabStop = false;
            this.groupBoxProperties.Text = "Properties";
            // 
            // btnViewer
            // 
            this.btnViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewer.Location = new System.Drawing.Point(8, 119);
            this.btnViewer.Name = "btnViewer";
            this.btnViewer.Size = new System.Drawing.Size(196, 24);
            this.btnViewer.TabIndex = 8;
            this.btnViewer.Text = "&Viewer (checked items only)";
            this.btnViewer.Click += new System.EventHandler(this.OnViewElementSet);
            // 
            // groupBoxInputExchangeItems
            // 
            this.groupBoxInputExchangeItems.Controls.Add(this.label1);
            this.groupBoxInputExchangeItems.Controls.Add(this._treeTarget);
            this.groupBoxInputExchangeItems.Controls.Add(this.DimensionFilterCheckBox);
            this.groupBoxInputExchangeItems.Controls.Add(this.ElementTypeFilterCheckBox);
            this.groupBoxInputExchangeItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBoxInputExchangeItems.Location = new System.Drawing.Point(215, 0);
            this.groupBoxInputExchangeItems.Name = "groupBoxInputExchangeItems";
            this.groupBoxInputExchangeItems.Size = new System.Drawing.Size(204, 294);
            this.groupBoxInputExchangeItems.TabIndex = 0;
            this.groupBoxInputExchangeItems.TabStop = false;
            this.groupBoxInputExchangeItems.Text = "Targets";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 263);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Filter by";
            // 
            // _treeTarget
            // 
            this._treeTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._treeTarget.Location = new System.Drawing.Point(8, 16);
            this._treeTarget.Name = "_treeTarget";
            this._treeTarget.Size = new System.Drawing.Size(188, 239);
            this._treeTarget.TabIndex = 1;
            this._treeTarget.SelectionChanged += new System.EventHandler(this.acceptorExchangeItemSelector_SelectionChanged);
            this._treeTarget.ExchangeItemChanged += new System.EventHandler(this.acceptorExchangeItemSelector_CheckboxesChanged);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.btnViewer);
            this.panelBottom.Controls.Add(this.labelWarning);
            this.panelBottom.Controls.Add(this.buttonClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(8, 298);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(636, 143);
            this.panelBottom.TabIndex = 28;
            // 
            // splitterHorizontal
            // 
            this.splitterHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterHorizontal.Location = new System.Drawing.Point(8, 294);
            this.splitterHorizontal.MinExtra = 150;
            this.splitterHorizontal.MinSize = 150;
            this.splitterHorizontal.Name = "splitterHorizontal";
            this.splitterHorizontal.Size = new System.Drawing.Size(636, 4);
            this.splitterHorizontal.TabIndex = 30;
            this.splitterHorizontal.TabStop = false;
            // 
            // splitterVertical2
            // 
            this.splitterVertical2.Location = new System.Drawing.Point(419, 0);
            this.splitterVertical2.MinExtra = 150;
            this.splitterVertical2.MinSize = 150;
            this.splitterVertical2.Name = "splitterVertical2";
            this.splitterVertical2.Size = new System.Drawing.Size(3, 294);
            this.splitterVertical2.TabIndex = 24;
            this.splitterVertical2.TabStop = false;
            // 
            // splitterVertical1
            // 
            this.splitterVertical1.Location = new System.Drawing.Point(212, 0);
            this.splitterVertical1.MinExtra = 150;
            this.splitterVertical1.MinSize = 150;
            this.splitterVertical1.Name = "splitterVertical1";
            this.splitterVertical1.Size = new System.Drawing.Size(3, 294);
            this.splitterVertical1.TabIndex = 27;
            this.splitterVertical1.TabStop = false;
            // 
            // ConnectionDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(652, 449);
            this.Controls.Add(this.groupBoxProperties);
            this.Controls.Add(this.splitterVertical2);
            this.Controls.Add(this.groupBoxLinks);
            this.Controls.Add(this.groupBoxInputExchangeItems);
            this.Controls.Add(this.splitterVertical1);
            this.Controls.Add(this.groupBoxOutputExchnageItems);
            this.Controls.Add(this.splitterHorizontal);
            this.Controls.Add(this.panelBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(660, 400);
            this.Name = "ConnectionDialog";
            this.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connection properties";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.OnClosing);
            this.groupBoxOutputExchnageItems.ResumeLayout(false);
            this.groupBoxLinks.ResumeLayout(false);
            this.groupBoxProperties.ResumeLayout(false);
            this.groupBoxInputExchangeItems.ResumeLayout(false);
            this.groupBoxInputExchangeItems.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        private void OnAddSources(object sender, System.EventArgs e)
        {
            UIOutputItem source = SelectedSource;
            UIInputItem target = SelectedTarget;

            if (source == null || target == null)
                return;

			FactoriesDialog dlg = new FactoriesDialog(_models, source, target);

            if (dlg.ShowDialog(this) == DialogResult.Cancel)
                return;

            _treeSource.TreeAdd(dlg.Adapters);

            UpdateButtonsEnabledStatus();
        }

        private void OnEditArgs(object sender, System.EventArgs e)
        {
            UIOutputItem source = SelectedSource;

            if (source == null)
                return;

            IAdaptedOutput adapted = source.IExchangeItem as IAdaptedOutput;

            if (adapted == null)
                return;

            AdaptedArguments dlg = new AdaptedArguments();
            dlg.Initialise(adapted.Arguments);

            dlg.ShowDialog(this);
        }
    }
}