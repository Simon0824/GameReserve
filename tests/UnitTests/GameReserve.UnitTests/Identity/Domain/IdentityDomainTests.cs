using System.Reflection;
using FluentAssertions;
using Identity.Domain.Enums;
using Identity.Domain.UserAggregate;
using NSubstitute;

namespace GameReserve.UnitTests;
public class IdentityDomainTests
{
    private static string testFullName = "FullName";
    private static string testEmail = "test@email.com";
    [Fact]
    public void User_Should_BeReturned_WhenUserIsCreated()
    {
        //Act
        var user = User.Create(testFullName, testEmail);

        //Assert
        user.DomainEvents.Should().ContainSingle();
        user.FullName.Should().Be(testFullName);
        user.Email.Should().Be(testEmail);
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