using Microsoft.AspNetCore.Identity;

namespace Identity.Domain;
public class User : IdentityUser
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