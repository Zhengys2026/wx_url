using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml;


namespace SQLHelper
{
    public class SqlHelper
    {
         // Methods
    private SqlHelper()
    {
    }

    private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
    {
        if ((commandParameters != null) && (dataRow != null))
        {
            int i = 0;
            foreach (SqlParameter commandParameter in commandParameters)
            {
                if ((commandParameter.ParameterName == null) || (commandParameter.ParameterName.Length <= 1))
                {
                    throw new Exception(string.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.", i, commandParameter.ParameterName));
                }
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                {
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                }
                i++;
            }
        }
    }

    private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
    {
        if ((commandParameters != null) && (parameterValues != null))
        {
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }
            int i = 0;
            int j = commandParameters.Length;
            while (i < j)
            {
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter) parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
                i++;
            }
        }
    }

    private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
    {
        if (command == null)
        {
            throw new ArgumentNullException("command");
        }
        if (commandParameters != null)
        {
            foreach (SqlParameter p in commandParameters)
            {
                if (p != null)
                {
                    if (((p.Direction == ParameterDirection.InputOutput) || (p.Direction == ParameterDirection.Input)) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }
                    command.Parameters.Add(p);
                }
            }
        }
    }

    public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        SqlCommand cmd = new SqlCommand(spName, connection);
        cmd.CommandType = CommandType.StoredProcedure;
        if ((sourceColumns != null) && (sourceColumns.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            for (int index = 0; index < sourceColumns.Length; index++)
            {
                commandParameters[index].SourceColumn = sourceColumns[index];
            }
            AttachParameters(cmd, commandParameters);
        }
        return cmd;
    }

    public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
    {
        return ExecuteDataset(connection, commandType, commandText, null);
    }

