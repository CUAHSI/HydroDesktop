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
using Search3.Area;
using Search3.Keywords;
using Search3.Properties;
using Search3.Searching;
using Search3.Searching.Exceptions;
using Search3.Settings;
using Search3.Settings.UI;
using Msg = ShaleDataNetwork.MessageStrings;
using DotSpatial.Topology;
using DotSpatial.Symbology;
using ShaleDataNetwork.Settings.UI;
using ShaleDataNetwork.Measure;

namespace ShaleDataNetwork
{
    public class SearchPlugin : Extension
    {
        #region Fields

        private SimpleActionItem _shaleData;
        private readonly string _searchKey = SharedConstants.SearchRootkey;
        private MapFunctionMeasure _Painter;
        private Search3.SearchPlugin search3;

        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        #endregion

        #region Plugin operations

        public override void Activate()
        {
            AddSearchRibbon();
            base.Activate();

            App.ExtensionsActivated += AppOnExtensionsActivated;
            if (App.GetExtension("Search3") != null)
            {
                search3 = (Search3.SearchPlugin)App.GetExtension("Search3");
                search3.setSearchTabCaption(MessageStrings.Shale);
            }
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();

            if (App.GetExtension("Search3") != null)
            {
                search3.setSearchTabCaption("Search");
            }
        }

        #endregion

        #region Private methods

        private void AddSearchRibbon()
        {
            var head = App.HeaderControl;

            App.HeaderControl.Add(new SimpleActionItem(_searchKey, "Measure", MeasureTool_Click) { GroupCaption = Msg.Controls, SmallImage = ShaleDataNetwork.Properties.Resources.measure_16x16, LargeImage = ShaleDataNetwork.Properties.Resources.measure_32x32 });

            head.Add(new SimpleActionItem(_searchKey, "SN Website", SN_Website_Click) { GroupCaption = Msg.Shale });
            head.Add(new SimpleActionItem(_searchKey, "Contact SN", Contact_SN_Click) { GroupCaption = Msg.Shale });

            _shaleData = new SimpleActionItem(_searchKey, Msg.Shale_Data, shaleData_Click)
            {
                LargeImage = ShaleDataNetwork.Properties.Resources.SN_Logo,
                GroupCaption = Msg.Keyword,
                ToolTipText = Msg.Shale_Tooltip
            };

            App.HeaderControl.Add(_shaleData);

        }

        private void AppOnExtensionsActivated(object sender, EventArgs eventArgs)
        {
            if (App.GetExtension("Search3") != null)
            {
                search3 = (Search3.SearchPlugin)App.GetExtension("Search3");
                search3.setSearchTabCaption(MessageStrings.Shale);
            }
        }

        #region Search

        void SN_Website_Click(object sender, EventArgs e)
        {
            OpenUri(ShaleDataNetwork.Properties.Settings.Default.SN_Website);
        }

        void Contact_SN_Click(object sender, EventArgs e)
        {
            OpenUri(ShaleDataNetwork.Properties.Settings.Default.Contact_SN);
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

        void shaleData_Click(object sender, EventArgs e)
        {
            if (App.GetExtension("Search3") != null && App.GetExtension("Search3").IsActive)
            {
                //ShaleDataDialog dialog = new ShaleDataDialog();
                if (ShaleDataDialog.ShowDialog(search3._searchSettings.KeywordsSettings) == DialogResult.OK)
                {
                    var selectedKeywords = search3._searchSettings.KeywordsSettings.SelectedKeywords.ToList();

                    if (selectedKeywords.Count > 1)
                    {
                        search3._dropdownKeywords.MultiSelect = true;

                        // This code has no other purpose than to  immediately trigger the text to change to Multiple Selected.
                        // Without it, you have to hover or click on something in the ribbon for the change to occur.
                        search3._dropdownKeywords.Enabled = false;
                        search3._dropdownKeywords.Enabled = true;

                    }
                    else if (selectedKeywords.Count == 1)
                    {
                        search3._dropdownKeywords.MultiSelect = false;
                        search3._dropdownKeywords.SelectedItem = selectedKeywords[0];
                    }
                    else
                    {
                        search3._dropdownKeywords.MultiSelect = false;
                        search3._dropdownKeywords.SelectedItem = null;
                    }

                    search3.UpdateKeywordsCaption();
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
