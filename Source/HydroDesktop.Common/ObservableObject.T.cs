using System;
using System.ComponentModel;
using System.Linq.Expressions;
using HydroDesktop.Common.Tools;

namespace HydroDesktop.Common
{
    /// <summary>
    /// Base class for classes which implements INotifyPropertyChanged
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObservableObject<T> : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="property">The property.</param>
        protected virtual void NotifyPropertyChanged(Expression<Func<T, object>> property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(NameHelper.Name(property)));
        }

        #endregion
    }
}
