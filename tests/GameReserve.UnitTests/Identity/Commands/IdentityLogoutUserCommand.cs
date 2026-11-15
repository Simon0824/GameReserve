using FluentAssertions;
using Identity.Application.Commands;
using Identity.Domain.Interfaces;
using NSubstitute;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;
public class IdentityLogoutUserCommand
{
    private const long BaseTimestamp = 1794571200L;
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
        var expires = BaseTimestamp.ToString();
        
        _blackListRepository.BlackListUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(false);

        //Act
        Result result = await _handler.Handle(Command(tokenId, expires), default);

        //Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.FailedToSaveLogout");
        result.Error.Description.Should().Be("Failed to save logout to Redis");
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserLoggedOut()
    {
        //Arrange
        var tokenId = "test-access-token-id";
        var expires = BaseTimestamp.ToString();

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
        result.IsFailed.Should().BeFalse();
        result.IsSucceeded.Should().BeTrue();

        await _blackListRepository.BlackListUserAsync(
            Arg.Is<string>(string.Empty), 
            Arg.Is<string>(tokenId), 
            Arg.Is<TimeSpan>(timeSpan));
    }
}