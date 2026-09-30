using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Queries;
public record GetUsersQuery : IRequest<Result<IEnumerable<GetUsersResultDTO>>>;
public class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, Result<IEnumerable<GetUsersResultDTO>>>
{
    public async Task<Result<IEnumerable<GetUsersResultDTO>>> Handle(GetUsersQuery request, CancellationToken token)
    {
        var users = await userRepository.GetUsers();
        var result = new List<GetUsersResultDTO>();
        if(users is null || !users.Any())
        {
            return Result<IEnumerable<GetUsersResultDTO>>.Succes(Enumerable.Empty<GetUsersResultDTO>());
        }

        foreach(var user in users)
        {
            result.Add(new GetUsersResultDTO(
                user.Id,
                user.FullName,
                user.Email!,
                user.Status
            ));
        }

        return Result<IEnumerable<GetUsersResultDTO>>.Succes(result);
    }
}