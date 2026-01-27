using Npgsql;
using System.Data;
using System.Data.Common;

namespace kudos.backend.ndbunit;

public class PostgreSQLNDbUnit : NDbUnit
{
    public PostgreSQLNDbUnit(DataSet dataSet, string connectionString)
        : base(dataSet, connectionString)
    {
    }

    public override DbCommandBuilder CreateCommandBuilder(DbDataAdapter dataAdapter)
    {
        return new NpgsqlCommandBuilder((NpgsqlDataAdapter)dataAdapter);
    }

    public override DbConnection CreateConnection()
    {
        return new NpgsqlConnection(ConnectionString);
    }

    public override DbDataAdapter CreateDataAdapter()
    {
        return new NpgsqlDataAdapter();
    }

    protected override void DisableTableConstraints(IDbTransaction dbTransaction, DataTable dataTable)
    {
        if (dbTransaction?.Connection == null)
            throw new ArgumentNullException(nameof(dbTransaction));

        using NpgsqlCommand command = (NpgsqlCommand)dbTransaction.Connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction)dbTransaction;
        command.CommandText = $"ALTER TABLE {dataTable.TableName} DISABLE TRIGGER ALL";
        command.ExecuteNonQuery();
    }

    protected override void EnabledTableConstraints(IDbTransaction dbTransaction, DataTable dataTable)
    {
        if (dbTransaction?.Connection == null)
            throw new ArgumentNullException(nameof(dbTransaction));

        using NpgsqlCommand command = (NpgsqlCommand)dbTransaction.Connection.CreateCommand();
        command.Transaction = (NpgsqlTransaction)dbTransaction;
        command.CommandText = $"ALTER TABLE {dataTable.TableName} ENABLE TRIGGER ALL";
        command.ExecuteNonQuery();
    }
}
