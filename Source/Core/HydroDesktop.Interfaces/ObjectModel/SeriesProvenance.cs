using System;

namespace HydroDesktop.Interfaces.ObjectModel
{
    public class SeriesProvenance : BaseEntity
    {
        public virtual DateTime ProvenanceDateTime { get; set; }
        public virtual Series InputSeries { get; set; }
        public virtual Series OutputSeries { get; set; }
        public virtual Method Method { get; set; }
        public virtual string Comment { get; set; }
    }
}