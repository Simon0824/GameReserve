using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Reservations.Domain.Aggregates;
using Reservations.Domain.Enums;
using Reservations.Domain.Interfaces;
using Reservations.Domain.Primitives;
using Reservations.Infrastructure.Dtos;

namespace Reservations.Infrastructure.Repositories;

public class CachedReservationRepository(
                    IDistributedCache cache, 
                    IReservationsRepository decorated) : IReservationsRepository
{
    private const string _cachedReservationsProfiles = "reservationProfiles:all";
    private static string ReservationProfileByIdKey(string id) => $"reservationProfile:{id}";
    private static string ReservationProfileByUserIdKey(string userId) => $"reservationProfile:{userId}";
    private static string ReservationByIdKey(string id) => $"reservation:{id}";

    DistributedCacheEntryOptions AllProfilesTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
    };

    DistributedCacheEntryOptions ProfileTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    DistributedCacheEntryOptions ReservationTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    private static Reservation ToDomain(CachedReservationDto dto)
    {
        if(!Enum.TryParse(dto.Status, out ReservationStatus reservationStatus))
        {
            throw new JsonException(
                $"Invalid reservation status in Redis cache: '{dto.Status}'."
            );
        }

        return Reservation.Rehydrate(
            new ReservationId(dto.Id),
            new ReservationProfileId(dto.ReservationProfileId),
            dto.GameId,
            dto.StartDate,
            dto.EndDate,
            reservationStatus
        );
    }

    public void AddReservationProfile(
        ReservationProfile reservationProfile)
    {
        decorated.AddReservationProfile(reservationProfile);
        cache.Remove(_cachedReservationsProfiles);
    }

    public async Task<IReadOnlyList<ReservationProfile?>> GetReservationProfiles(CancellationToken cancellationToken)
    {
        var cachedProfiles = await cache.GetStringAsync(_cachedReservationsProfiles, cancellationToken);

        IReadOnlyList<ReservationProfile?> reservationProfiles;

        if(string.IsNullOrEmpty(cachedProfiles))
        {
            reservationProfiles = await decorated.GetReservationProfiles(cancellationToken);


            if(reservationProfiles == null)
                return reservationProfiles!;

            var cachedDtos = reservationProfiles.Select(profile => new CachedReservationProfileDto(
                profile!.Id.Value,
                profile.UserId.Value,
                profile.Reservations.Select(reservation =>
                        new CachedReservationDto(
                            reservation.Id.Value,
                            reservation.ReservationProfileId.Value,
                            reservation.GameId,
                            reservation.StartDate,
                            reservation.EndDate,
                            reservation.Status.ToString()
                        )
                    ).ToList()
                )
            ).ToList();

            await cache.SetStringAsync(
                    _cachedReservationsProfiles,
                    JsonSerializer.Serialize(cachedDtos),
                    AllProfilesTtl,
                    cancellationToken);

            return reservationProfiles;
        }

        var deserializedProfiles = JsonSerializer.Deserialize<List<CachedReservationProfileDto>>(cachedProfiles)!;
        reservationProfiles =  deserializedProfiles
            .Select(profile =>
                    ReservationProfile.Rehydrate(
                        new ReservationProfileId(profile.Id),
                        new UserId(profile.UserId),
                        profile.Reservations.Select(ToDomain))
                        ).ToList();

        return reservationProfiles;
    }

    public async Task<Reservation?> GetReservationById(ReservationId reservationId, CancellationToken cancellationToken)
    {
        string stringId = reservationId.ToString();
        var cachedReservation = await cache.GetStringAsync(ReservationByIdKey(stringId), cancellationToken);

        Reservation? reservation;

        if(string.IsNullOrEmpty(cachedReservation))
        {
            reservation = await decorated.GetReservationById(reservationId, cancellationToken);

            if(reservation is null)
                return null;

            var cacheReserve = new CachedReservationDto(
                reservation.Id.Value,
                reservation.ReservationProfileId.Value,
                reservation.GameId,
                reservation.StartDate,
                reservation.EndDate,
                reservation.Status.ToString()
            );

            await cache.SetStringAsync(
                ReservationByIdKey(stringId), 
                JsonSerializer.Serialize(cacheReserve), 
                ReservationTtl,
                cancellationToken);

            return reservation;
        }

        var deserializedReservation = JsonSerializer.Deserialize<CachedReservationDto>(cachedReservation);

        Enum.TryParse(deserializedReservation!.Status, out ReservationStatus reservationStatus);

        reservation = Reservation.Rehydrate(
            new ReservationId(deserializedReservation.Id),
            new ReservationProfileId(deserializedReservation.ReservationProfileId),
            deserializedReservation.GameId,
            deserializedReservation.StartDate,
            deserializedReservation.EndDate,
            reservationStatus
        );

        return reservation;
    }

    public async Task<ReservationProfile?> GetReservationProfileById(ReservationProfileId reservationProfileId, CancellationToken cancellationToken)
    {
        string stringId = reservationProfileId.Value.ToString();
        var cachedProfileId = await cache.GetStringAsync(ReservationProfileByIdKey(stringId), cancellationToken);

        ReservationProfile? reservationProfile;

        if(string.IsNullOrEmpty(cachedProfileId))
        {
            reservationProfile = await decorated.GetReservationProfileById(reservationProfileId, cancellationToken);

            if(reservationProfile is null)
                return null;

            
            var cachedProfile = new CachedReservationProfileDto(
                reservationProfile.Id.Value, 
                reservationProfile.UserId.Value, 
                reservationProfile.Reservations.Select(reservation => 
                    new CachedReservationDto(
                        reservation.Id.Value, 
                        reservation.ReservationProfileId.Value, 
                        reservation.GameId, 
                        reservation.StartDate, 
                        reservation.EndDate,
                        reservation.Status.ToString())
                        ).ToList()
                    );

            await cache.SetStringAsync(
                ReservationProfileByIdKey(stringId), 
                JsonSerializer.Serialize(cachedProfile), 
                ProfileTtl,
                cancellationToken);

            return reservationProfile;
        }

        var deserializedProfile = JsonSerializer.Deserialize<CachedReservationProfileDto>(cachedProfileId);

        reservationProfile = ReservationProfile.Rehydrate(
            new ReservationProfileId(deserializedProfile!.Id), 
            new UserId(deserializedProfile.UserId), 
            deserializedProfile.Reservations.Select(ToDomain).ToList()
            );

        return reservationProfile;
    }

    public async Task<ReservationProfile?> GetReservationProfileByUserId(UserId userId, CancellationToken cancellationToken)
    {
        string stringId = userId.Value.ToString();
        var cachedProfileId = await cache.GetStringAsync(ReservationProfileByUserIdKey(stringId), cancellationToken);

        ReservationProfile? reservationProfile;

        if(string.IsNullOrEmpty(cachedProfileId))
        {
            reservationProfile = await decorated.GetReservationProfileByUserId(userId, cancellationToken);

            if(reservationProfile is null)
                return null;

            var cachedProfile = new CachedReservationProfileDto(
                reservationProfile.Id.Value, 
                reservationProfile.UserId.Value, 
                reservationProfile.Reservations.Select(reservation => 
                    new CachedReservationDto(
                        reservation.Id.Value, 
                        reservation.ReservationProfileId.Value, 
                        reservation.GameId, 
                        reservation.StartDate, 
                        reservation.EndDate,
                        reservation.Status.ToString())
                        ).ToList()
                    );

            await cache.SetStringAsync(
                ReservationProfileByUserIdKey(stringId), 
                JsonSerializer.Serialize(cachedProfile), 
                ProfileTtl,
                cancellationToken);

            return reservationProfile;
        }

        var deserializedProfile = JsonSerializer.Deserialize<CachedReservationProfileDto>(cachedProfileId);

        reservationProfile = ReservationProfile.Rehydrate(
            new ReservationProfileId(deserializedProfile!.Id), 
            new UserId(deserializedProfile.UserId), 
            deserializedProfile.Reservations.Select(ToDomain).ToList()
            );

        return reservationProfile;
    }

    public async Task<bool> CheckReservationDates(Guid gameId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        return await decorated.CheckReservationDates(gameId, startDate, endDate, cancellationToken);
    }

    public void CreateReservation(Reservation reservation)
    {
        decorated.CreateReservation(reservation);
    }
}
