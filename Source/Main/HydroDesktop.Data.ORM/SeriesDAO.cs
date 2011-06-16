using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.ObjectModel;
using NHibernate.Criterion;

namespace HydroDesktop.Database.DataManagers.DataRepository
{
    /// <summary>
    /// Use this class for retrieving data series
    /// </summary>
    public class SeriesDAO : BaseDAO<Series>
    {
        internal SeriesDAO(NHibernateHelper hibernate) :
            base(hibernate) {}

        #region Private Variables

        //internal dictionary caches to store existing objects
        private Dictionary<string, Site> _savedSites = new Dictionary<string, Site>();

        private Dictionary<string, Variable> _savedVariables = new Dictionary<string, Variable>();

        private Dictionary<string, SpatialReference> _spatialReferences = new Dictionary<string, SpatialReference>();

        private Dictionary<string, Unit> _units = new Dictionary<string, Unit>();

        private Dictionary<string, Method> _methods = new Dictionary<string, Method>();
           
        private Dictionary<string, QualityControlLevel> _qualControlLevels = new Dictionary<string, QualityControlLevel>();

        private Dictionary<string, Source> _sources = new Dictionary<string,Source>();

        private Dictionary<string, Qualifier> _qualifiers = new Dictionary<string, Qualifier>();

        private Dictionary<string, OffsetType> _offsetTypes = new Dictionary<string, OffsetType>();

        private Dictionary<string, Sample> _samples = new Dictionary<string, Sample>();

        public RepositoryManager Manager { get; set; }

        #endregion

        /// <summary>
        /// Saves a data series
        /// </summary>
        /// <param name="series"></param>
        /// <param name="theme"></param>
        /// <param name="overwrite"></param>
        public void Save(Series series, Theme theme, OverwriteOptions overwrite)
        {
            if (overwrite == OverwriteOptions.Copy)
            {
                SaveAsCopy(series, theme);
                return;
            }
        }

        /// <summary>
        /// New method for saving a series to the database
        /// </summary>
        /// <param name="series"></param>
        /// <param name="theme"></param>
        /// <returns></returns>
        public void SaveAsCopy(Series series, Theme theme)
        {
            //the whole saving process is done from within a transaction
            using (NHibernate.ITransaction tx = Hibernate.BeginTransaction())
            {

            }
        }

