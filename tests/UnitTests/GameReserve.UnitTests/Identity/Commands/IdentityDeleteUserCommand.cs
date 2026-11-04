using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;
public class IdentityDeleteUserCommand
{
    private static readonly User User = User.Create("FullName", "test@email.com");
    private static readonly DeleteUserCommand Command = new(User.Id);
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
    public async Task Handle_Should_ReturnError_WhenCannotDeleteUser()
    {
        //Arrange
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(User));

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Failed());

        //Act
        Result result = await _handler.Handle(Command, default);

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
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(User));

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Success);

        //Act
        Result result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        await _userRepo.Received(1).FindUser(Arg.Is<string>(Command.UserId), default);

        await _userRepo.Received(1).DeleteUser(Arg.Is<User>(User));
    }
}