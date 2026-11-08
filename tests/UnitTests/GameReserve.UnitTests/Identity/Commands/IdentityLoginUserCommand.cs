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
    private static string testPassword = "Password123!";
    private static LoginUserCommand CreateCommand(string userEmail, string userPassword) => new(userEmail, userPassword);
    private static User CreateTestUser(bool isBanned = false)
    {
        var user = User.Create("FullName", "test@email.com");

        if(isBanned)
        {
            user.BanUser();
        }

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
        //Arrange
        var user = CreateTestUser();

        _userRepo.FindUserByEmail(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(null));

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(CreateCommand(user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserNotFound", result.Error.Code);
        Assert.Equal("User not found in DB", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenCheckPasswordFailed()
    {
        //Arrange
        var user = CreateTestUser();

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(false);

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(CreateCommand(user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.NotValidPass", result.Error.Code);
        Assert.Equal("Password not valid", result.Error.Description);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserIsBanned()
    {
        //Arrange
        var user = CreateTestUser(true);
        
        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(true);

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(CreateCommand(user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserIsBanned", result.Error.Code);
        Assert.Equal("User is banned", result.Error.Description);
    }

    [Fact]
    public async Task Repository_Should_ReturnSuccess_WhenLoginCorrect()
    {
        //Arrange
        var user = CreateTestUser();

        string accessToken = "test-jwt-token";
        string testRefreshToken = "test-refresh-token";

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(true);

        _tokenProvider.CreateToken(user).Returns(Task.FromResult(accessToken));

        _tokenProvider.GenerateRefreshToken().Returns(testRefreshToken);

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(CreateCommand(user.Email!, testPassword), default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        Assert.NotNull(result.Value);

        Assert.Equal("FullName", result.Value.FullName);
        Assert.Equal(user.Email, result.Value.Email);

        await _userRepo.Received(1).FindUserByEmail(Arg.Is<string>(user.Email!), default);

        await _userRepo.Received(1).CheckPassword(Arg.Is<User>(user), Arg.Is<string>(testPassword));

        _userRepo.Received(1).AddRefreshToken(Arg.Is<RefreshToken>(rt =>
                                                                        rt.UserId == user.Id &&
                                                                        rt.Token == testRefreshToken &&
                                                                        rt.User == user));
        await _tokenProvider.Received(1).CreateToken(Arg.Is<User>(user));

        _tokenProvider.Received(1).GenerateRefreshToken();
        
        await _userRepo.Received(1).SaveChangesAsync(default);
    }
}