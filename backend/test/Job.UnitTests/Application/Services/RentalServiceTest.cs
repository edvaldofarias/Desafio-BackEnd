using Job.Application.Repositories;
using Job.Application.Services;
using Job.Commons.Domain.Commands.Rent;
using Job.Commons.Domain.Entities.Moto;
using Job.Commons.Domain.Entities.Rental;
using Job.Commons.Domain.Entities.User;
using Job.Domain.Entities.Moto;
using Job.Domain.Entities.Rental;
using Job.Domain.Entities.User;
using Job.Domain.Enums;


namespace Job.UnitTests.Application.Services;

[Trait("Services", "RentalService")]
public class RentalServiceTest
{
    private readonly Mock<ILogger<RentalService>> _logger = new();
    private readonly Mock<IRentalRepository> _rentRepository = new();
    private readonly Mock<IMotoRepository> _motoRepository = new();
    private readonly Mock<IMotoboyRepository> _motoboyRepository = new();
    private readonly RentalService _rentalService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public RentalServiceTest()
    {
        _rentalService = new RentalService(
            _logger.Object,
            _rentRepository.Object,
            _motoboyRepository.Object,
            _motoRepository.Object);
    }

    #region CreateRentAsync

    [Fact]
    public async Task CreateRentAsync_WhenCommandIsValid_ShouldCreateRent()
    {
        // Arrange
        var command = CreateRentCommandFaker.Default().Generate();
        var moto = MotoEntityFaker.Default().Generate();
        var motoboy = MotoboyEntityFaker.DefaultWithTypeCnh(ECnhType.A).Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(command.IdMoto, _cancellationToken))
            .ReturnsAsync(moto);
        _motoboyRepository.Setup(x => x.GetByCnpjAsync(command.Cnpj, _cancellationToken))
            .ReturnsAsync(motoboy);
        _rentRepository.Setup(x => x.CreateAsync(It.IsAny<RentalEntity>(), _cancellationToken));

        // Act
        var response = await _rentalService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeSuccess();
        _motoRepository.Verify(x => x.GetByIdAsync(command.IdMoto, _cancellationToken), Times.Once);
        _motoboyRepository.Verify(x => x.GetByCnpjAsync(command.Cnpj, _cancellationToken), Times.Once);
        _rentRepository.Verify(x => x.CreateAsync(It.IsAny<RentalEntity>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateRentAsync_WhenMotoNotFound_ShouldReturnError()
    {
        // Arrange
        var command = CreateRentCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdAsync(command.IdMoto, _cancellationToken))
            .ReturnsAsync((MotoEntity?)null);


        // Act
        var response = await _rentalService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeFailure();
        _motoRepository.Verify(x => x.GetByIdAsync(command.IdMoto, _cancellationToken), Times.Once);
        _rentRepository.Verify(x => x.CreateAsync(It.IsAny<RentalEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CreateRentAsync_WhenMotoboyNotFound_ShouldReturnError()
    {
        // Arrange
        var command = CreateRentCommandFaker.Default().Generate();
        _motoboyRepository.Setup(x => x.GetByCnpjAsync(command.Cnpj, _cancellationToken))
            .ReturnsAsync((MotoboyEntity?)null);


        // Act
        var response = await _rentalService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeFailure();
        _motoboyRepository.Verify(x => x.GetByCnpjAsync(command.Cnpj, _cancellationToken), Times.Once);
        _rentRepository.Verify(x => x.CreateAsync(It.IsAny<RentalEntity>(), _cancellationToken), Times.Never);
    }

    #endregion

    #region CancelRentAsync

    [Fact]
    public async Task CancelRentAsync_WhenCommandIsValid_ShouldCancelRent()
    {
        // Arrange
        var command = CancelRentCommandFaker.Default().Generate();
        var rent = RentalEntityFaker.Default().Generate();
        _rentRepository.Setup(x => x.GetByIdAsync(command.Id, _cancellationToken))
            .ReturnsAsync(rent);
        _rentRepository.Setup(x => x.UpdateAsync(rent, _cancellationToken));
        var fine = rent.CalculateFine(DateOnly.FromDateTime(command.DatePreview));

        // Act
        var response = await _rentalService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeSuccess();
        response.Value.Fine.Should().Be(fine);
        _rentRepository.Verify(x => x.GetByIdAsync(command.Id, _cancellationToken), Times.Once);
        _rentRepository.Verify(x => x.UpdateAsync(rent, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CancelRentAsync_WhenRentNotFound_ShouldReturnError()
    {
        // Arrange
        var command = CancelRentCommandFaker.Default().Generate();
        _rentRepository.Setup(x => x.GetByIdAsync(command.Id, _cancellationToken))
            .ReturnsAsync((RentalEntity?)null);

        // Act
        var response = await _rentalService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeFailure();
        _rentRepository.Verify(x => x.GetByIdAsync(command.Id, _cancellationToken), Times.Once);
        _rentRepository.Verify(x => x.UpdateAsync(It.IsAny<RentalEntity>(), _cancellationToken), Times.Never);
    }


    #endregion
}