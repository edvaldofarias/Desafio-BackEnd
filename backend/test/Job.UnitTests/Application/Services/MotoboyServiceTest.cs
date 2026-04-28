using Bogus;
using Job.Application.Commands.Motoboy;
using Job.Application.Repositories;
using Job.Application.Services;
using Job.Commons.Domain.Commands.User.Motoboy;
using Job.Commons.Domain.Entities.User;
using Job.Domain.Commons;
using Job.Domain.Entities.User;

namespace Job.UnitTests.Application.Services;

[Trait("Services", "MotoboyService")]
public class MotoboyServiceTest
{
    private readonly Mock<ILogger<MotoboyService>> _logger = new();
    private readonly Mock<IMotoboyRepository> _repository = new();
    private readonly Mock<IFileStorageService> _fileStorage = new();
    private readonly MotoboyService _motoboyService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public MotoboyServiceTest()
    {
        _motoboyService = new MotoboyService(_logger.Object, _repository.Object, _fileStorage.Object);
    }

    [Fact]
    public async Task Authentication_WhenValid_ReturnsMotoboy()
    {
        var password = new Faker().Internet.Password();
        var command = AuthenticationMotoboyCommandFaker.Default(password).Generate();
        var motoboy = MotoboyEntityFaker.Default(password).Generate();
        var cnpj = CnpjValidation.FormatCnpj(command.Cnpj);
        _repository.Setup(x => x.GetByCnpjAsync(cnpj, _cancellationToken))
            .ReturnsAsync(motoboy);

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeSuccess();
        response.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Authentication_WhenMotoboyNotFound_ReturnsNullValue()
    {
        var command = AuthenticationMotoboyCommandFaker.Default().Generate();
        _repository.Setup(x => x.GetByCnpjAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync((MotoboyEntity?)null);

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeSuccess();
        response.Value.Should().BeNull();
    }

    [Fact]
    public async Task Authentication_WhenInvalid_ShouldFail()
    {
        var command = AuthenticationMotoboyCommandFaker.Invalid().Generate();

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
    }

    [Fact]
    public async Task Create_WhenValid_ShouldCreate()
    {
        var command = CreateMotoboyCommandFaker.Default().Generate();

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeSuccess();
        _repository.Verify(x => x.CreateAsync(It.IsAny<MotoboyEntity>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Create_WhenIdentifierExists_ShouldFail()
    {
        var command = CreateMotoboyCommandFaker.Default().Generate();
        _repository.Setup(x => x.CheckIdentifierExistsAsync(command.Identifier!, _cancellationToken))
            .ReturnsAsync(true);

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        _repository.Verify(x => x.CreateAsync(It.IsAny<MotoboyEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Create_WhenCnpjExists_ShouldFail()
    {
        var command = CreateMotoboyCommandFaker.Default().Generate();
        _repository.Setup(x => x.CheckCnpjExistsAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(true);

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        _repository.Verify(x => x.CreateAsync(It.IsAny<MotoboyEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Create_WhenCnhExists_ShouldFail()
    {
        var command = CreateMotoboyCommandFaker.Default().Generate();
        _repository.Setup(x => x.CheckCnhExistsAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(true);

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        _repository.Verify(x => x.CreateAsync(It.IsAny<MotoboyEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Create_WhenInvalid_ShouldFail()
    {
        var command = CreateMotoboyCommandFaker.Invalid().Generate();

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        _repository.Verify(x => x.CreateAsync(It.IsAny<MotoboyEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task UploadCnh_WhenImageExceeds1Mb_ShouldFail()
    {
        var motoboy = MotoboyEntityFaker.Default().Generate();
        _repository.Setup(x => x.GetByIdentifierAsync(motoboy.Identifier, _cancellationToken))
            .ReturnsAsync(motoboy);

        var bytes = new byte[(1024 * 1024) + 1];
        bytes[0] = 0x89;
        bytes[1] = 0x50;
        bytes[2] = 0x4E;
        bytes[3] = 0x47;
        var base64 = Convert.ToBase64String(bytes);
        var command = new UploadCnhMotoboyCommand
        {
            Identifier = motoboy.Identifier,
            CnhImage = base64
        };

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeFailure();
        response.Errors.Should().Contain(x => x.Message.Contains("1 MB"));
        _fileStorage.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>(), _cancellationToken), Times.Never);
        _repository.Verify(x => x.UpdateAsync(It.IsAny<MotoboyEntity>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task UploadCnh_WhenJpegImage_ShouldSaveSuccessfully()
    {
        var motoboy = MotoboyEntityFaker.Default().Generate();
        _repository.Setup(x => x.GetByIdentifierAsync(motoboy.Identifier, _cancellationToken))
            .ReturnsAsync(motoboy);
        _fileStorage.Setup(x => x.SaveAsync("cnh.jpg", It.IsAny<Stream>(), _cancellationToken))
            .ReturnsAsync("uploads/cnh.jpg");

        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xDB, 0x00, 0x01 };
        var base64 = Convert.ToBase64String(jpegBytes);
        var command = new UploadCnhMotoboyCommand
        {
            Identifier = motoboy.Identifier,
            CnhImage = $"data:image/jpeg;base64,{base64}"
        };

        var response = await _motoboyService.Handle(command, _cancellationToken);

        response.Should().BeSuccess();
        _fileStorage.Verify(x => x.SaveAsync("cnh.jpg", It.IsAny<Stream>(), _cancellationToken), Times.Once);
        _repository.Verify(x => x.UpdateAsync(It.IsAny<MotoboyEntity>(), _cancellationToken), Times.Once);
    }
}
