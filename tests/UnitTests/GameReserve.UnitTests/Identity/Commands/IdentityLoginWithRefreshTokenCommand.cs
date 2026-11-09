using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;
public class IdentityLoginWithRefreshTokenCommand
{
    private LoginWithRefreshTokenCommand CreateCommand(string refreshToken) => new(refreshToken);
    private User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", "test@email.com");

        if(isBanned)
            user.BanUser();

        return user;
    }
    private RefreshToken CreateTestRefreshToken(string userId, User user, string refreshToken) => new()
    {
        Id = Guid.NewGuid(),
        Token = refreshToken,
        UserId = userId,
        ExpiresOnUtc = DateTime.UtcNow.AddDays(6),
        User = user,
    };
    private readonly LoginWithRefreshTokenCommandHandler _handler;
    private readonly IUserRepository _userRepo;
    private readonly ITokenProvider _tokenProvider;
    public IdentityLoginWithRefreshTokenCommand()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _tokenProvider = Substitute.For<ITokenProvider>();

        _handler = new LoginWithRefreshTokenCommandHandler(_userRepo, _tokenProvider);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenInvalidRefreshToken()
    {
        //Arrange
        var testRefreshToken = "test-refresh-token";

        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(null));

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(CreateCommand(testRefreshToken), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.CannotLogInRefreshToken", result.Error.Code);
        Assert.Equal("Refresh token is expired or is not set in db", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenFindUserFailed()
    {
        //Arrange
        var user = CreateTestUser();

        var testRefreshToken = "test-refresh-token";
        var refreshToken = CreateTestRefreshToken(user.Id, user, testRefreshToken);

        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(refreshToken));

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(null));

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(CreateCommand(testRefreshToken), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserNotFound", result.Error.Code);
        Assert.Equal("User not found in DB", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserIsBanned()
    {
        //Arrange
        var user = CreateTestUser(true);

        var testRefreshToken = "test-refresh-token";
        var refreshToken = CreateTestRefreshToken(user.Id, user, testRefreshToken);

        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(refreshToken));

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(user));

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(CreateCommand(testRefreshToken), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserIsBanned", result.Error.Code);
        Assert.Equal("User is banned", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenRefreshTokenIsCorrect()
    {
        //Arrange
        var testRefreshToken = "test-refresh-token";
        var testNewRefreshToken = "test-new-refresh-token";
        var testAccessToken = "test-acces-token";

        var user = CreateTestUser();

        var refreshToken = CreateTestRefreshToken(user.Id, user, testRefreshToken);

        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(refreshToken));

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(user));

        _tokenProvider.CreateToken(user).Returns(Task.FromResult(testAccessToken));

        _tokenProvider.GenerateRefreshToken().Returns(testNewRefreshToken);

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(CreateCommand(testRefreshToken), default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        Assert.NotNull(result.Value);
        Assert.Equal(testNewRefreshToken, result.Value.RefreshToken);
        Assert.Equal(testAccessToken, result.Value.AccessToken);

        await _userRepo.Received(1).FindRefreshToken(Arg.Is<string>(rt => rt == testRefreshToken), default);

        await _userRepo.Received(1).FindUser(Arg.Is<string>(id => id == refreshToken.UserId), default);

        await _tokenProvider.Received(1).CreateToken(Arg.Is<User>(user));

        _tokenProvider.Received(1).GenerateRefreshToken();

        _userRepo.Received(1).AddRefreshToken(Arg.Is<RefreshToken>(rt => 
                                                                   rt.User == refreshToken.User &&
                                                                   rt.UserId == refreshToken.UserId &&
                                                                   rt.Token == testNewRefreshToken));

        await _userRepo.Received(1).SaveChangesAsync(default);
    }
}