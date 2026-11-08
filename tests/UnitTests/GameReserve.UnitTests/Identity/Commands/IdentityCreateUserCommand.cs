using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;

public class IdentityCreateUserCommand
{
    private static string testPassword = "Password123!";
    private static CreateUserCommand CreateCommand(string userFullName, string userEmail, string userPassword)
    {
        return new CreateUserCommand(userFullName, userEmail, userPassword);
    }
    private static User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", "test@email.com");

        if(isBanned)
        {
            user.BanUser();
        }

        return user;
    }
    private readonly CreateUserCommandHandler _handler;
    private readonly IUserRepository _userRepo;

    public IdentityCreateUserCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();

        _handler = new CreateUserCommandHandler(_userRepo);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenCreateUserIsNotSucceeded()
    {
        //Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Failed());

        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(CreateCommand(user.FullName, user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotCreate", result.Error.Code);
        Assert.Equal("User cannot be created", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAddRoleToUserIsNotSucceeded()
    {
        //Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Success);

        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(CreateCommand(user.FullName, user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotAddRole", result.Error.Code);
        Assert.Equal("Cannot add a role to user", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAddRoleToUserIsNotSucceeded_AndCannotDeleteUser()
    {
        //Arrange
        var user = CreateTestUser();
        
        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);

         _userRepo.AddUserRole(Arg.Any<User>())
        .Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>())
        .Returns(IdentityResult.Failed());

        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(CreateCommand(user.FullName, user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotDelete", result.Error.Code);
        Assert.Equal("User cannot be deleted", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCreateUserAndAddRoleIsSucceeded()
    {
        //Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Success);
                        
        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(CreateCommand(user.FullName, user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        Assert.NotNull(result.Value);

        Assert.Equal(user.Email, result.Value.Email);
        Assert.Equal(user.FullName, result.Value.FullName);

        await _userRepo.Received(1).CreateUser(Arg.Is<User>(u => u.Email == user.Email && u.FullName == user.FullName), testPassword);

        await _userRepo.Received(1).AddUserRole(Arg.Is<User>(u => u.Email == user.Email && u.FullName == user.FullName));
    }

    [Fact]
    public async Task Repository_Should_Receive_WhenDeleteUser()
    {
        //Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(),Arg.Any<string>()).Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Success);
                        
        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(CreateCommand(user.FullName, user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        await _userRepo.Received(1).DeleteUser(Arg.Is<User>(u =>
                                                             u.FullName == user.FullName &&
                                                             u.Email == user.Email));
    }
}
