using Job.Application.Commands.Rental;
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
    private readonly Mock<IRentalRepository> _rentalRepository = new();
    private readonly Mock<IMotoRepository> _motoRepository = new();
    private readonly Mock<IMotoboyRepository> _motoboyRepository = new();
    private readonly RentalService _rentalService;
    private readonly CancellationToken _ct = CancellationToken.None;

    public RentalServiceTest()
    {
        _rentalService = new RentalService(
            _logger.Object,
            _rentalRepository.Object,
            _motoboyRepository.Object,
            _motoRepository.Object);
    }

    [Fact]
    public async Task Create_WhenValid_ShouldCreate()
    {
        var command = CreateRentCommandFaker.Default().Generate();
        var moto = MotoEntityFaker.Default().Generate();
        var motoboy = MotoboyEntityFaker.DefaultWithTypeCnh(ECnhType.A).Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(command.MotoIdentifier, _ct))
            .ReturnsAsync(moto);
        _motoboyRepository.Setup(x => x.GetByIdentifierAsync(command.MotoboyIdentifier, _ct))
            .ReturnsAsync(motoboy);

        var response = await _rentalService.Handle(command, _ct);

        response.Should().BeSuccess();
        _rentalRepository.Verify(x => x.CreateAsync(It.IsAny<RentalEntity>(), _ct), Times.Once);
    }

    [Fact]
    public async Task Create_WhenMotoboyNotEligible_ShouldFail()
    {
        var command = CreateRentCommandFaker.Default().Generate();
        var moto = MotoEntityFaker.Default().Generate();
        var motoboy = MotoboyEntityFaker.DefaultWithTypeCnh(ECnhType.B).Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(command.MotoIdentifier, _ct))
            .ReturnsAsync(moto);
        _motoboyRepository.Setup(x => x.GetByIdentifierAsync(command.MotoboyIdentifier, _ct))
            .ReturnsAsync(motoboy);

        var response = await _rentalService.Handle(command, _ct);

        response.Should().BeFailure();
        _rentalRepository.Verify(x => x.CreateAsync(It.IsAny<RentalEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Create_WhenMotoNotFound_ShouldFail()
    {
        var command = CreateRentCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(command.MotoIdentifier, _ct))
            .ReturnsAsync((MotoEntity?)null);
        _motoboyRepository.Setup(x => x.GetByIdentifierAsync(command.MotoboyIdentifier, _ct))
            .ReturnsAsync(MotoboyEntityFaker.DefaultWithTypeCnh(ECnhType.A).Generate());

        var response = await _rentalService.Handle(command, _ct);

        response.Should().BeFailure();
        _rentalRepository.Verify(x => x.CreateAsync(It.IsAny<RentalEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Create_WhenMotoboyNotFound_ShouldFail()
    {
        var command = CreateRentCommandFaker.Default().Generate();
        _motoRepository.Setup(x => x.GetByIdentifierAsync(command.MotoIdentifier, _ct))
            .ReturnsAsync(MotoEntityFaker.Default().Generate());
        _motoboyRepository.Setup(x => x.GetByIdentifierAsync(command.MotoboyIdentifier, _ct))
            .ReturnsAsync((MotoboyEntity?)null);

        var response = await _rentalService.Handle(command, _ct);

        response.Should().BeFailure();
        _rentalRepository.Verify(x => x.CreateAsync(It.IsAny<RentalEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Return_WhenRentalFound_ShouldRegister()
    {
        var rental = RentalEntityFaker.Default().Generate();
        var command = new CancelRentalCommand(rental.Identifier, rental.DateEnd.ToDateTime(TimeOnly.MinValue));
        _rentalRepository.Setup(x => x.GetByIdentifierAsync(rental.Identifier, _ct))
            .ReturnsAsync(rental);

        var response = await _rentalService.Handle(command, _ct);

        response.Should().BeSuccess();
        _rentalRepository.Verify(x => x.UpdateAsync(rental, _ct), Times.Once);
    }

    [Fact]
    public async Task Return_WhenRentalNotFound_ShouldFail()
    {
        var command = CancelRentCommandFaker.Default().Generate();
        _rentalRepository.Setup(x => x.GetByIdentifierAsync(command.Identifier, _ct))
            .ReturnsAsync((RentalEntity?)null);

        var response = await _rentalService.Handle(command, _ct);

        response.Should().BeFailure();
        _rentalRepository.Verify(x => x.UpdateAsync(It.IsAny<RentalEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task GetById_WhenFound_ShouldReturnDto()
    {
        var rental = RentalEntityFaker.Default().Generate();
        _rentalRepository.Setup(x => x.GetByIdentifierAsync(rental.Identifier, _ct))
            .ReturnsAsync(rental);

        var response = await _rentalService.Handle(new GetRentalCommand(rental.Identifier), _ct);

        response.Should().BeSuccess();
        response.Value.Identifier.Should().Be(rental.Identifier);
    }
}
