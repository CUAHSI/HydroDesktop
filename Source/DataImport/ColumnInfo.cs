using System;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport
{
    /// <summary>
    /// Contains info about column to import
    /// </summary>
    public class ColumnInfo : ICloneable
    {
        /// <summary>
        /// Gets or sets value indicating that this column should be imported into database.
        /// </summary>
        public bool ImportColumn { get; set; }

        /// <summary>
        /// Gets or sets index of this column in the source data table.
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets name of this column in the source data table.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets value indicating that Site from this column should be applied to all other columns.
        /// </summary>
        public bool ApplySiteToAllColumns { get; set; }

        /// <summary>
        /// Gets or sets value indicating that Variable from this column should be applied to all other columns.
        /// </summary>
        public bool ApplyVariableToAllColumns { get; set; }

        /// <summary>
        /// Gets or sets value indicating that Source from this column should be applied to all other columns.
        /// </summary>
        public bool ApplySourceToAllColumns { get; set; }

        /// <summary>
        /// Gets or sets value indicating that Method from this column should be applied to all other columns.
        /// </summary>
        public bool ApplyMethodToAllColumns { get; set; }

        /// <summary>
        /// Gets or sets value indicating that QualityControl from this column should be applied to all other columns.
        /// </summary>
        public bool ApplyQualityControlToAllColumns { get; set; }

        /// <summary>
        /// Gets or sets value indicating that OffsetType and  OffsetValue from this column should be applied to all other columns.
        /// </summary>
        public bool ApplyOffsetToAllColumns { get; set; }

        /// <summary>
        /// Gets or sets Site for this column.
        /// </summary>
        public Site Site { get; set; }

        /// <summary>
        /// Gets or sets Variable for this column.
        /// </summary>
        public Variable Variable { get; set; }

        /// <summary>
        /// Gets or sets QualityControlLevel for this column.
        /// </summary>
        public QualityControlLevel QualityControlLevel { get; set; }

        /// <summary>
        /// Gets or sets Method for this column.
        /// </summary>
        public Method Method { get; set; }

        /// <summary>
        /// Gets or sets Source for this column.
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// Gets or sets OffsetType for this column.
        /// </summary>
        public OffsetType OffsetType { get; set; }

        /// <summary>
        /// Gets or sets OffsetValue for this column.
        /// </summary>
        public double OffsetValue { get; set; }

        public object Clone()
        {
            var copy = (ColumnInfo) MemberwiseClone();

            if (copy.Site != null) copy.Site = (Site) copy.Site.Clone();
            if (copy.Variable != null) copy.Variable = (Variable) copy.Variable.Clone();
            if (copy.QualityControlLevel != null)
                copy.QualityControlLevel = (QualityControlLevel) copy.QualityControlLevel.Clone();
            if (copy.Method != null) copy.Method = (Method) copy.Method.Clone();
            if (copy.Source != null) copy.Source = (Source) copy.Source.Clone();
            if (copy.OffsetType != null) copy.OffsetType = (OffsetType) copy.OffsetType.Clone();

            return copy;
        }
    }
}