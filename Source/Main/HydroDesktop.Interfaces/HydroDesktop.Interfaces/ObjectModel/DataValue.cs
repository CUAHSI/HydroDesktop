using System;
using System.Collections.Generic;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Information about a data value. This allows to specify a method, source
    /// and quality control level associated with the data value.
    /// Some data values can have multiple qualifiers. In this case, the Qualifier
    /// property is the composite qualifier from the multiple qualifiers.
    /// </summary>
    public class DataValue : BaseEntity
    {
        private DateTime _localDateTime = DateTime.MinValue;
        private DateTime _dateTimeUTC = DateTime.MinValue;
        private double _utcOffset = 0.0;
        
        public DataValue()
        {
            CensorCode = "nc";
        }

        /// <summary>
        /// Creates a new data value object. The local time and utc time needs to
        /// be specified.
        /// </summary>
        /// <param name="localDateTime">The local time</param>
        /// <param name="DateTimeUTC">The time in UTC</param>
        /// <param name="value">the data value</param>
        public DataValue(double value, DateTime localDateTime, DateTime dateTimeUTC)
        {
            CensorCode = "nc";
            _dateTimeUTC = dateTimeUTC;
            LocalDateTime = localDateTime;
            DateTimeUTC = dateTimeUTC;
            Value = value;
        }

        /// <summary>
        /// Creates a new data value object with the user-specified value.
        /// The local time and the UTC Offset (UTC Offset = time[LocalTime] - time[UTC]) 
        /// needs to be specified.
        /// </summary>
        /// <param name="value">the data value</param>
        /// <param name="localDateTime">the local time</param>
        /// <param name="utcOffset">the UTC Offset (UTC Offset = time[LocalTime] - time[UTC]) </param>
        public DataValue(double value, DateTime localDateTime, double utcOffset)
        {
            CensorCode = "nc";
            LocalDateTime = localDateTime;
            UTCOffset = utcOffset;
            Value = value;
        }

        /// <summary>
        /// The Copy Constructor.
        /// Creates a new copy of the data value from the
        /// original data value. The new copy is not associated
        /// with any series or data file.
        /// </summary>
        /// <param name="originalDataValue"></param>
        public DataValue(DataValue original)
        {   
            CensorCode = original.CensorCode;
            DataFile = null;
            Series = null;
            DateTimeUTC = original.DateTimeUTC;
            LocalDateTime = original.LocalDateTime;
            OffsetType = original.OffsetType;
            OffsetValue = original.OffsetValue;
            Qualifier = original.Qualifier;
            Sample = original.Sample;
            UTCOffset = original.UTCOffset;
            ValueAccuracy = original.ValueAccuracy;
            Value = original.Value;
        }
        
        /// <summary>
        /// Gets or sets the recorded data value. The units of the value can be obtained by
        /// calling this.Series.Variable.Units
        /// </summary>
        public virtual double Value { get; set; }

        /// <summary>
        /// Gets or sets the data value accuracy (default value is zero)
        /// </summary>
        public virtual double ValueAccuracy { get; set; }

        /// <summary>
        /// Gets or sets the local date time in the local time zone
        /// </summary>
        public virtual DateTime LocalDateTime 
        {
            get
            {
                return _localDateTime;
            }

            set
            {
                _localDateTime = value;
                if (_dateTimeUTC > DateTime.MinValue)
                {
                    _utcOffset = (_localDateTime - _dateTimeUTC).TotalHours;
                }
                else
                {
                    _dateTimeUTC = _localDateTime.AddHours(_utcOffset);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the UTC Offset (difference between local time and UTC)
        /// The UTC offset should be a value between -12 and +12. For example, America has negative
        /// UTC Offset values and India has positive utc offset values.
        /// </summary>
        public virtual double UTCOffset 
        {
            get
            {
                return _utcOffset;
            }

            set
            {
                _utcOffset = value;
                if (_localDateTime > DateTime.MinValue)
                {
                    DateTime calculatedTimeUTC = _localDateTime.AddHours(-_utcOffset);
                    if (_dateTimeUTC != calculatedTimeUTC)
                    {
                        _dateTimeUTC = calculatedTimeUTC;
                    }
                }
                else if (_dateTimeUTC > DateTime.MinValue)
                {
                    DateTime calculatedLocalTime = _dateTimeUTC.AddHours(-_utcOffset);
                    if (_localDateTime != calculatedLocalTime)
                    {
                        _localDateTime = calculatedLocalTime;
                    }
                }
                
            }
        }
        
        /// <summary>
        /// Gets or sets the Time in UTC when the value was recorded. Setting DateTimeUTC
        /// will update the UTCOffset property of the data value.
        /// </summary>
        public virtual DateTime DateTimeUTC 
        {
            get
            {
                return _dateTimeUTC;
            }
            set
            {
                _dateTimeUTC = value;
                if (LocalDateTime > DateTime.MinValue)
                {
                    double calculatedUTCOffset = (LocalDateTime - _dateTimeUTC).TotalHours;

                    if (_utcOffset != calculatedUTCOffset)
                    {
                        _utcOffset = calculatedUTCOffset;
                    }
                }
                else
                {
                    _localDateTime = _dateTimeUTC.AddHours(-_utcOffset);
                }
            }
        }

        /// <summary>
        /// The vertical offset of the observation
        /// </summary>
        public virtual double OffsetValue { get; set; }
        
        /// <summary>
        /// The censor code specifies data value uncertainity.
        /// If the value is not censored then the censor code is 'nc'.
        /// </summary>
        public virtual string CensorCode { get; set; }

        /// <summary>
        /// The data series to which this data value belongs.
        /// One data value can only be part of one data series.
        /// </summary>
        public virtual Series Series { get; set; }

        /// <summary>
        /// The qualifier information explaining measurement circumstances
        /// In some cases this is a compound qualifier (such as 'Ae' or 'p,ice')
        /// </summary>
        public virtual Qualifier Qualifier { get; set; }

        /// <summary>
        /// The DataFile is the xml or other file from which the values had been read
        /// </summary>
        public virtual DataFile DataFile { get; set; }

        /// <summary>
        /// The Sample property should be specified if the data value is part of a water quality sample
        /// </summary>
        public virtual Sample Sample { get; set; }

        /// <summary>
        /// The type of the vertical offset (should be null if vertical offset is zero)
        /// </summary>
        public virtual OffsetType OffsetType { get; set; }

        /// <summary>
        /// Creates a copy of the data value. The copied value is not associated with any series.
        /// </summary>
        /// <returns></returns>
        public virtual DataValue Copy()
        {
            return new DataValue(this);
        }

        public override string ToString()
        {
            return LocalDateTime.ToString() + "|" + Value.ToString();
        }
    }
}
