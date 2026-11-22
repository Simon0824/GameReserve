using FluentAssertions;
using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using NSubstitute;
using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;

public class IdentityBanUserCommand
{
    private User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", "email@test.com");

        if (isBanned)
            user.BanUser();

        return user;
    }

    private BanUserCommand Command(Guid userId) => new(userId);

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
        var user = CreateTestUser(true);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(null));

        // Act
        Result result = await _handler.Handle(Command(Guid.Parse(user.Id)), default);

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
        var user = CreateTestUser(true);

        _userRepo.FindUser(Arg.Any<Guid>(), default)
            .Returns(Task.FromResult<User?>(user));

        _blacklistRepo.BlackListUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(false);

        // Act
        Result result = await _handler.Handle(Command(Guid.Parse(user.Id)), default);

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

        _userRepo.FindUser(Arg.Any<Guid>(), default)
            .Returns(Task.FromResult<User?>(user));

        

        _blacklistRepo.BlackListUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(true);

        // Act
        Result result = await _handler.Handle(Command(Guid.Parse(user.Id)), default);

        // Assert
        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        await _userRepo.Received(1)
            .FindUser(Arg.Is<Guid>(Guid.Parse(user.Id)), default);

        await _blacklistRepo.BlackListUserAsync(
            Arg.Is<string>(user.Id), 
            Arg.Is<string>(string.Empty), 
            Arg.Is<TimeSpan>(TimeSpan.Zero));

        await _userRepo.Received(1)
            .SaveChangesAsync(default);
    }
}
