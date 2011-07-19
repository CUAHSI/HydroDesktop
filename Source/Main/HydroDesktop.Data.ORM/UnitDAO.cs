using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database.DataManagers;

namespace HydroDesktop.Database.DataManagers.DataRepository
{
    
    
    public class UnitDAO : BaseDAO<Unit>
    {
        internal UnitDAO(NHibernateHelper hibernate) :
            base(hibernate) { }
        
        /// <summary>
        /// Check if an identical object already exists
        /// in the database
        /// </summary>
        /// <param name="entity">the entity to be checked</param>
        /// <returns>The entity retrieved from the DB or null if no match found</returns>
        public override Unit FindExisting(Unit entity)
        {
            NHibernate.IQuery qry = Hibernate.Query("from Unit where Name = :p1 and Abbreviation = :p2");
            qry.SetCacheable(true);
            qry.SetCacheRegion("Units");
            qry.SetParameter("p1", entity.Name);
            qry.SetParameter("p2", entity.Abbreviation);
            return Hibernate.UniqueResult<Unit>(qry);
        }
    }
}
