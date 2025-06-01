using WebApplication1.DTOs;
using WebApplication1.Exceptions;

namespace WebApplication1.Services;

public interface IPageService
{
    public Task<PagedTripsGetDto> GetPagedTrips(int pageSize, int pageNum);
}

public class PageService(IDbService dbService) : IPageService
{
    public async Task<PagedTripsGetDto> GetPagedTrips(int pageSize, int pageNum)
    {
        if (pageSize < 1)
            throw new IncorrectPageSizeException("Page size must be greater than 0");
        
        if (pageNum < 1)
            throw new IncorrectPageNumberException("Page number must be greater than 0");
        
        var trips = await dbService.GetTrips();

        var allPagesCount = (int)Math.Ceiling((double)trips.Count / pageSize);
        
        if (pageNum > allPagesCount)
            throw new NotFoundException($"Page {pageNum} not found");

        var firstPage = pageSize * (pageNum - 1);
        var lastPage = trips.Count - firstPage <= pageSize ? trips.Count - firstPage : firstPage + pageSize;
        
        return new PagedTripsGetDto
        {
            PageNum = pageNum,
            PageSize = pageSize,
            AllPages = allPagesCount,
            Trips = trips.Slice(firstPage, lastPage)
        };
    }
}