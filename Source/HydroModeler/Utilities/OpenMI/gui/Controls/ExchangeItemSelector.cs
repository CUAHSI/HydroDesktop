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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
//using System.Data;
using System.Windows.Forms;
using OpenMI.Standard;
using Oatc.OpenMI.Gui.Core;

namespace Oatc.OpenMI.Gui.Controls
{
	/// <summary>
	/// This control represents tree of exchange items. First level nodes represent
	/// <see cref="IQuantity">IQuantity</see>, second level nodes represent 
	/// <see cref="IElementSet">IElementSet</see> and 
	/// third level nodes represent <see cref="IDataOperation">IDataOperation</see>s in case of 
	/// <see cref="IOutputExchangeItem">IOutputExchangeItem</see>s.
	/// </summary>	
	public class ExchangeItemSelector: System.Windows.Forms.UserControl
	{
		#region Form controls
		private System.Windows.Forms.TreeView treeView1;
		#endregion

		#region Public events

		/// <summary>
		/// Occurs when the selection of tree node changes.
		/// </summary>
		public event EventHandler SelectionChanged;

		/// <summary>
		/// Occurs when the check-state of checkboxes in the tree changes.
		/// </summary>
		public event EventHandler CheckboxesChanged;

		#endregion

		#region Member variables

		private ArrayList _exchangeItems;

		private IQuantity _checkedQuantity;
		private IElementSet _checkedElementSet;
		private ArrayList _checkedDataOperations;
		
		/// <summary>
		/// Hashtable used to find exchange item corresponding to quantity and element set.
		/// As key we use strign created with method <see cref="CreateExchangeItemID">CreateExchangeItemID</see>.
		/// The value is IExchangeItem, if there's no other exchange item with same key,
		/// if yes, value is ArrayList of such exchange items.
		/// </summary>
		private Hashtable _exchangeItemsSearcher;

		private IDimension _filterDimension;
		private IElementSet _filterElementSet;

		private System.Windows.Forms.ImageList imageList1;		

		bool _blockAfterCheckEventHandler;

		#endregion		

		/// <summary>
		/// Creates a new instance of <see cref="ExchangeItemSelector">ExchangeItemSelector</see> control.
		/// </summary>
        public ExchangeItemSelector()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();			
		}


		private class ExchangeItemsComparer: IComparer
		{
			public int Compare(object x, object y)
			{
				IExchangeItem itemX = (IExchangeItem)x;
				IExchangeItem itemY = (IExchangeItem)y;

				int result = string.Compare( itemX.Quantity.ID, itemY.Quantity.ID, false );

				if( result==0 )				
					result = string.Compare( itemX.ElementSet.ID, itemY.ElementSet.ID, false );

				return( result );
			}
		}



		#region Methods

		/// <summary>
		/// Populates this control with exchange items.
		/// </summary>
		/// <param name="exchangeItems">Exchange items.</param>
		/// <param name="showCheckboxes">Determines whether to show check-boxes in the tree.</param>
		public void PopulateExchangeItemTree( IExchangeItem[] exchangeItems, bool showCheckboxes )
		{
			_checkedQuantity = null;
			_checkedElementSet = null;
			_checkedDataOperations = new ArrayList();

			_filterDimension = null;
			_filterElementSet = null;

			_blockAfterCheckEventHandler = false;

			_exchangeItems = new ArrayList( exchangeItems );

			ExchangeItemsComparer comparer = new ExchangeItemsComparer();
			_exchangeItems.Sort( comparer );
			

			// fill _exchangeItemsSearcher
			_exchangeItemsSearcher = new Hashtable();
			foreach( IExchangeItem exchangeItem in _exchangeItems )
			{
				if( exchangeItem.ElementSet==null || exchangeItem.Quantity==null )
					throw( new Exception("Exchange item cannot have ElementSet or Quantity null.") );

				string key = CreateExchangeItemID(exchangeItem.Quantity, exchangeItem.ElementSet);
				object entry = _exchangeItemsSearcher[ key ];
				
				if( entry == null )
				{
					// first usage of this key
					_exchangeItemsSearcher[key] = exchangeItem;
				}
				else if( entry is IExchangeItem )
				{
					// this key already used once => create ArrayList of exchange items with this key
					ArrayList list = new ArrayList();
					list.Add( entry );
					list.Add( exchangeItem );

					_exchangeItemsSearcher[key] = list;
				} 
				else
				{
					// key used more times
					((ArrayList)entry).Add( exchangeItem );
				}				
			}				

			treeView1.CheckBoxes = showCheckboxes;

			CreateTree();
		}


