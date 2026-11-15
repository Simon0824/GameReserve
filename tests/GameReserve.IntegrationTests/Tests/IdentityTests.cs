using GameReserve.IntegrationTests.Infrastructure;
using Identity.Application.Commands;
using FluentAssertions;

namespace GameReserve.IntegrationTests.Tests;
public class IdentityTests : BaseIntegrationTests
{
    public IdentityTests(IntegrationTestsWebFactory factory) : base(factory){}

    [Fact]
    public async Task Create_ShouldAdd_UserToDatabase()
    {
        //Arrange
        var command = new CreateUserCommand("FullName", "test@email.com", "Password123!");

        //Act
        await Sender.Send(command);

        //Assert
        var user = await UserManager.FindByEmailAsync(command.Email);
        user.Should().NotBeNull();
    }
}