    public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
    }

    public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
    {
        return ExecuteDataset(transaction, commandType, commandText, null);
    }

    public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
    }

    public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
    {
        return ExecuteDataset(connectionString, commandType, commandText, null);
    }

    public static DataSet ExecuteDesEncryptDataset(string connectionString, CommandType commandType, string commandText)
    {

        return ExecuteDataset(connectionString, commandType, commandText, null);
    }

    public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
    }

    public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
            }
            return ds;
        }
    }

    public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            DataSet ds = new DataSet();
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }
    }

    public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            return ExecuteDataset(connection, commandType, commandText, commandParameters);
        }
    }

    public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, string spName, DataRow dataRow)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
    }

    public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
    }

    public static DataSet ExecuteDatasetTypedParams(string connectionString, string spName, DataRow dataRow)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
    }

    public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
    {
        return ExecuteNonQuery(connection, commandType, commandText, null);
    }

    public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
    }

    public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
    {
        return ExecuteNonQuery(transaction, commandType, commandText, null);
    }

    public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
    }

    public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
    {
        return ExecuteNonQuery(connectionString, commandType, commandText, null);
    }

    public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
    }

    public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
        int retval = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
        if (mustCloseConnection)
        {
            connection.Close();
        }
        return retval;
    }

    public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
        int retval = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
        return retval;
    }

    public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
        }
    }

    public static int ExecuteNonQueryTypedParams(SqlConnection connection, string spName, DataRow dataRow)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
    }

    public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
    }

    public static int ExecuteNonQueryTypedParams(string connectionString, string spName, DataRow dataRow)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
    }

    public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
    {
        return ExecuteReader(connection, commandType, commandText, null);
    }

    public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteReader(connection, CommandType.StoredProcedure, spName);
    }

    public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
    {
        return ExecuteReader(transaction, commandType, commandText, null);
    }

    public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
    }

    public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
    {
        return ExecuteReader(connectionString, commandType, commandText, null);
    }

    public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
    }

    public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
    }

    public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
    }

    public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        SqlDataReader reader;
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        SqlConnection connection = null;
        try
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            reader = ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
        }
        catch
        {
            if (connection != null)
            {
                connection.Close();
            }
            throw;
        }
        return reader;
    }

    private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
    {
        SqlDataReader reader;
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        bool mustCloseConnection = false;
        SqlCommand cmd = new SqlCommand();
        try
        {
            SqlDataReader dataReader;
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            if (connectionOwnership == SqlConnectionOwnership.External)
            {
                dataReader = cmd.ExecuteReader();
            }
            else
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            bool canClear = true;
            foreach (SqlParameter commandParameter in cmd.Parameters)
            {
                if (commandParameter.Direction != ParameterDirection.Input)
                {
                    canClear = false;
                }
            }
            if (canClear)
            {
                cmd.Parameters.Clear();
            }
            reader = dataReader;
        }
        catch
        {
            if (mustCloseConnection)
            {
                connection.Close();
            }
            throw;
        }
        return reader;
    }

    public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, string spName, DataRow dataRow)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteReader(connection, CommandType.StoredProcedure, spName);
    }

    public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
    }

    public static SqlDataReader ExecuteReaderTypedParams(string connectionString, string spName, DataRow dataRow)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
    }

    public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
    {
        return ExecuteScalar(connection, commandType, commandText, null);
    }

    public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
    }

    public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
    {
        return ExecuteScalar(transaction, commandType, commandText, null);
    }

    public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
    }

    public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
    {
        return ExecuteScalar(connectionString, commandType, commandText, null);
    }

    public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
    }

    public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
        object retval = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        if (mustCloseConnection)
        {
            connection.Close();
        }
        return retval;
    }

    public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
        object retval = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        return retval;
    }

    public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            return ExecuteScalar(connection, commandType, commandText, commandParameters);
        }
    }

    public static object ExecuteScalarTypedParams(SqlConnection connection, string spName, DataRow dataRow)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
    }

    public static object ExecuteScalarTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
    }

    public static object ExecuteScalarTypedParams(string connectionString, string spName, DataRow dataRow)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
    }

    public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
    {
        return ExecuteXmlReader(connection, commandType, commandText, null);
    }

    public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
    }

    public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
    {
        return ExecuteXmlReader(transaction, commandType, commandText, null);
    }

    public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
    }

    public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        XmlReader reader;
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        bool mustCloseConnection = false;
        SqlCommand cmd = new SqlCommand();
        try
        {
            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
            XmlReader retval = cmd.ExecuteXmlReader();
            cmd.Parameters.Clear();
            reader = retval;
        }
        catch
        {
            if (mustCloseConnection)
            {
                connection.Close();
            }
            throw;
        }
        return reader;
    }

    public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
        XmlReader retval = cmd.ExecuteXmlReader();
        cmd.Parameters.Clear();
        return retval;
    }

    public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, string spName, DataRow dataRow)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
    }

    public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, dataRow);
            return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
        }
        return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
    }

    public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
    {
        FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
    }

    public static void FillDataset(SqlConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if (dataSet == null)
        {
            throw new ArgumentNullException("dataSet");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
        }
        else
        {
            FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }
    }

    public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
    {
        FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
    }

    public static void FillDataset(SqlTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException("transaction");
        }
        if ((transaction != null) && (transaction.Connection == null))
        {
            throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        }
        if (dataSet == null)
        {
            throw new ArgumentNullException("dataSet");
        }
        if ((spName == null) || (spName.Length == 0))
        {
            throw new ArgumentNullException("spName");
        }
        if ((parameterValues != null) && (parameterValues.Length > 0))
        {
            SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
            AssignParameterValues(commandParameters, parameterValues);
            FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
        }
        else
        {
            FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
        }
    }

    public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if (dataSet == null)
        {
            throw new ArgumentNullException("dataSet");
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            FillDataset(connection, commandType, commandText, dataSet, tableNames);
        }
    }

    public static void FillDataset(string connectionString, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if (dataSet == null)
        {
            throw new ArgumentNullException("dataSet");
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            FillDataset(connection, spName, dataSet, tableNames, parameterValues);
        }
    }

    public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
    {
        FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
    }

    public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
    {
        FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
    }

    public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
    {
        if ((connectionString == null) || (connectionString.Length == 0))
        {
            throw new ArgumentNullException("connectionString");
        }
        if (dataSet == null)
        {
            throw new ArgumentNullException("dataSet");
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
        }
    }

    private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
    {
        if (connection == null)
        {
            throw new ArgumentNullException("connection");
        }
        if (dataSet == null)
        {
            throw new ArgumentNullException("dataSet");
        }
        SqlCommand command = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
        {
            if ((tableNames != null) && (tableNames.Length > 0))
            {
                string tableName = "Table";
                for (int index = 0; index < tableNames.Length; index++)
                {
                    if ((tableNames[index] == null) || (tableNames[index].Length == 0))
                    {
                        throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                    }
                    dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                    tableName = tableName + ((index + 1)).ToString();
                }
            }
            dataAdapter.Fill(dataSet);
            command.Parameters.Clear();
        }
        if (mustCloseConnection)
        {
            connection.Close();
        }
    }

    public static DataSet GetExecuteDataset(string con, CommandType commandType, string commandText)
    {
        return ExecuteDataset(con, commandType, commandText);
    }

    private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
    {
        if (command == null)
        {
            throw new ArgumentNullException("command");
        }
        if ((commandText == null) || (commandText.Length == 0))
        {
            throw new ArgumentNullException("commandText");
        }
        if (connection.State != ConnectionState.Open)
        {
            mustCloseConnection = true;
            connection.Open();
        }
        else
        {
            mustCloseConnection = false;
        }
        command.Connection = connection;
        command.CommandText = commandText;
        if (transaction != null)
        {
            if (transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            command.Transaction = transaction;
        }
        command.CommandType = commandType;
        if (commandParameters != null)
        {
            AttachParameters(command, commandParameters);
        }
    }

    public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
    {
        if (insertCommand == null)
        {
            throw new ArgumentNullException("insertCommand");
        }
        if (deleteCommand == null)
        {
            throw new ArgumentNullException("deleteCommand");
        }
        if (updateCommand == null)
        {
            throw new ArgumentNullException("updateCommand");
        }
        if ((tableName == null) || (tableName.Length == 0))
        {
            throw new ArgumentNullException("tableName");
        }
        using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
        {
            dataAdapter.UpdateCommand = updateCommand;
            dataAdapter.InsertCommand = insertCommand;
            dataAdapter.DeleteCommand = deleteCommand;
            dataAdapter.Update(dataSet, tableName);
            dataSet.AcceptChanges();
        }
    }



    // Nested Types
    private enum SqlConnectionOwnership
    {
        Internal,
        External
    }

    }
}
