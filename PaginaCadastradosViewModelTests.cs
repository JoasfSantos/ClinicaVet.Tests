using ClinicaVet.Model;
using ClinicaVet.Repositories;
using ClinicaVet.ViewModel;
using Moq;
using ClinicaVet.Data;


namespace ClinicaVet.Tests
{
    public class PaginaCadastradosViewModelTests
    {
        public readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public readonly Mock<UsuarioRepository> _usuarioRepositoryMock;
        public readonly Mock<AgendamentoRepository> _agendamentoRepositoryMock;
        public readonly Mock<MyDbContext> _dbContextMock;

        public PaginaCadastradosViewModelTests()
        {
            _dbContextMock = new Mock<MyDbContext>();
            _usuarioRepositoryMock = new Mock<UsuarioRepository>(_dbContextMock.Object);
            _agendamentoRepositoryMock = new Mock<AgendamentoRepository>(_dbContextMock.Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.UsuarioRepository).Returns(_usuarioRepositoryMock.Object);
            _unitOfWorkMock.Setup(a => a.AgendamentoRepository).Returns(_agendamentoRepositoryMock.Object);
        }

        [Fact]
        public async Task LoadUsuariosAsync_LoadsNonColaboradores()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var usuario = new Usuario("Tester", "test@test.com", "password", false);
            var viewModel = new PaginaCadastradosViewModel(unitOfWork);

            var usuarios = new List<Usuario>
                {
                    new Usuario("Tester1", "test1@test.com", "password", false),
                    new Usuario("Tester2", "test2@test.com", "password", false),
                    new Usuario("Tester3", "test3@test.com", "password", true),
                };
            var nonColaboradores = usuarios.Where(u => !u.Colaborador).ToList(); // Cria a lista de não colaboradore.
            _unitOfWorkMock.Setup(u => u.UsuarioRepository.GetNonColaboradores()).ReturnsAsync(nonColaboradores); // Configura o mock para retornar a lista de não colaboradores.

            // Act
            await viewModel.LoadUsuariosAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.UsuarioRepository.GetNonColaboradores(), Times.Exactly(2)); //Verifica se a função foi chamada exatamente duas vezes.
            Assert.Equal(nonColaboradores, viewModel.Usuarios); // Verifica se a lista de usuários retornada e igual à lista de não colaboradores 
        }


        [Fact]
        public async Task OnExcluirClickedAsync_NoAgendamentos_DeletesUsuario()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var viewModel = new PaginaCadastradosViewModel(unitOfWork);

            var usuarioSelecionado = new Usuario("Tester1", "test1@test.com", "password", false);
            usuarioSelecionado.Id = 1;

            // Configurando o mock para retornar uma lista vazia de agendamentos
            _unitOfWorkMock.Setup(u => u.AgendamentoRepository.GetAgendamentosByIdTutor(usuarioSelecionado.Id)).ReturnsAsync(new List<Agendamento>());

            // Act
            await viewModel.OnExcluirClickedAsync(usuarioSelecionado);

            // Assert
            // Verificando se o método Remove foi chamado exatamente uma vez
            _unitOfWorkMock.Verify(u => u.UsuarioRepository.Remove(usuarioSelecionado), Times.Once);
        }

        [Fact]
        public async Task OnExcluirClickedAsync_HasAgendamentos_DoesNotDeleteUsuario()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var viewModel = new PaginaCadastradosViewModel(unitOfWork);

            var usuarioSelecionado = new Usuario("Tester1", "test1@test.com", "password", false);
            usuarioSelecionado.Id = 1;

            // Configurando o mock para retornar uma lista de agendamentos
            var agendamentos = new List<Agendamento> { new Agendamento(DateTime.Now, "AGENDADO", "CACHORRO", 1, "Tester", 0) };
            _unitOfWorkMock.Setup(a => a.AgendamentoRepository.GetAgendamentosByIdTutor(usuarioSelecionado.Id)).ReturnsAsync(agendamentos);

            // Act
            await viewModel.OnExcluirClickedAsync(usuarioSelecionado);

            // Assert
            _unitOfWorkMock.Verify(u => u.UsuarioRepository.Remove(usuarioSelecionado), Times.Never); // Verificando se o método Remove nunca foi chamado
        }
    }
}