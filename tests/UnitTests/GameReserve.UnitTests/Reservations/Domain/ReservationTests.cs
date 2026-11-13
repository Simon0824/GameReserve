using FluentAssertions;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Enums;
using Reservations.Domain.Primitives;
using SharedKernel.Domain.Abstractions;

namespace GameReserve.UnitTests;
public class ReservationTests
{
    private static readonly DateTimeOffset StartDate = new(2026, 11, 13, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset EndDate = DateTimeOffset.UtcNow.AddDays(5);

    [Fact]
    public async Task Rehydrate_Should_ReturnCorrectReservation()
    {
        //Act 
        Reservation result = Reservation.Rehydrate(
            new ReservationId(Guid.NewGuid()),
            new ReservationProfileId(Guid.NewGuid()),
            Guid.NewGuid(),
            StartDate,
            EndDate,
            ReservationStatus.Pending
        );
            
        //Assert
        result.StartDate.Should().Be(StartDate);
        result.EndDate.Should().Be(EndDate);
        result.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public async Task CreateReservation_Should_ReturnError_WhenStartDateAfterEndDate()
    {
        //Act 
        Result<Reservation> result = Reservation.CreateReservation(
            new ReservationProfileId(Guid.NewGuid()), 
            Guid.NewGuid(),
            EndDate,
            StartDate);
            
        //Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("Reservation.EndBeforeStart");
        result.Error.Description.Should().Be("Start date has to be set before end date");
    }

    [Fact]
    public async Task Complete_Should_ReturnError_WhenTryCompleteFailedReservation()
    {
        //Arrange 
        var reservation = Reservation.CreateReservation(
            new ReservationProfileId(Guid.NewGuid()), 
            Guid.NewGuid(),
            StartDate,
            EndDate).Value;
        
        reservation!.Failed();

        //Act
        Result result = reservation.Completed();
        
        //Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("Reservation.AlreadyFailed");
        result.Error.Description.Should().Be("Reservation payment has already failed");
    }

    [Fact]
    public async Task Complete_Should_ReturnSuccess_WhenTryCompleteReservation()
    {
        //Arrange
        var reservation = Reservation.CreateReservation(
            new ReservationProfileId(Guid.NewGuid()), 
            Guid.NewGuid(),
            StartDate,
            EndDate).Value;

        //Act
        Result result = reservation!.Completed();

        //Assert
        result.IsFailed.Should().BeFalse();
        result.IsSucceeded.Should().BeTrue();

        reservation.Status.Should().Be(ReservationStatus.Completed);
    }

    [Fact]
    public async Task CalculatePrice_Should_ReturnError_WhenTryInvalidGameGenre()
    {
        //Act
        Result<decimal> result = Reservation.CalculatePrice("abc", 5);

        //Assert
        result.IsFailed.Should().BeTrue();
        result.IsSucceeded.Should().BeFalse();

        result.Error.Code.Should().Be("Game.InvalidGenre");
        result.Error.Description.Should().Be("You've entered invalid game genre");
    }

    [Theory]
    [InlineData("AAA", 7, 700.00)]
    [InlineData("AA", 3, 210.00)]
    [InlineData("III", 5, 250.00)]
    [InlineData("Indie", 6, 120.00)]
    public async Task CalculatePrice_Should_ReturnPrice_WhenValidGameGenre(string gameCategory, int leadTimeDays, decimal expectedPrice)
    {
        //Act
        Result<decimal> result = Reservation.CalculatePrice(gameCategory, leadTimeDays);

        //Assert
        result.IsFailed.Should().BeFalse();
        result.IsSucceeded.Should().BeTrue();

        result.Value.Should().Be(expectedPrice);
    }
}