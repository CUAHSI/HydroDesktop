using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.ObjectModel;
using NHibernate;

namespace HydroDesktop.Database.DataManagers.DataRepository
{
    /// <summary>
    /// Use this class for saving or loading of themes
    /// </summary>
    public class ThemeDAO : BaseDAO<Theme>
    {
        internal ThemeDAO(NHibernateHelper hibernate) :
            base(hibernate) {}

        public RepositoryManager Manager { get; set; }
        
        /// <summary>
        /// Check if the same theme already exists in the database.
        /// If the theme already exists, retrieve an instance from
        /// the database.
        /// </summary>
        /// <param name="newEntity"></param>
        /// <returns>entity from the database (if found) or null if no match found</returns>
        public override Theme FindExisting(Theme entity)
        {
            if (entity.Id > 0) return entity;
            
            string queryString = "from Theme where Name=:p1 and Description=:p2";          
            NHibernate.IQuery qry = Hibernate.Query(queryString);
            qry.SetCacheable(true); 
            qry.SetCacheRegion("Generic");
            qry.SetParameter("p1", entity.Name);
            qry.SetParameter("p2", entity.Description);

            return Hibernate.UniqueResult<Theme>(qry);
        }
        //public IList<Theme> ListAll()
        //{
        //    //string queryString = string.Format("from {0} order by {1}", _className, orderBy);
        //    ICriteria criteria = Hibernate.GetCurrentSession().CreateCriteria<Theme>();
        //    return criteria.List<Theme>();
        //}

    }
}
