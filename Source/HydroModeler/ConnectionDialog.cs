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
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Oatc.OpenMI.Gui.Controls;
using Oatc.OpenMI.Gui.Core;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard;


namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	/// <summary>
	/// Summary description for ConnectionDialog.
	/// </summary>
	public class ConnectionDialog : System.Windows.Forms.Form
	{
		#region Form controls

		private System.Windows.Forms.ListBox listLinks;
		private ExchangeItemSelector providerExchangeItemSelector;
		private ExchangeItemSelector acceptorExchangeItemSelector;
		private System.Windows.Forms.CheckBox DimensionFilterCheckBox;
		private System.Windows.Forms.CheckBox ElementTypeFilterCheckBox;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Label labelInfo;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Label labelWarning;
		private System.Windows.Forms.Button buttonClose;

		private System.Windows.Forms.Button buttonViewElementSet;
		private System.Windows.Forms.GroupBox groupBoxOutputExchnageItems;
		private System.Windows.Forms.GroupBox groupBoxLinks;
		private System.Windows.Forms.GroupBox groupBoxProperties;
		private System.Windows.Forms.GroupBox groupBoxTools;
		private System.Windows.Forms.GroupBox groupBoxInputExchangeItems;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Splitter splitterHorizontal;
		private System.Windows.Forms.Splitter splitterVertical2;
		private System.Windows.Forms.Splitter splitterVertical1;

		#endregion

		#region Member variables		

		/// <summary>
		/// This Hashtable is used to store already constructed PropertyManagers of 
		/// selected object. Its key is selected object, value is corresponding PropertyManager
		/// or <c>null</c> if it not exists at the time.
		/// </summary>
		private Hashtable _propertyManagerCache;

		private UIConnection _uilink;
		private int _startingLinkID;
		private bool _shouldBeSaved;

		private ElementSetViewer _elementSetViewer;

		#endregion

		/// <summary>
		/// Creates a new instance of <see cref="ConnectionDialog">ConnectionDialog</see> dialog.
		/// </summary>
		public ConnectionDialog()
		{
			InitializeComponent();

			_uilink = null;
			_propertyManagerCache = new Hashtable();
			_startingLinkID = 0;

			_shouldBeSaved = false;

			_elementSetViewer = new ElementSetViewer();
		}

		
		#region Methods

		/// <summary>
		/// Populates this <see cref="ConnectionDialog">ConnectionDialog</see> with specific connection.
		/// </summary>
		/// <param name="uilink"></param>
		/// <param name="startingLinkID"></param>
		public void PopulateDialog( UIConnection uilink, int startingLinkID )
		{
			_uilink = uilink;
			_startingLinkID = startingLinkID;
			_propertyManagerCache = new Hashtable();

			_shouldBeSaved = false;

			ElementTypeFilterCheckBox.Checked = false;
			DimensionFilterCheckBox.Checked = false;

			int count;
			ILinkableComponent component;

			component = uilink.ProvidingModel.LinkableComponent;
			IExchangeItem[] outputExchangeItems = new IExchangeItem[ component.OutputExchangeItemCount ];
			count = component.OutputExchangeItemCount;			
			for( int i = 0; i < count; i++ )
				outputExchangeItems[i] = component.GetOutputExchangeItem(i);

			providerExchangeItemSelector.PopulateExchangeItemTree( outputExchangeItems, true );

			component = uilink.AcceptingModel.LinkableComponent;
			IExchangeItem[] inputExchangeItems = new IExchangeItem[ component.InputExchangeItemCount ];
			count = component.InputExchangeItemCount;
			for( int i = 0; i < count; i++ )
				inputExchangeItems[i] = component.GetInputExchangeItem(i);

			acceptorExchangeItemSelector.PopulateExchangeItemTree( inputExchangeItems, true );
			
			UpdateListLinks();
			
			labelInfo.Text = "Connection "+uilink.ProvidingModel.ModelID+" => "+uilink.AcceptingModel.ModelID;
		}


		private void UpdateListLinks()
		{
			int selectedIndex = listLinks.SelectedIndex;

			listLinks.Items.Clear();

			listLinks.Items.Add( "<New...>" );

			foreach( Link link in _uilink.Links )
			{
				StringBuilder str = new StringBuilder( 200 );

				str.Append( link.SourceQuantity.ID );
				str.Append( ", " );
				str.Append( link.SourceElementSet.ID );
				
				if( link.DataOperationsCount > 0 )
				{
					str.Append( " (" );
					for( int i=0; i<link.DataOperationsCount; i++ )
					{
						str.Append( link.GetDataOperation(i).ID );
						if( i < link.DataOperationsCount-1 )
							str.Append( ", " );
					}
					str.Append( ")" );
				}

				str.Append( " --> " );
				str.Append( link.TargetQuantity.ID );
				str.Append( ", " );
				str.Append( link.TargetElementSet.ID );

				listLinks.Items.Add( str.ToString() );
			}

			// select "<new...>" link if there isn't any other
			// or if no other was selected
			if( 0<=selectedIndex && selectedIndex<listLinks.Items.Count )
                listLinks.SelectedIndex = selectedIndex;
			else
				listLinks.SelectedIndex = 0;
		}


		private void ApplyDimensionFilters()
		{
			// save checkstate of acceptor
			IQuantity acceptorQuantity;
			IElementSet acceptorElementSet;
			IDataOperation[] dummyDataOperations;
			acceptorExchangeItemSelector.GetCheckedExchangeItem( out acceptorQuantity, out acceptorElementSet, out dummyDataOperations );
                
			// Get filters from provider
			IQuantity providerQuantity;
			IElementSet providerElementSet;
			IDataOperation[] providerDataOperations;
			providerExchangeItemSelector.GetCheckedExchangeItem( out providerQuantity, out providerElementSet, out providerDataOperations );
         
			// Apply filters to acceptor (implicitly redraws its tree if any change is done)
			if( DimensionFilterCheckBox.Checked )
			{
				if( providerQuantity != null )
					acceptorExchangeItemSelector.EnableDimensionFilter( providerQuantity.Dimension );
				else
					acceptorExchangeItemSelector.EnableDimensionFilter( null );
			}
			else
			{
				acceptorExchangeItemSelector.EnableDimensionFilter( null );
			}

			if( ElementTypeFilterCheckBox.Checked )
			{
				acceptorExchangeItemSelector.EnableElementSetFilter( providerElementSet );
			}
			else
			{
				acceptorExchangeItemSelector.EnableElementSetFilter( null );
			}
			
			// try to restore as many of the check-state of acceptorExchangeItemSelector as possible
			acceptorExchangeItemSelector.SetCheckedExchangeItem( acceptorQuantity, acceptorElementSet, dummyDataOperations );
			acceptorExchangeItemSelector.ExpandChecked();
		}


		private void PropertyGridSelectObject( object obj )
		{
			// show properties of selected object (if any)					
			if( obj!=null )
			{
				// first look into cache if propertyManager wasn't already constructed for this object,
				// if not, construct new one
				if( _propertyManagerCache.Contains(obj) )
					propertyGrid.SelectedObject = _propertyManagerCache[obj];				
				else
				{
					Oatc.OpenMI.Gui.Controls.PropertyManager manager = Oatc.OpenMI.Gui.Controls.PropertyManager.ConstructPropertyManager( obj, false );
					propertyGrid.SelectedObject = manager;

					// store propertyManager of actually showing properties into cache,
					// so next time user selects same object the properties won't be newly constructed.
					// It's most useful for making changes of DataOperation arguments persistent
					// between selection of other items
					_propertyManagerCache.Add( obj, manager );
				}
				
				// Modify text in properties group-box 
				if( obj is IQuantity )
					this.groupBoxProperties.Text = " Quantity properties";
				else if( obj is IElementSet )
					groupBoxProperties.Text = " ElementSet properties";				
				else if( obj is IDataOperation )
					groupBoxProperties.Text = " DataOperation properties";
				else
				{
					Debug.Assert( false );
					groupBoxProperties.Text = " Properties";
				}
			}
			else
			{
				// no object is selected, show no properties
				propertyGrid.SelectedObject = null;
				groupBoxProperties.Text = " Properties";
			}
			
		}


		private bool CheckIfDataOperationsAreValid()
		{
			bool isValid = true;
			
			IQuantity providerQuantity, acceptorQuantity;
			IElementSet providerElementSet, acceptorElementSet;
			IDataOperation[] providerDataOperations, acceptorDataOperations;

			providerExchangeItemSelector.GetCheckedExchangeItem( out providerQuantity, out providerElementSet, out providerDataOperations );
			acceptorExchangeItemSelector.GetCheckedExchangeItem( out acceptorQuantity, out acceptorElementSet, out acceptorDataOperations );

			// Check if combination of providerDataOperations is valid
			if( providerDataOperations!=null && providerElementSet!=null && acceptorElementSet!=null)
			{
				Debug.Assert( providerQuantity!=null && acceptorQuantity!=null );

				IExchangeItem outputExchangeItem = providerExchangeItemSelector.GetExchangeItem( providerQuantity, providerElementSet );
				IExchangeItem inputExchangeItem = acceptorExchangeItemSelector.GetExchangeItem( acceptorQuantity, acceptorElementSet );

				Debug.Assert( outputExchangeItem!=null && inputExchangeItem!=null );
				Debug.Assert( outputExchangeItem is IOutputExchangeItem );
				Debug.Assert( inputExchangeItem is IInputExchangeItem );
					
				foreach( IDataOperation dataOperation in providerDataOperations )
				{
					isValid = dataOperation.IsValid( (IInputExchangeItem)inputExchangeItem, (IOutputExchangeItem)outputExchangeItem, providerDataOperations );
					if( !isValid )
						break;
				}				
			}

			labelWarning.Visible = !isValid;
			return( isValid );
		}		


		private void UpdateViewElementSetButton()
		{
			IQuantity providerQuantity, acceptorQuantity;
			IElementSet providerElementSet, acceptorElementSet;
			IDataOperation[] providerDataOperations, acceptorDataOperations;

			acceptorExchangeItemSelector.GetCheckedExchangeItem( out acceptorQuantity, out acceptorElementSet, out acceptorDataOperations );
           	providerExchangeItemSelector.GetCheckedExchangeItem( out providerQuantity, out providerElementSet, out providerDataOperations );
         
			buttonViewElementSet.Enabled = acceptorElementSet!=null || providerElementSet!=null;
		}


		#endregion

		#region Event handlers

		private void listLinks_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			PropertyGridSelectObject( null );

			if( listLinks.SelectedIndex == 0 )
			{
				// "<new ...>" link is selected
				buttonRemove.Enabled = false;
				
				providerExchangeItemSelector.ClearCheckboxes();
				acceptorExchangeItemSelector.ClearCheckboxes();
			}
			else
			{
				ILink link = (ILink)_uilink.Links[listLinks.SelectedIndex-1];

				ArrayList dataOperations = new ArrayList( link.DataOperationsCount );
				for( int i=0; i<link.DataOperationsCount; i++ )
					dataOperations.Add( link.GetDataOperation(i) );

				providerExchangeItemSelector.SetCheckedExchangeItem( link.SourceQuantity, link.SourceElementSet, (IDataOperation[])dataOperations.ToArray(typeof(IDataOperation)) );
				providerExchangeItemSelector.ExpandChecked();

				acceptorExchangeItemSelector.SetCheckedExchangeItem( link.TargetQuantity, link.TargetElementSet, null );
				acceptorExchangeItemSelector.ExpandChecked();

				buttonRemove.Enabled = true;
			}

			UpdateViewElementSetButton();			
		}

	
		private void buttonClose_Click(object sender, System.EventArgs e)
		{			
			Close();			
		}

		
		private void LinkDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// if DialogResult is DialogResult.OK, dialog changed something, so
			// composition should be saved
			DialogResult = _shouldBeSaved ? DialogResult.OK : DialogResult.No;

			// clean-up
			_propertyManagerCache.Clear();			
		}

	
		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			// get checked items
			IQuantity providerQuantity, acceptorQuantity;
			IElementSet providerElementSet, acceptorElementSet;
			IDataOperation[] providerDataOperations, acceptorDataOperations;

			providerExchangeItemSelector.GetCheckedExchangeItem( out providerQuantity, out providerElementSet, out providerDataOperations );
			acceptorExchangeItemSelector.GetCheckedExchangeItem( out acceptorQuantity, out acceptorElementSet, out acceptorDataOperations );

			// check wheather all needed informations are avaliable
			if( providerElementSet==null )
			{
				MessageBox.Show( "No Output Exchange Item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				return;
			}
			if( acceptorElementSet==null )
			{
				MessageBox.Show( "No Input Exchange Item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				return;
			}
			if( _uilink.AcceptingModel.ModelID == CompositionManager.TriggerModelID
				&& _uilink.Links.Count>=1
				&& listLinks.SelectedIndex==0 )
			{
				MessageBox.Show( "Trigger can have only one link.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				return;
			}

			if( !CheckIfDataOperationsAreValid() )
			{
				switch( MessageBox.Show("Selected combination of DataOperations is invalid. Adding such link to LinkableComponents may\nway to unexpected result, maybe whole application will crash. If you are sure what you do,\nclick 'Yes', but in this case it's STRONGLY recommended to save your project before you proceed.\n\nDo you really want to continue ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) )
				{
					case DialogResult.Yes:
						break;
					default:
						return;
				}
			}

			Debug.Assert( providerQuantity!=null && acceptorQuantity!=null );
			Debug.Assert( listLinks.SelectedIndex >= 0 );

			int linkID;

			// TODO: shouldn't be this functionallity in UIConnection class ???
			//       - only problem with unique linkID

			if( listLinks.SelectedIndex==0 )
			{
				// Creating new link, so create new ID for it
				linkID = ++_startingLinkID; 
			}
			else
			{
				// Modifying existing link, use its previous ID
				string oldLinkID = ((ILink)_uilink.Links[listLinks.SelectedIndex-1]).ID ;
				linkID = int.Parse( oldLinkID );

				// Remove this link from both LinkableComponents
				_uilink.AcceptingModel.LinkableComponent.RemoveLink( oldLinkID ); 
				_uilink.ProvidingModel.LinkableComponent.RemoveLink( oldLinkID );
			}

			// Create a new link even if modifing existing one.
			// That's because if some DataOperations were not selected,
			// we wouldn't be able to remove them from the link
			Link link = new Link(
				_uilink.ProvidingModel.LinkableComponent,
				providerElementSet,
				providerQuantity,
				_uilink.AcceptingModel.LinkableComponent,
				acceptorElementSet,
				acceptorQuantity,
				linkID.ToString() );

			// add DataOperations
			foreach( IDataOperation dataOperation in providerDataOperations )
			{
				// set all changed writeable Arguments to dataOperation from property box 
				if( _propertyManagerCache.Contains(dataOperation) )
					for( int i=0; i<dataOperation.ArgumentCount; i++ )
					{
						IArgument argument = dataOperation.GetArgument(i);

						if( !argument.ReadOnly )
						{
							string newValue = ((Oatc.OpenMI.Gui.Controls.PropertyManager)_propertyManagerCache[dataOperation]).GetProperty( argument.Key );
							if( argument.Value != newValue )
								argument.Value = newValue;
						}
					}				

				link.AddDataOperation( dataOperation );
			}
		
			// add/set new link to list
			if( listLinks.SelectedIndex==0 )
				_uilink.Links.Add(link);
			else
				_uilink.Links[ listLinks.SelectedIndex-1 ] = link;

			// ...and add new link to both LinkableComponents
			_uilink.ProvidingModel.LinkableComponent.AddLink( link );
			_uilink.AcceptingModel.LinkableComponent.AddLink( link );
			
			UpdateListLinks();
			UpdateViewElementSetButton();
		
			_shouldBeSaved = true;
		}


		private void buttonRemove_Click(object sender, System.EventArgs e)
		{
			string id = ((ILink)_uilink.Links[listLinks.SelectedIndex-1]).ID;

			_uilink.ProvidingModel.LinkableComponent.RemoveLink( id );
			_uilink.AcceptingModel.LinkableComponent.RemoveLink( id );

			_uilink.Links.RemoveAt( listLinks.SelectedIndex-1 );

			UpdateListLinks();
			UpdateViewElementSetButton();
			
			_shouldBeSaved = true;
		}

	
		private void DimensionFilterCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			PropertyGridSelectObject( null );
			ApplyDimensionFilters();

			UpdateViewElementSetButton();
		}


		private void ElementTypeFilterCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			PropertyGridSelectObject( null );
			ApplyDimensionFilters();

			UpdateViewElementSetButton();
		}

		
		private void providerExchangeItemSelector_SelectionChanged(object sender, System.EventArgs e)
		{
			PropertyGridSelectObject( providerExchangeItemSelector.GetSelectedObject() );		
		}


		private void acceptorExchangeItemSelector_SelectionChanged(object sender, System.EventArgs e)
		{
			PropertyGridSelectObject( acceptorExchangeItemSelector.GetSelectedObject() );		
		}

	
		private void providerExchangeItemSelector_CheckboxesChanged(object sender, System.EventArgs e)
		{
			ApplyDimensionFilters();
			UpdateViewElementSetButton();

			// CheckIfDataOperationsAreValid() call is ensured by previous call, because
			// it changes checkboxes of acceptor so acceptorExchangeItemSelector_CheckboxesChanged()
			// event handler is called
		}


		private void acceptorExchangeItemSelector_CheckboxesChanged(object sender, System.EventArgs e)
		{			
			CheckIfDataOperationsAreValid();
			UpdateViewElementSetButton();
		}


		private void buttonViewElementSet_Click(object sender, System.EventArgs e)
		{
			IQuantity providerQuantity, acceptorQuantity;
			IElementSet providerElementSet, acceptorElementSet;
			IDataOperation[] providerDataOperations, acceptorDataOperations;

			acceptorExchangeItemSelector.GetCheckedExchangeItem( out acceptorQuantity, out acceptorElementSet, out acceptorDataOperations );
			providerExchangeItemSelector.GetCheckedExchangeItem( out providerQuantity, out providerElementSet, out providerDataOperations );

			ArrayList elementSets = new ArrayList();
			if( acceptorElementSet!=null )
                elementSets.Add( acceptorElementSet );
			if( providerElementSet!=null )
				elementSets.Add( providerElementSet );

			if( elementSets.Count > 0 )
			{				
				_elementSetViewer.PopulateDialog( elementSets );
				_elementSetViewer.ShowDialog();
			}	
			else
				Debug.Assert(false);
		}
	

		#endregion
		
		#region .NET generated members

		private System.ComponentModel.Container components = null;		
		
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionDialog));
            this.listLinks = new System.Windows.Forms.ListBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.providerExchangeItemSelector = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
            this.acceptorExchangeItemSelector = new Oatc.OpenMI.Gui.Controls.ExchangeItemSelector();
            this.DimensionFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.ElementTypeFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.groupBoxOutputExchnageItems = new System.Windows.Forms.GroupBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.groupBoxLinks = new System.Windows.Forms.GroupBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.groupBoxProperties = new System.Windows.Forms.GroupBox();
            this.buttonViewElementSet = new System.Windows.Forms.Button();
            this.groupBoxTools = new System.Windows.Forms.GroupBox();
            this.groupBoxInputExchangeItems = new System.Windows.Forms.GroupBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.splitterHorizontal = new System.Windows.Forms.Splitter();
            this.splitterVertical2 = new System.Windows.Forms.Splitter();
            this.splitterVertical1 = new System.Windows.Forms.Splitter();
            this.groupBoxOutputExchnageItems.SuspendLayout();
            this.groupBoxLinks.SuspendLayout();
            this.groupBoxProperties.SuspendLayout();
            this.groupBoxTools.SuspendLayout();
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
            this.listLinks.Location = new System.Drawing.Point(8, 16);
            this.listLinks.Name = "listLinks";
            this.listLinks.Size = new System.Drawing.Size(450, 106);
            this.listLinks.TabIndex = 5;
            this.listLinks.SelectedIndexChanged += new System.EventHandler(this.listLinks_SelectedIndexChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonClose.Location = new System.Drawing.Point(550, 140);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(84, 28);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // providerExchangeItemSelector
            // 
            this.providerExchangeItemSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.providerExchangeItemSelector.Location = new System.Drawing.Point(8, 16);
            this.providerExchangeItemSelector.Name = "providerExchangeItemSelector";
            this.providerExchangeItemSelector.Size = new System.Drawing.Size(188, 262);
            this.providerExchangeItemSelector.TabIndex = 0;
            this.providerExchangeItemSelector.SelectionChanged += new System.EventHandler(this.providerExchangeItemSelector_SelectionChanged);
            this.providerExchangeItemSelector.CheckboxesChanged += new System.EventHandler(this.providerExchangeItemSelector_CheckboxesChanged);
            // 
            // acceptorExchangeItemSelector
            // 
            this.acceptorExchangeItemSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.acceptorExchangeItemSelector.Location = new System.Drawing.Point(8, 16);
            this.acceptorExchangeItemSelector.Name = "acceptorExchangeItemSelector";
            this.acceptorExchangeItemSelector.Size = new System.Drawing.Size(188, 262);
            this.acceptorExchangeItemSelector.TabIndex = 1;
            this.acceptorExchangeItemSelector.SelectionChanged += new System.EventHandler(this.acceptorExchangeItemSelector_SelectionChanged);
            this.acceptorExchangeItemSelector.CheckboxesChanged += new System.EventHandler(this.acceptorExchangeItemSelector_CheckboxesChanged);
            // 
            // DimensionFilterCheckBox
            // 
            this.DimensionFilterCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DimensionFilterCheckBox.Checked = true;
            this.DimensionFilterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DimensionFilterCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DimensionFilterCheckBox.Location = new System.Drawing.Point(8, 295);
            this.DimensionFilterCheckBox.Name = "DimensionFilterCheckBox";
            this.DimensionFilterCheckBox.Size = new System.Drawing.Size(172, 16);
            this.DimensionFilterCheckBox.TabIndex = 2;
            this.DimensionFilterCheckBox.Text = "Use &Dimension filter";
            this.DimensionFilterCheckBox.CheckedChanged += new System.EventHandler(this.DimensionFilterCheckBox_CheckedChanged);
            // 
            // ElementTypeFilterCheckBox
            // 
            this.ElementTypeFilterCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ElementTypeFilterCheckBox.Checked = true;
            this.ElementTypeFilterCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ElementTypeFilterCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ElementTypeFilterCheckBox.Location = new System.Drawing.Point(8, 279);
            this.ElementTypeFilterCheckBox.Name = "ElementTypeFilterCheckBox";
            this.ElementTypeFilterCheckBox.Size = new System.Drawing.Size(172, 16);
            this.ElementTypeFilterCheckBox.TabIndex = 3;
            this.ElementTypeFilterCheckBox.Text = "Use &ElementType filter";
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
            this.propertyGrid.Size = new System.Drawing.Size(215, 295);
            this.propertyGrid.TabIndex = 4;
            // 
            // groupBoxOutputExchnageItems
            // 
            this.groupBoxOutputExchnageItems.Controls.Add(this.labelWarning);
            this.groupBoxOutputExchnageItems.Controls.Add(this.providerExchangeItemSelector);
            this.groupBoxOutputExchnageItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBoxOutputExchnageItems.Location = new System.Drawing.Point(8, 28);
            this.groupBoxOutputExchnageItems.Name = "groupBoxOutputExchnageItems";
            this.groupBoxOutputExchnageItems.Size = new System.Drawing.Size(204, 315);
            this.groupBoxOutputExchnageItems.TabIndex = 23;
            this.groupBoxOutputExchnageItems.TabStop = false;
            this.groupBoxOutputExchnageItems.Text = " Output Exchange Items";
            // 
            // labelWarning
            // 
            this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelWarning.ForeColor = System.Drawing.Color.Red;
            this.labelWarning.Location = new System.Drawing.Point(8, 282);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(188, 28);
            this.labelWarning.TabIndex = 2;
            this.labelWarning.Text = "Warning: Selected combination of DataOperations is invalid !";
            this.labelWarning.Visible = false;
            // 
            // labelInfo
            // 
            this.labelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.ForeColor = System.Drawing.SystemColors.Desktop;
            this.labelInfo.Location = new System.Drawing.Point(8, 0);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(642, 28);
            this.labelInfo.TabIndex = 24;
            this.labelInfo.Text = "Connection XXX =>\n YYY";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBoxLinks
            // 
            this.groupBoxLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLinks.Controls.Add(this.buttonApply);
            this.groupBoxLinks.Controls.Add(this.buttonRemove);
            this.groupBoxLinks.Controls.Add(this.listLinks);
            this.groupBoxLinks.Location = new System.Drawing.Point(104, 4);
            this.groupBoxLinks.Name = "groupBoxLinks";
            this.groupBoxLinks.Size = new System.Drawing.Size(534, 128);
            this.groupBoxLinks.TabIndex = 25;
            this.groupBoxLinks.TabStop = false;
            this.groupBoxLinks.Text = " Links";
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonApply.Location = new System.Drawing.Point(462, 16);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(64, 24);
            this.buttonApply.TabIndex = 6;
            this.buttonApply.Text = "&Apply";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRemove.Location = new System.Drawing.Point(462, 48);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(64, 24);
            this.buttonRemove.TabIndex = 7;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // groupBoxProperties
            // 
            this.groupBoxProperties.Controls.Add(this.propertyGrid);
            this.groupBoxProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProperties.Location = new System.Drawing.Point(422, 28);
            this.groupBoxProperties.Name = "groupBoxProperties";
            this.groupBoxProperties.Size = new System.Drawing.Size(228, 315);
            this.groupBoxProperties.TabIndex = 26;
            this.groupBoxProperties.TabStop = false;
            this.groupBoxProperties.Text = "DataOperation properties";
            // 
            // buttonViewElementSet
            // 
            this.buttonViewElementSet.Enabled = false;
            this.buttonViewElementSet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonViewElementSet.Location = new System.Drawing.Point(8, 16);
            this.buttonViewElementSet.Name = "buttonViewElementSet";
            this.buttonViewElementSet.Size = new System.Drawing.Size(80, 32);
            this.buttonViewElementSet.TabIndex = 8;
            this.buttonViewElementSet.Text = "ElementSet &viewer";
            this.buttonViewElementSet.Click += new System.EventHandler(this.buttonViewElementSet_Click);
            // 
            // groupBoxTools
            // 
            this.groupBoxTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxTools.Controls.Add(this.buttonViewElementSet);
            this.groupBoxTools.Location = new System.Drawing.Point(4, 4);
            this.groupBoxTools.Name = "groupBoxTools";
            this.groupBoxTools.Size = new System.Drawing.Size(96, 128);
            this.groupBoxTools.TabIndex = 27;
            this.groupBoxTools.TabStop = false;
            this.groupBoxTools.Text = "Tools";
            // 
            // groupBoxInputExchangeItems
            // 
            this.groupBoxInputExchangeItems.Controls.Add(this.acceptorExchangeItemSelector);
            this.groupBoxInputExchangeItems.Controls.Add(this.DimensionFilterCheckBox);
            this.groupBoxInputExchangeItems.Controls.Add(this.ElementTypeFilterCheckBox);
            this.groupBoxInputExchangeItems.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBoxInputExchangeItems.Location = new System.Drawing.Point(215, 28);
            this.groupBoxInputExchangeItems.Name = "groupBoxInputExchangeItems";
            this.groupBoxInputExchangeItems.Size = new System.Drawing.Size(204, 315);
            this.groupBoxInputExchangeItems.TabIndex = 0;
            this.groupBoxInputExchangeItems.TabStop = false;
            this.groupBoxInputExchangeItems.Text = " Input Exchange Items";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonClose);
            this.panelBottom.Controls.Add(this.groupBoxLinks);
            this.panelBottom.Controls.Add(this.groupBoxTools);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(8, 347);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(642, 168);
            this.panelBottom.TabIndex = 28;
            // 
            // splitterHorizontal
            // 
            this.splitterHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterHorizontal.Location = new System.Drawing.Point(8, 343);
            this.splitterHorizontal.MinExtra = 150;
            this.splitterHorizontal.MinSize = 150;
            this.splitterHorizontal.Name = "splitterHorizontal";
            this.splitterHorizontal.Size = new System.Drawing.Size(642, 4);
            this.splitterHorizontal.TabIndex = 30;
            this.splitterHorizontal.TabStop = false;
            // 
            // splitterVertical2
            // 
            this.splitterVertical2.Location = new System.Drawing.Point(419, 28);
            this.splitterVertical2.MinExtra = 150;
            this.splitterVertical2.MinSize = 150;
            this.splitterVertical2.Name = "splitterVertical2";
            this.splitterVertical2.Size = new System.Drawing.Size(3, 315);
            this.splitterVertical2.TabIndex = 24;
            this.splitterVertical2.TabStop = false;
            // 
            // splitterVertical1
            // 
            this.splitterVertical1.Location = new System.Drawing.Point(212, 28);
            this.splitterVertical1.MinExtra = 150;
            this.splitterVertical1.MinSize = 150;
            this.splitterVertical1.Name = "splitterVertical1";
            this.splitterVertical1.Size = new System.Drawing.Size(3, 315);
            this.splitterVertical1.TabIndex = 27;
            this.splitterVertical1.TabStop = false;
            // 
            // ConnectionDialog
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(658, 523);
            this.Controls.Add(this.groupBoxProperties);
            this.Controls.Add(this.splitterVertical2);
            this.Controls.Add(this.groupBoxInputExchangeItems);
            this.Controls.Add(this.splitterVertical1);
            this.Controls.Add(this.groupBoxOutputExchnageItems);
            this.Controls.Add(this.splitterHorizontal);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.panelBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(660, 400);
            this.Name = "ConnectionDialog";
            this.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connection properties";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.LinkDialog_Closing);
            this.groupBoxOutputExchnageItems.ResumeLayout(false);
            this.groupBoxLinks.ResumeLayout(false);
            this.groupBoxProperties.ResumeLayout(false);
            this.groupBoxTools.ResumeLayout(false);
            this.groupBoxInputExchangeItems.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion	

		

		
		#endregion
	}
}
