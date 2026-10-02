using System.Security.Cryptography;
using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;
using SharedKernel.PublicApi.Games;

namespace Reservations.Application.Commands;
public record AddNewReservationCommand(UserId UserId, Guid GameId, DateTimeOffset StartDate, DateTimeOffset EndDate) : ICommand<Result<AddNewReservationResultDTO>>;
public class AddNewReservationCommandHandler(IReservationsRepository reservationsRepository, IGamePublicApi gamePublicApi, IUnitOfWork unitOfWork) : IRequestHandler<AddNewReservationCommand, Result<AddNewReservationResultDTO>>
{
    public async Task<Result<AddNewReservationResultDTO>> Handle(AddNewReservationCommand request, CancellationToken cancellationToken)
    {
        var game = await gamePublicApi.GetGameById(request.GameId, cancellationToken);

        if(game is null)
        {
            return ReservationErrors.GameIdNotFoundInDB;
        }

        var profile = await reservationsRepository.GetReservationProfileByUserId(request.UserId, cancellationToken);

        if(profile is null)
        {
            return ReservationErrors.ReservationProfileNotFoundByUserId;
        }

        var invalidDates = await reservationsRepository.CheckReservationDates(game.GameId, request.StartDate, request.EndDate, cancellationToken);

        if(invalidDates is not false)
        {
            return ReservationErrors.ReservationOverlapping;
        }

        decimal amount = game.Category switch
        {
            "AAA" => 100.00m,
            "AA" => 70.00m,
            "III" => 50.00m,
            "Indie" => 20.00m,
            _ => 5.00m
        };

        var reservation = profile.AddReservation(request.GameId, request.StartDate, request.EndDate, amount);
        reservationsRepository.CreateReservation(reservation);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddNewReservationResultDTO(
            reservation.Id.Value,
            game.Title,
            reservation.EndDate,
            reservation.Status
        ); 
    }
}