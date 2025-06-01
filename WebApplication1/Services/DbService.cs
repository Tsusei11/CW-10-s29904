using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Exceptions;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IDbService
{
    public Task<List<TripGetDto>> GetTrips();
    
    public Task DeleteClient(int idClient);
    
    public Task<ClientTripGetDto> PostClientTrip(int idTrip, ClientTripPostDto clientTripBody);
}

public class DbService(TripsContext data) : IDbService
{
    public async Task<List<TripGetDto>> GetTrips()
    {
        var trips = await data.Trips.Select(t => new TripGetDto
        {
            Name = t.Name,
            Description = t.Description,
            DateFrom = t.DateFrom,
            DateTo = t.DateTo,
            MaxPeople = t.MaxPeople,
            Countries = data.Countries.Where(c => c.IdTrips.Contains(t)).Select(c => new CountryGetDto
            {
                Name = c.Name
            }).ToList(),
            Clients = data.Clients.Where(c => c.ClientTrips.Any(ct => ct.IdTripNavigation == t)).Select(c => new ClientGetDto
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
            }).ToList()
        }).ToListAsync();
        
        return trips;
    }

    public async Task DeleteClient(int idClient)
    {
        var client = await data.Clients.Include(c => c.ClientTrips).FirstOrDefaultAsync(c => c.IdClient == idClient);
        
        if (client == null)
            throw new NotFoundException($"Client {idClient} not found");
        
        if (client.ClientTrips.Count > 0)
            throw new ClientIsAssignedToTripException("Client is already assigned to some trip");
        
        data.Clients.Remove(client);
        await data.SaveChangesAsync();
    }

    public async Task<ClientTripGetDto> PostClientTrip(int idTrip, ClientTripPostDto clientTripBody)
    {
        var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        if (trip == null)
            throw new NotFoundException($"Trip {idTrip} not found");
        
        if (trip.DateFrom < DateTime.Now)
            throw new TripIsOverException(idTrip);
        
        if (await data.Clients.FirstOrDefaultAsync(c => c.Pesel == clientTripBody.Pesel) != null)
            throw new PESELOverlappingException($"Client with PESEL {clientTripBody.Pesel} already exists");
        
        if (await data.Clients.Where(c => c.ClientTrips.Any(ct => ct.IdTrip == idTrip))
                .FirstOrDefaultAsync(c => c.Pesel == clientTripBody.Pesel) != null)
            throw new PESELOverlappingException($"Client with PESEL {clientTripBody.Pesel} is already assigned to trip {idTrip}");

        var client = new Client
        {
            FirstName = clientTripBody.FirstName,
            LastName = clientTripBody.LastName,
            Email = clientTripBody.Email,
            Telephone = clientTripBody.Telephone,
            Pesel = clientTripBody.Pesel,
        };
        
        await data.Clients.AddAsync(client);
        await data.SaveChangesAsync();

        var clientTrip = new ClientTrip
        {
            IdTrip = idTrip,
            IdClient = client.IdClient,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientTripBody.PaymentDate
        };
        
        await data.ClientTrips.AddAsync(clientTrip);
        await data.SaveChangesAsync();

        return new ClientTripGetDto
        {
            IdClient = client.IdClient,
            FirstName = client.FirstName,
            LastName = client.LastName,
            TripName = clientTrip.IdTripNavigation.Name,
            RegisteredAt = clientTrip.RegisteredAt,
            PaymentDate = clientTrip.PaymentDate
        };
    }
}