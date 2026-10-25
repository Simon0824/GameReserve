using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Reservations.Domain.Aggregates;
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
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    DistributedCacheEntryOptions ReservationTtl = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

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

            var cachedDtos = reservationProfiles.Select(profile => new CachedReservationProfileDto(
                profile!.Id,
                profile.UserId,
                profile.Reservations
            )).ToList();

            if(reservationProfiles == null)
                return reservationProfiles!;

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
                        profile.Id, 
                        profile.UserId, 
                        profile.Reservations
                        ))
            .ToList();

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

            await cache.SetStringAsync(ReservationByIdKey(stringId), JsonSerializer.Serialize(reservation), ReservationTtl);

            return reservation;
        }

        reservation = JsonSerializer.Deserialize<Reservation>(cachedReservation);

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

            await cache.SetStringAsync(ReservationProfileByIdKey(stringId), JsonSerializer.Serialize(reservationProfile), ProfileTtl);

            return reservationProfile;
        }

        reservationProfile = JsonSerializer.Deserialize<ReservationProfile>(cachedProfileId);

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

            await cache.SetStringAsync(ReservationProfileByUserIdKey(stringId), JsonSerializer.Serialize(reservationProfile), ProfileTtl);

            return reservationProfile;
        }

        reservationProfile = JsonSerializer.Deserialize<ReservationProfile>(cachedProfileId);

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
