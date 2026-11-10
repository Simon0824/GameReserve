using Identity.Domain.Abstractions;
using Identity.Domain.Enums;
using SharedKernel.Domain.Abstractions;

namespace Identity.Domain.UserAggregate;
public class User : Entity
{
    public string FullName {get; private set;} = string.Empty;
    public UserStatus Status {get; private set;}

    private User()
    {}

    public static User Create(string fullName, string email)
    {
        var user = new User()
        {
            FullName = fullName,
            Status = UserStatus.Active,
            Email = email,
            UserName = email
        };

        user.Raise(new UserCreatedDomainEvent(Guid.NewGuid(), Guid.Parse(user.Id)));

        return user;
    }

    public Result BanUser()
    {
        if(Status == UserStatus.Banned)
            return UserErrors.UserBanned;

        Status = UserStatus.Banned;
        return Result.Success;
    }
}