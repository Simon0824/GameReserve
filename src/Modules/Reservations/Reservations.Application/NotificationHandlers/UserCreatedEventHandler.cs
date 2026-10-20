using MediatR;
using Microsoft.Extensions.Logging;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.IntegrationEvents;

namespace Reservations.Application.EventHandlers;
public class UserCreatedEventHandler(
ILogger<UserCreatedEventHandler> logger, 
IReservationsRepository reservationsRepository,
IUnitOfWork unitOfWork) : INotificationHandler<UserCreatedIntegrationEvent>
{
    public async Task Handle(UserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Consuming UserCreated event");

        var userId = new UserId(notification.UserId);
        var profile = ReservationProfile.CreateProfile(userId);

        reservationsRepository.AddReservationProfile(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation($"UserCreated {userId} event consumed succesfully! {profile.Id}");
    }
}