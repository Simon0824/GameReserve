using FluentAssertions;
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
    private const string TestPassword = "Password123!";

    private CreateUserCommand CreateCommand(string userFullName, string userEmail, string userPassword)
    {
        return new CreateUserCommand(userFullName, userEmail, userPassword);
    }

    private User CreateTestUser()
    {
        return User.Create("FullName", "test@email.com");
    }

    private readonly CreateUserCommandHandler _handler;
    private readonly IUserRepository _userRepo;

    public IdentityCreateUserCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _handler = new CreateUserCommandHandler(_userRepo);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenCreateUserFails()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed());

        // Act
        Result<CreateUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.FullName, user.Email!, TestPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.CannotCreate");
        result.Error.Description.Should().Be("User cannot be created");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAddRoleToUserFails()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>())
            .Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>())
            .Returns(IdentityResult.Success);

        // Act
        Result<CreateUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.FullName, user.Email!, TestPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.CannotAddRole");
        result.Error.Description.Should().Be("Cannot add a role to user");

        await _userRepo.Received(1).CreateUser(Arg.Is<User>(u =>
                                                            u.FullName == user.FullName &&
                                                            u.Email == user.Email), 
                                                            Arg.Is<string>(TestPassword));

        await _userRepo.Received(1).AddUserRole(Arg.Is<User>(u =>
                                                            u.FullName == user.FullName &&
                                                            u.Email == user.Email));

        await _userRepo.Received(1).DeleteUser(Arg.Is<User>(u =>
                                                            u.FullName == user.FullName &&
                                                            u.Email == user.Email));
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenAddRoleToUserFails_AndCannotDeleteUser()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>())
            .Returns(IdentityResult.Failed());

        _userRepo.DeleteUser(Arg.Any<User>())
            .Returns(IdentityResult.Failed());

        // Act
        Result<CreateUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.FullName, user.Email!, TestPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.CannotDelete");
        result.Error.Description.Should().Be("User cannot be deleted");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCreateUserAndAddRoleSuccess()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.CreateUser(Arg.Any<User>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        _userRepo.AddUserRole(Arg.Any<User>())
            .Returns(IdentityResult.Success);

        // Act
        Result<CreateUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.FullName, user.Email!, TestPassword), default);

        // Assert
        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        result.Value.Should().NotBeNull();

        result.Value.Email.Should().Be(user.Email);
        result.Value.FullName.Should().Be(user.FullName);

        await _userRepo.Received(1)
            .CreateUser(
                Arg.Is<User>(u => u.Email == user.Email && u.FullName == user.FullName),
                TestPassword);

        await _userRepo.Received(1)
            .AddUserRole(
                Arg.Is<User>(u => u.Email == user.Email && u.FullName == user.FullName));
    }
}
