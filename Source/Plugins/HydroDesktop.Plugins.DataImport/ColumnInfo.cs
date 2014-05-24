using System;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.DataImport
{
    /// <summary>
    /// Contains info about column to import
    /// </summary>
    public class ColumnInfo : ICloneable, IEquatable<ColumnInfo>
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

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
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

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual bool Equals(ColumnInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return other.ColumnIndex == ColumnIndex && other.ColumnName == ColumnName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Equals(obj as ColumnInfo);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (ColumnName + ColumnIndex).GetHashCode();
        }
    }
}