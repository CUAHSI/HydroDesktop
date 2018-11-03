using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Help;

namespace HydroDesktop.Plugins.HelpTab
{
    /// <summary>
    /// Main class of the HelpTab plugin
    /// </summary>
    public class Main : Extension
    {
        #region Private Member Variables

        // These are resources, that may be changed by Language (in the future)
        private readonly string _helpPanelName = Resources.helpPanelName;
        private readonly string _helpTabName = Resources.helpTabName;

        // These are configurable
        private readonly string _localHelpUri = Properties.Settings.Default.localHelpUri;
        private readonly string _remoteHelpUri = Properties.Settings.Default.remoteHelpUri;
        private readonly string _quickStartUri = Properties.Settings.Default.quickStartUri;
        private readonly string _discussionForumUri = Properties.Settings.Default.discussionForumUri;
        private readonly string _issueTrackerUri = Properties.Settings.Default.issueTrackerUri;
        private readonly string _commentMailtoLink = Properties.Settings.Default.commentMailtoLink;
        private readonly string _hisCommentUri = Properties.Settings.Default.hisCommentUri;

        #endregion

        #region Private Methods

        private void OpenUri(string uriString)
        {
            if (WebUtilities.IsInternetAvailable() == false)
            {
                MessageBox.Show("Internet connection not available.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                WebUtilities.OpenUri(uriString);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No URI provided.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UriFormatException ex)
            {
                MessageBox.Show("Invalid URI format for '" + uriString + "'.\n(" + ex.Message + ")", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                if (ex.Message == "The system cannot find the path specified")
                {
                    MessageBox.Show("Could not find the target at '" + uriString + "'.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Could not open target at '" + uriString + "'.\n(" + ex.Message + ")", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            // Remove ribbon tab
           App.HeaderControl.RemoveAll();

            base.Deactivate();
        }
        /// <summary>
        /// Activates the HelpTab plugin
        /// </summary>
        public override void Activate()
        {
            // Initialize the Ribbon controls in the "Help" ribbon tab
            const string HelpTabKey = "kHelp";
            App.HeaderControl.Add(new RootItem(HelpTabKey, _helpTabName) { SortOrder = 200 });

            // Help quick access image
            var quickHelpButton = new SimpleActionItem("Help", onlineHelpButton_Click);
            quickHelpButton.ToolTipText = "View Help";
            quickHelpButton.GroupCaption = HeaderControl.HeaderHelpItemKey;
            quickHelpButton.LargeImage = Resources.help_32x32;
            quickHelpButton.SmallImage = Resources.help_16x16;
            App.HeaderControl.Add(quickHelpButton);
            //helpPanel.Image = Resources.help_32x32;

            // Add a button to open help documentation
            var userGuideButton = new SimpleActionItem("User Guide", onlineHelpButton_Click);
            userGuideButton.RootKey = HelpTabKey;
            userGuideButton.LargeImage = Resources.help_32x32;
            userGuideButton.SmallImage = Resources.help_16x16;
            userGuideButton.ToolTipText = "Open the help documentation.";
            userGuideButton.GroupCaption = _helpPanelName;
            App.HeaderControl.Add(userGuideButton);

            // Add a button to open quick start guide
            var quickStartButton = new SimpleActionItem("Quick Start", quickStartButton_Click);
            quickStartButton.RootKey = HelpTabKey;
            quickStartButton.LargeImage = Resources.quickstart_32x32;
            quickStartButton.SmallImage = Resources.quickstart_16x16;
            quickStartButton.ToolTipText = "Open the quick start guide.";
            quickStartButton.GroupCaption = _helpPanelName;
            App.HeaderControl.Add(quickStartButton);

            //Separator
            App.HeaderControl.Add(new SeparatorItem(HelpTabKey, _helpPanelName));

            // Add a button to open the discussion forums
            var discussionButton = new SimpleActionItem("Forum", discussionButton_Click);
            discussionButton.RootKey = HelpTabKey;
            discussionButton.LargeImage = Resources.discuss_32x32;
            discussionButton.SmallImage = Resources.discuss_16x16;
            discussionButton.ToolTipText = "Open the HydroDesktop online discussion forum.";
            discussionButton.GroupCaption = _helpPanelName;
            App.HeaderControl.Add(discussionButton);

            // Add a button to open the issue tracker
            var issueTrackerButton = new SimpleActionItem("Issues", issueTrackerButton_Click);
            issueTrackerButton.RootKey = HelpTabKey;
            issueTrackerButton.LargeImage = Resources.onebit_bug_32x32;
            issueTrackerButton.SmallImage = Resources.onebit_bug_16x16;
            issueTrackerButton.ToolTipText = "Report a bug or feature request on the online HydroDesktop issue tracker.";
            issueTrackerButton.GroupCaption = _helpPanelName;
            App.HeaderControl.Add(issueTrackerButton);

            //Separator
            App.HeaderControl.Add(new SeparatorItem(HelpTabKey, _helpPanelName));

            // Add a button to send email to the user support specialist
            //var submitEmailButton = new SimpleActionItem("Contact Support\n", submitEmailButton_Click); // !!! \n gets around a bug where the "t" in "Support" is wrapped to a new line
            //submitEmailButton.RootKey = HelpTabKey;
            //submitEmailButton.LargeImage = Resources.email_32x32;
            //submitEmailButton.SmallImage = Resources.email_16x16;
            //submitEmailButton.ToolTipText = "Send an e-mail to HydroDesktop User Support using your default e-mail program.";
            //submitEmailButton.GroupCaption = _helpPanelName;
            //App.HeaderControl.Add(submitEmailButton);

            // Add a button to leave a comment
            var submitCommentButton = new SimpleActionItem("Contact Support\n", submitCommentButton_Click);
            submitCommentButton.RootKey = HelpTabKey;
            submitCommentButton.LargeImage = Resources.comment_32x32;
            submitCommentButton.SmallImage = Resources.comment_16x16;
            submitCommentButton.ToolTipText = "Contact support using the online HIS contact form.";
            submitCommentButton.GroupCaption = _helpPanelName;
            App.HeaderControl.Add(submitCommentButton);

            //Separator
            App.HeaderControl.Add(new SeparatorItem(HelpTabKey, _helpPanelName));

            // Add a button to show the About dialog
            var aboutButton = new SimpleActionItem("About", aboutButton_Click);
            aboutButton.RootKey = HelpTabKey;
            aboutButton.LargeImage = Resources.info_32x32;
            aboutButton.SmallImage = Resources.info_16x16;
            aboutButton.ToolTipText = "Open the HydroDesktop About dialog.";
            aboutButton.GroupCaption = _helpPanelName;
            App.HeaderControl.Add(aboutButton);

            //add the about button to application menu
            var aboutButton2 = new SimpleActionItem(HeaderControl.ApplicationMenuKey, "About", aboutButton_Click);
            aboutButton2.GroupCaption = HeaderControl.ApplicationMenuKey;
            aboutButton2.LargeImage = Resources.info_32x32;
            aboutButton2.SmallImage = Resources.info_16x16;
            aboutButton2.ToolTipText = "Open the HydroDesktop About dialog.";
            //aboutButton2.GroupCaption = HeaderControl.ApplicationMenuKey;
            App.HeaderControl.Add(aboutButton2);

            // This line ensures that "Enabled" is set to true
            base.Activate();
        }


        #endregion

        #region Event Handlers



        void onlineHelpButton_Click(object sender, EventArgs e)
        {
            try
            {

                if (WebUtilities.IsInternetAvailable() == false)
                {
                    LocalHelp.OpenHelpFile(_localHelpUri);
                }
                else
                {
                    OpenUri(_remoteHelpUri);    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open help file at " + _localHelpUri + "\n" + ex.Message, "Could not open help", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void quickStartButton_Click(object sender, EventArgs e)
        {
            try
            {

                if (WebUtilities.IsInternetAvailable() == false)
                {
                    string quickStartGuideFile = Properties.Settings.Default.QuickStartGuideName;
                    LocalHelp.OpenHelpFile(quickStartGuideFile);
                }
                else
                {
                    OpenUri(_quickStartUri);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open help file at " + _localHelpUri + "\n" + ex.Message, "Could not open help", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        void discussionButton_Click(object sender, EventArgs e)
        {
            OpenUri(_discussionForumUri);
        }

        void issueTrackerButton_Click(object sender, EventArgs e)
        {
            OpenUri(_issueTrackerUri);
        }

        void submitEmailButton_Click(object sender, EventArgs e)
        {
            try
            {
                WebUtilities.OpenMailtoLink(_commentMailtoLink);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No mailto link provided.", "Could not e-mail support", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open mailto link '" + _commentMailtoLink + "'.\n(" + ex.Message + ")", "Could not e-mail support", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void submitCommentButton_Click(object sender, EventArgs e)
        {
            OpenUri(_hisCommentUri);
        }

        void aboutButton_Click(object sender, EventArgs e)
        {
            AboutBox frm = new AboutBox();

            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        #endregion
    }
}
