using Moq;
using ClinicaVet.Data;
using ClinicaVet.Model;
using ClinicaVet.Repositories;
using ClinicaVet.ViewModel;


namespace ClinicaVet.Tests
{
    public class PaginaPrincipalViewModelTests
    {
        public readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public readonly Mock<UsuarioRepository> _usuarioRepositoryMock;
        public readonly Mock<AgendamentoRepository> _agendamentoRepositoryMock;
        public readonly Mock<MyDbContext> _dbContextMock;
        public readonly PaginaPrincipalViewModel _viewModel;
        public readonly Usuario _usuario;
        public readonly Agendamento _agendamento;

        public PaginaPrincipalViewModelTests()
        {
            _dbContextMock = new Mock<MyDbContext>();
            _usuarioRepositoryMock = new Mock<UsuarioRepository>(_dbContextMock.Object);
            _agendamentoRepositoryMock = new Mock<AgendamentoRepository>(_dbContextMock.Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.UsuarioRepository).Returns(_usuarioRepositoryMock.Object);
            _unitOfWorkMock.Setup(a => a.AgendamentoRepository).Returns(_agendamentoRepositoryMock.Object);
            _usuario = new Usuario("Tester", "test@test.com", "password", false);
            _agendamento = new Agendamento(DateTime.Now, "AGENDADO", "CACHORRO", 1, "Tester", 0);
            _viewModel = new PaginaPrincipalViewModel(_unitOfWorkMock.Object, _usuario);
        }

        [Fact]
        public async Task TestOnEditarClickedAsync()
        {
            // Definir novos valores da ViewModel
            _viewModel.NomeNovo = "Novo Tester";
            _viewModel.EmailNovo = "novo@teste.com";
            _viewModel.SenhaNovo = "87654321";

            // Chamar o método OnEditarClickedAsync
            await _viewModel.OnEditarClickedAsync();

            // Verificar se os valores foram alterados
            Assert.Equal(_viewModel.NomeNovo, _viewModel.Nome);
            Assert.Equal(_viewModel.EmailNovo, _viewModel.Email);
            Assert.Equal(_viewModel.SenhaNovo, _viewModel.Senha);

            // Verificar se o método AtualizarUsuario foi chamado
            _unitOfWorkMock.Verify(u => u.UsuarioRepository.Update(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task TestAtualizarUsuarioAsync()
        {
            // Definir novos valores do usuário
            _usuario.Nome = "Novo Tester";
            _usuario.Email = "novo@teste.com";
            _usuario.Senha = "87654321";

            // Mockar o retorno do método GetAgendamentosByIdTutor
            var agendamentos = new List<Agendamento> { _agendamento };
            _agendamentoRepositoryMock.Setup(a => a.GetAgendamentosByIdTutor(It.IsAny<int>())).ReturnsAsync(agendamentos);

            // Chamar o método AtualizarUsuario
            await _viewModel.AtualizarUsuario();

            // Verificar se os valores foram alterados
            Assert.Equal(_usuario.Nome, agendamentos[0].NomeTutor);

            
            _usuarioRepositoryMock.Verify(u => u.Update(It.IsAny<Usuario>()), Times.Once); // Verificar se o método Update foi chamado
            _agendamentoRepositoryMock.Verify(a => a.Update(It.IsAny<Agendamento>()), Times.Exactly(agendamentos.Count)); // Verificar se o método Update de agendamentos foi chamado pela número de agendamentos existentes.
        }

    }
}
