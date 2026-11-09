using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;
public class IdentityDeleteUserCommand
{
    private User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", "test@email.com");

        if(isBanned)
            user.BanUser();

        return user;
    }
    private DeleteUserCommand CreateCommand(string userId)
    {
        return new DeleteUserCommand(userId);
    }
    private readonly DeleteUserCommandHandler _handler;
    private readonly IUserRepository _userRepo;
    public IdentityDeleteUserCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();

        _handler = new(_userRepo);
    }

   [Fact]
    public async Task Handle_Should_ReturnError_WhenUserNotFound()
    {
        //Arrange
        var user = CreateTestUser();
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(null));

        //Act
        Result result = await _handler.Handle(CreateCommand(user.Id), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserNotFound", result.Error.Code);
        Assert.Equal("User not found in DB", result.Error.Description);
    }

   [Fact]
    public async Task Handle_Should_ReturnError_WhenCannotDeleteUser()
    {
        //Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(user));

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Failed());

        //Act
        Result result = await _handler.Handle(CreateCommand(user.Id), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotDelete", result.Error.Code);
        Assert.Equal("User cannot be deleted", result.Error.Description);
    }

   [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserDeleted()
    {
        //Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(user));

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Success);

        //Act
        Result result = await _handler.Handle(CreateCommand(user.Id), default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        await _userRepo.Received(1).FindUser(Arg.Is<string>(user.Id), default);

        await _userRepo.Received(1).DeleteUser(Arg.Is<User>(user));
    }
}