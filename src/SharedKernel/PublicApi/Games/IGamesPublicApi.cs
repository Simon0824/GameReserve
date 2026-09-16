namespace SharedKernel.PublicApi.Games;
public interface IGamePublicApi
{
    Task<GameResultDTO?> GetGameById(Guid gameId, CancellationToken cancellationToken);
}

public record GameResultDTO(Guid GameId, string Title);