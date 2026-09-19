using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;

public record GetReservationProfilesQuery() : IRequest<GetReservationProfilesResultDTO>;
public class GetReservationProfilesQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationProfilesQuery,
                                                                                                                  GetReservationProfilesResultDTO>
{
    public async Task<GetReservationProfilesResultDTO> Handle(GetReservationProfilesQuery request, CancellationToken cancellationToken)
    {
        var profiles = await reservationsRepository.GetReservationProfiles(cancellationToken);

        var profilesResult = profiles.Select(profile =>
        {
            var reservations = profile!.Reservations
            .Select(
                r => new GetReservationDTO(r.Id.Value, r.GameId, r.StartDate, r.EndDate))
            .ToList();

            return new GetReservationProfilesListResultDTO(profile.Id.Value, profile.UserId.Value, reservations);
        });


        var message = profiles.Count == 0 ? 
                                "No reservation profiles found"
                                : "Reservation profiles retrieved succesfully";

        return new GetReservationProfilesResultDTO(message, profilesResult);
    }
}
