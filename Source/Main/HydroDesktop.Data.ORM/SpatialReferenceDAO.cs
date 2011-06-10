using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database.DataManagers;

namespace HydroDesktop.Database.DataManagers.DataRepository
{
    public class SpatialReferenceDAO : BaseDAO<SpatialReference>
    {
        internal SpatialReferenceDAO(NHibernateHelper hibernate) :
            base(hibernate) {}
        
        /// <summary>
        /// Check if an identical object already exists
        /// in the database
        /// </summary>
        /// <param name="entity">the entity to be checked</param>
        /// <returns>The entity retrieved from the DB or null if no match found</returns>
        public override SpatialReference FindExisting(SpatialReference entity)
        {
            NHibernate.IQuery qry = Hibernate.Query("from SpatialReference where SRSName = :p1");
            qry.SetCacheable(true); 
            qry.SetParameter("p1", entity.SRSName);
            return Hibernate.UniqueResult<SpatialReference>(qry);
        }
    }
}
