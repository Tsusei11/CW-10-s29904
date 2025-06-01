namespace WebApplication1.DTOs;

public class PagedTripsGetDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public ICollection<TripGetDto> Trips { get; set; }
}