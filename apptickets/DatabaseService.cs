using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace apptickets
{
    public interface IDatabaseConnection
    {
        IDbConnection CreateConnection();
        IDbCommand CreateCommand(string query, IDbConnection connection);
    }

    public class MySqlDatabaseConnection : IDatabaseConnection
    {
        private readonly string connectionString;

        public MySqlDatabaseConnection(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
        }

        public IDbCommand CreateCommand(string query, IDbConnection connection)
        {
            return new MySql.Data.MySqlClient.MySqlCommand(query, (MySql.Data.MySqlClient.MySqlConnection)connection);
        }
    }

    public class DatabaseService
    {
        private readonly IDatabaseConnection databaseConnection;

        public DatabaseService(IDatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
        }

        public bool EsAdministrador(string nombreUsuario)
        {
            if (string.IsNullOrEmpty(nombreUsuario)) 
                throw new ArgumentNullException(nameof(nombreUsuario));

            bool esAdmin = false;
            using (var connection = databaseConnection.CreateConnection())
            {
                connection.Open();
                string query = "SELECT es_administrador FROM usuarios WHERE nombre_usuario = @nombreUsuario";
                var command = databaseConnection.CreateCommand(query, connection);
                
                var param = command.CreateParameter();
                param.ParameterName = "@nombreUsuario";
                param.Value = nombreUsuario;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        esAdmin = reader.GetBoolean(0);
                    }
                }
            }
            return esAdmin;
        }

        public void AgregarUsuario(string nuevoNombre, bool esAdministrador)
        {
            if (string.IsNullOrEmpty(nuevoNombre)) 
                throw new ArgumentNullException(nameof(nuevoNombre));

            using (var connection = databaseConnection.CreateConnection())
            {
                connection.Open();
                string query = "INSERT INTO usuarios (nombre_usuario, es_administrador) VALUES (@nombre_usuario, @es_administrador)";
                var command = databaseConnection.CreateCommand(query, connection);
                
                var param1 = command.CreateParameter();
                param1.ParameterName = "@nombre_usuario";
                param1.Value = nuevoNombre;
                command.Parameters.Add(param1);
                
                var param2 = command.CreateParameter();
                param2.ParameterName = "@es_administrador";
                param2.Value = esAdministrador;
                command.Parameters.Add(param2);

                command.ExecuteNonQuery();
            }
        }

        public void CambiarEstadoTicket(int idTicket, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(nuevoEstado)) 
                throw new ArgumentNullException(nameof(nuevoEstado));

            using (var connection = databaseConnection.CreateConnection())
            {
                connection.Open();
                string query = "UPDATE tickets SET estado_ticket = @estado_ticket WHERE id_ticket = @id_ticket";
                var command = databaseConnection.CreateCommand(query, connection);
                
                var param1 = command.CreateParameter();
                param1.ParameterName = "@estado_ticket";
                param1.Value = nuevoEstado;
                command.Parameters.Add(param1);
                
                var param2 = command.CreateParameter();
                param2.ParameterName = "@id_ticket";
                param2.Value = idTicket;
                command.Parameters.Add(param2);

                command.ExecuteNonQuery();
            }
        }

        public void CrearTicket(string nombreUsuario, string solicitud)
        {
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(solicitud))
                throw new ArgumentNullException("nombreUsuario o solicitud no pueden ser nulos o vacíos");

            using (var connection = databaseConnection.CreateConnection())
            {
                connection.Open();
                string query = "INSERT INTO tickets (id_usuario, solicitud, estado_ticket) VALUES ((SELECT id_usuario FROM usuarios WHERE nombre_usuario = @nombreUsuario), @solicitud, 'Abierto')";
                var command = databaseConnection.CreateCommand(query, connection);
                
                var param1 = command.CreateParameter();
                param1.ParameterName = "@nombreUsuario";
                param1.Value = nombreUsuario;
                command.Parameters.Add(param1);
                
                var param2 = command.CreateParameter();
                param2.ParameterName = "@solicitud";
                param2.Value = solicitud;
                command.Parameters.Add(param2);

                command.ExecuteNonQuery();
            }
        }
    }
}