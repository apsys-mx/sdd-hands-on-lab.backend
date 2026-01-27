using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace kudos.backend.infrastructure.nhibernate;

/// <summary>
/// Session factory using C# 12 primary constructor syntax.
/// Configured for PostgreSQL.
/// </summary>
public class NHSessionFactory(string connectionString)
{
    public string ConnectionString { get; } = connectionString;

    /// <summary>
    /// Create the NHibernate Session Factory
    /// </summary>
    public ISessionFactory BuildNHibernateSessionFactory()
    {
        var mapper = new ModelMapper();
        // Register all mappers from the assembly
        mapper.AddMappings(typeof(NHSessionFactory).Assembly.ExportedTypes);
        HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

        var cfg = new Configuration();
        cfg.DataBaseIntegration(c =>
        {
            // PostgreSQL configuration
            c.Driver<NpgsqlDriver>();
            c.Dialect<PostgreSQL83Dialect>();
            c.ConnectionString = this.ConnectionString;
            c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
        });

        cfg.AddMapping(domainMapping);
        return cfg.BuildSessionFactory();
    }
}
