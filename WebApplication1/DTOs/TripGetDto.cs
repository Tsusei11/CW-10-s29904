namespace WebApplication1.DTOs;

public class TripGetDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }

    public ICollection<CountryGetDto> Countries { get; set; }
    public ICollection<ClientGetDto> Clients { get; set; }
}