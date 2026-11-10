using FluentAssertions;
using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;

public class IdentityLoginUserCommand
{
    private const string TestPassword = "Password123!";

    private LoginUserCommand CreateCommand(string userEmail, string userPassword)
        => new(userEmail, userPassword);

    private User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", "test@email.com");

        if (isBanned)
            user.BanUser();

        return user;
    }

    private readonly LoginUserCommandHandler _handler;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _userRepo;

    public IdentityLoginUserCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _tokenProvider = Substitute.For<ITokenProvider>();

        _handler = new LoginUserCommandHandler(_userRepo, _tokenProvider);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(null));

        // Act
        Result<LoginUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.Email!, TestPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.UserNotFound");
        result.Error.Description.Should().Be("User not found in DB");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenCheckPasswordFails()
    {
        // Arrange
        var user = CreateTestUser();

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>())
            .Returns(false);

        // Act
        Result<LoginUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.Email!, TestPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.NotValidPass");
        result.Error.Description.Should().Be("Password not valid");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserIsBanned()
    {
        // Arrange
        var user = CreateTestUser(true);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>())
            .Returns(true);

        // Act
        Result<LoginUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.Email!, TestPassword), default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.UserIsBanned");
        result.Error.Description.Should().Be("User is banned");
    }

    [Fact]
    public async Task Repository_Should_ReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var user = CreateTestUser();

        string accessToken = "test-jwt-token";
        string testRefreshToken = "test-refresh-token";

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
            .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>())
            .Returns(true);

        _tokenProvider.CreateToken(user)
            .Returns(Task.FromResult(accessToken));

        _tokenProvider.GenerateRefreshToken()
            .Returns(testRefreshToken);

        // Act
        Result<LoginUserResultDTO> result = await _handler.Handle(
            CreateCommand(user.Email!, TestPassword), default);

        // Assert
        result.IsSucceeded.Should().BeTrue();
        result.IsFailed.Should().BeFalse();

        result.Value.Should().NotBeNull();

        result.Value.FullName.Should().Be("FullName");
        result.Value.Email.Should().Be(user.Email);

        await _userRepo.Received(1)
            .FindUserByEmail(Arg.Is<string>(user.Email!), default);

        await _userRepo.Received(1)
            .CheckPassword(Arg.Is<User>(user), Arg.Is<string>(TestPassword));

        _userRepo.Received(1)
            .AddRefreshToken(Arg.Is<RefreshToken>(rt =>
                rt.UserId == user.Id &&
                rt.Token == testRefreshToken &&
                rt.User == user));

        await _tokenProvider.Received(1)
            .CreateToken(Arg.Is<User>(user));

        _tokenProvider.Received(1)
            .GenerateRefreshToken();

        await _userRepo.Received(1)
            .SaveChangesAsync(default);
    }
}
