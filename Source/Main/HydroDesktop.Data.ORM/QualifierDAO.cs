using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database.DataManagers;

namespace HydroDesktop.Database.DataManagers.DataRepository
{
    
    
    public class QualifierDAO : BaseDAO<Qualifier>
    {
        internal QualifierDAO(NHibernateHelper hibernate) :
            base(hibernate) { }
        
        /// <summary>
        /// Check if an identical object already exists
        /// in the database
        /// </summary>
        /// <param name="entity">the entity to be checked</param>
        /// <returns>The entity retrieved from the DB or null if no match found</returns>
        public override Qualifier FindExisting(Qualifier entity)
        {
            NHibernate.IQuery qry = Hibernate.Query("from Qualifier where Code = :p1 and Description = :p2");
            qry.SetCacheable(true); 
            qry.SetParameter("p1", entity.Code);
            qry.SetParameter("p2", entity.Description);
            return Hibernate.UniqueResult<Qualifier>(qry);
        }
    }
}
