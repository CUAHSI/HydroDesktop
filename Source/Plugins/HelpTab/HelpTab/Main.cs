using System;
using System.ComponentModel;
using System.Windows.Forms;

using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using HydroDesktop.Help;
using HydroDesktop;
using HydroDesktop.Main;

namespace HelpTab
{
	[Plugin ( "Help Tab", Author = "Tim Whiteaker", UniqueName = "mw_HelpTab_1", Version = "1" )]
	public class Main : Extension, IMapPlugin
	{
		#region Private Member Variables

		// These are resources, that may be changed by Language (in the future)
		private readonly string _helpPanelName = Resources.helpPanelName;
		private readonly string _helpTabName = Resources.helpTabName;

		// These are configurable
		private readonly string _localHelpUri = Properties.Settings.Default.localHelpUri;
		private readonly string _discussionForumUri = Properties.Settings.Default.discussionForumUri;
		private readonly string _issueTrackerUri = Properties.Settings.Default.issueTrackerUri;
		private readonly string _commentMailtoLink = Properties.Settings.Default.commentMailtoLink;
		private readonly string _hisCommentUri = Properties.Settings.Default.hisCommentUri;

		private IMapPluginArgs _pluginArgs;
		private RibbonTab _ribbonHelpTab;

		#endregion

		#region Private Methods

