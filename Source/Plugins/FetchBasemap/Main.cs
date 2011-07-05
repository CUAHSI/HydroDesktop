using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Projections.GeographicCategories;
using DotSpatial.Topology;
using FetchBasemap.Resources;
using FetchBasemap.Tiling;
using MWPoint = DotSpatial.Topology.Point;
using System.Windows.Forms;
using System.Collections;

namespace FetchBasemap
{
    [Plugin("Fetch Basemap", Author = "James Seppi", UniqueName = "mw_FetchBasemap_1", Version = "1")]
    public class Main : Extension, IMapPlugin
    {
        #region Variables

        //reference to the main application and its UI items
        private MapImageLayer _baseMapLayer;
        private BackgroundWorker _bw;
        private IMapPluginArgs _mapPluginArgs;

        private RibbonComboBox _opacityBox;

        private Bitmap _originalBasemapImage;
        private RibbonPanel _rPanelOnlineBasemap;

        private RibbonComboBox _selectServiceBox;

        private ToolStripSeparator _separator;
        private ToolStripComboBox _selectServiceComboBox;

        private TileManager _tileManager;

        private const string PluginName = "FetchBasemap";

        #endregion

        #region IMapPlugin Members

        /// <summary>
        /// Initialize the DotSpatial plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public void Initialize(IMapPluginArgs args)
        {
            _mapPluginArgs = args;

            if (args.Ribbon != null)
            {
                InitializeRibbon();
            }
            else
            {
                InitializeMenuItems();
            }

            

            //Create the tile manager
            _tileManager = new TileManager();


            //Add handlers for saving/restoring settings
            _mapPluginArgs.AppManager.SerializationManager.Serializing += SerializationManagerSerializing;
            _mapPluginArgs.AppManager.SerializationManager.Deserializing += SerializationManagerDeserializing;

            //Setup the background worker
            _bw = new BackgroundWorker {WorkerSupportsCancellation = true, WorkerReportsProgress = true};
            _bw.DoWork += BwDoWork;
            _bw.RunWorkerCompleted += BwRunWorkerCompleted;
            _bw.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);
        }

        void SerializationManagerDeserializing(object sender, SerializingEventArgs e)
        {
            var opacity =
               _mapPluginArgs.AppManager.SerializationManager.GetCustomSetting(PluginName + "_Opacity",
                                                                                       "100");
            var basemapName =
                _mapPluginArgs.AppManager.SerializationManager.GetCustomSetting(PluginName + "_BasemapName",
                                                                                        "None");
            //Set opacity
            if (_opacityBox != null)
            {
                _opacityBox.TextBoxText = opacity;
            }

            _baseMapLayer = (MapImageLayer)_mapPluginArgs.Map.MapFrame.GetAllLayers().Where(
                layer => layer.LegendText == resources.Legend_Title).FirstOrDefault();

            if (basemapName == "None")
            {
                if (_baseMapLayer != null)
                {
                    DisableBasemapLayer();
                    if (_selectServiceBox != null)
                    {
                        _selectServiceBox.TextBoxText = "None";
                    }
                }
                return;
            }
            
            if (_selectServiceBox != null)
            {
                var dropDownItem = _selectServiceBox.DropDownItems.Where(d => d.Text.Equals(basemapName)).FirstOrDefault();
                if (dropDownItem != null)
                    EnableBasemapFetching(dropDownItem.Text, dropDownItem.Value);
            }
            else if (_selectServiceComboBox != null)
            {
                foreach (object item in _selectServiceComboBox.Items)
                {
                    MapServiceInfo msInfo = item as MapServiceInfo;
                    if (msInfo != null)
                    {
                        _selectServiceComboBox.SelectedItem = msInfo;
                        EnableBasemapFetching(msInfo.Text, msInfo.Url);
                    }
                }
            }
            
        }

