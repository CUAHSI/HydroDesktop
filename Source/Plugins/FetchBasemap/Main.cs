using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Controls.RibbonControls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Projections.GeographicCategories;
using DotSpatial.Topology;
using FetchBasemap.Resources;
using FetchBasemap.Tiling;
using MWPoint = DotSpatial.Topology.Point;

namespace FetchBasemap
{
    public class Main : Extension
    {
        private const string STR_KeyServiceDropDown = "kServiceDropDown";
        private const string STR_KeyOpacityDropDown = "kOpacityDropDown";

        private DropDownActionItem _serviceDropDown;
        private DropDownActionItem _opacityDropDown;

        public const string HomeTabKey = "kHome";

        #region Variables

        //reference to the main application and its UI items
        private MapImageLayer _baseMapLayer;
        private BackgroundWorker _bw;

        private Bitmap _originalBasemapImage;
        private TileManager _tileManager;

        private ServiceProvider _provider;
        private ServiceProvider _emptyProvider;
        private string _defaultOpacity;
        private Int16 _opacity = 100;
        private const string PluginName = "FetchBasemap";

        #endregion Variables

        #region IMapPlugin Members

        /// <summary>
        /// Initialize the DotSpatial plugin
        /// </summary>
        public override void Activate()
        {
            // Add Menu or Ribbon buttons.
            AddButtons();

            //Create the tile manager
            _tileManager = new TileManager();

            //Add handlers for saving/restoring settings
            App.SerializationManager.Serializing += SerializationManagerSerializing;
            App.SerializationManager.Deserializing += SerializationManagerDeserializing;

            //Setup the background worker
            _bw = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bw.DoWork += BwDoWork;
            _bw.RunWorkerCompleted += BwRunWorkerCompleted;
            _bw.ProgressChanged += new ProgressChangedEventHandler(BwProgressChanged);

            base.Activate();
        }

        private void SerializationManagerDeserializing(object sender, SerializingEventArgs e)
        {
            var opacity =
               App.SerializationManager.GetCustomSetting(PluginName + "_Opacity",
                                                                                       "100");
            var basemapName =
                App.SerializationManager.GetCustomSetting(PluginName + "_BasemapName",
                                                                                        resources.None);
            //Set opacity
            _opacityDropDown.SelectedItem = opacity;


            _baseMapLayer = (MapImageLayer)App.Map.MapFrame.GetAllLayers().Where(
                layer => layer.LegendText == resources.Legend_Title).FirstOrDefault();

            if (basemapName == "None")
            {
                if (_baseMapLayer != null)
                {
                    DisableBasemapLayer();
                    _provider = _emptyProvider;
                    _serviceDropDown.SelectedItem = _provider;
                }
            }
            else
            {
                //hack: need to set provider to original object, not a new one.
                _provider = ServiceProvider.GetDefaultServiceProviders().Where(p => p.Name == basemapName).FirstOrDefault();
                _serviceDropDown.SelectedItem = _provider;
                EnableBasemapFetching(_provider.Name, _provider.Url);
            }
        }

        private void SerializationManagerSerializing(object sender, SerializingEventArgs e)
        {
            App.SerializationManager.SetCustomSetting(PluginName + "_BasemapName", _provider.Name);
            App.SerializationManager.SetCustomSetting(PluginName + "_Opacity", _opacity.ToString());
        }

        #endregion IMapPlugin Members

        #region Setup Ribbon or Menu

        private void AddButtons()
        {
            if (App.HeaderControl == null)
                return;

            var header = App.HeaderControl;
            AddServiceDropDown(header);
            AddOpaticyDropDown(header);
        }

        private void AddServiceDropDown(IHeaderControl header)
        {
            _serviceDropDown = new DropDownActionItem();
            _serviceDropDown.Key = STR_KeyServiceDropDown;

            //Create "None" Option
            _emptyProvider = new ServiceProvider(resources.None, null);
            _serviceDropDown.Items.Add(_emptyProvider);

            // no option presently for group image.
            // Image = resources.AddOnlineBasemap.GetThumbnailImage(32, 32, null, IntPtr.Zero),

            _serviceDropDown.Width = 145;
            _serviceDropDown.AllowEditingText = false;
            _serviceDropDown.ToolTipText = resources.Service_Box_ToolTip;
            _serviceDropDown.SelectedValueChanged += ServiceSelected;
            _serviceDropDown.GroupCaption = resources.Panel_Name;
            _serviceDropDown.Items.AddRange(ServiceProvider.GetDefaultServiceProviders());
            _serviceDropDown.RootKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;

            //Add it to the Header
            header.Add(_serviceDropDown);
         
            _serviceDropDown.SelectedItem = _emptyProvider;
        }
        private void AddOpaticyDropDown(IHeaderControl header)
        {
            _opacityDropDown = new DropDownActionItem()
                        {
                            AllowEditingText = true,
                            Caption = resources.Opacity_Box_Text,
                            ToolTipText = resources.Opacity_Box_ToolTip,
                            Width = 40,
                            Key = STR_KeyOpacityDropDown
                        };

            //Make some opacity settings
            for (var i = 100; i > 0; i -= 10)
            {
                string opacity = i.ToString();
                if (i == 100)
                {
                    _defaultOpacity = opacity;
                }
                _opacityDropDown.Items.Add(opacity);

            }

            _opacityDropDown.GroupCaption = resources.Panel_Name;
            _opacityDropDown.SelectedValueChanged += OpacitySelected;
            _opacityDropDown.RootKey = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;

            //Add it to the Header
            header.Add(_opacityDropDown);
            _opacityDropDown.SelectedItem = _defaultOpacity;
        }
        #endregion Setup Ribbon or Menu

