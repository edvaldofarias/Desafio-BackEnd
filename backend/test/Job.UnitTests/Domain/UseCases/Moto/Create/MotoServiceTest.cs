using Job.Commons.Domain.Commands.Moto;
using Job.Domain.Entities.Moto;
using Job.Domain.UseCases.Moto.Create;
using Job.Domain.UseCases.Moto.Create.Commands;
using Job.Domain.UseCases.Moto.Create.Commands.Validations;

namespace Job.UnitTests.Domain.UseCases.Moto.Create;

[Trait("MotoService", "Create")]
public sealed class MotoServiceTest
{
    private readonly Mock<IMotoRepository> _motoRepository = new();
    private readonly Mock<ILogger<MotoService>> _logger = new();
    private readonly IValidator<CreateMotoCommand> _validator = new CreateMotoValidation();
    private readonly MotoService _motoService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public MotoServiceTest()
    {
        _motoService = new MotoService(_logger.Object, _validator, _motoRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCommandIsValid_ShouldCreateMoto()
    {
        // Arrange
        var command = CreateMotoCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.CheckPlateExistsAsync(command.Plate, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var response = await _motoService.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        _motoRepository.Verify(x => x.CreateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenPlateExists_ShouldReturnError()
    {
        // Arrange
        var command = CreateMotoCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.CheckPlateExistsAsync(command.Plate, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var response = await _motoService.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        _motoRepository.Verify(x => x.CreateAsync(It.IsAny<MotoEntity>(), _cancellationToken), Times.Never);
    }
}