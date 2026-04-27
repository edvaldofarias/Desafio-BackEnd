using Job.Application.Commands.Moto;
using Job.Application.Messaging;
using Job.Application.Repositories;
using Job.Application.Services;
using Job.Commons.Application.Commands.Moto;
using Job.Commons.Domain.Entities.Moto;
using Job.Domain.Entities.Moto;

namespace Job.UnitTests.Application.Services;

[Trait("Services", "MotoService")]
public sealed class MotoServiceTest
{
    private readonly Mock<IMotoRepository> _motoRepository = new();
    private readonly Mock<ILogger<MotoService>> _logger = new();
    private readonly Mock<IRentalRepository> _rentRepository = new();
    private readonly Mock<IMessagePublisher> _publisher = new();
    private readonly MotoService _motoService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public MotoServiceTest()
    {
        _motoService = new MotoService(_logger.Object, _motoRepository.Object, _rentRepository.Object, _publisher.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCommandIsValid_ShouldCreateMoto()
    {
        var command = CreateMotoCommandFaker.Default().Generate();

        var response = await _motoService.Handle(command, _cancellationToken);

        response.Should().BeSuccess();
        _motoRepository.Verify(x => x.CreateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Once);
        _publisher.Verify(x => x.PublishMotoCreatedAsync(It.IsAny<MotoCreatedEvent>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenIdentifierExists_ShouldFail()
    {
        var command = CreateMotoCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.CheckIdentifierExistsAsync(command.Identifier!, _cancellationToken))
            .ReturnsAsync(true);

        var response = await _motoService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        _motoRepository.Verify(x => x.CreateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenPlateExists_ShouldFail()
    {
        var command = CreateMotoCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.CheckPlateExistsAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(true);

        var response = await _motoService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        _motoRepository.Verify(x => x.CreateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task UpdatePlate_WhenMotoFound_ShouldUpdate()
    {
        var entity = MotoEntityFaker.Default().Generate();
        var command = new UpdateMotoCommand(entity.Identifier, "NEW1234");
        _motoRepository.Setup(x => x.GetByIdentifierAsync(entity.Identifier, _cancellationToken))
            .ReturnsAsync(entity);

        var response = await _motoService.Handle(command, _cancellationToken);

        response.Should().BeSuccess();
        _motoRepository.Verify(x => x.UpdateAsync(entity, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdatePlate_WhenNotFound_ShouldFail()
    {
        var command = UpdateMotoCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(command.Identifier, _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);

        var response = await _motoService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        _motoRepository.Verify(x => x.UpdateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenMotoExists_ShouldDelete()
    {
        var entity = MotoEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(entity.Identifier, _cancellationToken))
            .ReturnsAsync(entity);

        var response = await _motoService.Handle(new DeleteMotoCommand(entity.Identifier), _cancellationToken);

        response.Should().BeSuccess();
        _motoRepository.Verify(x => x.DeleteAsync(entity, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenMotoHasRental_ShouldFail()
    {
        var entity = MotoEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(entity.Identifier, _cancellationToken))
            .ReturnsAsync(entity);
        _rentRepository.Setup(x => x.ExistsForMotoAsync(entity.Id, _cancellationToken))
            .ReturnsAsync(true);

        var response = await _motoService.Handle(new DeleteMotoCommand(entity.Identifier), _cancellationToken);

        response.Should().BeFailure();
        _motoRepository.Verify(x => x.DeleteAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task GetById_WhenFound_ShouldReturnDto()
    {
        var entity = MotoEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(entity.Identifier, _cancellationToken))
            .ReturnsAsync(entity);

        var response = await _motoService.Handle(new GetByIdMotoCommand(entity.Identifier), _cancellationToken);

        response.Should().BeSuccess();
        response.Value.Identifier.Should().Be(entity.Identifier);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldFail()
    {
        _motoRepository.Setup(x => x.GetByIdentifierAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);

        var response = await _motoService.Handle(new GetByIdMotoCommand("missing"), _cancellationToken);

        response.Should().BeFailure();
    }

    [Fact]
    public async Task GetAll_WithPlateFilter_DelegatesToSearch()
    {
        var entities = MotoEntityFaker.Default().Generate(3);
        _motoRepository.Setup(x => x.SearchAsync("ABC", _cancellationToken))
            .ReturnsAsync(entities);

        var response = await _motoService.Handle(new GetAllMotoCommand("ABC"), _cancellationToken);

        response.Should().BeSuccess();
        response.Value.Should().HaveCount(3);
    }
}
