using Microsoft.AspNetCore.Identity;

namespace Identity.Domain;
public class User : IdentityUser
{
    public string FullName {get; private set;} = string.Empty;

    public static User Create(Guid id, string fullname, string email)
    {
        var user = new User()
        {
            Id = id.ToString(),
            FullName = fullname,
            Email = email
        };

        return user;
    }
}