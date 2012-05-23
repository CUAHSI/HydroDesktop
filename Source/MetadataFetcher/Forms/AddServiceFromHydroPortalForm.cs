using System;
using System.Windows.Forms;

namespace HydroDesktop.MetadataFetcher.Forms
{
	public partial class AddServiceFromHydroPortalForm : Form
	{
		#region Variables

		private HydroDesktop.MetadataFetcher.HydroPortalUtils _portalUtils;

		#endregion

		#region Constructor
        /// <summary>
        /// Creates a new Add Service from HydroPortal form
        /// </summary>
		public AddServiceFromHydroPortalForm ()
		{
			InitializeComponent ();
		}

		#endregion

		#region UI Events

		/// <summary>
		/// Cancels or closes the form if the user presses the ESC key
		/// </summary>
		/// <param name="msg">message</param>
		/// <param name="keyData">key data (esc key press)</param>
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
			string portalUrl = txtUrl.Text;
			if ( HydroDesktop.MetadataFetcher.WebOperations.IsUrlValid ( portalUrl, true ) == false )
			{
				this.Cursor = Cursors.Default;
				lblStatus.Visible = false;
				MessageBox.Show ( "Please enter a valid URL to a HydroPortal",
								  this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			// Create a new HydroPortal helper utility object
			try
			{
				_portalUtils = new HydroDesktop.MetadataFetcher.HydroPortalUtils ( portalUrl );
			}
			catch ( Exception ex )
			{
				this.Cursor = Cursors.Default;
				lblStatus.Visible = false;
				MessageBox.Show ( "Could not connect to HydroPortal at " + portalUrl + ".\n" + ex.Message,
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
        /// <summary>
        /// Initializes the HydroPortal connection
        /// </summary>
		public HydroDesktop.MetadataFetcher.HydroPortalUtils HydroPortalConnection
		{
			get
			{
				return _portalUtils;
			}
		}

		#endregion

	}
}
