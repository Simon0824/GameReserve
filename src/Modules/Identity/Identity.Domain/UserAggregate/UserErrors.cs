using SharedKernel.Domain.Abstractions;

namespace Identity.Domain.UserAggregate;

public class UserErrors
{
    public static readonly Error UserNotFoundException = new("User.UserNoutFound", "User not found in DB");
    public static readonly Error UserBanned = new("User.UserIsBanned", "User is banned");
    public static readonly Error CannotDeleteUserException = new("User.CannotDelete", "User cannot be deleted");
    public static readonly Error CannotCreateUserException = new("User.CannotCreate", "User cannot be created");
    public static readonly Error CannotLogInUserException = new("User.CannotLogIn", "Cannot log a user");
    public static readonly Error CannotLogInRefreshTokenException = new("User.CannotLogInRefreshToken", "Refresh token is expired or is not set in db");
    public static readonly Error CannotAddUserRoleException = new("User.CannotAddRole", "Cannot add a role to user");
    public static readonly Error PasswordNotValid = new("User.NotValidPass", "Password not valid");
}
