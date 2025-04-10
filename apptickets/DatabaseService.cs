using MySql.Data.MySqlClient;
using System;
using System.Data;

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
        this.connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(connectionString);
    }

    public IDbCommand CreateCommand(string query, IDbConnection connection)
    {
        return new MySqlCommand(query, (MySqlConnection)connection);
    }
}

public class DatabaseService
{
    private readonly IDatabaseConnection databaseConnection;

    public DatabaseService(IDatabaseConnection databaseConnection)
    {
        this.databaseConnection = databaseConnection;
    }

    public bool EsAdministrador(string nombreUsuario)
    {
        bool esAdmin = false;
        using (var connection = databaseConnection.CreateConnection())
        {
            connection.Open();
            string query = "SELECT es_administrador FROM usuarios WHERE nombre_usuario = @nombreUsuario";
            var command = (MySqlCommand)databaseConnection.CreateCommand(query, connection);
            command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                esAdmin = reader.GetBoolean(0);
            }
        }
        return esAdmin;
    }

    public void AgregarUsuario(string nuevoNombre, bool esAdministrador)
    {
        using (var connection = databaseConnection.CreateConnection())
        {
            connection.Open();
            string query = "INSERT INTO usuarios (nombre_usuario, es_administrador) VALUES (@nombre_usuario, @es_administrador)";
            var command = (MySqlCommand)databaseConnection.CreateCommand(query, connection);
            command.Parameters.AddWithValue("@nombre_usuario", nuevoNombre);
            command.Parameters.AddWithValue("@es_administrador", esAdministrador);
            command.ExecuteNonQuery();
        }
    }

    public void CambiarEstadoTicket(int idTicket, string nuevoEstado)
    {
        using (var connection = databaseConnection.CreateConnection())
        {
            connection.Open();
            string query = "UPDATE tickets SET estado_ticket = @estado_ticket WHERE id_ticket = @id_ticket";
            var command = (MySqlCommand)databaseConnection.CreateCommand(query, connection);
            command.Parameters.AddWithValue("@estado_ticket", nuevoEstado);
            command.Parameters.AddWithValue("@id_ticket", idTicket);
            command.ExecuteNonQuery();
        }
    }

    public void CrearTicket(string nombreUsuario, string solicitud)
    {
        using (var connection = databaseConnection.CreateConnection())
        {
            connection.Open();
            string query = "INSERT INTO tickets (id_usuario, solicitud, estado_ticket) VALUES ((SELECT id_usuario FROM usuarios WHERE nombre_usuario = @nombreUsuario), @solicitud, 'Abierto')";
            var command = (MySqlCommand)databaseConnection.CreateCommand(query, connection);
            command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
            command.Parameters.AddWithValue("@solicitud", solicitud);
            command.ExecuteNonQuery();
        }
    }
}
