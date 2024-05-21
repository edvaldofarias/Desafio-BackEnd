using Job.Commons.Domain.Entities.Moto;
using Job.Domain.Entities.Moto;
using Job.Domain.UseCases.Moto.GetAll;
using Job.Domain.UseCases.Moto.GetAll.Queries;

namespace Job.UnitTests.Domain.UseCases.Moto.GetAll;

[Trait("MotoService", "GetAll")]
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

    [Fact]
    public async Task GetAllAsync_WhenMotosExists_ShouldReturnMotos()
    {
        // Arrange
        var query = new GetAllMotoQuery();
        var entities = MotoEntityFaker.Default().Generate(30);
        _motoRepository.Setup(x => x.GetAllAsync(query.Quantity, query.Page, _cancellationToken))
            .ReturnsAsync(entities);

        // Act
        var response = await _motoService.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
    }

    [Fact]
    public async Task GetAllAsync_WhenMotosNotExists_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllMotoQuery();
        _motoRepository.Setup(x => x.GetAllAsync(query.Quantity, query.Page, _cancellationToken))
            .ReturnsAsync(new List<MotoEntity>());

        // Act
        var response = await _motoService.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response);
    }
}
