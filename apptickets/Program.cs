using System;

public class Program
{
    private readonly DatabaseService databaseService;

    public Program(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Ingrese su nombre de usuario:");
        string nombreUsuario = Console.ReadLine();

        IDatabaseConnection databaseConnection = new MySqlDatabaseConnection("Server=localhost; Database=sistematickets; Uid=root; Pwd=;");
        var databaseService = new DatabaseService(databaseConnection);
        var program = new Program(databaseService);
        
        if (program.EsAdministrador(nombreUsuario))
        {
            Console.WriteLine("Bienvenido, Administrador.");
            program.AdministrarSistema();
        }
        else
        {
            Console.WriteLine("Bienvenido, Usuario.");
            program.CrearTicket(nombreUsuario);
        }
    }

    public bool EsAdministrador(string nombreUsuario)
    {
        return databaseService.EsAdministrador(nombreUsuario);
    }

    public void AdministrarSistema()
    {
        bool continuar = true;

        while (continuar)
        {
            Console.WriteLine("\nSeleccione una opción:");
            Console.WriteLine("1. Agregar nuevo usuario");
            Console.WriteLine("2. Cambiar estado de ticket");
            Console.WriteLine("3. Consultar tickets");
            Console.WriteLine("4. Salir");
            int opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1:
                    AgregarUsuario();
                    break;
                case 2:
                    CambiarEstadoTicket();
                    break;
                case 3:
                    ConsultarTickets();
                    break;
                case 4:
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        Console.WriteLine("Gracias por usar el sistema.");
    }

    public void AgregarUsuario()
    {
        Console.WriteLine("Ingrese el nombre del nuevo usuario:");
        string nuevoNombre = Console.ReadLine();

        Console.WriteLine("¿Es administrador? (true/false):");
        bool esAdministrador = bool.Parse(Console.ReadLine());

        databaseService.AgregarUsuario(nuevoNombre, esAdministrador);
        Console.WriteLine("Usuario agregado exitosamente.");
    }

    public void CambiarEstadoTicket()
    {
        Console.WriteLine("Ingrese el ID del ticket:");
        int idTicket = int.Parse(Console.ReadLine());

        Console.WriteLine("Ingrese el nuevo estado del ticket:");
        string nuevoEstado = Console.ReadLine();

        databaseService.CambiarEstadoTicket(idTicket, nuevoEstado);
        Console.WriteLine("Estado del ticket actualizado.");
    }

    public void ConsultarTickets()
    {
        Console.WriteLine("Ingrese el estado del ticket que desea consultar (deje vacío para todos):");
        string estadoTicket = Console.ReadLine();

        string query = "SELECT id_ticket, id_usuario, estado_ticket, solicitud FROM tickets";
        if (!string.IsNullOrEmpty(estadoTicket))
        {
            query += " WHERE estado_ticket = @estado_ticket";
        }

        Console.WriteLine("Consultar tickets aún no implementado.");
    }

    public void CrearTicket(string nombreUsuario)
    {
        Console.WriteLine("\nIngrese su solicitud:");
        string solicitud = Console.ReadLine();

        databaseService.CrearTicket(nombreUsuario, solicitud);
        Console.WriteLine("Ticket creado exitosamente.");
    }
}
