namespace Identity.Domain;
public class User
{
    public Guid Id {get; private set;} = Guid.Empty;
    public string FullName {get; private set;} = string.Empty;
    public string Email {get; private set;} = string.Empty;
    public string Password {get; private set;} = string.Empty;

}