        /// <summary>
        /// Saves a new data series with its data values to the database.
        /// If a series with the same site, variable, method, quality control and source
        /// already exists in the database, the existing series will be ignored and
        /// the new series will be saved as a 'copy'. The two series can be distinquished
        /// by the CreationDateTime.
        /// </summary>
        /// <param name="series">The data series to save</param>
        /// <param name="theme">The associated data theme</param>
        public void SaveAsCopy_OLD(Series series, Theme theme)
        {
            using (NHibernate.ITransaction tx = Hibernate.BeginTransaction())
            {

                //check theme
                bool themeExists = (Manager.ThemeDAO.FindExisting(theme) != null);

                //the series to be saved
                Series newSeries = new Series();

                //Site site = series.Site;
                //Variable variable = series.Variable;
                Method method = series.Method;
                QualityControlLevel qualControl = series.QualityControlLevel;
                Source source = series.Source;

                //-----------------------------------------

                //-----------------------------------------
                //check site
                //-----------------------------------------
                Site newSite = null;
                //check site
                string siteCode = series.Site.Code;
                if (_savedSites.ContainsKey(siteCode))
                {
                    newSite = _savedSites[siteCode];
                }
                else
                {
                    newSite = Manager.SiteDAO.ExistsOrPersist(newSite);
                    newSite.Name = series.Site.Name;
                    newSite.Comments = series.Site.Comments;
                    newSite.County = series.Site.County;
                    newSite.Elevation_m = series.Site.Elevation_m;
                    newSite.Latitude = series.Site.Latitude;
                    newSite.Longitude = series.Site.Longitude;
                    newSite.DefaultTimeZone = series.Site.DefaultTimeZone;
                    newSite.VerticalDatum = series.Site.VerticalDatum;

                    //-----------------------------------------
                    //check spatial references
                    //-----------------------------------------
                    string srsName = series.Site.SpatialReference.SRSName;
                    if (!_spatialReferences.ContainsKey(srsName))
                    {
                        SpatialReference srs = series.Site.SpatialReference;
                        int srsID = series.Site.SpatialReference.SRSID;
                        SpatialReference latLongSRS = Manager.SpatialReferenceDAO.ExistsOrPersist(srs);
                        _spatialReferences.Add(srsName, latLongSRS);
                    }
                    newSite.SpatialReference = _spatialReferences[srsName];

                    if (series.Site.LocalProjection != null)
                    {
                        string srsName2 = series.Site.LocalProjection.SRSName;
                        if (!_spatialReferences.ContainsKey(srsName2))
                        {
                            int srsID2 = series.Site.LocalProjection.SRSID;
                            SpatialReference localSRS =
                                Manager.SpatialReferenceDAO.ExistsOrPersist(series.Site.LocalProjection);
                            _spatialReferences.Add(srsName2, localSRS);
                        }
                        newSite.LocalProjection = _spatialReferences[srsName2];
                    }
                    _savedSites.Add(siteCode, newSite);
                }
                newSeries.Site = newSite;

                //-----------------------------------------
                //check variable
                //-----------------------------------------

                Variable newVariable = null;
                string variableCode = series.Variable.Code;
                if (_savedVariables.ContainsKey(variableCode))
                {
                    newVariable = _savedVariables[variableCode];
                }
                else
                {
                    newVariable = Manager.VariableDAO.ExistsOrPersist(series.Variable);
                    //copy variable properties
                    newVariable.DataType = series.Variable.DataType;
                    newVariable.IsCategorical = series.Variable.IsCategorical;
                    newVariable.IsRegular = series.Variable.IsRegular;
                    newVariable.Name = series.Variable.Name;
                    newVariable.NoDataValue = series.Variable.NoDataValue;
                    newVariable.GeneralCategory = series.Variable.GeneralCategory;
                    newVariable.SampleMedium = series.Variable.SampleMedium;
                    newVariable.Speciation = series.Variable.Speciation;
                    newVariable.TimeSupport = series.Variable.TimeSupport;
                    newVariable.ValueType = series.Variable.ValueType;

                    //-----------------------------------------------------
                    //check variable units..
                    Unit variableUnit1 = series.Variable.VariableUnit;
                    string unitCode = variableUnit1.Name + "||" + variableUnit1.Abbreviation;
                    if (!_units.ContainsKey(unitCode))
                    {
                        Unit variableUnit2 = Manager.UnitDAO.ExistsOrPersist(series.Variable.VariableUnit);
                        _units.Add(unitCode, variableUnit2);
                    }
                    newVariable.VariableUnit = _units[unitCode];

                    //-----------------------------------------------------
                    //check time units..
                    Unit timeUnit1 = series.Variable.TimeUnit;
                    string timeUnitCode = timeUnit1.Name + "||" + timeUnit1.Abbreviation;
                    if (!_units.ContainsKey(timeUnitCode))
                    {
                        Unit timeUnit2 = Manager.UnitDAO.ExistsOrPersist(series.Variable.TimeUnit);
                        _units.Add(timeUnitCode, timeUnit2);
                    }
                    newVariable.TimeUnit = _units[timeUnitCode];

                    _savedVariables.Add(variableCode, newVariable);
                }
                newSeries.Variable = newVariable;

                //------------------------------------------------
                //check method
                //------------------------------------------------
                string description = series.Method.Description;
                string link = series.Method.Link;
                string methodCode = description + "||" + link;
                if (!_methods.ContainsKey(methodCode))
                {
                    Method newMethod = CheckExistingMethod(description, link);
                    _methods.Add(methodCode, newMethod);
                }
                newSeries.Method = _methods[methodCode];

                //--------------------------------------------------
                //check source
                //--------------------------------------------------
                string sourceCode = series.Source.Organization;
                if (!_sources.ContainsKey(sourceCode))
                {
                    Source newSource = CheckExistingSource(sourceCode);
                    newSource.Address = series.Source.Address;
                    newSource.Citation = series.Source.Citation;
                    newSource.City = series.Source.City;
                    newSource.ContactName = series.Source.ContactName;
                    newSource.Description = series.Source.Description;
                    newSource.Email = series.Source.Email;
                    newSource.ISOMetadata = ISOMetadata.Unknown;
                    newSource.Link = series.Source.Link;
                    newSource.Organization = series.Source.Organization;
                    newSource.Phone = series.Source.Phone;
                    newSource.State = series.Source.State;
                    newSource.ZipCode = series.Source.ZipCode;
                    _sources.Add(sourceCode, newSource);
                }
                newSeries.Source = _sources[sourceCode];

                //--------------------------------------------------
                //check quality control level
                //--------------------------------------------------
                string qcCode = series.QualityControlLevel.Code;
                string qcDefinition = series.QualityControlLevel.Definition;
                string qualityCode = qcCode + "||" + qcDefinition;

                if (!_qualControlLevels.ContainsKey(qualityCode))
                {
                    QualityControlLevel newQC = CheckExistingQualityControl(qcCode, qcDefinition);
                    newQC.Explanation = series.QualityControlLevel.Explanation;
                    _qualControlLevels.Add(qualityCode, newQC);
                }
                newSeries.QualityControlLevel = _qualControlLevels[qualityCode];


                //for each data value, check qualifier, offsetType and sample


                //update some of the series' properties
                newSeries.CreationDateTime = series.CreationDateTime;
                newSeries.LastCheckedDateTime = series.LastCheckedDateTime;
                newSeries.UpdateDateTime = series.UpdateDateTime;
                newSeries.IsCategorical = series.IsCategorical;

                foreach (DataValue val in series.DataValueList)
                {
                    //create a 'value' instance
                    DataValue newVal = newSeries.AddDataValue(val.LocalDateTime, val.Value, val.UTCOffset);
                    newVal.CensorCode = val.CensorCode;

                    // don't worry about qualifier for now..
                    //if (val.Qualifier != null)
                    //{
                    //    Qualifier qual = val.Qualifier;
                    //    string qualCode = qual.Code;

                    //    if (!qualifierCache.ContainsKey(qualCode))
                    //    {
                    //        bool qualExists = Manager.QualifierRepository.CheckForExisting(ref qual);
                    //        qualifierCache.Add(qualCode, qual);
                    //    }
                    //    newVal.Qualifier = qualifierCache[qualCode];
                    //}
                }

                theme.AddSeries(newSeries);
                Hibernate.Save(newSeries);

                //commit the transaction
                Hibernate.CommitTransaction(tx);



                ////check the data values
                //foreach (DataValue valInfo in series.DataValues)
                //{
                //    //check qualifier
                //    if (valInfo.Qualifier != null)
                //    {
                //        Qualifier qual = valInfo.Qualifier;
                //        string qualifierCode = valInfo.Qualifier.Code;
                //        if (qualifierCache.ContainsKey(qualifierCode))
                //        {
                //            valInfo.Qualifier = qualifierCache[qualifierCode];
                //        }  
                //        else
                //        {
                //            bool qualifierExists = Manager.QualifierRepository.CheckForExisting(ref qual);
                //            if (qualifierExists == false)
                //            {
                //                qualifierCache.Add(qualifierCode, qual);
                //            }
                //        }
                //    }

                //    //check offset type
                //    if (valInfo.OffsetType != null)
                //    {
                //        //string offsetCode = 
                //        //    valInfo.OffsetType.Description + "|||" + valInfo.OffsetType.Unit.Abbreviation;
                //        //if (offsetCache.ContainsKey(offsetCode))
                //        //{
                //        //    valInfo.OffsetType = offsetCache[offsetCode];
                //        //}
                //        //else
                //        //{
                //        //    OffsetType newOffset = Manager.OffsetTypeRepository.CheckForExisting(valInfo.OffsetType);
                //        //    offsetCache.Add(offsetCode, newOffset);
                //        //    valInfo.OffsetType = newOffset;
                //        //}
                //    }
                //}

                //series.CreationDateTime = DateTime.Now;
                //theme.AddSeries(series);
                //Hibernate.Save(series);

                //Manager.Commit();
            }
        }

