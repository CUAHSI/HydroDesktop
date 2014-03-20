using System.ComponentModel;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Used to populate the SampleMedium field in the Variables table
    /// </summary>
    /// <remarks>
    /// See http://his.cuahsi.org/mastercvreg/edit_cv11.aspx?tbl=SampleMediumCV&id=533576939
    /// </remarks>
    public enum  SampleMediumCV
    {
        /// <summary>
        /// The sample medium is unknown
        /// </summary>
        [Description("Unknown")]
        Unknown = 0,

        /// <summary>
        /// Sample taken from the atmosphere
        /// </summary>
        [Description("Air")]
        Air,

        /// <summary>
        /// A mixture of formation water and hydraulic fracturing injectates deriving from oil and gas wells prior to placing wells into production
        /// </summary>
        [Description("Flowback water")]
        Flowbackwater,

        /// <summary>
        /// Sample taken from water located below the surface of the ground, such as from a well or spring
        /// </summary>
        [Description("Groundwater")]
        Groundwater,

        /// <summary>
        /// Sample taken from raw municipal waste water stream.
        /// </summary>
        [Description("Municipal waste water")]
        Municipalwastewater,

        /// <summary>
        /// Sample medium not relevant in the context of the measurement
        /// </summary>
        [Description("Not Relevant")]
        NotRelevant,

        /// <summary>
        /// Sample medium other than those contained in the CV
        /// </summary>
        [Description("Other")]
        Other,

        /// <summary>
        /// Sample taken from solid or liquid precipitation
        /// </summary>
        [Description("Precipitation")]
        Precipitation,

        /// <summary>
        /// Fluids produced from wells during oil or gas production which may include formation water, injected fluids, oil and gas.
        /// </summary>
        [Description("Production water")]
        Productionwater,

        /// <summary>
        /// Sample taken from the sediment beneath the water column
        /// </summary>
        [Description("Sediment")]
        Sediment,

        /// <summary>
        /// Observation in, of or sample taken from snow
        /// </summary>
        [Description("Snow")]
        Snow,

        /// <summary>
        /// Sample taken from the soil
        /// </summary>
        [Description("Soil")]
        Soil,

        /// <summary>
        /// Air contained in the soil pores
        /// </summary>
        [Description("Soil air")]
        Soilair,

        /// <summary>
        /// the water contained in the soil pores
        /// </summary>
        [Description("Soil water")]
        Soilwater,

        /// <summary>
        /// Observation or sample of surface water such as a stream, river, lake, pond, reservoir, ocean, etc.
        /// </summary>
        [Description("Surface Water")]
        SurfaceWater,

        /// <summary>
        /// Sample taken from the tissue of a biological organism
        /// </summary>
        [Description("Tissue")]
        Tissue,
    }
}
