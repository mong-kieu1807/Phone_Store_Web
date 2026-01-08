using Microsoft.Data.SqlClient;

namespace PhoneStore.Helper
{
	public class DatabaseHelper
	{
		private readonly string _connectionString;

		public DatabaseHelper(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("SqlServerConnection") ?? throw new InvalidOperationException("Missing connection string: SqlServerConnection");
		}

		public SqlConnection GetConnection()
		{
			return new SqlConnection(_connectionString);
		}

		public SqlDataReader ExecuteQuery(string query, SqlConnection conn)
		{
			return new SqlCommand(query, conn).ExecuteReader();
		}

		public SqlDataReader ExecuteQueryWithParameters(string query, SqlParameter[] parameters, SqlConnection conn)
		{
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddRange(parameters);
			return cmd.ExecuteReader();
		}

		public int ExecuteNonQuery(string query, SqlConnection conn)
		{
			return new SqlCommand(query, conn).ExecuteNonQuery();
		}
		public int ExecuteNonQueryWithParameters(string query, SqlParameter[] parameters, SqlConnection conn)
		{
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddRange(parameters);
			return cmd.ExecuteNonQuery();
		}

		public int ExecuteScalar(string query, SqlParameter[] parameters, SqlConnection conn)
		{
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddRange(parameters);
			return Convert.ToInt32(cmd.ExecuteScalar());
		}

		public object? ExecuteScalarWithParameters(string query, SqlParameter[] parameters, SqlConnection conn)
		{
			SqlCommand cmd = new SqlCommand(query, conn);
			cmd.Parameters.AddRange(parameters);
			return cmd.ExecuteScalar();
		}
	}
}
