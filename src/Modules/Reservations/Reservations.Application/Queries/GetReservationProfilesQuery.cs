using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;
using SharedKernel.Domain.Abstractions;

public record GetReservationProfilesQuery() : IRequest<Result<GetReservationProfilesResultDTO>>;
public class GetReservationProfilesQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationProfilesQuery,
                                                                                                                  Result<GetReservationProfilesResultDTO>>
{
    public async Task<Result<GetReservationProfilesResultDTO>> Handle(GetReservationProfilesQuery request, CancellationToken cancellationToken)
    {
        var profiles = await reservationsRepository.GetReservationProfiles(cancellationToken);

        var profilesResult = profiles.Select(profile =>
        {
            var reservations = profile!.Reservations
            .Select(
                r => new GetReservationDTO(r.Id.Value, r.GameId, r.StartDate, r.EndDate, r.Status))
            .ToList();

            return new GetReservationProfilesListResultDTO(profile.Id.Value, profile.UserId.Value, reservations);
        });


        var message = profiles.Count == 0 ? 
                                "No reservation profiles found"
                                : "Reservation profiles retrieved succesfully";

        return Result<GetReservationProfilesResultDTO>.Succes(new GetReservationProfilesResultDTO(message, profilesResult));
    }
}
