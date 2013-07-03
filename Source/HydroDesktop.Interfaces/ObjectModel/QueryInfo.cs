using System;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Information about the query parameters submitted when calling
    /// a WaterOneFlow web service
    /// </summary>
    public class QueryInfo : BaseEntity
    {
        /// <summary>
        /// The site code specified as the location parameter
        /// </summary>
        public virtual string LocationParameter { get; set; }

        /// <summary>
        /// The variable code specified as the variable parameter
        /// </summary>
        public virtual string VariableParameter { get; set; }

        /// <summary>
        /// The begin date time parameter
        /// </summary>
        public virtual DateTime BeginDateParameter { get; set; }

        /// <summary>
        /// The end date time parameter
        /// </summary>
        public virtual DateTime EndDateParameter { get; set; }

        /// <summary>
        /// The authentication token (only required by some services)
        /// </summary>
        public virtual string AuthenticationToken { get; set; }

        /// <summary>
        /// The date and time when the query was submitted and the web
        /// service method was called
        /// </summary>
        public virtual DateTime QueryDateTime { get; set; }
        
        /// <summary>
        /// The web service associated with this query
        /// </summary>
        public virtual DataServiceInfo DataService { get; set; }
    }
}
