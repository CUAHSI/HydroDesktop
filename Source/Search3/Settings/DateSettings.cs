using System;
using HydroDesktop.Common;

namespace Search3.Settings
{
    public class DateSettings : ObservableObject<DateSettings>
    {
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(x => StartDate);
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged(x => EndDate);
            }
        }

        public DateSettings()
        {
            EndDate = DateTime.Now.Date;
            StartDate = EndDate.AddYears(-5);
        }

        /// <summary>
        /// Create deep copy of current instance.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public DateSettings Copy()
        {
            var result = new DateSettings();
            result.Copy(this);
            return result;

        }

        /// <summary>
        /// Create deep from source into current instance.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>must be not null.</exception>
        public void Copy(DateSettings source)
        {
            if (source == null) throw new ArgumentNullException("source");

            StartDate = source.StartDate;
            EndDate = source.EndDate;
        }
    }
}