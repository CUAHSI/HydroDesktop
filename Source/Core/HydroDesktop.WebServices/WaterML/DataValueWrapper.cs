using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterML
{
    class DataValueWrapper
    {
        public DataValue DataValue { get; set; }
        public string SeriesCode { get; set; }
        public string SourceID { get; set; }
        public string MethodID { get; set; }
        public string OffsetID { get; set; }
        public string SampleID { get; set; }
        public string QualityID { get; set; }
    }
}