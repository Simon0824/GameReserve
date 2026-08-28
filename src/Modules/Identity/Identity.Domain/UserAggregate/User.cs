using Identity.Domain.Abstractions;

namespace Identity.Domain.UserAggregate;
public class User : Entity
{
    public string FullName {get; private set;} = string.Empty;

    public static User Create(string fullName, string email)
    {
        var user = new User()
        {
            FullName = fullName,
            Email = email
        };

        return user;
    }
}