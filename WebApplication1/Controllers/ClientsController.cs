using Microsoft.AspNetCore.Mvc;
using WebApplication1.Exceptions;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController(IDbService dbService) : ControllerBase
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient([FromRoute] int id)
    {
        try
        {
            await dbService.DeleteClient(id);

            return NoContent();
        }
        catch (ClientIsAssignedToTripException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}