using FluentAssertions;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;

public class IdentityDeleteUserCommand
{
    private User CreateTestUser()
    {
        return User.Create("FullName", "test@email.com");
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
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(null));

        // Act
        Result result = await _handler.Handle(CreateCommand(user.Id), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.UserNotFound");
        result.Error.Description.Should().Be("User not found in DB");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenCannotDeleteUser()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        _userRepo.DeleteUser(Arg.Any<User>())
            .Returns(IdentityResult.Failed());

        // Act
        Result result = await _handler.Handle(CreateCommand(user.Id), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.CannotDelete");
        result.Error.Description.Should().Be("User cannot be deleted");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserDeleted()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUser(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        _userRepo.DeleteUser(Arg.Any<User>())
            .Returns(IdentityResult.Success);

        // Act
        Result result = await _handler.Handle(CreateCommand(user.Id), default);

        // Assert
        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        await _userRepo.Received(1)
            .FindUser(Arg.Is<string>(user.Id), default);

        await _userRepo.Received(1)
            .DeleteUser(Arg.Is<User>(user));
    }
}
