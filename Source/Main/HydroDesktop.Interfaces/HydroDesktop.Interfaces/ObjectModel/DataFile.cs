using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// represents either a WaterML data file returned
    /// by the WaterOneFlow service or a source file used
    /// for importing data into HydroDesktop.
    /// </summary>
    public class DataFile : BaseEntity
    {
        /// <summary>
        /// The WaterML file name
        /// </summary>
        public virtual string FileName { get; set; }
        /// <summary>
        /// The WaterML file description
        /// </summary>
        public virtual string FileDescription { get; set; }
        /// <summary>
        /// The WaterML file type
        /// </summary>
        public virtual string FileType { get; set; }
        /// <summary>
        /// The WaterML file path
        /// </summary>
        public virtual string FilePath { get; set; }
        /// <summary>
        /// The WaterML file origin
        /// </summary>
        public virtual string FileOrigin { get; set; }
        /// <summary>
        /// The method used for retrieving the WaterML file
        /// (from GetValues call, created by HydroDesktop, other method)
        /// </summary>
        public virtual string LoadMethod { get; set; }
        /// <summary>
        /// The time of loading data from the  WaterML file
        /// </summary>
        public virtual DateTime LoadDateTime { get; set; }
        //public virtual int QueryID { get; set; }
        /// <summary>
        /// Information about WaterML query parameters
        /// </summary>
        public virtual QueryInfo QueryInfo { get; set; }

        /// <summary>
        /// Determines if the file was retrieved from a water one flow service
        /// </summary>
        public virtual bool IsFromWaterOneFlow
        {
            get { return (QueryInfo != null); }
        }

        /// <inheritdoc/>
        public override bool Equals(BaseEntity other)
        {
            DataFile file2 = other as DataFile;
            if (file2 == null)
            {
                return base.Equals(other);
            }
            else
            {
                return (FileName.Equals(file2.FileName) && FilePath.Equals(file2.FilePath));
            }
        }
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (FileName + FilePath).GetHashCode();
        }
    }
}
