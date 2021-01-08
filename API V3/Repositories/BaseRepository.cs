﻿using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFreeV2.Repositories
{
	public abstract class BaseRepository
	{	public BaseRepository()
		{	
		}

		public int ExecuteGetAffected(string query, Object param = null, int? commandTimeout = null)
		{
			try
			{
				using (MySqlConnection conn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["nff"].ToString()))
				{
					conn.Open();
					var result = conn.Execute(query, param, commandTimeout: commandTimeout);
					conn.Close();
					return result;
				}
			}
			catch(Exception e) { return -1; }
		}

		public bool Execute(string query, Object param = null, int? commandTimeout = null)
		{
			return ExecuteGetAffected(query, param, commandTimeout) >= 0 ? true : false;
		}

		public T QuerySingle<T>(string query, Object param = null, int? commandTimeout = null)
		{
			using (MySqlConnection conn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["nff"].ToString()))
			{
				conn.Open();
				var result = conn.Query<T>(query, param, commandTimeout: commandTimeout).FirstOrDefault();
				conn.Close();
				return result;
			}
		}

		public List<T> QueryList<T>(string query, Object param = null, int? commandTimeout = null)
		{
			using (MySqlConnection conn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["nff"].ToString()))
			{
				conn.Open();
				var result = conn.Query<T>(query, param, commandTimeout: commandTimeout).ToList();
				conn.Close();
				return result;
			}
		}

		public string WithNolock(string query)
		{
			var nolockQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ;";
			nolockQuery += query +";";
			nolockQuery += "COMMIT ;";
			return nolockQuery;		
		}

	}
}