		/// <summary>
		/// Creates the tree based on element sets and quantities in exchange items
		/// passed with <see cref="PopulateExchangeItemTree">PopulateExchangeItemTree</see> method.
		/// </summary>
		public void CreateTree()
		{
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();


			string lastQuantityID = null;

			// fill tree with exchange items (if there are any because DrawTree could be called
			// before _exchangeItems was assigned )
			if( _exchangeItems!=null )
				foreach( IExchangeItem exchangeItem in _exchangeItems )
				{
					TreeNode elementSetNode = null; // newly added node (if any)

					// apply filters (if any)
					if( _filterDimension != null
					    && !Utils.CompareDimensions(_filterDimension,exchangeItem.Quantity.Dimension) )
						continue;
					
					if( _filterElementSet != null
						&& _filterElementSet.ElementType != ElementType.IDBased
						&& (exchangeItem.ElementSet.ElementType != ElementType.IDBased
						    || exchangeItem.ElementSet.ElementCount != _filterElementSet.ElementCount) )
						continue;

				
					TreeNode quantityNode;

					if( lastQuantityID != exchangeItem.Quantity.ID )
					{
						// adding new quantity node							
						quantityNode = treeView1.Nodes.Add( exchangeItem.Quantity.ID );
						quantityNode.Tag = exchangeItem.Quantity;
						if( exchangeItem.Quantity.ValueType == global::OpenMI.Standard.ValueType.Scalar )
							quantityNode.ImageIndex = quantityNode.SelectedImageIndex = 0;
						else
							quantityNode.ImageIndex = quantityNode.SelectedImageIndex = 1;

						lastQuantityID = exchangeItem.Quantity.ID;
					}
					else
					{
						// last node corresponds to quantity with same ID
						quantityNode = treeView1.Nodes[ treeView1.Nodes.Count-1 ];
					}

					// Add ElementSet node									
					elementSetNode = quantityNode.Nodes.Add( exchangeItem.ElementSet.ID );

					Debug.Assert( 0<=(int)exchangeItem.ElementSet.ElementType && (int)exchangeItem.ElementSet.ElementType<=9 );
					elementSetNode.ImageIndex = elementSetNode.SelectedImageIndex = (int)exchangeItem.ElementSet.ElementType + 2;

					elementSetNode.Tag = exchangeItem.ElementSet;
						

					// add DataOperation subnodes only if newly added node is IOutputExchangeItem 
					if( exchangeItem is IOutputExchangeItem )
					{
						IOutputExchangeItem item = (IOutputExchangeItem)exchangeItem;
						for( int j=0; j<item.DataOperationCount; j++ )
						{
							TreeNode dataOperationNode = elementSetNode.Nodes.Add( item.GetDataOperation(j).ID );
							dataOperationNode.ImageIndex = dataOperationNode.SelectedImageIndex = 12;
							dataOperationNode.Tag = item.GetDataOperation(j);
						}
					}
				}

			treeView1.CollapseAll();
			treeView1.EndUpdate();
		
			_checkedQuantity = null;
			_checkedElementSet = null;
			_checkedDataOperations = new ArrayList();
		}


	
		/// <summary>
		/// Sets which one Quantity, one ElementSet and some DataOperations are currently checked.
		/// </summary>
		/// <param name="elementSet">ElementSet</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="selectedDataOperations">Array of DataOperations which will be checked. If null no one will be checked.</param>.
		/// <remarks>Only one Quantity and one ElementSet can be checked at the time. DataOperations
		/// corresponding to Quantity->ElementSet exchange item can be checked as needed,
		/// all other cannot be checked.</remarks>
		public void SetCheckedExchangeItem( IQuantity quantity, IElementSet elementSet, IDataOperation[] selectedDataOperations )
		{
			ClearCheckboxes(); 

			if( quantity==null )
				return;

			treeView1.BeginUpdate();

			// note: we don't have to set _checked... members here,
			// because treeView1_AfterCheck event handler will do it

			// check Quantity (if any matching)
			for( int i = 0; i < treeView1.Nodes.Count; i++ )
				if( treeView1.Nodes[i].Text == quantity.ID )
				{
					treeView1.Nodes[i].Checked = true;
					
					// check ElementSet (if any matching)
					if( elementSet!=null )
						for( int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++ )
							if( treeView1.Nodes[i].Nodes[j].Text == elementSet.ID )
							{
								treeView1.Nodes[i].Nodes[j].Checked = true;
								
								// check selected data operations (if any matching)
								if( selectedDataOperations != null )
								{
									foreach( IDataOperation dataOperation in selectedDataOperations )
										for( int k=0; k<treeView1.Nodes[i].Nodes[j].Nodes.Count; k++ )
											if( treeView1.Nodes[i].Nodes[j].Nodes[k].Text == dataOperation.ID )
											{
												treeView1.Nodes[i].Nodes[j].Nodes[k].Checked = true;												
											}
								}

								//goto Finish;
								break;
							}
					//goto Finish;
					break;
				}

			//Finish:

			treeView1.EndUpdate();
		}

	
		/// <summary>
		/// Gets nodes of tree that are currently checked.
		/// </summary>
		/// <param name="quantity">Currently checked quantity or <c>null</c> if not checked.</param>
		/// <param name="elementSet">Currently checked elementSet or <c>null</c> if not checked.</param>
		/// <param name="selectedDataOperations">Currently checked data operations or <c>null</c> if no checked.</param>
		public void GetCheckedExchangeItem( out IQuantity quantity, out IElementSet elementSet, out IDataOperation[] selectedDataOperations )
		{
			quantity = _checkedQuantity;
			elementSet = _checkedElementSet;
			if( _checkedDataOperations==null )
				selectedDataOperations = null;
			else
				selectedDataOperations = (IDataOperation[])_checkedDataOperations.ToArray( typeof(IDataOperation) );
		}


