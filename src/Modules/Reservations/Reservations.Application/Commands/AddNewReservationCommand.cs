using MediatR;
using Microsoft.EntityFrameworkCore;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using SharedKernel.Application.Abstractions.Messaging;
using SharedKernel.Domain.Abstractions;
using SharedKernel.Domain.Enums;
using SharedKernel.PublicApi.Games;

namespace Reservations.Application.Commands;
public record AddNewReservationCommand(UserId UserId, 
                                       Guid GameId, 
                                       DateTimeOffset StartDate, 
                                       DateTimeOffset EndDate,
                                       PaymentCurrency Currency,
                                       decimal Amount,
                                       CardTypes Card) : ICommand<Result<AddNewReservationResultDTO>>;
public class AddNewReservationCommandHandler(IReservationsRepository reservationsRepository, IGamePublicApi gamePublicApi, IUnitOfWork unitOfWork) : IRequestHandler<AddNewReservationCommand, Result<AddNewReservationResultDTO>>
{
    public async Task<Result<AddNewReservationResultDTO>> Handle(AddNewReservationCommand request, CancellationToken cancellationToken)
    {
        var game = await gamePublicApi.GetGameById(request.GameId, cancellationToken);

        if(game is null)
            return ReservationErrors.GameIdNotFoundInDB;

        var profile = await reservationsRepository.GetReservationProfileByUserId(request.UserId, cancellationToken);

        if(profile is null)
            return ReservationErrors.ReservationProfileNotFoundByUserId;

        var leadTime = request.EndDate - request.StartDate;


        if(request.StartDate > request.EndDate)
            return ReservationErrors.ReservationStartDateGreaterOrEqual;

        if(leadTime < TimeSpan.FromDays(1))
            return ReservationErrors.ReservationTimeTooShort;

        var days = (int)Math.Ceiling(leadTime.TotalDays);

        var calculatedPrice = Reservation.CalculatePrice(game.Category, days);

        if(calculatedPrice.IsFailed)
            throw new Exception("Cannot calculate price");

        var reservation = profile.AddReservation(
                                        request.GameId, 
                                        request.StartDate, 
                                        request.EndDate, 
                                        request.Amount,
                                        calculatedPrice.Value,
                                        request.Currency,
                                        request.Card);

        reservationsRepository.CreateReservation(reservation);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (reservationsRepository.IsOverlapConstraint(ex))
        {
            return ReservationErrors.ReservationOverlapping;
        }


        return new AddNewReservationResultDTO(
            reservation.Id.Value,
            game.Title,
            reservation.EndDate,
            reservation.Status
        ); 
    }
}