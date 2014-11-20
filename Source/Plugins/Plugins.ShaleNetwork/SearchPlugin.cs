using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Data;
using DotSpatial.Projections;
using HydroDesktop.Common;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Interfaces.PluginContracts;
using HydroDesktop.Help;
using Msg = Plugins.ShaleNetwork.MessageStrings;
using DotSpatial.Topology;
using DotSpatial.Symbology;
using Plugins.ShaleNetwork.Settings.UI;
using Plugins.ShaleNetwork.Measure;

namespace Plugins.ShaleNetwork
{
    public class SearchPlugin : Extension
    {
        #region Fields

        private SimpleActionItem _Shale;
        private readonly string _searchKey = SharedConstants.SearchRootkey;
        private MapFunctionMeasure _Painter;
        private HydroDesktop.Plugins.Search.SearchPlugin searchPlugin;

        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        #endregion

        #region Plugin operations

        public override void Activate()
        {
            AddSearchRibbon();
            base.Activate();

            App.ExtensionsActivated += AppOnExtensionsActivated;
            searchPlugin = (HydroDesktop.Plugins.Search.SearchPlugin)App.GetExtension("HydroDesktop.Plugins.Search");

            if (searchPlugin != null)
                searchPlugin.setSearchTabCaption(MessageStrings.Shale);
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();

            if (searchPlugin != null)
                searchPlugin.setSearchTabCaption("Search");
        }

        #endregion

        #region Private methods

        private void AddSearchRibbon()
        {
            var head = App.HeaderControl;

            App.HeaderControl.Add(new SimpleActionItem(_searchKey, "Measure", MeasureTool_Click) { GroupCaption = Msg.Controls, SmallImage = ShaleNetwork.Properties.Resources.measure_16x16, LargeImage = ShaleNetwork.Properties.Resources.measure_32x32 });

            head.Add(new SimpleActionItem(_searchKey, "SN Website", SN_Website_Click) { GroupCaption = Msg.Shale });
            head.Add(new SimpleActionItem(_searchKey, "Contact SN", Contact_SN_Click) { GroupCaption = Msg.Shale });

            _Shale = new SimpleActionItem(_searchKey, Msg.Shale_Data, Shale_Click)
            {
                LargeImage = ShaleNetwork.Properties.Resources.SN_Logo,
                GroupCaption = Msg.Keyword,
                ToolTipText = Msg.Shale_Tooltip
            };

            App.HeaderControl.Add(_Shale);

        }

        private void AppOnExtensionsActivated(object sender, EventArgs eventArgs)
        {
            searchPlugin = (HydroDesktop.Plugins.Search.SearchPlugin)App.GetExtension("HydroDesktop.Plugins.Search");

            if (searchPlugin != null)
                searchPlugin.setSearchTabCaption(MessageStrings.Shale);
        }

        #region Search

        void SN_Website_Click(object sender, EventArgs e)
        {
            OpenUri(ShaleNetwork.Properties.Settings.Default.SN_Website);
        }

        void Contact_SN_Click(object sender, EventArgs e)
        {
            OpenUri(ShaleNetwork.Properties.Settings.Default.Contact_SN);
        }

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

        #region Keywords

        void Shale_Click(object sender, EventArgs e)
        {
            if (searchPlugin != null && searchPlugin.IsActive)
            {
                //ShaleDialog dialog = new ShaleDialog();
                if (ShaleDialog.ShowDialog(searchPlugin._searchSettings.KeywordsSettings) == DialogResult.OK)
                {
                    var selectedKeywords = searchPlugin._searchSettings.KeywordsSettings.SelectedKeywords.ToList();

                    if (selectedKeywords.Count > 1)
                    {
                        searchPlugin._dropdownKeywords.MultiSelect = true;

                        // This code has no other purpose than to  immediately trigger the text to change to Multiple Selected.
                        // Without it, you have to hover or click on something in the ribbon for the change to occur.
                        searchPlugin._dropdownKeywords.Enabled = false;
                        searchPlugin._dropdownKeywords.Enabled = true;

                    }
                    else if (selectedKeywords.Count == 1)
                    {
                        searchPlugin._dropdownKeywords.MultiSelect = false;
                        searchPlugin._dropdownKeywords.SelectedItem = selectedKeywords[0];
                    }
                    else
                    {
                        searchPlugin._dropdownKeywords.MultiSelect = false;
                        searchPlugin._dropdownKeywords.SelectedItem = null;
                    }

                    searchPlugin.UpdateKeywordsCaption();
                }
            }
        }

        private void MeasureTool_Click(object sender, EventArgs e)
        {
            if (_Painter == null)
                _Painter = new MapFunctionMeasure(App.Map);

            if (!App.Map.MapFunctions.Contains(_Painter))
                App.Map.MapFunctions.Add(_Painter);

            App.Map.FunctionMode = FunctionMode.None;
            App.Map.Cursor = Cursors.Cross;
            _Painter.Activate();
        }

        #endregion

        #endregion
    }
}