        void SerializationManagerSerializing(object sender, SerializingEventArgs e)
        {
            if (_selectServiceBox != null)
            {
                _mapPluginArgs.AppManager.SerializationManager.SetCustomSetting(PluginName + "_BasemapName", _selectServiceBox.TextBoxText);
            }
            else if (_selectServiceComboBox != null)
            {
                _mapPluginArgs.AppManager.SerializationManager.SetCustomSetting(PluginName + "_BasemapName", _selectServiceComboBox.Text);
            }

            if (_opacityBox != null)
            {
                _mapPluginArgs.AppManager.SerializationManager.SetCustomSetting(PluginName + "_Opacity", _opacityBox.TextBoxText);
            }
        }

        #endregion

        #region Setup Ribbon or Menu


        private void InitializeRibbon()
        {
            //Setup the Panel and Add it to the MapView tab
            _rPanelOnlineBasemap = new RibbonPanel(resources.Panel_Name)
            {
                Image = resources.AddOnlineBasemap.GetThumbnailImage(32, 32, null, IntPtr.Zero),
                ButtonMoreEnabled = false,
                ButtonMoreVisible = false
            };
            _mapPluginArgs.Ribbon.Tabs[0].Panels.Add(_rPanelOnlineBasemap);

            //--------Select Service Combo Box
            _selectServiceBox = new RibbonComboBox
            {
                TextBoxWidth = 135,
                AllowTextEdit = false,
                Text = "",
                ToolTip = resources.Service_Box_ToolTip,
                DrawIconsBar = false
            };

            //Add it to the Panel
            _rPanelOnlineBasemap.Items.Add(_selectServiceBox);

            //Create "None" Option
            var noneSvcBtn = new RibbonButton
            {
                Text = resources.None,
                ToolTip = resources.None,
                Style = RibbonButtonStyle.Normal
            };
            noneSvcBtn.Click += NoneSvcBtnClick;
            _selectServiceBox.DropDownItems.Add(noneSvcBtn);
            _selectServiceBox.TextBoxText = resources.None;

            foreach (var item in Services.Default.List)
            {
                var serviceDescArr = item.Split(',');

                var serviceName = serviceDescArr[0];
                var serviceUrl = serviceDescArr[1];

                var svcBtn = new RibbonButton
                {
                    Text = serviceName,
                    Value = serviceUrl,
                    Style = RibbonButtonStyle.Normal
                };
                //svcBtn.Style = RibbonButtonStyle.DropDownListItem;

                svcBtn.Click += ServiceNameClick;

                _selectServiceBox.DropDownItems.Add(svcBtn);
            }

            //-------Opacity Combo Box
            _opacityBox = new RibbonComboBox
            {
                AllowTextEdit = true,
                Text = resources.Opacity_Box_Text,
                TextBoxWidth = 40,
                TextBoxText = "100",
                ToolTip = resources.Opacity_Box_ToolTip,
                DrawIconsBar = false
            };

            //Make some opacity settings
            for (var i = 100; i > 0; i -= 10)
            {
                var rb = new RibbonButton(i.ToString()) { Style = RibbonButtonStyle.Normal };
                _opacityBox.DropDownItems.Add(rb);
            }

            _opacityBox.TextBoxTextChanged += OpacityBoxTextBoxTextChanged;

            //Add it to the Panel
            _rPanelOnlineBasemap.Items.Add(_opacityBox);
        }

