using System;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport
{
    public class ColumnInfo : ICloneable
    {
        public bool ImportColumn { get; set; }
        public int ColumnIndex { get; set; }
        public string ColumnName { get; set; }
        public bool ApplySiteToAllColumns { get; set; }

        public Site Site { get; set; }
        public Variable Variable { get; set; }

        public QualityControlLevel QualityControlLevel { get; set; }
        public Method Method { get; set; }
        public Source Source { get; set; }

        public object Clone()
        {
            var copy = (ColumnInfo) MemberwiseClone();

            if (copy.Site != null) copy.Site = (Site) copy.Site.Clone();
            if (copy.Variable != null) copy.Variable = (Variable) copy.Variable.Clone();
            if (copy.QualityControlLevel != null) copy.QualityControlLevel = (QualityControlLevel) copy.QualityControlLevel.Clone();
            if (copy.Method != null) copy.Method = (Method) copy.Method.Clone();
            if (copy.Source != null) copy.Source = (Source) copy.Source.Clone();

            return copy;
        }
    }
}