using SharedKernel.Domain.Abstractions;

namespace Identity.Domain.UserAggregate;

public class UserErrors
{
    public static readonly Error UserNotFound = Error.NotFound("User.UserNoutFound", "User not found in DB");
    public static readonly Error UserBanned = Error.Forbidden("User.UserIsBanned", "User is banned");
    public static readonly Error CannotDeleteUser = Error.Forbidden("User.CannotDelete", "User cannot be deleted");
    public static readonly Error CannotCreateUser = Error.Forbidden("User.CannotCreate", "User cannot be created");
    public static readonly Error CannotLogInUser = Error.Unauthorized("User.CannotLogIn", "Cannot log a user");
    public static readonly Error CannotLogInRefreshToken = Error.Unauthorized("User.CannotLogInRefreshToken", "Refresh token is expired or is not set in db");
    public static readonly Error CannotAddUserRole = Error.Forbidden("User.CannotAddRole", "Cannot add a role to user");
    public static readonly Error CannotChangePassword = Error.Forbidden("User.CannotChangePassword", "Cannot change a user password");
    public static readonly Error PasswordNotValid = Error.Validation("User.NotValidPass", "Password not valid");
}
