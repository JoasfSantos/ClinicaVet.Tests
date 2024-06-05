using ClinicaVet.Repositories;
using ClinicaVet.Model;
using ClinicaVet.ViewModel;
using Moq;
using ClinicaVet.Data;
using ClinicaVet.View;

namespace ClinicaVet.Tests
{
    public class PagLoginViewModelTests
    {
        public readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public readonly Mock<UsuarioRepository> _usuarioRepositoryMock;
        public readonly Mock<MyDbContext> _dbContextMock;

        public PagLoginViewModelTests()
        {
            _dbContextMock = new Mock<MyDbContext>();
            _usuarioRepositoryMock = new Mock<UsuarioRepository>(_dbContextMock.Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.UsuarioRepository).Returns(_usuarioRepositoryMock.Object);
        }

        [Fact]
        public async Task ValidarLogin_BothFieldsFilled_ReturnsTrue()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var viewModel = new PagLoginViewModel(unitOfWork);

            viewModel.Email = "test@test.com";
            viewModel.Senha = "password";

            // Act
            var result = viewModel.ValidarLogin();

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("test@test.com", null)]
        [InlineData(null, null)]
        public async Task ValidarLogin_NullFields_ReturnsFalse(string email, string senha)
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var viewModel = new PagLoginViewModel(unitOfWork);

            viewModel.Email = email;
            viewModel.Senha = senha;

            // Act
            var result = viewModel.ValidarLogin();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task OnLoginClicked_ValidLogin_FindsUser()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var viewModel = new PagLoginViewModel(unitOfWork);

            var email = "test@test.com";
            var senha = "password";
            var usuario = new Usuario("Tester", email, senha, false);

            // Configurando o comportamento do mock do usuário repository
            _usuarioRepositoryMock.Setup(repo => repo.GetUserByEmailAndPassword(email, senha)).ReturnsAsync(usuario);

            viewModel.Email = email;
            viewModel.Senha = senha;

            // Act
            await viewModel.OnLoginClicked();

            // Assert
            // Verificando se o método GetUserByEmailAndPassword foi chamado corretamente uma vez.
            _usuarioRepositoryMock.Verify(repo => repo.GetUserByEmailAndPassword(email, senha), Times.Once);
        }

        [Fact]
        public async Task OnLoginClicked_InvalidLogin_DoesNotFindUser()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var viewModel = new PagLoginViewModel(unitOfWork);

            var email = "test@test.com";
            var senha = "wrongpassword";

            // Configurando o comportamento do mock do usuário repository para retornar null
            _usuarioRepositoryMock.Setup(repo => repo.GetUserByEmailAndPassword(email, senha)).ReturnsAsync((Usuario)null);

            viewModel.Email = email;
            viewModel.Senha = senha;

            // Act
            await viewModel.OnLoginClicked();

            // Assert
            // Verificando se o método GetUserByEmailAndPassword foi chamado corretamente
            _usuarioRepositoryMock.Verify(repo => repo.GetUserByEmailAndPassword(email, senha), Times.Once);
        }
    }
}