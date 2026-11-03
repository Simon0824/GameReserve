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
    private static readonly CreateUserCommand Command = new("FullName", "test@email.com", "TestPassword123!");
    private readonly CreateUserCommandHandler _handler;
    private readonly IUserRepository _userRepo;

    public IdentityCreateUserCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();

        _handler = new CreateUserCommandHandler(_userRepo);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenCreateUserIsNotSucceded()
    {
        //Arrange
        CreateUserCommand invalidCommand = Command with {Password = "error"};

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Failed());

        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(invalidCommand, default);

        //Assert
        Assert.True(result.IsFailed);

        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotCreate", result.Error.Code);
        Assert.Equal("User cannot be created", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAddRoleToUserIsNotSucceded()
    {
        //Arrange
        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);

         _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Success);

        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsFailed);

        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotAddRole", result.Error.Code);
        Assert.Equal("Cannot add a role to user", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAddRoleToUserIsNotSucceded_AndCannotDeleteUser()
    {
        //Arrange
        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(Task.FromResult(IdentityResult.Success));

         _userRepo.AddUserRole(Arg.Any<User>())
        .Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>())
        .Returns(IdentityResult.Failed());

        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsFailed);

        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotDelete", result.Error.Code);
        Assert.Equal("User cannot be deleted", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCreateUserAndAddRoleIsSucceded()
    {
        //Arrange
        _userRepo.CreateUser(Arg.Any<User>(),Arg.Any<string>()).Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Success);
                        
        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsSucceeded);

        Assert.False(result.IsFailed);

        Assert.NotNull(result.Value);

        Assert.Equal(Command.Email, result.Value.Email);
        Assert.Equal(Command.FullName, result.Value.FullName);
    }

    [Fact]
    public async Task Repository_Should_Receive_WhenCreateUser()
    {
        //Arrange
        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>()).Returns(IdentityResult.Success);;

        _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Success);
                        
        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        await _userRepo.Received(1).CreateUser(Arg.Is<User>(u => 
                                                            u.FullName == Command.FullName &&
                                                            u.Email == Command.Email),
                                                            Command.Password);
    }

    [Fact]
    public async Task Repository_Should_Receive_WhenAddUserRole()
    {
        //Arrange
        _userRepo.CreateUser(Arg.Any<User>(),Arg.Any<string>()).Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Success);
                        
        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        await _userRepo.Received(1).AddUserRole(Arg.Is<User>(u =>
                                                             u.FullName == Command.FullName &&
                                                             u.Email == Command.Email));
    }

    [Fact]
    public async Task Repository_Should_Receive_WhenDeleteUser()
    {
        //Arrange
        _userRepo.CreateUser(Arg.Any<User>(),Arg.Any<string>()).Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>()).Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>()).Returns(IdentityResult.Success);;
                        
        //Act
        Result<CreateUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        await _userRepo.Received(1).DeleteUser(Arg.Is<User>(u =>
                                                             u.FullName == Command.FullName &&
                                                             u.Email == Command.Email));
    }
}
