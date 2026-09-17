using MediatR;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Interfaces;

namespace Reservations.Application.Queries;
public record GetReservationProfilesQuery(Guid Id) : IRequest<IEnumerable<ReservationProfile>>;
public class GetReservationProfilesQueryHandler(IReservationsRepository reservationsRepository) : IRequestHandler<GetReservationProfilesQuery, 
                                                                                                                     <IEnumerable<ReservationProfile>>>
{
    public async Task<IEnumerable<ReservationProfile>> Handle(GetReservationProfilesQuery request, CancellationToken cancellationToken)
    {
    }
}
