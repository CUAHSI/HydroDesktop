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
    internal class WorldProjectTemplate
    {
        /// <summary>
        /// Load base maps for World template project. The base shapefiles
        /// are loaded from the [Program Files]\[Cuahsi HIS]\HydroDesktop\maps\BaseData folder.
        /// </summary>
        public static Boolean LoadBaseMaps(AppManager applicationManager1, Map mainMap)
        {
            Extent defaultMapExtent = new Extent(-170, -50, 170, 50);

            string[] corePlugins = new string[]
            {
                "Search V2",
                "Table View",
                "Graph",
                "Edit",
                //"EPA Delineation",
                "Fetch Basemap",
                "Help Tab",
                "HydroR",
                "Metadata Fetcher",
                "Data Export"
            };

            string baseMapFolder = Settings.Instance.DefaultBaseMapDirectory;

            //SetDefaultMapExtents(mainMap);

            MapGroup baseGroup = new MapGroup(mainMap.Layers, mainMap.MapFrame, mainMap.ProgressHandler);
            baseGroup.LegendText = "Base Map Data";
            baseGroup.ParentMapFrame = mainMap.MapFrame;
            baseGroup.MapFrame = mainMap.MapFrame;
            baseGroup.IsVisible = true;

            //load the 'Countries of the world' layer
            try
            {
                string fileName = Path.Combine(baseMapFolder, "world_countries.shp");
                IFeatureSet fsCountries = FeatureSet.OpenFile(fileName);
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
            catch { }
            

            //load a rivers layer
            try
            {
                string fileName = Path.Combine(baseMapFolder, "world_rivers.shp");
                IFeatureSet fsRivers = FeatureSet.OpenFile(fileName);
                fsRivers.Reproject(mainMap.Projection);
                MapLineLayer layRivers = new MapLineLayer(fsRivers);
                layRivers.LegendText = "rivers";
                LineSymbolizer symRivers = new LineSymbolizer(Color.Blue, 1.0);
                layRivers.Symbolizer = symRivers;
                baseGroup.Layers.Add(layRivers);
                layRivers.MapFrame = mainMap.MapFrame;
            }
            catch { }

            //load a lakes layer
            try
            {
                string fileName = Path.Combine(baseMapFolder, "world_lakes.shp");
                IFeatureSet fsLakes = FeatureSet.OpenFile(fileName);
                fsLakes.Reproject(mainMap.Projection);
                MapPolygonLayer layLakes = new MapPolygonLayer(fsLakes);
                layLakes.LegendText = "lakes";
                PolygonSymbolizer symLakes = new PolygonSymbolizer(Color.Blue,
                    Color.Blue);
                layLakes.Symbolizer = symLakes;
                baseGroup.Layers.Add(layLakes);
                layLakes.MapFrame = mainMap.MapFrame;
            }
            catch { }

            //theme data group
            //create a new empty 'themes' data group
            MapGroup themeGroup = new MapGroup(mainMap.Layers,
                mainMap.MapFrame, mainMap.ProgressHandler);
            themeGroup.ParentMapFrame = mainMap.MapFrame;
            themeGroup.MapFrame = mainMap.MapFrame;
            themeGroup.LegendText = "Themes";

            double[] xy = new double[4];
            xy[0] = defaultMapExtent.MinX;
            xy[1] = defaultMapExtent.MinY;
            xy[2] = defaultMapExtent.MaxX;
            xy[3] = defaultMapExtent.MaxY;
            double[] z = new double[] { 0, 0 };
            ProjectionInfo wgs84 = new ProjectionInfo();
            wgs84.ReadEsriString("GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]");
            Reproject.ReprojectPoints(xy, z, wgs84, mainMap.Projection, 0, 2);

            Project.ActivatePlugins(applicationManager1, corePlugins);

            mainMap.ViewExtents = new Extent(xy);

            return true;
        }
    }
}
