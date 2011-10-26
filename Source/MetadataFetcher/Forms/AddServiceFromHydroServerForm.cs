using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.WebServices;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.MetadataFetcher.Forms
{
	public partial class AddServiceFromHydroServerForm : Form
	{
		#region Variables

		private HydroServerClient _hisServerClient;

		#endregion

		#region Constructor

		public AddServiceFromHydroServerForm ()
		{
			InitializeComponent ();
		}

		#endregion

		#region UI Events

		/// <summary>
		/// Cancels or closes the form if the user presses the ESC key
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns>true if key press was handled, false otherwise</returns>
		protected override bool ProcessCmdKey ( ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData )
		{
			try
			{
				if ( msg.WParam.ToInt32 () == (int)Keys.Escape )
				{
					this.DialogResult = DialogResult.Cancel;
					this.Close ();
					return true;
				}
				else
				{
					return base.ProcessCmdKey ( ref msg, keyData );
				}
			}
			catch ( Exception Ex )
			{
				MessageBox.Show ( "Key Overrided Events Error:" + Ex.Message );
			}

			return base.ProcessCmdKey ( ref msg, keyData );
		}

		private void btnGetServices_Click ( object sender, EventArgs e )
		{
			// Show hourglass while we check the URL
			this.Cursor = Cursors.WaitCursor;
			lblStatus.Visible = true;
			lblStatus.Update ();

			// Check that this is a valid URL
			string serverUrl = txtUrl.Text;

			if ( HydroDesktop.MetadataFetcher.WebOperations.IsUrlValid ( serverUrl, true ) == false )
			{
				this.Cursor = Cursors.Default;
				lblStatus.Visible = false;
				MessageBox.Show ( "Please enter a valid URL to a HydroServer",
								  this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}
			
			// Connect to the server
			try
			{
				_hisServerClient = new HydroServerClient ( serverUrl );
			}
			catch ( Exception ex )
			{
				this.Cursor = Cursors.Default;
				lblStatus.Visible = false;
				MessageBox.Show ( "Could not connect to HydroServer at " + serverUrl + ".\n" + ex.Message,
								  this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}


			// Restore the mouse cursor
			this.Cursor = Cursors.Default;
			lblStatus.Visible = false;

			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Update ();
		}

		#endregion

		#region Public Members

		public HydroServerClient HisServerConnection
		{
			get
			{
				return _hisServerClient;
			}
		}

		#endregion
	}
}
