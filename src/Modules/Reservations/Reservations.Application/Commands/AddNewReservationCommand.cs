using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.PublicApi.Games;

namespace Reservations.Application.Commands;
public record AddNewReservationCommand(UserId UserId, Guid GameId, DateTime StartDate, DateTime EndDate) : ICommand<AddNewReservationResultDTO>;
public class AddNewReservationCommandHandler(IReservationsRepository reservationsRepository, IGamePublicApi gamePublicApi) : IRequestHandler<AddNewReservationCommand, AddNewReservationResultDTO>
{
    public async Task<AddNewReservationResultDTO> Handle(AddNewReservationCommand request, CancellationToken cancellationToken)
    {
        var game = await gamePublicApi.GetGameById(request.GameId, cancellationToken);

        if(game is null)
        {
            throw new Exception($"Game with ID: {request.GameId} does not exist");
        }

        var profile = await reservationsRepository.GetReservationProfileByUserId(request.UserId);

        if(profile is null)
        {
            throw new Exception("You don't have a reservation profile");
        }

        var reservation = profile.AddReservation(request.GameId, request.StartDate, request.EndDate);
        await reservationsRepository.CreateReservation(reservation);

        return new AddNewReservationResultDTO(
            reservation.Id.Value,
            game.Title,
            reservation.EndDate
        );
    }
}