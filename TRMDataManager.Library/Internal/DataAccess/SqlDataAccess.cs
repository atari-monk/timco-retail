using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TRMDataManager.Library.Internal.DataAccess
{
	public class SqlDataAccess : IDisposable, ISqlDataAccess
	{
		private IDbConnection connection;
		private IDbTransaction transaction;
		private bool isClosed = false;
		private readonly IConfiguration config;

		public SqlDataAccess(IConfiguration config)
		{
			this.config = config;
		}

		public string GetConnectionString(string name)
		{
			return config.GetConnectionString(name);
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

		public void StartTransaction(string connectionStringName)
		{
			var connectionString = GetConnectionString(connectionStringName);
			connection = new SqlConnection(connectionString);
			connection.Open();
			transaction = connection.BeginTransaction();
			isClosed = false;
		}

		public void CommitTransaction()
		{
			transaction?.Commit();
			connection?.Close();
			isClosed = true;
		}

		public void RollbackTransaction()
		{
			transaction?.Rollback();
			connection?.Close();
			isClosed = true;
		}

		public void Dispose()
		{
			if (isClosed == false)
			{
				try
				{
					CommitTransaction();
				}
				catch
				{
				}
			}

			transaction = null;
			connection = null;
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
	}
}