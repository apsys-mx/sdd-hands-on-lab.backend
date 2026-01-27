using System.Data;
using System.Data.Common;

namespace kudos.backend.ndbunit;

public abstract class NDbUnit : INDbUnit
{
    protected NDbUnit(DataSet dataSet, string connectionString)
    {
        this.ConnectionString = connectionString;
        this.DataSet = dataSet;
    }

    public DataSet DataSet { get; private set; }
    public string ConnectionString { get; private set; }

    public DataSet GetDataSetFromDb()
    {
        using DbConnection cnn = this.CreateConnection();
        DataSet dsetResult = this.DataSet.Clone();
        dsetResult.EnforceConstraints = false;
        DbProviderFactory? dbFactory = DbProviderFactories.GetFactory(cnn)
            ?? throw new ArgumentException("Cannot create [DbProviderFactory] from configuration");
        foreach (DataTable table in this.DataSet.Tables)
        {
            DbCommand selectCommand = cnn.CreateCommand();
            selectCommand.CommandText = $"SELECT * FROM {table.TableName}";

            DbDataAdapter? adapter = dbFactory.CreateDataAdapter()
                ?? throw new ArgumentException("Cannot create [DbDataAdapter] from configuration");
            adapter.SelectCommand = selectCommand;
            adapter.Fill(dsetResult, table.TableName);
        }
        dsetResult.EnforceConstraints = true;
        return dsetResult;
    }

    public void ClearDatabase()
    {
        using IDbConnection cnn = this.CreateConnection();
        cnn.Open();

        using IDbTransaction transaction = cnn.BeginTransaction();
        try
        {
            foreach (DataTable dataTable in this.DataSet.Tables)
                this.DisableTableConstraints(transaction, dataTable);

            foreach (DataTable dataTable in this.DataSet.Tables)
            {
                var cmd = cnn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = $"DELETE FROM {dataTable.TableName}";
                cmd.Connection = cnn;
                cmd.ExecuteNonQuery();
            }

            foreach (DataTable dataTable in this.DataSet.Tables)
                this.EnabledTableConstraints(transaction, dataTable);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        cnn.Close();
    }

    public void SeedDatabase(DataSet dataSet)
    {
        using IDbConnection cnn = this.CreateConnection();
        cnn.Open();

        this.DataSet = dataSet;

        using (IDbTransaction transaction = cnn.BeginTransaction())
        {
            try
            {
                foreach (DataTable dataTable in this.DataSet.Tables)
                    this.DisableTableConstraints(transaction, dataTable);

                foreach (DataTable dataTable in this.DataSet.Tables)
                {
                    var selectCommand = cnn.CreateCommand();
                    selectCommand.CommandText = $"SELECT * FROM {dataTable.TableName}";
                    selectCommand.Transaction = transaction;
                    var adapter = this.CreateDataAdapter();
                    adapter.SelectCommand = selectCommand as DbCommand;
                    var commandBuilder = this.CreateCommandBuilder(adapter);
                    adapter.InsertCommand = commandBuilder.GetInsertCommand();
                    adapter.InsertCommand.Transaction = transaction as DbTransaction;
                    adapter.Update(dataTable);
                }

                foreach (DataTable dataTable in this.DataSet.Tables)
                    this.EnabledTableConstraints(transaction, dataTable);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        cnn.Close();
    }

    public abstract DbConnection CreateConnection();
    public abstract DbDataAdapter CreateDataAdapter();
    public abstract DbCommandBuilder CreateCommandBuilder(DbDataAdapter dataAdapter);
    protected abstract void EnabledTableConstraints(IDbTransaction dbTransaction, DataTable dataTable);
    protected abstract void DisableTableConstraints(IDbTransaction dbTransaction, DataTable dataTable);
}
