using FluentAssertions;
using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;
public class IdentityLogoutUserCommand
{
    private LogoutUserCommand Command(string tokenId, string expiresOnUtc) => new(tokenId, expiresOnUtc);
    private readonly IBlackListRepository _blackListRepository;
    private readonly LogoutUserCommandHandler _handler;
    public IdentityLogoutUserCommand()
    {
        _blackListRepository = Substitute.For<IBlackListRepository>();

        _handler = new(_blackListRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserFailedToSave()
    {
        //Arrange
        var tokenId = "test-access-token-id";
        var expires = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds().ToString();
        
        _blackListRepository.BlackListUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(false);

        //Act
        Result result = await _handler.Handle(Command(tokenId, expires), default);

        //Assert
        result.IsFailed.Should().Be(true);
        result.IsSucceeded.Should().Be(false);

        result.Error.Code.Should().Be("User.FailedToSaveLogout");
        result.Error.Description.Should().Be("Failed to save logout to Redis");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserLoggedOut()
    {
        //Arrange
        var tokenId = "test-access-token-id";
        var expires = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds().ToString();

        TimeSpan timeSpan = DateTimeOffset.FromUnixTimeSeconds(
                                                long.Parse(expires)).UtcDateTime - DateTime.UtcNow;
        
        _blackListRepository.BlackListUserAsync(
            Arg.Any<string>(), 
            Arg.Any<string>(), 
            Arg.Any<TimeSpan>())
            .Returns(true);

        //Act
        Result result = await _handler.Handle(Command(tokenId, expires), default);

        //Assert
        result.IsFailed.Should().Be(false);
        result.IsSucceeded.Should().Be(true);

        await _blackListRepository.BlackListUserAsync(
            Arg.Is<string>(string.Empty), 
            Arg.Is<string>(tokenId), 
            Arg.Is<TimeSpan>(timeSpan));
    }
}