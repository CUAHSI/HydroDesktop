using System;
using System.Collections.Generic;
using System.Text;


namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Represents a data theme. A data theme is a group of
    /// data series created by the user. One data series may belong
    /// to multiple themes.
    /// </summary>
    public class Theme : BaseEntity
    {
        public Theme()
        {
            this.DateCreated = DateTime.Now;
            SeriesList = new List<Series>();
        }

        /// <summary>
        /// Creates a new theme with the specified name
        /// </summary>
        /// <param name="name">the name of the theme</param>
        public Theme(string name)
        {
            this.Name = name;
            this.DateCreated = DateTime.Now;
        }

        /// <summary>
        /// Creates a new theme with the specified name and 
        /// description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public Theme(string name, string description)
        {
            this.Name = name;
            this.Description = description;
            this.DateCreated = DateTime.Now;
        }
        
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// The collection of data series belonging to this theme
        /// </summary>
        public virtual IList<Series> SeriesList { get; set; }

        #region Methods

        /// <summary>
        /// Adds a data series to this theme
        /// </summary>
        public virtual void AddSeries(Series series)
        {
            if (SeriesList == null)
            {
                SeriesList = new List<Series>();
            }
            this.SeriesList.Add(series);
            series.ThemeList.Add(this);
        }

        #endregion
    }
}
