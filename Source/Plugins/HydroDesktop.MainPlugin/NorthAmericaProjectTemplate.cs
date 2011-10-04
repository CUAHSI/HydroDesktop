using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using DotSpatial.Symbology;
using DotSpatial.Data;
using DotSpatial.Controls;
using HydroDesktop.Configuration;
using DotSpatial.Projections;

namespace HydroDesktop.MainPlugin
{
    internal class NorthAmericaProjectTemplate
    {
        /// <summary>
        /// This method is only used when opening the default project fails. The base shapefiles
        /// are loaded from the [Program Files]\[Cuahsi HIS]\HydroDesktop\maps\baseData-mercatorSphere folder.
        /// </summary>
        public static Boolean LoadBaseMaps(AppManager applicationManager1, Map mainMap)
        {
            //set the projection of main map
            if (mainMap.Projection == null)
            {
                mainMap.Projection = KnownCoordinateSystems.Projected.World.WebMercator;
            }
            
            string baseMapFolder = Settings.Instance.DefaultBaseMapDirectory;

            SetMapExtent(mainMap);
            MapPolygonLayer layStates;

            MapGroup baseGroup = new MapGroup(mainMap.Layers, mainMap.MapFrame, mainMap.ProgressHandler);
            baseGroup.LegendText = "Base Map Data";
            baseGroup.ParentMapFrame = mainMap.MapFrame;
            baseGroup.MapFrame = mainMap.MapFrame;
            baseGroup.IsVisible = true;

            //load the 'Countries of the world' layer
            
            string fileName1 = Path.Combine(baseMapFolder, "world_countries.shp");
            if (File.Exists(fileName1))
            {
                IFeatureSet fsCountries = FeatureSet.OpenFile(fileName1);
                fsCountries.Reproject(mainMap.Projection);
                MapPolygonLayer layCountries = new MapPolygonLayer(fsCountries);
                layCountries.LegendText = "Countries";
                PolygonScheme schmCountries = new PolygonScheme();
                schmCountries.EditorSettings.StartColor = Color.Orange;
                schmCountries.EditorSettings.EndColor = Color.Silver;
                schmCountries.EditorSettings.ClassificationType =
                    ClassificationType.UniqueValues;
                schmCountries.EditorSettings.FieldName = "NAME";
                schmCountries.EditorSettings.UseGradient = true;
                schmCountries.CreateCategories(layCountries.DataSet.DataTable);
                layCountries.Symbology = schmCountries;
                baseGroup.Layers.Add(layCountries);
                layCountries.MapFrame = mainMap.MapFrame;
            }
            
            //load U.S. states layer         
            string fileName2 = Path.Combine(baseMapFolder, "us_states.shp");
            if (File.Exists(fileName2))
            {
                IFeatureSet fsStates = FeatureSet.OpenFile(fileName2);
                fsStates.Reproject(mainMap.Projection);
                layStates = new MapPolygonLayer(fsStates);
                PolygonScheme schmStates = new PolygonScheme();
                layStates.IsVisible = true;
                layStates.LegendText = "U.S. States";
                schmStates.EditorSettings.StartColor = Color.LemonChiffon;
                schmStates.EditorSettings.EndColor = Color.LightPink;
                schmStates.EditorSettings.ClassificationType =
                    ClassificationType.UniqueValues;
                schmStates.EditorSettings.FieldName = "NAME";
                schmStates.EditorSettings.UseGradient = true;
                schmStates.CreateCategories(layStates.DataSet.DataTable);
                layStates.Symbology = schmStates;
                baseGroup.Layers.Add(layStates);
                layStates.MapFrame = mainMap.MapFrame;
            }

            //load Canada Provinces layer
            try
            {
                string fileName3 = Path.Combine(baseMapFolder, "canada_provinces.shp");
                if (File.Exists(fileName3))
                {
                    IFeatureSet fsProvince = FeatureSet.OpenFile(fileName3);
                    fsProvince.Reproject(mainMap.Projection);
                    MapPolygonLayer layProvince = new MapPolygonLayer(fsProvince);
                    PolygonScheme schmProvince = new PolygonScheme();
                    layProvince.IsVisible = true;
                    layProvince.LegendText = "Canada Provinces";
                    schmProvince.EditorSettings.StartColor = Color.Green;
                    schmProvince.EditorSettings.EndColor = Color.Yellow;
                    schmProvince.EditorSettings.ClassificationType =
                        ClassificationType.UniqueValues;
                    schmProvince.EditorSettings.FieldName = "NAME";
                    schmProvince.EditorSettings.UseGradient = true;
                    schmProvince.CreateCategories(layProvince.DataSet.DataTable);
                    layProvince.Symbology = schmProvince;
                    baseGroup.Layers.Add(layProvince);
                    layProvince.MapFrame = mainMap.MapFrame;
                }
            }
            catch { }

            //load a U.S. counties layer
            try
            {
                string fileName4 = Path.Combine(baseMapFolder, "us_counties.shp");
                if (File.Exists(fileName4))
                {
                    IFeatureSet fsCounties = FeatureSet.OpenFile(fileName4);
                    fsCounties.Reproject(mainMap.Projection);
                    MapPolygonLayer layCounties = new MapPolygonLayer(fsCounties);
                    layCounties.LegendText = "U.S. Counties";
                    layCounties.IsVisible = false;
                    layCounties.Symbolizer = new PolygonSymbolizer(Color.FromArgb(120, Color.Beige), Color.LightGray);
                    baseGroup.Layers.Add(layCounties);
                    layCounties.MapFrame = mainMap.MapFrame;
                }
                //PolygonScheme schmCounties = new PolygonScheme();

                //schmCounties.EditorSettings.StartColor = Color.LemonChiffon;
                //schmCounties.EditorSettings.EndColor = Color.LightPink;
                //schmCounties.EditorSettings.ClassificationType =
                //    ClassificationType.UniqueValues;
                //schmCounties.EditorSettings.FieldName = "NAME";
                //schmCounties.EditorSettings.UseGradient = true;
                //schmCounties.CreateCategories(layCounties.DataSet.DataTable);
                //layCounties.Symbology = schmCounties;

                //layCounties.Symbolizer.SetFillColor(Color.Transparent);
                //layCounties.DynamicVisibilityWidth = 1000000; //approximately 1:1Million
                //layCounties.DynamicVisibilityMode = DynamicVisibilityMode.ZoomedIn;
                //layCounties.UseDynamicVisibility = true;
            }
            catch { }

            //load a U.S. HUC layer
            try
            {
                string fileName5 = Path.Combine(baseMapFolder, "us_huc.shp");
                if (File.Exists(fileName5))
                {
                    IFeatureSet fsHUC = FeatureSet.OpenFile(fileName5);
                    fsHUC.Reproject(mainMap.Projection);
                    MapPolygonLayer layHUC = new MapPolygonLayer(fsHUC);
                    layHUC.LegendText = "U.S. HUC";
                    layHUC.IsVisible = false;
                    layHUC.Symbolizer = new PolygonSymbolizer(Color.FromArgb(100, Color.PaleGreen), Color.Gray);
                    //PolygonScheme schmHUC = new PolygonScheme();
                    //layHUC.IsVisible = false;
                    //layHUC.LegendText = "HUC";
                    //schmHUC.EditorSettings.StartColor = Color.PaleGreen;
                    //schmHUC.EditorSettings.EndColor = Color.OrangeRed;
                    //schmHUC.EditorSettings.ClassificationType =
                    //    ClassificationType.UniqueValues;
                    //schmHUC.EditorSettings.FieldName = "HUC";
                    //schmHUC.EditorSettings.UseGradient = true;
                    //schmHUC.CreateCategories(layHUC.DataSet.DataTable);
                    //layHUC.Symbology = schmHUC;
                    baseGroup.Layers.Add(layHUC);
                    layHUC.MapFrame = mainMap.MapFrame;
                }
            }
            catch { }

            //load a rivers layer
            try
            {
                string fileName6 = Path.Combine(baseMapFolder, "world_rivers.shp");
                if (File.Exists(fileName6))
                {
                    IFeatureSet fsRivers = FeatureSet.OpenFile(fileName6);
                    fsRivers.Reproject(mainMap.Projection);
                    MapLineLayer layRivers = new MapLineLayer(fsRivers);
                    layRivers.LegendText = "rivers";
                    LineSymbolizer symRivers = new LineSymbolizer(Color.Blue, 1.0);
                    layRivers.Symbolizer = symRivers;
                    baseGroup.Layers.Add(layRivers);
                    layRivers.MapFrame = mainMap.MapFrame;
                }
            }
            catch { }

            //load a lakes layer
            try
            {
                string fileName7 = Path.Combine(baseMapFolder, "world_lakes.shp");
                if (File.Exists(fileName7))
                {
                    IFeatureSet fsLakes = FeatureSet.OpenFile(fileName7);
                    fsLakes.Reproject(mainMap.Projection);
                    MapPolygonLayer layLakes = new MapPolygonLayer(fsLakes);
                    layLakes.LegendText = "lakes";
                    PolygonSymbolizer symLakes = new PolygonSymbolizer(Color.Blue,
                        Color.Blue);
                    layLakes.Symbolizer = symLakes;
                    baseGroup.Layers.Add(layLakes);
                    layLakes.MapFrame = mainMap.MapFrame;
                }
            }
            catch { }

            //this is now done by MEF...
            //Project.ActivatePlugins(applicationManager1, _corePlugins);

            SetMapExtent(mainMap);
            
            return true;
        }

        private static void SetMapExtent(Map map)
        {
            Extent defaultMapExtent = new Extent(-130, 5, -70, 60);
            
            double[] xy = new double[4];
            xy[0] = defaultMapExtent.MinX;
            xy[1] = defaultMapExtent.MinY;
            xy[2] = defaultMapExtent.MaxX;
            xy[3] = defaultMapExtent.MaxY;
            double[] z = new double[] { 0, 0 };
            ProjectionInfo wgs84 = new ProjectionInfo();
            wgs84.ReadEsriString("GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]");
            Reproject.ReprojectPoints(xy, z, wgs84, map.Projection, 0, 2);
            map.ViewExtents = new Extent(xy);

        }
    }
}
