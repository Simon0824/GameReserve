
using FluentAssertions;
using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;

public class IdentityBanUserCommand
{
    private readonly BanUserCommand Command = new("test@email.com");

    private User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", Command.Email);

        if (isBanned)
            user.BanUser();

        return user;
    }

    private readonly BanUserCommandHandler _handler;
    private readonly IUserRepository _userRepo;

    public IdentityBanUserCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _handler = new(_userRepo);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(null));

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.UserNotFound");
        result.Error.Description.Should().Be("User not found in DB");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserIsAlreadyBanned()
    {
        // Arrange
        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(CreateTestUser(true)));

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.UserIsBanned");
        result.Error.Description.Should().Be("User is banned");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserBanned()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        await _userRepo.Received(1)
            .FindUserByEmail(Arg.Is<string>(user.Email!), default);

        await _userRepo.Received(1)
            .UpdateAsync(Arg.Is<User>(user));
    }
}