        /// <summary>
        /// Check if there is any existing method with the 
        /// given description and link
        /// </summary>
        /// <param name="description"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public Method CheckExistingMethod(string description, string link)
        {
            NHibernate.IQuery qry = Hibernate.Query("from Method where Description = :p1 and Link = :p2");
            qry.SetParameter("p1", description);
            qry.SetParameter("p2", link);
            Method existing = Hibernate.UniqueResult<Method>(qry);

            if (existing == null)
            {
                existing = new Method();
                existing.Description = description;
                existing.Link = link;
                existing.Code = 0;
            }
            return existing;
        }

        /// <summary>
        /// Check if there is any existing source in the database with
        /// the same organization name
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public Source CheckExistingSource(string organization)
        {
            NHibernate.IQuery qry = Hibernate.Query("from Source where Organization = :p1");
            qry.SetParameter("p1", organization);

            Source existing = Hibernate.UniqueResult<Source>(qry);

            if (existing == null)
            {
                existing = new Source();
                existing.Organization = organization;
            }
            return existing;
        }

        public QualityControlLevel CheckExistingQualityControl(string code, string definition)
        {
            NHibernate.IQuery qry = Hibernate.Query("from QualityControlLevel where Code = :p1 and Definition = :p2");
            qry.SetParameter("p1", code);
            qry.SetParameter("p2", definition);

            QualityControlLevel existing = Hibernate.UniqueResult<QualityControlLevel>(qry);

            if (existing == null)
            {
                existing = new QualityControlLevel();
                existing.Code = code;
                existing.Definition = definition;
            }
            return existing;
        }
        
