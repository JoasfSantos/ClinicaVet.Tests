using Moq;
using ClinicaVet.Repositories;
using ClinicaVet.Data;
using ClinicaVet.Model;
using ClinicaVet.ViewModel;


namespace ClinicaVet.Tests
{
    public class RegistroAgendamentoViewModelTests
    {
        public readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public readonly Mock<UsuarioRepository> _usuarioRepositoryMock;
        public readonly Mock<AgendamentoRepository> _agendamentoRepositoryMock;
        public readonly Mock<MyDbContext> _dbContextMock;
        public readonly RegistroAgendamentoViewModel _viewModelFluxoNormal;
        public readonly RegistroAgendamentoViewModel _viewModelFluxoEdicao;
        public readonly Usuario _usuario;
        public readonly Agendamento _agendamento;

        public RegistroAgendamentoViewModelTests()
        {
            _dbContextMock = new Mock<MyDbContext>();
            _usuarioRepositoryMock = new Mock<UsuarioRepository>(_dbContextMock.Object);
            _agendamentoRepositoryMock = new Mock<AgendamentoRepository>(_dbContextMock.Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.UsuarioRepository).Returns(_usuarioRepositoryMock.Object);
            _unitOfWorkMock.Setup(a => a.AgendamentoRepository).Returns(_agendamentoRepositoryMock.Object);
            _usuario = new Usuario("Tester", "test@test.com", "password", false);
            _agendamento = new Agendamento(DateTime.Now, "AGENDADO", "CACHORRO", 1, "Tester", 0);
            _viewModelFluxoNormal = new RegistroAgendamentoViewModel(_unitOfWorkMock.Object, _usuario, false);
            _viewModelFluxoEdicao = new RegistroAgendamentoViewModel(_unitOfWorkMock.Object, _agendamento, true);
        }

        [Fact]
        public async Task TestOnRegistroAgendamentoClicked_FluxoEdicao_True()
        {
            // Arrange
            // Definir novos valores da ViewModel
            _viewModelFluxoEdicao.DataAgendamento = DateTime.Today.AddDays(1);
            _viewModelFluxoEdicao.TipoPet = "GATO";

            // Act
            // Chamar o método OnRegistroAgendamentoClicked com fluxoEdicao como true
            await _viewModelFluxoEdicao.OnRegistroAgendamentoClicked(true);

            // Assert
            // Verificar se o método Update foi chamado
            _unitOfWorkMock.Verify(a => a.AgendamentoRepository.Update(It.IsAny<Agendamento>()), Times.Once);

            // Verificar se os valores foram alterados
            Assert.Equal(_viewModelFluxoEdicao.DataAgendamento, _agendamento.DataAgendamento);
            Assert.Equal(_viewModelFluxoEdicao.TipoPet, _agendamento.TipoPet);
        }

        [Fact]
        public async Task TestOnRegistroAgendamentoClicked_FluxoEdicao_False()
        {
            // Arrange
            // Definir novos valores da ViewModel
            _viewModelFluxoNormal.DataAgendamento = DateTime.Today.AddDays(1);
            _viewModelFluxoNormal.TipoPet = "GATO";

            // Act
            // Chamar o método OnRegistroAgendamentoClicked com fluxoEdicao como false
            await _viewModelFluxoNormal.OnRegistroAgendamentoClicked(false);

            // Assert
            // Verificar se o método Add foi chamado
            _unitOfWorkMock.Verify(a => a.AgendamentoRepository.Add(It.IsAny<Agendamento>()), Times.Once);
        }

    }
}