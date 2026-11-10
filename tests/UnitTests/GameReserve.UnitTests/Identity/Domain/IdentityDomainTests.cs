using FluentAssertions;
using Identity.Domain.Enums;
using Identity.Domain.UserAggregate;
using SharedKernel.Domain.Abstractions;

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
    public void User_Should_ReturnError_WhenUserIsAlreadyBanned()
    {
        //Arrange
        var user = User.Create("FullName", "test@email.com");
        user.BanUser();

        // Act
        Result result = user.BanUser();
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("User.UserIsBanned");
        result.Error.Description.Should().Be("User is banned");
    }
}