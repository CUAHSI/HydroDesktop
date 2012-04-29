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

namespace HydroDesktop.Main
{
    internal class NorthAmericaProjectTemplate
    {
        static DotSpatial.Projections.ProjectedCategories.World projWorld = KnownCoordinateSystems.Projected.World;
        
        /// <summary>
        /// This method is only used when opening the default project fails. The base shapefiles
        /// are loaded from the [Documents]\HydroDesktop\maps folder.
        /// </summary>
        public static Boolean LoadBaseMaps(AppManager applicationManager1, Map mainMap)
        {
            //set the projection of main map
            mainMap.Projection = projWorld.WebMercator;
            
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
                //fsCountries.Reproject(projWorld.WebMercator);
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
                layCountries.ProgressReportingEnabled = false;
                baseGroup.Layers.Add(layCountries);
                layCountries.MapFrame = mainMap.MapFrame;
            }

            //load Canada Provinces layer
            try
            {
                string fileName3 = Path.Combine(baseMapFolder, "canada_provinces.shp");
                if (File.Exists(fileName3))
                {
                    IFeatureSet fsProvince = FeatureSet.OpenFile(fileName3);
                    //fsProvince.Reproject(projWorld.WebMercator);
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
                    layProvince.ProgressReportingEnabled = false;
                    baseGroup.Layers.Add(layProvince);
                    layProvince.MapFrame = mainMap.MapFrame;
                }
            }
            catch { }
            
            //load U.S. states layer         
            string fileName2 = Path.Combine(baseMapFolder, "us_states.shp");
            if (File.Exists(fileName2))
            {
                IFeatureSet fsStates = FeatureSet.OpenFile(fileName2);
                //fsStates.Reproject(projWorld.WebMercator);
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
                layStates.ProgressReportingEnabled = false;
                baseGroup.Layers.Add(layStates);
                layStates.MapFrame = mainMap.MapFrame;
            }    

            //load a U.S. counties layer
            try
            {
                string fileName4 = Path.Combine(baseMapFolder, "us_counties.shp");
                if (File.Exists(fileName4))
                {
                    IFeatureSet fsCounties = FeatureSet.OpenFile(fileName4);
                    //fsCounties.Reproject(projWorld.WebMercator);
                    MapPolygonLayer layCounties = new MapPolygonLayer(fsCounties);
                    layCounties.LegendText = "U.S. Counties";
                    layCounties.IsVisible = false;
                    layCounties.Symbolizer = new PolygonSymbolizer(Color.FromArgb(120, Color.Beige), Color.LightGray);
                    layCounties.ProgressReportingEnabled = false;
                    baseGroup.Layers.Add(layCounties);
                    layCounties.MapFrame = mainMap.MapFrame;
                }
                
            }
            catch { }

            //load a U.S. HUC layer
            try
            {
                string fileName5 = Path.Combine(baseMapFolder, "us_huc.shp");
                if (File.Exists(fileName5))
                {
                    IFeatureSet fsHUC = FeatureSet.OpenFile(fileName5);
                    //fsHUC.Reproject(projWorld.WebMercator);
                    MapPolygonLayer layHUC = new MapPolygonLayer(fsHUC);
                    layHUC.LegendText = "U.S. HUC";
                    layHUC.IsVisible = false;
                    layHUC.Symbolizer = new PolygonSymbolizer(Color.FromArgb(100, Color.PaleGreen), Color.Gray);
                    layHUC.ProgressReportingEnabled = false;
                    baseGroup.Layers.Add(layHUC);
                    layHUC.MapFrame = mainMap.MapFrame;
                }
            }
            catch { }

            ////load a rivers layer
            //try
            //{
            //    string fileName6 = Path.Combine(baseMapFolder, "world_rivers.shp");
            //    if (File.Exists(fileName6))
            //    {
            //        IFeatureSet fsRivers = FeatureSet.OpenFile(fileName6);
            //        //fsRivers.Reproject(projWorld.WebMercator);
            //        MapLineLayer layRivers = new MapLineLayer(fsRivers);
            //        layRivers.LegendText = "rivers";
            //        LineSymbolizer symRivers = new LineSymbolizer(Color.Blue, 1.0);
            //        layRivers.Symbolizer = symRivers;
            //        baseGroup.Layers.Add(layRivers);
            //        layRivers.MapFrame = mainMap.MapFrame;
            //    }
            //}
            //catch { }

            ////load a lakes layer
            //try
            //{
            //    string fileName7 = Path.Combine(baseMapFolder, "world_lakes.shp");
            //    if (File.Exists(fileName7))
            //    {
            //        IFeatureSet fsLakes = FeatureSet.OpenFile(fileName7);
            //        //fsLakes.Reproject(projWorld.WebMercator);
            //        MapPolygonLayer layLakes = new MapPolygonLayer(fsLakes);
            //        layLakes.LegendText = "lakes";
            //        PolygonSymbolizer symLakes = new PolygonSymbolizer(Color.Blue,
            //            Color.Blue);
            //        layLakes.Symbolizer = symLakes;
            //        layLakes.ProgressReportingEnabled = false;
            //        baseGroup.Layers.Add(layLakes);
            //        layLakes.MapFrame = mainMap.MapFrame;                
            //    }
            //}
            //catch { }

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
            ProjectionInfo wgs84 =ProjectionInfo.FromEsriString(Properties.Resources.wgs_84_esri_string);
            Reproject.ReprojectPoints(xy, z, wgs84, projWorld.WebMercator, 0, 2);
            map.ViewExtents = new Extent(xy);
            map.MapFrame.ResetExtents();

        }
    }
}
