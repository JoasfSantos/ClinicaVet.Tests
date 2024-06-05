using ClinicaVet.Model;
using ClinicaVet.Repositories;
using ClinicaVet.ViewModel;
using Moq;
using ClinicaVet.Data;


namespace ClinicaVet.Tests
{
    public class PagAgendamentosViewModelTests
    {
        public readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public readonly Mock<AgendamentoRepository> _agendamentoRepositoryMock;
        public readonly Mock<MyDbContext> _dbContextMock;

        public PagAgendamentosViewModelTests()
        {
            _dbContextMock = new Mock<MyDbContext>();
            _agendamentoRepositoryMock = new Mock<AgendamentoRepository>(_dbContextMock.Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(a => a.AgendamentoRepository).Returns(_agendamentoRepositoryMock.Object);
        }

        [Fact]
        public async Task LoadAgendamentosAsync_LoadsAllAgendamentos()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var usuario = new Usuario("Tester", "test@test.com", "password", false);
            var viewModel = new PagAgendamentosViewModel(usuario, unitOfWork, true);

            var agendamentos = new List<Agendamento>
            {
                new Agendamento(DateTime.Now, "AGENDADO", "CACHORRO", 1, "Tester", 0),
                new Agendamento(DateTime.Now.AddDays(1), "AGENDADO", "CACHORRO", 2, "Tester2", 0),
            };

            _unitOfWorkMock.Setup(a => a.AgendamentoRepository.GetAll()).ReturnsAsync(agendamentos);

            // Act
            await viewModel.LoadAgendamentosAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.AgendamentoRepository.GetAll(), Times.Exactly(2));
            Assert.Equal(agendamentos, viewModel.Agendamentos);
        }

        [Fact]
        public async Task LoadAgendamentosTutor_LoadsTutorAgendamentos()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object; // Obtenção do objeto mockado do UnitOfWork
            var usuario = new Usuario("Tester", "test@test.com", "password", false);
            usuario.Id = 1; // Definindo o ID do usuário
            var viewModel = new PagAgendamentosViewModel(usuario, unitOfWork, false);

            var agendamentos = new List<Agendamento> // Criação de uma lista de agendamentos de teste
                {
                    new Agendamento(DateTime.Now, "AGENDADO", "CACHORRO", 1, "Tester", 0),
                    new Agendamento(DateTime.Now.AddDays(1), "AGENDADO", "CACHORRO", 2, "Tester2", 0),
                };
            var agendamentosDoTutor = agendamentos.Where(a => a.IdTutor == usuario.Id).ToList(); // Filtrando os agendamentos que pertencem ao tutor
            _unitOfWorkMock.Setup(a => a.AgendamentoRepository.GetAgendamentosByIdTutor(usuario.Id)).ReturnsAsync(agendamentosDoTutor); // Configurando o mock para retornar os agendamentos do tutor quando o método GetAgendamentosByIdTutor for chamado

            // Act
            await viewModel.LoadAgendamentosTutor();

            // Assert
            _unitOfWorkMock.Verify(a => a.AgendamentoRepository.GetAgendamentosByIdTutor(usuario.Id), Times.Exactly(2)); // Verificando se o método GetAgendamentosByIdTutor foi chamado exatamente duas vezes
            Assert.Equal(agendamentosDoTutor, viewModel.Agendamentos); // Verificando se os agendamentos do ViewModel são iguais aos agendamentos do tutor
        }

        [Fact]
        public async Task OnExcluirClickedAsync_DeletesAgendamento()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var usuario = new Usuario("Tester", "test@test.com", "password", false);
            var viewModel = new PagAgendamentosViewModel(usuario, unitOfWork, true);

            var agendamento = new Agendamento(DateTime.Now, "AGENDADO", "CACHORRO", 1, "Tester", 0);

            // Act
            await viewModel.OnExcluirClickedAsync(agendamento);

            // Assert
            _unitOfWorkMock.Verify(a => a.AgendamentoRepository.Remove(agendamento), Times.Once()); //Verifica se a função foi chamada exatamente uma vez.
        }

        [Fact]
        public async Task OnEditarStatusAgendamento_UpdatesAgendamentoStatus()
        {
            // Arrange
            var unitOfWork = _unitOfWorkMock.Object;
            var usuario = new Usuario("Tester", "test@test.com", "password", false);
            var viewModel = new PagAgendamentosViewModel(usuario, unitOfWork, true);

            var agendamento = new Agendamento(DateTime.Now, "AGENDADO", "CACHORRO", 1, "Tester", 0);

            // Act
            await viewModel.OnEditarStatusAgendamento(agendamento);

            // Assert
            _unitOfWorkMock.Verify(a => a.AgendamentoRepository.Update(agendamento), Times.Once); //Verifica se a função foi chamada exatamente uma vez.
            Assert.Equal("EM ANDAMENTO", agendamento.Status); // Verifica se o status do agendamento foi alterado.
        }
    }
}