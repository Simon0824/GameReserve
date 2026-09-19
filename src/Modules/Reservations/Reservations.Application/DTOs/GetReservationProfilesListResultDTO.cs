namespace Reservations.Application.DTOs;
public record GetReservationProfilesListResultDTO(Guid ProfileId, Guid UserId, List<GetReservationDTO> Reservations);