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
    private User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", "test@email.com");

        if(isBanned)
            user.BanUser();

        return user;
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
        //Arrange
        var user = CreateTestUser();
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(null));

        //Act
        Result result = await _handler.Handle(CreateCommand(user.Id, TestPassword, TestNewPassword), default);

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
        var user = CreateTestUser();
        _userRepo.FindUser(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(CreateTestUser()));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(false);
        //Act
        Result result = await _handler.Handle(CreateCommand(user.Id, TestPassword, TestNewPassword), default);

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
        var user = CreateTestUser();
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(CreateTestUser()));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(true);
    
        _userRepo.ChangePassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>()).Returns(IdentityResult.Failed());
        //Act
        Result result = await _handler.Handle(CreateCommand(user.Id, TestPassword, TestNewPassword), default);

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
        var user = CreateTestUser();
        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(true);
    
        _userRepo.ChangePassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        //Act
        Result result = await _handler.Handle(CreateCommand(user.Id, TestPassword, TestNewPassword), default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        await _userRepo.Received(1).FindUser(Arg.Is<string>(user.Id), default);

        await _userRepo.Received(1).CheckPassword(Arg.Is<User>(user), Arg.Is<string>(TestPassword));

        await _userRepo.Received(1).ChangePassword(
                        Arg.Is<User>(user), 
                        Arg.Is<string>(TestPassword), 
                        Arg.Is<string>(TestNewPassword));
    }
}