		/// <summary>
		/// Gets <see cref="IExchangeItem">IExchangeItem</see> corresponding to <c>quantity</c> and <c>elementSet</c>.
		/// </summary>
		/// <param name="quantity">Quantity of this exchange item.</param>
		/// <param name="elementSet">ElementSet of this exchange item.</param>
		/// <returns>Returns corresponding exchange item or <c>null</c> if not found.</returns>
		public IExchangeItem GetExchangeItem( IQuantity quantity, IElementSet elementSet )
		{
			Debug.Assert( quantity!=null && elementSet!=null );

			object val = _exchangeItemsSearcher[ CreateExchangeItemID(quantity,elementSet) ];

			if( val == null )
				return( null );

			if( val is ArrayList )
			{
				// there are more exchange items with this combintaion of quantity ID and elementSet ID
				ArrayList list = (ArrayList)val;

				foreach( IExchangeItem item in list )				
					if( item.ElementSet == elementSet
						&& item.Quantity == quantity )
						return( item );

				return( null );
			}
			else
			{				
				return( (IExchangeItem)val );
			}			
		}

		private string CreateExchangeItemID( IQuantity quantity, IElementSet elementSet )
		{
			return( "Oatc.OpenMI.Gui;Q:" + quantity.ID + "E:" + elementSet.ID );
		}


		/// <summary>
		/// Gets object corresponding to currently selected node.
		/// </summary>
		/// <returns>Returns IOutputExchangeItem, IInputExchangeItem, IDataOperation or <c>null</c> if no node is selected.</returns>
		public object GetSelectedObject()
		{
			if( treeView1.SelectedNode!=null )			
				return( treeView1.SelectedNode.Tag );			
			else
				return( null );
		}


		/// <summary>
		/// Expands the node of the tree which is currently checked.
		/// </summary>
		public void ExpandChecked()
		{
			treeView1.BeginUpdate();

			treeView1.CollapseAll();

			for( int i = 0; i < treeView1.Nodes.Count; i++ )
				if( treeView1.Nodes[i].Checked )
					for( int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++ )
						if( treeView1.Nodes[i].Nodes[j].Checked )
						{
							treeView1.Nodes[i].Nodes[j].Expand(); // expand DataOperations (if any)
							treeView1.Nodes[i].Nodes[j].EnsureVisible();
							goto Finish;
						}

			Finish:

			treeView1.EndUpdate();
		}


		/// <summary>
		/// Sets check-state of all checkboxes in tree to unchecked.
		/// </summary>
		public void ClearCheckboxes()
		{
			// this method may be called from AfterCheck event handler so
			// so we must preserve previous value of _blockAfterCheckEventHandler variable
			bool prevValue = _blockAfterCheckEventHandler;
			_blockAfterCheckEventHandler = true;

			// tree can be in incorrect state, so we must really uncheck all checkboxes
			for( int i = 0; i < treeView1.Nodes.Count; i++ )				
			{
				treeView1.Nodes[i].Checked = false;
				for( int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++ )					
				{
					treeView1.Nodes[i].Nodes[j].Checked = false;
					for( int k = 0; k < treeView1.Nodes[i].Nodes[j].Nodes.Count; k++ )
						treeView1.Nodes[i].Nodes[j].Nodes[k].Checked = false;					
				}				
			}

			_checkedQuantity = null;
			_checkedElementSet = null;
			_checkedDataOperations = new ArrayList();

			_blockAfterCheckEventHandler = prevValue;
		}