        private void InitializeMenuItems()
        {
            //--------Toolstrip
            _separator = new ToolStripSeparator();

            //--------Select Service Combo Box
            _selectServiceComboBox = new ToolStripComboBox
            {
                Width = 135,
                Text = "",
            };
            


            //Create "None" Option
            ArrayList serviceList = new ArrayList();
            _selectServiceComboBox.Items.Add(resources.None);

            foreach (var item in Services.Default.List)
            {
                var serviceDescArr = item.Split(',');

                var serviceName = serviceDescArr[0];
                var serviceUrl = serviceDescArr[1];

                var svcItem = new MapServiceInfo(serviceName, serviceUrl);
                _selectServiceComboBox.Items.Add(svcItem);               
            }
            _mapPluginArgs.MainToolStrip.Items.Add(_selectServiceComboBox);

            _selectServiceComboBox.SelectedIndexChanged += new EventHandler(_selectServiceComboBox_SelectedIndexChanged);

            ////-------Opacity Menu Item
            //ToolStripMenuItem opacityMenu = new ToolStripMenuItem(resources.Opacity_Box_Text);

            ////Make some opacity settings
            //for (var i = 100; i > 0; i -= 10)
            //{
            //    var opacityItem = new ToolStripMenuItem();
            //    opacityItem.Text = i.ToString();
            //    opacityItem.CheckOnClick = true;
            //    opacityMenu.DropDownItems.Add(opacityItem);
            //}
            //((ToolStripMenuItem)opacityMenu.DropDownItems[0]).Checked = true;

            //opacityMenu.DropDownItemClicked += OpacityBoxTextBoxTextChanged;

            //onlineBasemapMenu.DropDownItems.Add(opacityMenu);
        }