        /// <summary>
        /// Check if a series with the same site, variable, method, source
        /// and quality control level already exists in the database.
        /// If the series already exists, retrieve an instance from
        /// the database. If the series does not exist, return null.
        /// </summary>
        /// <param name="seriesToCheck">The series to be checked</param>
        /// <returns></returns>
        public Series CheckForExisting(Series seriesToCheck)
        {
            if (seriesToCheck.Id > 0) return seriesToCheck;

            Series fromDB = null;

            using (NHibernate.ITransaction tx = Hibernate.BeginTransaction())
            {
                //Manager.BeginTransaction();

                // HQL query to retrieve series
                string queryString = "from Series where Site.Code = :p1 and Variable.Code = :p2 " +
                    "and Method.Description = :p3 and Method.Link = :p4 " +
                    "and QualityControlLevel.Code = :p5 and QualityControlLevel.Definition = :p6 " +
                    "and Source.Organization = :p7";

                NHibernate.IQuery query = Hibernate.Query(queryString);
                query.SetMaxResults(1);
                query.SetParameter("p1", seriesToCheck.Site.Code);
                query.SetParameter("p2", seriesToCheck.Variable.Code);
                query.SetParameter("p3", seriesToCheck.Method.Description);
                query.SetParameter("p4", seriesToCheck.Method.Link);
                query.SetParameter("p5", seriesToCheck.QualityControlLevel.Code);
                query.SetParameter("p6", seriesToCheck.QualityControlLevel.Definition);
                query.SetParameter("p7", seriesToCheck.Source.Organization);
               
                fromDB = Hibernate.UniqueResult<Series>(query);

                //Manager.Commit();
                Hibernate.CommitTransaction(tx);
            }

            return fromDB;
        }
    }
}
