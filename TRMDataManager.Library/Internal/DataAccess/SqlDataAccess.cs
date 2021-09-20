using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TRMDataManager.Library.Internal.DataAccess
{
	public class SqlDataAccess : IDisposable
	{
		public string GetConnectionString(string name)
		{
			return ConfigurationManager.ConnectionStrings[name].ConnectionString;
		}

		public List<T> LoadData<T, U>(
			string storedProcedure
			, U parameters
			, string connectionStringName)
		{
			var connectionString = GetConnectionString(connectionStringName);

			using (IDbConnection connection = new SqlConnection(connectionString))
			{
				List<T> rows = connection.Query<T>(
					storedProcedure
					, parameters
					, commandType: CommandType.StoredProcedure).ToList();

				return rows;
			}
		}

		public void SaveData<T>(
			string storedProcedure
			, T parameters
			, string connectionStringName)
		{
			var connectionString = GetConnectionString(connectionStringName);

			using (IDbConnection connection = new SqlConnection(connectionString))
			{
				connection.Execute(
					storedProcedure
					, parameters
					, commandType: CommandType.StoredProcedure);
			}
		}

		private IDbConnection connection;
		private IDbTransaction transaction;

		public void StartTransaction(string connectionStringName)
		{
			var connectionString = GetConnectionString(connectionStringName);
			connection = new SqlConnection(connectionString);
			connection.Open();
			transaction = connection.BeginTransaction();
		}

		public void CommitTransaction()
		{
			transaction?.Commit();
			connection?.Close();
		}

		public void RollbackTransaction()
		{
			transaction?.Rollback();
		}

		public void Dispose()
		{
			CommitTransaction();
		}

		public void SaveDataInTransaction<T>(
			string storedProcedure
			, T parameters)
		{
			connection.Execute(
				storedProcedure
				, parameters
				, commandType: CommandType.StoredProcedure
				, transaction: transaction);
		}

		public List<T> LoadDataInTransaction<T, U>(
			string storedProcedure
			, U parameters)
		{
			List<T> rows = connection.Query<T>(
					storedProcedure
					, parameters
					, commandType: CommandType.StoredProcedure
					, transaction: transaction).ToList();

			return rows;
		}

		//Open connection/start transaction method
		//load using transaction
		//save using transaction
		//Close connection/stop transaction
		//dispose
	}
}