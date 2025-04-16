using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using apptickets;
using System.Data;
using System;

namespace appticketstest.Tests
{
    [TestClass]
    public class ProgramTests
    {
        private Mock<IDatabaseConnection> _mockDatabaseConnection;
        private Mock<IDbConnection> _mockConnection;
        private Mock<IDbCommand> _mockCommand;
        private Mock<IDataParameterCollection> _mockParameters;
        private DatabaseService _databaseService;

        [TestInitialize]
        public void Initialize()
        {
            _mockDatabaseConnection = new Mock<IDatabaseConnection>();
            _mockConnection = new Mock<IDbConnection>();
            _mockCommand = new Mock<IDbCommand>();
            _mockParameters = new Mock<IDataParameterCollection>();

            // Configuración común
            _mockDatabaseConnection.Setup(x => x.CreateConnection())
                                .Returns(_mockConnection.Object);
            
            _mockDatabaseConnection.Setup(x => x.CreateCommand(It.IsAny<string>(), It.IsAny<IDbConnection>()))
                                .Returns(_mockCommand.Object);
            
            _mockCommand.SetupGet(x => x.Parameters).Returns(_mockParameters.Object);
            
            // Configuración para parámetros
            var mockParameter = new Mock<IDbDataParameter>();
            _mockCommand.Setup(x => x.CreateParameter()).Returns(mockParameter.Object);
            
            _databaseService = new DatabaseService(_mockDatabaseConnection.Object);
        }

        [TestMethod]
        public void prueba1() // Prueba para AgregarUsuario
        {
            // Act
            _databaseService.AgregarUsuario("testUser", true);

            // Assert
            _mockConnection.Verify(x => x.Open(), Times.Once);
            _mockCommand.Verify(x => x.ExecuteNonQuery(), Times.Once);
            _mockParameters.Verify(x => x.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(2));
        }

        [TestMethod]
        public void prueba2() // Prueba para CambiarEstadoTicket
        {
            // Act
            _databaseService.CambiarEstadoTicket(1, "Cerrado");

            // Assert
            _mockConnection.Verify(x => x.Open(), Times.Once);
            _mockCommand.Verify(x => x.ExecuteNonQuery(), Times.Once);
            _mockParameters.Verify(x => x.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(2));
        }

        [TestMethod]
        public void prueba3() // Prueba para EsAdministrador (true)
        {
            // Arrange
            var mockReader = new Mock<IDataReader>();
            mockReader.SetupSequence(x => x.Read())
                     .Returns(true)
                     .Returns(false);
            mockReader.Setup(x => x.GetBoolean(0)).Returns(true);
            _mockCommand.Setup(x => x.ExecuteReader()).Returns(mockReader.Object);

            // Act
            var resultado = _databaseService.EsAdministrador("admin");

            // Assert
            Assert.IsTrue(resultado);
            _mockParameters.Verify(x => x.Add(It.IsAny<IDbDataParameter>()), Times.Once);
        }

        [TestMethod]
        public void prueba4() // Prueba para EsAdministrador (false)
        {
            // Arrange
            var mockReader = new Mock<IDataReader>();
            mockReader.SetupSequence(x => x.Read())
                     .Returns(true)
                     .Returns(false);
            mockReader.Setup(x => x.GetBoolean(0)).Returns(false);
            _mockCommand.Setup(x => x.ExecuteReader()).Returns(mockReader.Object);

            // Act
            var resultado = _databaseService.EsAdministrador("usuarioNormal");

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void prueba5() // Prueba para CrearTicket
        {
            // Act
            _databaseService.CrearTicket("testUser", "Problema con login");

            // Assert
            _mockConnection.Verify(x => x.Open(), Times.Once);
            _mockCommand.Verify(x => x.ExecuteNonQuery(), Times.Once);
            _mockParameters.Verify(x => x.Add(It.IsAny<IDbDataParameter>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void prueba6() // Prueba adicional para validar nulos
        {
            _databaseService.EsAdministrador(null);
        }
    }
}