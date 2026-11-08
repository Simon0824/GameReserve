using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using MassTransit.Batching;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;
public class IdentityBanUserCommand
{
    private static readonly BanUserCommand Command = new("test@email.com");
    private static User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", Command.Email);

        if(isBanned)
        {
            user.BanUser();
        }

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
        //Arrange
        _userRepo.FindUserByEmail(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(null));

        //Act
        Result result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserNotFound", result.Error.Code);
        Assert.Equal("User not found in DB", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserBanned()
    {
        //Arrange
        _userRepo.FindUserByEmail(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(CreateTestUser(true)));

        //Act
        Result result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserIsBanned", result.Error.Code);
        Assert.Equal("User is banned", result.Error.Description);
    }
}