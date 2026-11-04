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
    private static readonly LoginWithRefreshTokenCommand Command = new("test-refresh-token");

    private static readonly User User = User.Create("FullName", "test@email.com");
    private static readonly RefreshToken TestRefreshToken = new()
    {
        Id = Guid.NewGuid(),
        Token = Command.RefreshToken,
        UserId = User.Id,
        ExpiresOnUtc = DateTime.UtcNow.AddDays(6),
        User = User,

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
        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(null));

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(Command, default);

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
        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(TestRefreshToken));

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(null));

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(Command, default);

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
        User bannedUser = User;

        bannedUser.BanUser();

        if(TestRefreshToken.ExpiresOnUtc <= DateTime.UtcNow)
            TestRefreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(6);

        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(TestRefreshToken));

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(User));

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(Command, default);

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
        _userRepo.FindRefreshToken(Arg.Any<string>(), default).Returns(Task.FromResult<RefreshToken?>(TestRefreshToken));

        _userRepo.FindUser(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(User));

        _tokenProvider.CreateToken(User).Returns(Task.FromResult("test-access-token"));

        _tokenProvider.GenerateRefreshToken().Returns("new-test-refresh-token");

        //Act
        Result<LoginWithRefreshTokenResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        Assert.NotNull(result.Value);
        Assert.Equal("new-test-refresh-token", result.Value.RefreshToken);
        Assert.Equal("test-access-token", result.Value.AccessToken);

        await _userRepo.Received(1).FindRefreshToken(Arg.Is<string>(rt => rt == Command.RefreshToken), default);

        await _userRepo.Received(1).FindUser(Arg.Is<string>(id => id == TestRefreshToken.UserId), default);

        await _tokenProvider.Received(1).CreateToken(Arg.Is<User>(User));

        _tokenProvider.Received(1).GenerateRefreshToken();

        _userRepo.Received(1).AddRefreshToken(Arg.Is<RefreshToken>(rt => 
                                                                   rt.User == TestRefreshToken.User &&
                                                                   rt.UserId == TestRefreshToken.UserId &&
                                                                   rt.Token == "new-test-refresh-token"));

        await _userRepo.Received(1).SaveChangesAsync(default);
    }
}