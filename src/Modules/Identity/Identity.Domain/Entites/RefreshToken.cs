namespace Identity.Domain.Entites;
public class RefreshToken
{
    public Guid Id {get; set;}
    public required string UserId {get; set;}
    public required string Token {get; set;}
    public DateTime ExpiresOnUtc {get; set;}
}