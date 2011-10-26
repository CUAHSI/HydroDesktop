using System;
using System.Reflection;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents time series metadata (without any actual data values). 
    /// The time series is a combination of a specific site, variable,
    /// method, source and quality control level.
    /// </summary>
    public class SeriesMetadata : BaseEntity
    {
        #region Constructors

        /// <summary>
        /// Creates a new series with properties set to default.
        /// </summary>
        public SeriesMetadata()
        {
            Method = Method.Unknown;
            Source = Source.Unknown;
            QualityControlLevel = QualityControlLevel.Unknown;
        }

        /// <summary>
        /// Creates a new data series associated with the specific site, variable,
        /// method, quality control level and source. This series will contain zero
        /// data values after creation.
        /// </summary>
        /// <param name="site">the observation site (location of measurement)</param>
        /// <param name="variable">the observed variable</param>
        /// <param name="method">the observation method</param>
        /// <param name="qualControl">the quality control level of observed values</param>
        /// <param name="source">the source of the data values for this series</param>
        public SeriesMetadata(Site site, Variable variable, Method method, QualityControlLevel qualControl, Source source)
        {
            this.Site = site;
            this.Variable = variable;
            this.Method = method;
            this.QualityControlLevel = qualControl;
            this.Source = source;
        }

        /// <summary>
        /// Creates a copy of the original series. If copyDataValues is set to true,
        /// then the data values are also copied. 
        /// The new series shares the same site, variable, source, method and quality
        /// control level. The new series does not belong to any data theme.
        /// </summary>
        /// <param name="original">The original series</param>
        public SeriesMetadata(SeriesMetadata original)
        {
            BeginDateTime = original.BeginDateTime;
            EndDateTime = original.EndDateTime;
            CreationDateTime = DateTime.Now;
            EndDateTime = original.EndDateTime;
            EndDateTimeUTC = original.EndDateTimeUTC;
            IsCategorical = original.IsCategorical;
            Method = original.Method;
            QualityControlLevel = original.QualityControlLevel;
            Source = original.Source;
            UpdateDateTime = DateTime.Now;
            ValueCount = original.ValueCount;
            Variable = original.Variable;
        }

        /// <summary>
        /// Creates seriesMetadata object from the original series.
        /// The new seriesMetadata object shares the same site, variable, source, method and quality
        /// control level but it doesn't have any data values
        /// </summary>
        /// <param name="original">The original series</param>
        public SeriesMetadata(Series original)
        {
            BeginDateTime = original.BeginDateTime;
            EndDateTime = original.EndDateTime;
            CreationDateTime = DateTime.Now;
            EndDateTime = original.EndDateTime;
            EndDateTimeUTC = original.EndDateTimeUTC;
            IsCategorical = original.IsCategorical;
            Method = original.Method;
            QualityControlLevel = original.QualityControlLevel;
            Source = original.Source;
            UpdateDateTime = DateTime.Now;
            ValueCount = original.ValueCount;
            Variable = original.Variable;
        }

        #endregion

        #region Private Variables
        private int _valueCount = 0;
        private DateTime _beginDateTime;
        private DateTime _endDateTime;
        private DateTime _beginDateTimeUTC;
        private DateTime _endDateTimeUTC;
        #endregion

        #region Properties
        /// <summary>
        /// True if the series represents categorical values, false otherwise
        /// </summary>
        public virtual bool IsCategorical { get; set; }

        /// <summary>
        /// The local time when the first value of the series was measured
        /// </summary>
        public virtual DateTime BeginDateTime
        {
            get
            {
                return _beginDateTime;
            }
            set
            {
                _beginDateTime = value;
            }
        }

        /// <summary>
        /// The local time when the last value of the series was measured
        /// </summary>
        public virtual DateTime EndDateTime
        {
            get
            {
                return _endDateTime;
            }
            set
            {
                _endDateTime = value;
            }
        }

        /// <summary>
        /// Begin time of the series in UTC
        /// </summary>
        public virtual DateTime BeginDateTimeUTC 
        {
            get
            {
                return _beginDateTimeUTC;
            }
            set
            {
                _beginDateTimeUTC = value;
            }
        }

        /// <summary>
        /// End time of the series in UTC
        /// </summary>
        public virtual DateTime EndDateTimeUTC 
        {
            get
            {
                return _endDateTimeUTC;
            }
            set
            {
                _endDateTimeUTC = value;
            }       
        }

        
        /// <summary>
        /// The number of data values in this series
        /// </summary>
        public virtual int ValueCount 
        { 
            get
            {
                return _valueCount;
            }

            set
            {
                _valueCount = value;
            }
        }

        /// <summary>
        /// The time when the series has been saved to the HydroDesktop 
        /// repository
        /// </summary>
        public virtual DateTime CreationDateTime { get; set; }

        /// <summary>
        /// A 'Subscribed' Data series may be regularly updated by appending data
        /// </summary>
        public virtual bool Subscribed { get; set; }

        /// <summary>
        /// The time when this data series was last updated (its data values were changed)
        /// </summary>
        public virtual DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// Thime when the series was last checked for updates
        /// </summary>
        public virtual DateTime LastCheckedDateTime { get; set; }

        /// <summary>
        /// The site where the data is measured
        /// </summary>
        public virtual Site Site { get; set; }

        /// <summary>
        /// The measured variable
        /// </summary>
        public virtual Variable Variable { get; set; }

        /// <summary>
        /// Specification of the data service that was used to retrieve
        /// this series
        /// </summary>
        public virtual DataServiceInfo DataService { get; set; }

        /// <summary>
        /// The method of measurement
        /// </summary>
        public virtual Method Method { get; set; }      

        /// <summary>
        /// The primary source of the data
        /// </summary>
        public virtual Source Source { get; set; }      

        /// <summary>
        /// The primary quality control level of the data
        /// </summary>
        public virtual QualityControlLevel QualityControlLevel { get; set; }

        /// <summary>
        /// Shortcut property to obtain time zone information
        /// </summary>
        public virtual TimeZoneInfo GetDefaultTimeZone()
        {
            return (Site != null) ? Site.DefaultTimeZone : null;
        }
        

        #endregion

        #region Methods
        /// <summary>
        /// String representation of the series metadata
        /// </summary>
        /// <returns>Site.Name | Variable.Name | Variable.DataType</returns>
        public override string ToString()
        {
            return Site.Name + "|" + Variable.Name + "|" + Variable.DataType;
        }        

        /// <summary>
        /// A shortcut method to obtain the 'no data' value used by the variable
        /// of this series
        /// </summary>
        /// <returns>the no data value of the variable in this series</returns>
        public virtual double GetNoDataValue()
        {
            return (Variable != null) ? Variable.NoDataValue : 0;
        }

        /// <summary>
        /// Creates a copy of this data series. The data values are also
        /// copied. the new data series shares the same site, variable, method,
        /// source and quality control level with the original series.
        /// </summary>
        /// <returns>The new copy of the data series</returns>
        public virtual SeriesMetadata Copy()
        {
            return new SeriesMetadata(this);
        }

        #endregion
    }
}
