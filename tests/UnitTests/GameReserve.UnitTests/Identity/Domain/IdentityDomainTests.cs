using FluentAssertions;
using Identity.Domain.Enums;
using Identity.Domain.UserAggregate;

namespace GameReserve.UnitTests;
public class IdentityDomainTests
{
    private const string TestFullName = "FullName";
    private const string TestEmail = "test@email.com";
    [Fact]
    public void User_Should_BeReturned_WhenUserIsCreated()
    {
        //Act
        var user = User.Create(TestFullName, TestEmail);

        //Assert
        user.DomainEvents.Should().ContainSingle();
        user.FullName.Should().Be(TestFullName);
        user.Email.Should().Be(TestEmail);
        user.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public void DomainEvent_Should_BeRaised_WhenUserIsCreated()
    {
        //Act
        var user = User.Create("FullName", "test@email.com");

        //Assert
        user.DomainEvents.Should().ContainSingle();
        user.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<UserCreatedDomainEvent>();
    }

    [Fact]
    public void User_Should_BeBanned_Correctly_WhenUserIsBanned()
    {
        //Arrange
        var user = User.Create("FullName", "test@email.com");

        //Act & Assert
        user.BanUser();

        user.DomainEvents.Should().ContainSingle();
        user.Status.Should().Be(UserStatus.Banned);
    }

    [Fact]
    public void User_Should_ThrowException_WhenUserIsAlreadyBanned()
    {
        //Arrange
        var user = User.Create("FullName", "test@email.com");

        user.BanUser();

        // Act
        Action act = () => user.BanUser();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("User is already banned");
    }
}