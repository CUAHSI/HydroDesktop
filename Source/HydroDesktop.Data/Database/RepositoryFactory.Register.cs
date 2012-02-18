using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    public partial class RepositoryFactory
    {
        partial void Register()
        {
            Add<IRepositoryManager>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new DbRepositoryManagerSQL(dbType, connStr),
                        CreatorByDbOperations = dbOp => new DbRepositoryManagerSQL(dbOp)
                    });
            Add<IDataSeriesRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new DataSeriesRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new DataSeriesRepository(dbOp)
                    });
            Add<IDataThemesRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new DataThemesRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new DataThemesRepository(dbOp)
                    });
            Add<IMethodsRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new MethodsRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new MethodsRepository(dbOp)
                    });
            Add<IVariablesRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new VariablesRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new VariablesRepository(dbOp)
                    });
            Add<IUnitsRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new UnitsRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new UnitsRepository(dbOp)
                    });
            Add<IDataValuesRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new DataValuesRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new DataValuesRepository(dbOp)
                    });
            Add<ISitesRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new SitesRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new SitesRepository(dbOp)
                    });
        }
    }
}
