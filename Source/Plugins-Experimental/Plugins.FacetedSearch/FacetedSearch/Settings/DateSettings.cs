using System;
using System.ComponentModel;

namespace FacetedSearch3.Settings
{
    public class DateSettings : INotifyPropertyChanged
    {
        internal const string PROPERTY_StartDate = "StartDate";
        internal const string PROPERTY_EndDate = "EndDate";

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(PROPERTY_StartDate);
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged(PROPERTY_EndDate);
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

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}