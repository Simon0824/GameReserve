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
    private static readonly LoginUserCommand Command = new("test@email.com", "Password123!");
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
        _userRepo.FindUserByEmail(Arg.Any<string>(), default).Returns(Task.FromResult<User?>(null));

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

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
        User user = User.Create("FullName", Command.Email);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(false);

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

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
        User user = User.Create("FullName", Command.Email);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(true);

        user.BanUser(user);

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsFailed);
        Assert.False(result.IsSucceeded);

        Assert.Equal("User.UserIsBanned", result.Error.Code);
        Assert.Equal("User is banned", result.Error.Description);
    }

    [Fact]
    public async Task Repository_Should_Receive_WhenFindUserByEmail()
    {
        //Arrange
        User user = User.Create("FullName", Command.Email);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        await _userRepo.Received(1).FindUserByEmail(Arg.Is<string>(Command.Email), default);
    }

    [Fact]
    public async Task Repository_Should_Receive_WhenCheckPassword()
    {
        //Arrange
        User user = User.Create("FullName", Command.Email);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(true);

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        await _userRepo.Received(1).CheckPassword(Arg.Is<User>(user), Arg.Is<string>(Command.Password));
    }

    [Fact]
    public async Task TokenProvider_Should_Receive_WhenCreateToken()
    {
        //Arrange
        User user = User.Create("FullName", Command.Email);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(true);

        _tokenProvider.CreateToken(user).Returns(Task.FromResult("test-jwt-token"));

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert

        Assert.True(result.IsSucceeded);
        await _tokenProvider.Received(1).CreateToken(user);
    }

    [Fact]
    public async Task TokenProvider_Should_Receive_WhenRefreshToken()
    {
        //Arrange
        User user = User.Create("FullName", Command.Email);

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(true);

        _tokenProvider.CreateToken(user).Returns(Task.FromResult("test-jwt-token"));

        _tokenProvider.GenerateRefreshToken().Returns("test-refresh-token");

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsSucceeded);
        _tokenProvider.Received(1).GenerateRefreshToken();
    }

    [Fact]
    public async Task Repository_Should_Receive_WhenAddRefreshToken()
    {
        //Arrange
        User user = User.Create("FullName", Command.Email);
        string accessToken = "test-jwt-token";
        string testRefreshToken = "test-refresh-token";

        _userRepo.FindUserByEmail(Arg.Any<string>(), default)
                                .Returns(Task.FromResult<User?>(user));

        _userRepo.CheckPassword(user, Arg.Any<string>()).Returns(true);

        _tokenProvider.CreateToken(user).Returns(Task.FromResult(accessToken));

        _tokenProvider.GenerateRefreshToken().Returns(testRefreshToken);

        //Act
        Result<LoginUserResultDTO> result = await _handler.Handle(Command, default);

        //Assert
        Assert.True(result.IsSucceeded);
        Assert.False(result.IsFailed);

        Assert.NotNull(result.Value);

        Assert.Equal("FullName", result.Value.FullName);
        Assert.Equal(Command.Email, result.Value.Email);

        _userRepo.Received(1).AddRefreshToken(Arg.Is<RefreshToken>(rt =>
                                                                        rt.UserId == user.Id &&
                                                                        rt.Token == testRefreshToken &&
                                                                        rt.User == user));
        await _userRepo.Received(1).SaveChangesAsync(default);
    }
}