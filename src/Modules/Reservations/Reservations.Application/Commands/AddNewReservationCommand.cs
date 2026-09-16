using Reservations.Application.DTOs;
using SharedKernel.Application.Abstractions.Messaging;

namespace Reservations.Application.Commands;
public record AddNewReservationCommand(Guid GameId, DateTime StartDate, DateTime EndDate) : ICommand<AddNewReservationResultDTO>;