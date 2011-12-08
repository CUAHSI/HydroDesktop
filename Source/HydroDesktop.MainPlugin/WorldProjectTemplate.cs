using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using DotSpatial.Symbology;
using DotSpatial.Data;
using DotSpatial.Controls;
using HydroDesktop.Configuration;
using DotSpatial.Projections;

namespace HydroDesktop.Main
{
    internal class WorldProjectTemplate
    {
        static DotSpatial.Projections.ProjectedCategories.World projWorld = KnownCoordinateSystems.Projected.World;
        
        /// <summary>
        /// Load base maps for World template project. The base shapefiles
        /// are loaded from the [Program Files]\[Cuahsi HIS]\HydroDesktop\maps\BaseData folder.
        /// </summary>
        public static Boolean LoadBaseMaps(AppManager applicationManager1, Map mainMap)
        {
            //set the projection of main map
            mainMap.Projection = projWorld.WebMercator;
            
            Extent defaultMapExtent = new Extent(-170, -50, 170, 50);

            string baseMapFolder = Settings.Instance.DefaultBaseMapDirectory;

            MapGroup baseGroup = new MapGroup(mainMap.Layers, mainMap.MapFrame, mainMap.ProgressHandler);
            baseGroup.LegendText = "Base Map Data";
            baseGroup.ParentMapFrame = mainMap.MapFrame;
            baseGroup.MapFrame = mainMap.MapFrame;
            baseGroup.IsVisible = true;

            //load the 'Countries of the world' layer
            try
            {
                string fileName = Path.Combine(baseMapFolder, "world_countries.shp");
                if (File.Exists(fileName))
                {
                    IFeatureSet fsCountries = FeatureSet.OpenFile(fileName);
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
                    layCountries.ProgressReportingEnabled = false;
                }
            }
            catch { }
            

            //load a rivers layer
            try
            {
                var fileName = Path.Combine(baseMapFolder, "world_rivers.shp");
                if (File.Exists(fileName))
                {
                    IFeatureSet fsRivers = FeatureSet.OpenFile(fileName);
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
                var fileName = Path.Combine(baseMapFolder, "world_lakes.shp");
                if (File.Exists(fileName))
                {
                    IFeatureSet fsLakes = FeatureSet.OpenFile(fileName);
                    fsLakes.Reproject(mainMap.Projection);
                    MapPolygonLayer layLakes = new MapPolygonLayer(fsLakes);
                    layLakes.LegendText = "lakes";
                    PolygonSymbolizer symLakes = new PolygonSymbolizer(Color.Blue,
                                                                       Color.Blue);
                    layLakes.Symbolizer = symLakes;
                    baseGroup.Layers.Add(layLakes);
                    layLakes.MapFrame = mainMap.MapFrame;
                    layLakes.ProgressReportingEnabled = false;
                }
            }
            catch { }

            double[] xy = new double[4];
            xy[0] = defaultMapExtent.MinX;
            xy[1] = defaultMapExtent.MinY;
            xy[2] = defaultMapExtent.MaxX;
            xy[3] = defaultMapExtent.MaxY;
            double[] z = new double[] { 0, 0 };
            var wgs84 = ProjectionInfo.FromEsriString(Properties.Resources.wgs_84_esri_string);
            Reproject.ReprojectPoints(xy, z, wgs84, mainMap.Projection, 0, 2);

            mainMap.ViewExtents = new Extent(xy);

            return true;
        }
    }
}
