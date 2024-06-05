using Moq;
using ClinicaVet.Repositories;
using ClinicaVet.Model;
using ClinicaVet.Data;
using ClinicaVet.ViewModel;

namespace ClinicaVet.Tests
{
    public class PagRegistroViewModelTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PagRegistroViewModel _viewModel;
        public readonly Mock<UsuarioRepository> _usuarioRepositoryMock;
        public readonly Mock<MyDbContext> _dbContextMock;

        public PagRegistroViewModelTests()
        {
            _dbContextMock = new Mock<MyDbContext>();
            _usuarioRepositoryMock = new Mock<UsuarioRepository>(_dbContextMock.Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.UsuarioRepository).Returns(_usuarioRepositoryMock.Object);
            _viewModel = new PagRegistroViewModel(_unitOfWorkMock.Object, false);
        }

        [Fact]
        public async Task TestOnRegistroClicked_ValidEmailAndPassword()
        {
            // Definir novos valores da ViewModel
            _viewModel.Email = "test@test.com";
            _viewModel.Senha = "12345678";

            // Chamar o método OnRegistroClicked
            await _viewModel.OnRegistroClicked();

            // Verificar se o método Add foi chamado
            _unitOfWorkMock.Verify(u => u.UsuarioRepository.Add(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task TestOnRegistroClicked_InvalidEmail()
        {
            // Definir novos valores da ViewModel
            _viewModel.Email = "invalid email";
            _viewModel.Senha = "12345678";

            // Chamar o método OnRegistroClicked
            await _viewModel.OnRegistroClicked();

            // Verificar se o método Add não foi chamado
            _unitOfWorkMock.Verify(u => u.UsuarioRepository.Add(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task TestOnRegistroClicked_InvalidPassword()
        {
            // Definir novos valores da ViewModel
            _viewModel.Email = "test@test.com";
            _viewModel.Senha = "123";

            // Chamar o método OnRegistroClicked
            await _viewModel.OnRegistroClicked();

            // Verificar se o método Add não foi chamado
            _unitOfWorkMock.Verify(u => u.UsuarioRepository.Add(It.IsAny<Usuario>()), Times.Never);
        }
    }
}