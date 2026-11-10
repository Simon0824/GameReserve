using FluentAssertions;
using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;

public class IdentityChangePasswordCommand
{
    private const string TestPassword = "Password123!";
    private const string TestNewPassword = "NewPassword123!";

    private User CreateTestUser()
    {
        return User.Create("FullName", "test@email.com");
    }

    private ChangePasswordCommand CreateCommand(string userId, string currentPassword, string newPassword)
    {
        return new ChangePasswordCommand(userId, currentPassword, newPassword);
    }

    private readonly ChangePasswordCommandHandler _handler;
    private readonly IUserRepository _userRepo;

    public IdentityChangePasswordCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _handler = new(_userRepo);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(null));

        // Act
        Result result = await _handler.Handle(
            CreateCommand(user.Id, TestPassword, TestNewPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.UserNotFound");
        result.Error.Description.Should().Be("User not found in DB");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenInvalidPassword()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(CreateTestUser()));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>())
            .Returns(false);

        // Act
        Result result = await _handler.Handle(
            CreateCommand(user.Id, TestPassword, TestNewPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.NotValidPass");
        result.Error.Description.Should().Be("Password not valid");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenChangePasswordFails()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(CreateTestUser()));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>())
            .Returns(true);

        _userRepo.ChangePassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed());

        // Act
        Result result = await _handler.Handle(
            CreateCommand(user.Id, TestPassword, TestNewPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.CannotChangePassword");
        result.Error.Description.Should().Be("Cannot change a user password");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenNewPasswordCorrect()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>())
            .Returns(true);

        _userRepo.ChangePassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        // Act
        Result result = await _handler.Handle(
            CreateCommand(user.Id, TestPassword, TestNewPassword), default);

        // Assert
        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        await _userRepo.Received(1)
            .FindUser(Arg.Is<string>(user.Id), default);

        await _userRepo.Received(1)
            .CheckPassword(Arg.Is<User>(user), Arg.Is<string>(TestPassword));

        await _userRepo.Received(1)
            .ChangePassword(
                Arg.Is<User>(user),
                Arg.Is<string>(TestPassword),
                Arg.Is<string>(TestNewPassword));
    }
}
