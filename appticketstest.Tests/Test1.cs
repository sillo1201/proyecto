using Moq;
using Xunit;
using System;
using System.Data; 


public class ProgramTests
{
    private readonly Mock<IDatabaseConnection> mockDatabaseConnection;
    private readonly DatabaseService databaseService;
    private readonly Program program;

    public ProgramTests()
    {
        mockDatabaseConnection = new Mock<IDatabaseConnection>();
        databaseService = new DatabaseService(mockDatabaseConnection.Object);
        program = new Program(databaseService);
    }

    [Fact]
    public void prueba1()
    {
        mockDatabaseConnection.Setup(db => db.CreateConnection()).Returns(new Mock<IDbConnection>().Object);
        databaseService.AgregarUsuario("NuevoUsuario", true);
        mockDatabaseConnection.Verify(db => db.CreateConnection(), Times.Once);
    }

    [Fact]
    public void prueba2()
    {
        mockDatabaseConnection.Setup(db => db.CreateConnection()).Returns(new Mock<IDbConnection>().Object);
        databaseService.CambiarEstadoTicket(1, "Cerrado");
        mockDatabaseConnection.Verify(db => db.CreateConnection(), Times.Once);
    }

    [Fact]
    public void prueba3()
    {
        mockDatabaseConnection.Setup(db => db.CreateConnection()).Returns(new Mock<IDbConnection>().Object);
        bool resultado = false;
        try
        {
            databaseService.AgregarUsuario("UsuarioNormal", false);
        }
        catch (Exception)
        {
            resultado = true;
        }
        Xunit.Assert.True(resultado);
    }

    [Fact]
    public void prueba4()
    {        
        mockDatabaseConnection.Setup(db => db.CreateConnection()).Returns(new Mock<IDbConnection>().Object);
        bool resultado = false;
        try
        {
            databaseService.CambiarEstadoTicket(2, "En Proceso");
        }
        catch (Exception)
        {
            resultado = true;
        }
        Xunit.Assert.True(resultado);  
    }

    [Fact]
    public void prueba5()
    {
        mockDatabaseConnection.Setup(db => db.CreateConnection()).Returns(new Mock<IDbConnection>().Object);
        databaseService.CrearTicket("UsuarioNormal", "Solicitud de prueba");
        mockDatabaseConnection.Verify(db => db.CreateConnection(), Times.Once);
    }
}