		private void OpenUri ( string uriString )
		{
			if (WebUtilities.IsInternetAvailable () == false )
			{
				MessageBox.Show ( "Internet connection not available.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			try
			{
				WebUtilities.OpenUri ( uriString );
			}
			catch ( NullReferenceException )
			{
				MessageBox.Show ( "No URI provided.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			catch ( UriFormatException ex )
			{
				MessageBox.Show ( "Invalid URI format for '" + uriString + "'.\n(" + ex.Message + ")", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			catch ( Exception ex )
			{
				if ( ex.Message == "The system cannot find the path specified" )
				{
					MessageBox.Show ( "Could not find the target at '" + uriString + "'.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MessageBox.Show ( "Could not open target at '" + uriString + "'.\n(" + ex.Message + ")", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		#endregion

		#region IExtension Members

		/// <summary>
		/// Fires when the plugin should become inactive
		/// </summary>
		protected override void OnDeactivate ()
		{
			// Remove ribbon tab
			if ( _pluginArgs.Ribbon.Tabs.Contains ( _ribbonHelpTab ) )
			{
				_pluginArgs.Ribbon.Tabs.Remove ( _ribbonHelpTab );
			}

			// This line ensures that "Enabled" is set to false
			base.OnDeactivate ();
		}

		protected override void OnActivate ()
		{
			// This line ensures that "Enabled" is set to true
			base.OnActivate ();
		}

		#endregion

		#region IPlugin Members

		/// <summary>
		/// Initialize the plugin
		/// </summary>
		/// <param name="args">The plugin arguments to access the main application</param>
		public void Initialize ( IMapPluginArgs args )
		{
			_pluginArgs = args;

			// Initialize the Ribbon controls in the "Help" ribbon tab
			_ribbonHelpTab = new RibbonTab ( _pluginArgs.Ribbon, _helpTabName );

			// Create a Help panel
			RibbonPanel helpPanel = new RibbonPanel ( _helpPanelName, RibbonPanelFlowDirection.Bottom );
			helpPanel.ButtonMoreVisible = false;
			helpPanel.ButtonMoreEnabled = false;
			helpPanel.Image = Resources.help_32x32;

			_ribbonHelpTab.Panels.Add ( helpPanel );

			// Add a button to open help documentation
			RibbonButton onlineHelpButton = new RibbonButton ( "User Guide" );
			onlineHelpButton.Image = Resources.help_32x32;
			onlineHelpButton.SmallImage = Resources.help_16x16;
			onlineHelpButton.ToolTipTitle = "User Guide";
			onlineHelpButton.ToolTip = "Open the help documentation.";
			onlineHelpButton.Click += new EventHandler ( onlineHelpButton_Click );

			helpPanel.Items.Add ( onlineHelpButton );

			//Separator
			RibbonSeparator separator1 = new RibbonSeparator ();
			helpPanel.Items.Add ( separator1 );

			// Add a button to open the discussion forums
			RibbonButton discussionButton = new RibbonButton ( "Forum" );
			discussionButton.Image = Resources.discuss_32x32;
			discussionButton.SmallImage = Resources.discuss_16x16;
			discussionButton.ToolTipTitle = "Forum";
			discussionButton.ToolTip = "Open the HydroDesktop online discussion forum.";
			discussionButton.Click += new EventHandler ( discussionButton_Click );

			helpPanel.Items.Add ( discussionButton );

			// Add a button to open the issue tracker
			RibbonButton issueTrackerButton = new RibbonButton ( "Issues" );
			issueTrackerButton.Image = Resources.onebit_bug_32x32;
			issueTrackerButton.SmallImage = Resources.onebit_bug_16x16;
			issueTrackerButton.ToolTipTitle = "Issues";
			issueTrackerButton.ToolTip = "Report a bug or feature request on the online HydroDesktop issue tracker.";
			issueTrackerButton.Click += new EventHandler ( issueTrackerButton_Click );

			helpPanel.Items.Add ( issueTrackerButton );

			//Separator
			RibbonSeparator separator2 = new RibbonSeparator ();
			helpPanel.Items.Add ( separator2 );

			// Add a button to send email to the user support specialist
			RibbonButton submitEmailButton = new RibbonButton ( "Contact Support\n" ); // !!! \n gets around a bug where the "t" in "Support" is wrapped to a new line
			submitEmailButton.Image = Resources.email_32x32;
			submitEmailButton.SmallImage = Resources.email_16x16;
			submitEmailButton.ToolTipTitle = "Contact Support";
			submitEmailButton.ToolTip = "Send an e-mail to HydroDesktop User Support using your default e-mail program.";
			submitEmailButton.Click += new EventHandler ( submitEmailButton_Click );

			helpPanel.Items.Add ( submitEmailButton );

			// Add a button to leave a comment
			RibbonButton submitCommentButton = new RibbonButton ( "Submit Comment" );
			submitCommentButton.Image = Resources.comment_32x32;
			submitCommentButton.SmallImage = Resources.comment_16x16;
			submitCommentButton.ToolTipTitle = "Submit Comment";
			submitCommentButton.ToolTip = "Submit a comment using the online HIS contact form.";
			submitCommentButton.Click += new EventHandler ( submitCommentButton_Click );

			helpPanel.Items.Add ( submitCommentButton );

			//Separator
			RibbonSeparator separator3 = new RibbonSeparator ();
			helpPanel.Items.Add ( separator3 );

			// Add a button to show the About dialog
			RibbonButton aboutButton = new RibbonButton ( "About" ); 
			aboutButton.Image = Resources.info_32x32;
			aboutButton.SmallImage = Resources.info_16x16;
			aboutButton.ToolTipTitle = "About";
			aboutButton.ToolTip = "Open the HydroDesktop About dialog.";
			aboutButton.Click += new EventHandler ( aboutButton_Click );

			helpPanel.Items.Add ( aboutButton );

			// Add the tab to the ribbon
			if ( _pluginArgs.Ribbon.Tabs.Contains ( _ribbonHelpTab ) == false )
			{
				_pluginArgs.Ribbon.Tabs.Add ( _ribbonHelpTab );
			}
            
            // To ensure, that HelpTab is the last tab
            _pluginArgs.AppManager.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);
		}

        

		#endregion

		#region Event Handlers

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            //ensure that help tab is last
            if ( _pluginArgs.Ribbon.Tabs.Contains ( _ribbonHelpTab ) )
            {
                if (_pluginArgs.Ribbon.Tabs[_pluginArgs.Ribbon.Tabs.Count - 1] != _ribbonHelpTab)
                {
                    _pluginArgs.Ribbon.Tabs.Remove(_ribbonHelpTab);
                    _pluginArgs.Ribbon.Tabs.Add(_ribbonHelpTab);
                }
            }
        }

		void onlineHelpButton_Click ( object sender, EventArgs e )
		{
			try
			{
				LocalHelp.OpenHelpFile ( _localHelpUri );
			}
			catch ( Exception ex )
			{
				MessageBox.Show ( "Could not open help file at " + _localHelpUri + "\n" + ex.Message, "Could not open help", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		void discussionButton_Click ( object sender, EventArgs e )
		{
			OpenUri ( _discussionForumUri );
		}

		void issueTrackerButton_Click ( object sender, EventArgs e )
		{
			OpenUri ( _issueTrackerUri );
		}

		void submitEmailButton_Click ( object sender, EventArgs e )
		{
			try
			{
				WebUtilities.OpenMailtoLink ( _commentMailtoLink );
			}
			catch ( NullReferenceException )
			{
				MessageBox.Show ( "No mailto link provided.", "Could not e-mail support", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			catch ( Exception ex )
			{
				MessageBox.Show ( "Could not open mailto link '" + _commentMailtoLink + "'.\n(" + ex.Message + ")", "Could not e-mail support", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		void submitCommentButton_Click ( object sender, EventArgs e )
		{
			OpenUri ( _hisCommentUri );
		}

		void aboutButton_Click ( object sender, EventArgs e )
		{
            AboutBox frm = new AboutBox();

            frm.StartPosition = FormStartPosition.CenterScreen;
			frm.Show ();
		}

		#endregion
	}
}
