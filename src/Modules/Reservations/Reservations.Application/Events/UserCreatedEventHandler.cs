using MassTransit;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.Events;
public class UserCreatedEventHandler(
ILogger<UserCreatedEventHandler> logger, 
IReservationsRepository reservationsRepository,
IUnitOfWork unitOfWork) : IConsumer<UserCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        logger.LogInformation("Consuming UserCreated event");

        var userId = new UserId(context.Message.Id);
        var profile = ReservationProfile.CreateProfile(userId);

        reservationsRepository.AddReservationProfile(profile);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation($"UserCreated event consumed succesfully! {profile.Id}");
    }
}