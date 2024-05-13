using Job.Commons.Domain.Entities.Moto;
using Job.Commons.Domain.Entities.Rental;
using Job.Domain.Entities.Moto;
using Job.Domain.UseCases.Moto.Delete;
using Job.Domain.UseCases.Moto.Delete.Commands;
using Job.Domain.UseCases.Moto.Delete.Commands.Validations;

namespace Job.UnitTests.Domain.UseCases.Moto.Delete;

[Trait("MotoService", "Delete")]
public sealed class MotoServiceTest
{

    private readonly Mock<IMotoRepository> _motoRepository = new();
    private readonly Mock<ILogger<MotoService>> _logger = new();
    private readonly Mock<IRentalRepository> _rentRepository = new();
    private readonly IValidator<DeleteMotoCommand> _validator = new DeleteMotoValidation();
    private readonly MotoService _motoService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public MotoServiceTest()
    {
        _motoService = new MotoService(_logger.Object, _validator, _motoRepository.Object, _rentRepository.Object);
    }

    [Fact]
    public async Task DeleteAsync_WhenMotoExists_ShouldDeleteMoto()
    {
        // Arrange
        var entity = MotoEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(entity);
        var request = new DeleteMotoCommand(entity.Id);

        // Act
        var response = await _motoService.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        _motoRepository.Verify(x => x.DeleteAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMotoNotFound_ShouldReturnError()
    {
        // Arrange
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);
        var request = new DeleteMotoCommand(Guid.NewGuid());

        // Act
        var response = await _motoService.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        _motoRepository.Verify(x => x.DeleteAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenMotoHasRent_ShouldReturnError()
    {
        // Arrange
        var motoEntity = MotoEntityFaker.Default().Generate();
        var rentEntity = RentalEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(motoEntity);
        _rentRepository.Setup(x => x.GetByMotoIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(rentEntity);
        var request = new DeleteMotoCommand(motoEntity.Id);

        // Act
        var response = await _motoService.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        _motoRepository.Verify(x => x.DeleteAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenMotoHasRentAndMotoNotFound_ShouldReturnError()
    {
        // Arrange
        var rentEntity = RentalEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);
        _rentRepository.Setup(x => x.GetByMotoIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(rentEntity);
        var request = new DeleteMotoCommand(Guid.NewGuid());

        // Act
        var response = await _motoService.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        _motoRepository.Verify(x => x.DeleteAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsEmpty_ShouldReturnError()
    {
        // Arrange
        var request = new DeleteMotoCommand(Guid.Empty);

        // Act
        var response = await _motoService.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        _motoRepository.Verify(x => x.DeleteAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }
}