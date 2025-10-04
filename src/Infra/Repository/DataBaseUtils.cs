using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infra.Model;

namespace Infra.Repository
{
    /// <summary>
    /// Repositório base para interagir com o PostgreSQL, focado na execução de stored procedures.
    /// Esta classe encapsula a lógica de conexão.
    /// </summary>
    public abstract class DataBaseUtils
    {
        
        private readonly string _connectionString;

        public DataBaseUtils(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgresConnection")
                ?? throw new InvalidOperationException("A string de conexão 'PostgresConnection' não foi encontrada.");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString), "A string de conexão não pode ser nula ou vazia.");
            }
        }

        // Método utilitário para criar e abrir a conexão
        protected IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Executa uma stored procedure que retorna um conjunto de resultados (como um SELECT).
        /// </summary>
        /// <typeparam name="T">O tipo do objeto a ser mapeado (Modelo de Domínio ou DTO).</typeparam>
        /// <param name="procedureName">Nome da procedure no PostgreSQL.</param>
        /// <param name="parameters">Objeto anônimo contendo os parâmetros da procedure.</param>
        /// <returns>Uma coleção de objetos do tipo T.</returns>
        public async Task<IEnumerable<T>> QueryProcedureAsync<T>(string procedureName, object? parameters = null)
        {
            
            using (var connection = CreateConnection())
            {

                return await connection.QueryAsync<T>(
                   sql: procedureName,
                   param: parameters,
                   commandType: CommandType.StoredProcedure
               );

                            
            }
        }

        /// <summary>
        /// Executa uma stored procedure que não retorna dados (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="procedureName">Nome da procedure no PostgreSQL.</param>
        /// <param name="parameters">Objeto anônimo contendo os parâmetros da procedure.</param>
        /// <returns>O número de linhas afetadas.</returns>
        public async Task<int> ExecuteProcedureAsync(string procedureName, object? parameters = null)
        {
            using (var connection = CreateConnection())
            {
                var sql = "CALL order_update_status(@p_id, @p_new_status)";
                return await connection.ExecuteAsync(sql, parameters);

            }
        }
    }
}