        #region Event Handlers

        private void OpacitySelected(object sender, SelectedValueChangedEventArgs e)
        {
            if (_baseMapLayer == null)
                return;

            Int16 opacityInt;

            //Check to make sure the text in the box is an integer and we are in the range
            if (!Int16.TryParse(e.SelectedItem as string, out opacityInt) || opacityInt > 100 || opacityInt < 0)
            {
                opacityInt = 100;
                _opacityDropDown.SelectedItem = opacityInt;
            }
            _opacity = opacityInt;
            ChangeBasemapOpacity(opacityInt);
        }

        private void ServiceSelected(object sender, SelectedValueChangedEventArgs e)
        {
            _provider = e.SelectedItem as ServiceProvider;

            if (_provider.Name == resources.None)
                DisableBasemapLayer();
            else
                EnableBasemapFetching(_provider.Name, _provider.Url);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseMapLayerRemoveItem(object sender, EventArgs e)
        {
            _baseMapLayer = null;
            _serviceDropDown.SelectedItem = _emptyProvider;
            App.Map.MapFrame.ViewExtentsChanged -= MapFrameExtentsChanged;
        }

        private void EnableBasemapFetching(string tileServerName, string tileServerUrl)
        {
            EnableBasemapLayer();

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
            var map = App.Map as Map;
            if (map != null) map.MapFrame.Invalidate();
            App.ProgressHandler.Progress(String.Empty, 0, String.Empty);
        }

        private void BwProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            App.ProgressHandler.Progress("Loading Basemap ...", 50, "Loading Basemap ...");
        }

        #endregion Event Handlers

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            if (App.HeaderControl != null)
            {
                App.HeaderControl.RemoveItems();
            }

            RemoveBasemapLayer();

            _baseMapLayer = null;
            _originalBasemapImage = null;

            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }

        /// <summary>
        /// Main method of this plugin: gets the tiles from the TileManager, stitches them together, and adds the layer to the map.
        /// </summary>
        private void UpdateStichedBasemap()
        {
            var map = App.Map as Map;
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
                var mapVertices = new[] { webMercTopLeftX, webMercTopLeftY, webMercBtmRightX, webMercBtmRightY };

                double[] z = { 0, 0 };

                //Reproject from web mercator to WGS1984 geographic
                Reproject.ReprojectPoints(mapVertices, z, projWorld.WebMercator, wgs1984, 0, mapVertices.Length / 2);
                var geogEnv = new Envelope(mapVertices[0], mapVertices[2], mapVertices[1], mapVertices[3]);

                //Grab the tiles
                var tiles = _tileManager.GetTiles(geogEnv, rectangle);

                //Stitch them into a single image
                var stitchedBasemap = TileCalculator.StitchTiles(tiles);

                _originalBasemapImage = stitchedBasemap;

                stitchedBasemap = GetTransparentBasemapImage(stitchedBasemap, _opacity);

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
                Reproject.ReprojectPoints(tileVertices, z, wgs1984, projWorld.WebMercator, 0, tileVertices.Length / 2);

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
                    Projection = App.Map.Projection,
                    LegendText = resources.Legend_Title
                };

                _baseMapLayer.RemoveItem += BaseMapLayerRemoveItem;

                AddBasemapLayerToMap();
            }

            App.Map.MapFrame.ViewExtentsChanged += MapFrameExtentsChanged;
        }

        /// <summary>
        ///
        /// </summary>
        private void DisableBasemapLayer()
        {
            RemoveBasemapLayer();

            _baseMapLayer = null;
            _originalBasemapImage = null;

            App.Map.MapFrame.ViewExtentsChanged -= MapFrameExtentsChanged;
        }

        /// <summary>
        /// Inserts the _baseMapLayer below the "Themes" group
        /// </summary>
        private void AddBasemapLayerToMap()
        {
            //check all top-level map layers or groups

            for (var i = 0; i < App.Map.Layers.Count; i++)
            {
                //test if there is a 'group' in the map layers
                var grp = App.Map.Layers[i] as IMapGroup;

                //test if the type of the map layer is 'Group'
                if (grp != null && grp.LegendText == "Data Sites")
                {
                    var viewExtents = App.Map.ViewExtents;

                    App.Map.Layers.Insert(i, _baseMapLayer);

                    App.Map.ViewExtents = viewExtents;
                    return;
                }
            }
            //otherwise, no 'Themes' group is found.
            //insert the basemap as the topmost layer.
            App.Map.Layers.Add(_baseMapLayer);
        }

        /// <summary>
        /// Finds and removes the online basemap layer
        /// </summary>
        private void RemoveBasemapLayer()
        {
            //attempt to remove from the top-level
            if (App.Map.Layers.Remove(_baseMapLayer)) return;

            //Remove from other groups if the user has moved it
            App.Map.Layers.OfType<IMapGroup>().Any(grp => grp.Remove(_baseMapLayer));
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
                    Matrix33 = opacity / 100f
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
        /// <param name="opacity">The opacity as a value between 0 and 100, inclusive.</param>
        private void ChangeBasemapOpacity(short opacity)
        {
            Bitmap newBasemapImage = GetTransparentBasemapImage(_originalBasemapImage, opacity);
            _baseMapLayer.Image.SetBitmap(newBasemapImage);
            App.Map.MapFrame.Invalidate();
        }
    }
}