        void _selectServiceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectServiceComboBox.Text == resources.None)
            {
                DisableBasemapLayer();
            }
            else
            {
                var item = _selectServiceComboBox.SelectedItem as MapServiceInfo;
                if (item != null)
                {
                    EnableBasemapFetching(item.Text, item.Url);
                }
            }
        }


        #endregion

        #region Event Handlers


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoneSvcBtnClick(object sender, EventArgs e)
        {
            DisableBasemapLayer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpacityBoxTextBoxTextChanged(object sender, EventArgs e)
        {
            if (_baseMapLayer == null)
                return;

            Int16 opacityInt;

            if (_opacityBox != null)
            {

                //Check to make sure the text in the box is an integer
                if (!Int16.TryParse(_opacityBox.TextBoxText, out opacityInt))
                {
                    opacityInt = 100;
                    _opacityBox.TextBoxText = "100";
                }

                //Check to make sure we are in the range
                if (opacityInt > 100 || opacityInt < 0)
                {
                    _opacityBox.TextBoxText = "100";
                }

            }
            
            ChangeBasemapOpacity();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseMapLayerRemoveItem(object sender, EventArgs e)
        {
            _baseMapLayer = null;
            _selectServiceBox.TextBoxText = resources.None;
            _mapPluginArgs.Map.MapFrame.ViewExtentsChanged -= MapFrameExtentsChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceNameClick(object sender, EventArgs e)
        {
            var item = sender as RibbonButton;
            if (item != null)
            {
                EnableBasemapFetching(item.Text, item.Value);
            }
        }


        private void EnableBasemapFetching(string tileServerName, string tileServerUrl)
        {
            EnableBasemapLayer();

            if (_selectServiceBox != null)
            {
                _selectServiceBox.TextBoxText = tileServerName;
            }

            _tileManager.ChangeService(tileServerName, tileServerUrl);
            

            if (_bw.IsBusy != true)
            {
                _bw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapFrameExtentsChanged(object sender, ExtentArgs e)
        {
            if (_bw.IsBusy != true)
            {
                _bw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BwDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            if (worker != null)
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    worker.ReportProgress(50);
                    UpdateStichedBasemap();
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var map = _mapPluginArgs.Map as Map;
            if (map != null) map.MapFrame.Invalidate();
            _mapPluginArgs.ProgressHandler.Progress(String.Empty, 0, String.Empty);
        }

        private void BwProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _mapPluginArgs.ProgressHandler.Progress("Loading Basemap ...", 50, "Loading Basemap ...");
        }

        #endregion

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            if (_mapPluginArgs.Ribbon != null)
            {
                _mapPluginArgs.Ribbon.Tabs[0].Panels.Remove(_rPanelOnlineBasemap);
            }

            if (_mapPluginArgs.MainToolStrip != null)
            {
                _mapPluginArgs.MainToolStrip.Items.Remove(_selectServiceComboBox);
                _mapPluginArgs.MainToolStrip.Items.Remove(_separator);
            }

            RemoveBasemapLayer();

            _baseMapLayer = null;
            _originalBasemapImage = null;

            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }

        /// <summary>
        /// Main method of this plugin: gets the tiles from the TileManager, stitches them together, and adds the layer to the map.
        /// </summary>
        private void UpdateStichedBasemap()
        {
            var map = _mapPluginArgs.Map as Map;
            if (map != null)
            {              
                var rectangle = map.Bounds;
                var webMercExtent = map.ViewExtents;

                //Clip the reported Web Merc Envelope to be within possible Web Merc extents
                //  This fixes an issue with Reproject returning bad results for very large (impossible) web merc extents reported from the Map
                var webMercTopLeftX = TileCalculator.Clip(webMercExtent.MinX, TileCalculator.MinWebMercX,
                                                             TileCalculator.MaxWebMercX);
                var webMercTopLeftY = TileCalculator.Clip(webMercExtent.MaxY, TileCalculator.MinWebMercY,
                                                             TileCalculator.MaxWebMercY);
                var webMercBtmRightX = TileCalculator.Clip(webMercExtent.MaxX, TileCalculator.MinWebMercX,
                                                              TileCalculator.MaxWebMercX);
                var webMercBtmRightY = TileCalculator.Clip(webMercExtent.MinY, TileCalculator.MinWebMercY,
                                                              TileCalculator.MaxWebMercY);

                var world = new World();
                var projWorld = new DotSpatial.Projections.ProjectedCategories.World();
                var wgs1984 = new ProjectionInfo();
                wgs1984.ReadEsriString(Properties.Resources.wgs84esriString);

                //Get the web mercator vertices of the current map view
                var mapVertices = new[] {webMercTopLeftX, webMercTopLeftY, webMercBtmRightX, webMercBtmRightY};

                double[] z = {0, 0};

                //Reproject from web mercator to WGS1984 geographic
                Reproject.ReprojectPoints(mapVertices, z, projWorld.WebMercator, wgs1984, 0, mapVertices.Length/2);
                var geogEnv = new Envelope(mapVertices[0], mapVertices[2], mapVertices[1], mapVertices[3]);

                //Grab the tiles
                var tiles = _tileManager.GetTiles(geogEnv, rectangle);

                //Stitch them into a single image
                var stitchedBasemap = TileCalculator.StitchTiles(tiles);

                _originalBasemapImage = stitchedBasemap;

                int opacity = 100;
                if (_opacityBox != null)
                {
                    opacity = Int16.Parse(_opacityBox.TextBoxText);
                }

                stitchedBasemap = GetTransparentBasemapImage(stitchedBasemap, opacity);

                var tileImage = new InRamImageData(stitchedBasemap);

                //Tiles will have often slightly different bounds from what we are displaying on screen
                // so we need to get the top left and bottom right tiles' bounds to get the proper extent
                // of the tiled image
                var topLeftTile = tiles[0, 0];
                var bottomRightTile = tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1];

                var tileVertices = new[]
                                       {
                                           topLeftTile.Envelope.TopLeft().X, topLeftTile.Envelope.TopLeft().Y,
                                           bottomRightTile.Envelope.BottomRight().X,
                                           bottomRightTile.Envelope.BottomRight().Y
                                       };

                //Reproject from WGS1984 geographic coordinates to web mercator so we can show on the map
                Reproject.ReprojectPoints(tileVertices, z, wgs1984, projWorld.WebMercator, 0, tileVertices.Length/2);

                //tileImage.Bounds = new RasterBounds(stitchedBasemap.Height, stitchedBasemap.Width, 
                //    new Extent(tileVertices[0], tileVertices[1], tileVertices[2], tileVertices[3]));

                tileImage.Bounds = new RasterBounds(stitchedBasemap.Height, stitchedBasemap.Width,
                                                    new Extent(tileVertices[0], tileVertices[3], tileVertices[2],
                                                               tileVertices[1]));

                _baseMapLayer.Image = tileImage;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void EnableBasemapLayer()
        {
            if (_baseMapLayer == null)
            {
                //Need to first initialize and add the basemap layer synchronously (it will fail if done in 
                // another thread.

                //First create a temporary imageData with an Envelope (otherwise adding to the map will fail)
                var tempImageData = new InRamImageData(resources.NoDataTile, new Extent(1, 1, 2, 2));

                _baseMapLayer = new MapImageLayer(tempImageData)
                                    {
                                        Projection = _mapPluginArgs.Map.Projection,
                                        LegendText = resources.Legend_Title
                                    };

                _baseMapLayer.RemoveItem += BaseMapLayerRemoveItem;

                AddBasemapLayerToMap();
            }

            _mapPluginArgs.Map.MapFrame.ViewExtentsChanged += MapFrameExtentsChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisableBasemapLayer()
        {
            RemoveBasemapLayer();

            _baseMapLayer = null;
            _originalBasemapImage = null;

            if (_mapPluginArgs != null)
                _mapPluginArgs.Map.MapFrame.ViewExtentsChanged -= MapFrameExtentsChanged;

        }

        /// <summary>
        /// Inserts the _baseMapLayer below the "Themes" group
        /// </summary>
        private void AddBasemapLayerToMap()
        {
            //check all top-level map layers or groups

            for (var i = 0; i < _mapPluginArgs.Map.Layers.Count; i++)
            {
                //test if there is a 'group' in the map layers
                var grp = _mapPluginArgs.Map.Layers[i] as IMapGroup;
                    
                //test if the type of the map layer is 'Group'
                if (grp != null && grp.LegendText == "Themes")
                {
                    var viewExtents = _mapPluginArgs.Map.ViewExtents;
                    
                    _mapPluginArgs.Map.Layers.Insert(i, _baseMapLayer);
                    
                    _mapPluginArgs.Map.ViewExtents = viewExtents;
                    return;
                }
            }
            //otherwise, no 'Themes' group is found.
            //insert the basemap as the topmost layer.
            _mapPluginArgs.Map.Layers.Add(_baseMapLayer);
            
        }

        /// <summary>
        /// Finds and removes the online basemap layer
        /// </summary>
        private void RemoveBasemapLayer()
        {
            //attempt to remove from the top-level
            if (_mapPluginArgs.Map.Layers.Remove(_baseMapLayer)) return;
            
            //Remove from other groups if the user has moved it
            _mapPluginArgs.Map.Layers.OfType<IMapGroup>().Any(grp => grp.Remove(_baseMapLayer));
        }




        /// <summary>
        /// Returns the input bitmap after making it transparent
        /// </summary>
        /// <returns>Opacity-modified bitmap of the basemap image</returns>
        private static Bitmap GetTransparentBasemapImage(Bitmap originalImage, int opacity)
        {
            if (originalImage == null)
                return null;

            try
            {
                var newImage = new Bitmap(originalImage.Width, originalImage.Height);
                var gfxPic = Graphics.FromImage(newImage);
                var cmxPic = new ColorMatrix
                                 {
                                     Matrix33 = opacity/100f
                                 };

                var iaPic = new ImageAttributes();
                iaPic.SetColorMatrix(cmxPic, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                gfxPic.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                                 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, iaPic);

                gfxPic.Dispose();

                return newImage;
            }
            catch
            {
                return originalImage;
            }
        }

        /// <summary>
        /// Changes the opacity of the current basemap image and invalidates the map
        /// </summary>
        private void ChangeBasemapOpacity()
        {
            int opacity = 100;
            if (_opacityBox != null)
            {
                opacity = Int16.Parse(_opacityBox.TextBoxText);
            }
            
            Bitmap newBasemapImage = GetTransparentBasemapImage(_originalBasemapImage, opacity);
            _baseMapLayer.Image.SetBitmap(newBasemapImage);
            _mapPluginArgs.Map.MapFrame.Invalidate();
        }
    }

    public class MapServiceInfo
    {
        public MapServiceInfo(string text, string url)
        {
            Text = text;
            Url = url;
        }
        public string Text;
        public string Url;

        public override string ToString()
        {
            return Text;
        }
    }


}