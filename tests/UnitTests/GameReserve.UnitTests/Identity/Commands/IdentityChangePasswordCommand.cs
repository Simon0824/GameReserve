using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using SharedKernel.Domain.Abstractions;
using SharedKernel.IntegrationEvents;

namespace GameReserve.UnitTests;
public class IdentityChangePasswordCommand
{
    private static readonly User User = User.Create("FullName", "test@email.com");
    private static readonly ChangePasswordCommand Command = new(User.Id, "Password123!", "NewPassword123!");
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
    public async Task Handle_Should_ReturnError_WhenInvalidPassword()
    {
        //Arrange
        _userRepo.FindUser(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(User));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(false);
        //Act
        Result result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.NotValidPass", result.Error.Code);
        Assert.Equal("Password not valid", result.Error.Description);
    }

  [Fact]
    public async Task Handle_Should_ReturnError_WhenCannotChangePassword()
    {
        //Arrange
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(User));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(true);
    
        _userRepo.ChangePassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>()).Returns(IdentityResult.Failed());
        //Act
        Result result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotChangePassword", result.Error.Code);
        Assert.Equal("Cannot change a user password", result.Error.Description);
    }

  [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenNewPasswordIsCorrect()
    {
        //Arrange
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(User));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(true);
    
        _userRepo.ChangePassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        //Act
        Result result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        await _userRepo.Received(1).FindUser(Arg.Is<string>(Command.UserId), default);

        await _userRepo.Received(1).CheckPassword(Arg.Is<User>(User), Arg.Is<string>(Command.CurrentPassword));

        await _userRepo.Received(1).ChangePassword(
                        Arg.Is<User>(User), 
                        Arg.Is<string>(Command.CurrentPassword), 
                        Arg.Is<string>(Command.NewPassword));
    }
}