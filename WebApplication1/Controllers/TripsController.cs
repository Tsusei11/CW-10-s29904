using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Exceptions;
using WebApplication1.Services;

namespace WebApplication1.Controllers;


[ApiController]
[Route("[controller]")]
public class TripsController(IPageService pageService, IDbService dbService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int pageSize = 10, [FromQuery] int pageNum = 1)
    {
        try
        {
            return Ok(await pageService.GetPagedTrips(pageSize, pageNum));
        }
        catch (Exception e) when (e is IncorrectPageNumberException || e is IncorrectPageSizeException)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("{id:int}/clients")]
    public async Task<IActionResult> PostClient([FromRoute] int id, [FromBody] ClientTripPostDto clientTripBody)
    {
        try
        {
            var clientTrip = await dbService.PostClientTrip(id, clientTripBody);
            return Created($"/clients/{clientTrip.IdClient}", clientTrip);
        }
        catch (Exception e) when(e is PESELOverlappingException || e is TripIsOverException)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
}