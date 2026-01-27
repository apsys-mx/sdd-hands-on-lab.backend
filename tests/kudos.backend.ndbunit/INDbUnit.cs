using System.Data;
using System.Data.Common;

namespace kudos.backend.ndbunit;

public interface INDbUnit
{
    /// <summary>
    /// Gets the dataSet containing the tables where the operations are execute
    /// </summary>
    DataSet DataSet { get; }

    /// <summary>
    /// Gets the connection string to the database where the operations are execute
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Get a dataset with the tables and data from the database
    /// </summary>
    DataSet GetDataSetFromDb();

    /// <summary>
    /// Create a data adapter to the database
    /// </summary>
    DbDataAdapter CreateDataAdapter();

    /// <summary>
    /// Clear all the database data
    /// </summary>
    void ClearDatabase();

    /// <summary>
    /// Seed the database with the information contained in the dataset
    /// </summary>
    void SeedDatabase(DataSet dataSet);
}