		/// <summary>
		/// Enables or disables Dimension filter.
		/// </summary>
		/// <param name="filterDimension">New Dimension filter or <c>null</c> if no filter should be used.</param>
		public void EnableDimensionFilter( IDimension filterDimension )
		{
			if( _filterDimension != filterDimension ) // little optimisation
			{
				_filterDimension = filterDimension;
				CreateTree();
			}
		}	


		/// <summary>
		/// Enables or disables ElementSet filter.
		/// </summary>
		/// <param name="filterElementSet">New ElementSet filter or <c>null</c> if no filter should be used.</param>
		public void EnableElementSetFilter( IElementSet filterElementSet )
		{
			if( _filterElementSet != filterElementSet ) // little optimisation
			{
				_filterElementSet = filterElementSet;
				CreateTree();
			}
		}		



		#endregion

		#region Event handlers

		private void Properties(object sender, System.EventArgs e)
		{
		
		}


		private void treeView1_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if( _blockAfterCheckEventHandler )
				return;			
			_blockAfterCheckEventHandler = true;

			//treeView1.BeginUpdate();

			TreeNode node = e.Node;
			bool isChecked = node.Checked;
			

			// Make check-state of nodes correct
			if( node.Tag is IQuantity )
			{
				ClearCheckboxes();
				node.Checked = isChecked;
				if( isChecked )
					_checkedQuantity = (IQuantity)node.Tag;
			}
			else if( node.Tag is IElementSet )
			{
				if( isChecked )
				{
					if( node.Parent.Checked )
					{
						for( int i=0; i<node.Parent.Nodes.Count; i++ )
						{
							node.Parent.Nodes[i].Checked = false;
							for( int j=0; j<node.Parent.Nodes[i].Nodes.Count; j++ )
								node.Parent.Nodes[i].Nodes[j].Checked = false;
						}

						node.Checked = true;
						_checkedElementSet = (IElementSet)node.Tag;

						_checkedDataOperations = new ArrayList();
					}
					else
					{
						ClearCheckboxes();

						node.Parent.Checked = true;
						_checkedQuantity = (IQuantity)node.Parent.Tag;
						
						node.Checked = true;
						_checkedElementSet = (IElementSet)node.Tag;
					}					
				}
				else
				{
					ClearCheckboxes();

					node.Parent.Checked = true;
					_checkedQuantity = (IQuantity)node.Parent.Tag;
				}
			}
			else if( node.Tag is IDataOperation )
			{
				if( isChecked )
				{
					if( node.Parent.Checked )
					{
						_checkedDataOperations.Add( node.Tag );		
					}
					else					
					{
						ClearCheckboxes();

						node.Parent.Parent.Checked = true;
						_checkedQuantity = (IQuantity)node.Parent.Parent.Tag;

						node.Parent.Checked = true;
						_checkedElementSet = (IElementSet)node.Parent.Tag;

						node.Checked = true;
						//_checkedDataOperations = new ArrayList();
						_checkedDataOperations.Clear();
						_checkedDataOperations.Add( node.Tag );
					}
				}
				else
				{
					_checkedDataOperations.Remove( node.Tag );
				}
			}	

			//treeView1.EndUpdate();

			_blockAfterCheckEventHandler = false;

			CheckboxesChanged(this, new EventArgs());
		}

		
		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			SelectionChanged(this, new EventArgs());			
		}


		private void ExchangeItemSelector_SizeChanged(object sender, System.EventArgs e)
		{
			// resize treeView so it fits control size
			/*const int border = 8;
			treeView1.Width = ClientRectangle.Width - 2*border;
			treeView1.Height = ClientRectangle.Height - 2*border;
			treeView1.Top = border;
			treeView1.Left = border;*/
		}

		
		private void treeView1_Enter(object sender, System.EventArgs e)
		{
			// control was activated, so behave like some node was selected
			treeView1_AfterSelect( null, null );		
		}

		
		private void ExchangeItemSelector_Load(object sender, System.EventArgs e)
		{
			ExchangeItemSelector_SizeChanged( sender, e );		
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
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExchangeItemSelector));
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.treeView1.CheckBoxes = true;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(256, 328);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.Enter += new System.EventHandler(this.treeView1_Enter);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// ExchangeItemSelector
			// 
			this.Controls.Add(this.treeView1);
			this.Name = "ExchangeItemSelector";
			this.Size = new System.Drawing.Size(256, 328);
			this.Load += new System.EventHandler(this.ExchangeItemSelector_Load);
			this.SizeChanged += new System.EventHandler(this.ExchangeItemSelector_SizeChanged);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion

	}
}
