using MediatR;
using Reservations.Application.DTOs;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;

namespace Reservations.Application.Queries;
public record GetReservationProfilesQuery() : IRequest<GetReservationProfilesResultDTO>;
public class GetReservationProfilesQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationProfilesQuery,
                                                                                                                  GetReservationProfilesResultDTO>
{
    public async Task<GetReservationProfilesResultDTO> Handle(GetReservationProfilesQuery request, CancellationToken cancellationToken)
    {
        var profiles = await reservationsRepository.GetReservationProfiles(cancellationToken);
        
        var message = profiles.Count == 0 ? 
                                "No reservation profiles found"
                                : "Reservation profiles retrieved succesfully";
        
        return new GetReservationProfilesResultDTO(
            message,
            profiles
        );
    }
}
