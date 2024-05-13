using Job.Commons.Domain.Commands.Moto;
using Job.Commons.Domain.Entities.Moto;
using Job.Commons.Domain.Entities.Rental;
using Job.Domain.Entities.Moto;
using Job.Domain.Services;

namespace Job.UnitTests.Domain.Services;

[Trait("Services", "MotoService")]
public sealed class MotoServiceTest
{
    private readonly Mock<IMotoRepository> _motoRepository = new();
    private readonly Mock<ILogger<MotoService>> _logger = new();
    private readonly MotoService _motoService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public MotoServiceTest()
    {
        _motoService = new MotoService(_logger.Object, _motoRepository.Object);
    }


    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_WhenMotosExists_ShouldReturnMotos()
    {
        // Arrange
        var entities = MotoEntityFaker.Default().Generate(30);
        _motoRepository.Setup(x => x.GetAllAsync(_cancellationToken))
            .ReturnsAsync(entities);

        // Act
        var response = await _motoService.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
    }

    [Fact]
    public async Task GetAllAsync_WhenMotosNotExists_ShouldReturnEmptyList()
    {
        // Arrange
        _motoRepository.Setup(x => x.GetAllAsync(_cancellationToken))
            .ReturnsAsync(new List<MotoEntity>());

        // Act
        var response = await _motoService.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenMotoExists_ShouldReturnMoto()
    {
        // Arrange
        var entity = MotoEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync(entity);

        // Act
        var response = await _motoService.GetByIdAsync(entity.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMotoNotExists_ShouldReturnNull()
    {
        // Arrange
        _motoRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);

        // Act
        var response = await _motoService.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(response);
    }

    #endregion

    #region GetByPlaceAsync

    [Fact]
    public async Task GetByPlateAsync_WhenMotoExists_ShouldReturnMoto()
    {
        // Arrange
        var entity = MotoEntityFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByPlateAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(entity);

        // Act
        var response = await _motoService.GetByPlateAsync(entity.Plate, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetByPlateAsync_WhenMotoNotExists_ShouldReturnNull()
    {
        // Arrange
        _motoRepository.Setup(x => x.GetByPlateAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);

        // Act
        var response = await _motoService.GetByPlateAsync(It.IsAny<string>(), CancellationToken.None);

        // Assert
        Assert.Null(response);
    }
    #endregion
}