using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Identity.Application.Queries;
public record GetUsersQuery : IRequest<Result<GetUsersQueryDTO>>;
public class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, Result<GetUsersQueryDTO>>
{
    public async Task<Result<GetUsersQueryDTO>> Handle(GetUsersQuery request, CancellationToken token)
    {
        var users = await userRepository.GetUsers();
        var result = new List<GetUsersResultDTO>();
        var message = users.Count() == 0 ?
                            "No users found" : "Users retrieved succesfully";

        if(users.Count() == 0) return new GetUsersQueryDTO(message, Enumerable.Empty<GetUsersResultDTO>());

        foreach(var user in users)
        {
            result.Add(new GetUsersResultDTO(
                user.Id,
                user.FullName,
                user.Email!,
                user.Status
            ));
        }

        return new GetUsersQueryDTO(message, result);
    }
}