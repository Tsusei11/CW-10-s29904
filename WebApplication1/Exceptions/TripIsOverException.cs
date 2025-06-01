namespace WebApplication1.Exceptions;

public class TripIsOverException(int idTrip) : Exception($"Trip {idTrip} is already over")
{
    
}