using FluentAssertions;
using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;

public class IdentityBanUserCommand
{
    private BanUserCommand Command = new("test@email.com");

    private User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", Command.Email);

        if (isBanned)
            user.BanUser();

        return user;
    }

    private readonly BanUserCommandHandler _handler;
    private readonly IBlackListRepository _blacklistRepo;
    private readonly IUserRepository _userRepo;

    public IdentityBanUserCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _blacklistRepo = Substitute.For<IBlackListRepository>();


        _handler = new(_userRepo, _blacklistRepo);
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

        _blacklistRepo.BlackListUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(false);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.FailedToSaveBanned");
        result.Error.Description.Should().Be("Failed to save ban to Redis");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserBanned()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        _blacklistRepo.BlackListUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(true);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        await _userRepo.Received(1)
            .FindUserByEmail(Arg.Is<string>(user.Email!), default);

        await _blacklistRepo.BlackListUserAsync(
            Arg.Is<string>(user.Id), 
            Arg.Is<string>(string.Empty), 
            Arg.Is<TimeSpan>(TimeSpan.Zero));

        await _userRepo.Received(1)
            .SaveChangesAsync(default);
    }
}
