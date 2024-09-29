﻿using Job.Application.Commands.Manager;
using Job.Application.Commands.Manager.Validations;
using Job.Application.Repositories;
using Job.Application.Services;
using Job.Commons.Application.Commands.Manager;
using Job.Commons.Domain.Entities.User;
using Job.Domain.Commons;
using Job.Domain.Entities.User;

namespace Job.UnitTests.Application.Services;

[Trait("Services", "ManagerService")]
public class ManagerServiceTest
{
    private readonly Mock<ILogger<ManagerService>> _logger = new();
    private readonly Mock<IManagerRepository> _managerRepository = new();
    private readonly IValidator<AuthenticationManagerCommand> _validator = new AuthenticationManagerValidation();
    private readonly ManagerService _managerService;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ManagerServiceTest()
    {
        _managerService = new ManagerService(_logger.Object, _managerRepository.Object, _validator);
    }

    #region GetManager

    [Fact]
    public async Task GetManager_WhenCommandIsValid_ShouldReturnManager()
    {
        // Arrange
        var command = AuthenticationManagerCommandFaker.Default().Generate();
        var manager = ManagerEntityFaker.Default().Generate();
        _managerRepository.Setup(x => x.GetAsync(command.Email, Cryptography.Encrypt(command.Password), _cancellationToken))
            .ReturnsAsync(manager);

        // Act
        var response = await _managerService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeSuccess();
        _managerRepository.Verify(x => x.GetAsync(command.Email, Cryptography.Encrypt(command.Password), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetManager_WhenCommandIsInvalid_ShouldReturnNull()
    {
        // Arrange
        var command = AuthenticationManagerCommandFaker.Invalid().Generate();
        _managerRepository.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync((ManagerEntity?)null);

        // Act
        var response = await _managerService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeFailure();
        response.Errors.Should().HaveCount(2);
        _managerRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task GetManager_WhenCommandIsNull_ShouldReturnNull()
    {
        // Arrange
        var command = AuthenticationManagerCommandFaker.Default().Generate();
        _managerRepository.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync((ManagerEntity?)null);

        // Act
        var response = await _managerService.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeSuccess();
        response.Value.Should().BeNull();
        _managerRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), _cancellationToken), Times.Once);
    }

    #endregion
}