using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// represents either a WaterML data file returned
    // by the WaterOneFlow service or a source file used
    // for importing data into HydroDesktop.
    /// </summary>
    public class DataFile : BaseEntity
    {
        //public int FileID { get; set; }
        public virtual string FileName { get; set; }
        public virtual string FileDescription { get; set; }
        public virtual string FileType { get; set; }
        public virtual string FilePath { get; set; }
        public virtual string FileOrigin { get; set; }
        public virtual string LoadMethod { get; set; }
        public virtual DateTime LoadDateTime { get; set; }
        //public virtual int QueryID { get; set; }

        public virtual QueryInfo QueryInfo { get; set; }

        /// <summary>
        /// Determines if the file was retrieved from a water one flow service
        /// </summary>
        public virtual bool IsFromWaterOneFlow
        {
            get { return (QueryInfo != null); }
        }

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

        public override int GetHashCode()
        {
            return (FileName + FilePath).GetHashCode();
        }
    }
}
