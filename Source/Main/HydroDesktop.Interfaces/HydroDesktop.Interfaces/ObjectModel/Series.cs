using System;
using System.Reflection;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents a time series. The time series is a combination of a specific site, variable,
    /// method, source and quality control level.
    /// </summary>
    public class Series : BaseEntity
    {
        #region Constructors

        /// <summary>
        /// Creates a new series with properties set to default.
        /// </summary>
        public Series()
        {
            DataValueList = new List<DataValue>();
            ValueCount = 0;
            ThemeList = new List<Theme>();
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
        public Series(Site site, Variable variable, Method method, QualityControlLevel qualControl, Source source)
        {
            DataValueList = new List<DataValue>();
            ValueCount = 0;
            ThemeList = new List<Theme>();
            
            this.Site = site;
            this.Variable = variable;
            this.Method = method;
            this.QualityControlLevel = qualControl;
            this.Source = source;
        }

        /// <summary>
        /// Creates a copy of the original series. The data values are also copied. 
        /// The new series shares the same site, variable, source, method and quality
        /// control level. The new series does not belong to any data theme.
        /// </summary>
        /// <param name="original">The original series</param>
        public Series(Series original)
            : this(original, true) { }

        /// <summary>
        /// Creates a copy of the original series. If copyDataValues is set to true,
        /// then the data values are also copied. 
        /// The new series shares the same site, variable, source, method and quality
        /// control level. The new series does not belong to any data theme.
        /// </summary>
        /// <param name="original">The original series</param>
        public Series(Series original, bool copyDataValues)
        {
            //TODO: need to include series provenance information
            
            Series newSeries = new Series();
            BeginDateTime = original.BeginDateTime;
            EndDateTime = original.EndDateTime;
            CreationDateTime = DateTime.Now;
            DataValueList = original.DataValueList;
            EndDateTime = original.EndDateTime;
            EndDateTimeUTC = original.EndDateTimeUTC;
            IsCategorical = original.IsCategorical;
            Method = original.Method;
            QualityControlLevel = original.QualityControlLevel;
            Source = original.Source;
            UpdateDateTime = DateTime.Now;
            ValueCount = original.ValueCount;
            Variable = original.Variable;

            //to copy the data values
            if (copyDataValues == true)
            {
                foreach (DataValue originalDataValue in original.DataValueList)
                {
                    AddDataValue(originalDataValue.Copy());
                }
            }
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

        public virtual bool HasDataValues
        {
            get { return (ValueCount > 0); }
        }

        public virtual bool IsCategorical { get; set; }

        /// <summary>
        /// The local time when the first value of the series was measured
        /// </summary>
        public virtual DateTime BeginDateTime
        {
            get
            {
                if (DataValueList == null) return _beginDateTime;
                
                //if (DataValueList.Count > 0)
                //{
                //    _beginDateTime = DataValueList[0].LocalDateTime;
                //}
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
                if (DataValueList == null) return _endDateTime;

                //if (DataValueList.Count > 0)
                //{
                //    _endDateTime = DataValueList[DataValueList.Count - 1].LocalDateTime;
                //}
                return _endDateTime;
            }
            set
            {
                _endDateTime = value;
            }
        }


        public virtual DateTime BeginDateTimeUTC 
        {
            get
            {
                if (DataValueList == null) return _beginDateTimeUTC;

                //if (DataValueList.Count > 0)
                //{
                //    _beginDateTimeUTC = DataValueList[0].DateTimeUTC;
                //}
                return _beginDateTimeUTC;
            }
            set
            {
                _beginDateTimeUTC = value;
            }
        }


        public virtual DateTime EndDateTimeUTC 
        {
            get
            {
                if (DataValueList == null) return _endDateTimeUTC;

                //if (DataValueList.Count > 0)
                //{
                //    _endDateTimeUTC = DataValueList[DataValueList.Count - 1].DateTimeUTC;
                //}
                return _endDateTimeUTC;
            }
            set
            {
                _endDateTimeUTC = value;
            }       
        }

        
        ///// <summary>
        ///// The number of data values in this series
        ///// </summary>
        //public virtual int ValueCount 
        //{ 
        //    get
        //    {
        //        if (DataValueList.Count > 0)
        //        {
        //            _valueCount = DataValueList.Count;
        //        } else
        //        {
        //            return 0;
        //        }
        //        return _valueCount;
        //    }

        //    set
        //    {
        //        _valueCount = value;
        //    }
        //}

        /// <summary>
        /// The number of data values in this series
        /// </summary>
        public virtual int ValueCount
        {
            get { return _valueCount; }
            set { _valueCount = value; }
        }

        private  void UpdateSeriesInfoFromDataValues()
        {
            if (DataValueList.Count > 0)
            {
                ValueCount = DataValueList.Count;
                _endDateTimeUTC = DataValueList[DataValueList.Count - 1].DateTimeUTC;
                _beginDateTimeUTC = DataValueList[0].DateTimeUTC;

                _endDateTime = DataValueList[DataValueList.Count - 1].LocalDateTime;
                _beginDateTime = DataValueList[0].LocalDateTime;

            } 
            else
            {
                ValueCount = 0;
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
        /// Optional specification of the data service that was used to retrieve
        /// this series
        /// </summary>
        //public virtual DataServiceInfo DataService { get; set; }

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
        /// The list of all values belonging to this data series
        /// </summary>
        public virtual IList<DataValue> DataValueList { get; protected set; }

        /// <summary>
        /// The list of all themes containing this series
        /// </summary>
        public virtual IList<Theme> ThemeList { get; protected set; }

        /// <summary>
        /// Shortcut property to obtain time zone information
        /// </summary>
        public virtual TimeZoneInfo GetDefaultTimeZone()
        {
            return (Site != null) ? Site.DefaultTimeZone : null;
        }

        /// <summary>
        /// Shortcut method to obtain 'No Data Value' information
        /// </summary>
        

        #endregion

        #region Methods

        public override string ToString()
        {
            return Site.Name + "|" + Variable.Name + "|" + Variable.DataType;
        }

        /// <summary>
        /// Creates a new empty data value object associated with this series.
        /// The value is added to the end of the data value list of this series.
        /// </summary>
        public virtual DataValue AddDataValue()
        {
            DataValue newValue = new DataValue();
            newValue.Series = this;
            DataValueList.Add(newValue);
            UpdateSeriesInfoFromDataValues();
            return newValue;
        }

        /// <summary>
        /// Associates an existing data value with this data series
        /// </summary>
        /// <param name="val"></param>
        public virtual DataValue AddDataValue(DataValue val)
        { 
            DataValueList.Add(val);
            val.Series = this;
            UpdateSeriesInfoFromDataValues();
            return val;
        }

        /// <summary>
        /// Adds a data value to the end of this series
        /// </summary>
        /// <param name="time">the local observation time of the data value</param>
        /// <param name="value">the observation value</param>
        /// <returns>the DataValue object</returns>
        public virtual DataValue AddDataValue(DateTime time, double value)
        {
            DataValue val = new DataValue(value, time, 0.0);
            DataValueList.Add(val);
            val.Series = this;
            UpdateSeriesInfoFromDataValues();
            return val;
        }

        /// <summary>
        /// Adds a data value to the end of this series
        /// </summary>
        /// <param name="time">the local observation time of the data value</param>
        /// <param name="value">the observed value</param>
        /// <param name="utcOffset">the difference between UTC and local time</param>
        /// <returns>the DataValue object</returns>
        public virtual DataValue AddDataValue(DateTime time, double value, double utcOffset)
        {
            DataValue val = new DataValue(value, time, utcOffset);
            DataValueList.Add(val);
            val.Series = this;
            UpdateSeriesInfoFromDataValues();
            return val;
        }

        /// <summary>
        /// Adds a data value to the end of this series
        /// </summary>
        /// <param name="time">the local observation time of the data value</param>
        /// <param name="value">the observed value</param>
        /// <param name="utcOffset">the difference between UTC and local time</param>
        /// <param name="qualifier">the qualifier (contains information about specific
        /// observation conditions</param>
        /// <returns>the DataValue object</returns>
        public virtual DataValue AddDataValue(DateTime time, double value, double utcOffset, Qualifier qualifier)
        {
            DataValue val = new DataValue(value, time, utcOffset);
            val.Qualifier = qualifier;
            DataValueList.Add(val);
            val.Series = this;
            UpdateSeriesInfoFromDataValues();
            return val;
        }

        /// <summary>
        /// A shortcut method to obtain the 'no data' value used by the variable
        /// of this series
        /// </summary>
        /// <returns></returns>
        public virtual double GetNoDataValue()
        {
            return (Variable != null) ? Variable.NoDataValue : 0;
        }

        /// <summary>
        /// Shortcut method, to obtain the ValueCount from the DataValueList
        /// </summary>
        /// <returns>The number of DataValues in the DataValueList</returns>
        public virtual int GetValueCount()
        {
            if (DataValueList == null) 
            {
                return 0;
            }
            else
            {
                return DataValueList.Count;
            }
        }

        /// <summary>
        /// Creates a copy of this data series. The data values are also
        /// copied. the new data series shares the same site, variable, method,
        /// source and quality control level with the original series.
        /// </summary>
        /// <returns>The new copy of the data series</returns>
        public virtual Series Copy()
        {
            return new Series(this);
        }

        /// <summary>
        /// Creates a copy of this data series. If the copyDataValues parameter
        /// is set to true, then data values are also
        /// copied. the new data series shares the same site, variable, method,
        /// source and quality control level with the original series.
        /// <param name="copyDataValues">Specifies whether data values should
        /// be copied</param>
        /// </summary>
        /// <returns>The new copy of the data series</returns>
        public virtual Series Copy(bool copyDataValues)
        {
            return new Series(this, copyDataValues);
        }

        /// <summary>
        /// updates the beginDateTime, endDateTime properties based on the DataValueList
        /// </summary>
        public virtual void UpdateProperties()
        {
            UpdateSeriesInfoFromDataValues();
        }

        #endregion
    }
}
